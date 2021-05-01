using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FancyFolders.Editor.Compiler;
using FancyFolders.Editor.FileSystem;

namespace FancyFolders.Editor.VersionControl.Git
{
    internal class GitVcs
        : IVersionControlSystem, IDisposable
    {
        // Type 1 status messages (normal)
        private static readonly Regex StatusPattern = new Regex(
            @"^1 (?<status_index>[\w\.])(?<status_wt>[\w\.]) (?<sub>[\w\.]{4}) (?<mH>\d+) (?<mI>\d+) (?<mW>\d+) (?<hH>[\d\w]+) (?<hI>[\d\w]+) (?<path>.*)$",
            RegexOptions.Compiled | RegexOptions.Multiline
        );

        // Type 2 status messages (renames)
        private static readonly Regex StatusRenamePattern = new Regex(
            @"^2 (?<status_index>[\w\.])(?<status_wt>[\w\.]) (?<sub>[\w\.]{4}) (?<mH>\d+) (?<mI>\d+) (?<mW>\d+) (?<hH>[\d\w]+) (?<hI>[\d\w]+) ([\w\.]\d+) (?<path>.*)\t(?<orig_path>.*)$",
            RegexOptions.Compiled | RegexOptions.Multiline
        );

        // Untracked status messages
        private static readonly Regex StatusUntrackedPattern = new Regex(
            @"^\? (?<path>.*)$",
            RegexOptions.Compiled | RegexOptions.Multiline
        );

        private static readonly Dictionary<string, VcsStatus> StatusConversions = new Dictionary<string, VcsStatus>
        {
            { "A", VcsStatus.Added },
            { "M", VcsStatus.Modified },
            { "R", VcsStatus.Renamed },
            { "C", VcsStatus.Added }
        };

        private volatile StatusCache _statusCache;
        public int Epoch => _statusCache.Epoch;

        private readonly List<FileSystemWatcher> _watchers;
        private readonly SemaphoreSlim _updateLock;
        
        private string _repoPath;
        private string _gitFolderPath;

        public GitVcs([CanBeNull] string repoPath = null)
        {
            _repoPath = repoPath;

            _statusCache = new StatusCache(0, "", new Dictionary<string, VcsStatus>());

            _watchers = new List<FileSystemWatcher>();
            _updateLock = new SemaphoreSlim(1, 1);
        }

        public bool Initialize()
        {
            // Find the .git repo location (or work-tree root)
            if (_repoPath == null)
                _repoPath = FindRepoPath();

            if (!string.IsNullOrEmpty(_repoPath))
            {
                // Find the actual .git directory (won't be the same as _repoPath if we are in a working dir)
                _gitFolderPath = Path.Combine(_repoPath, FixPath(Git("rev-parse --git-dir").Result));

                // Monitor changes to the .git folder so we can spot commits
                var watcher = new FileSystemWatcher(_gitFolderPath) {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName, Filter = "*", EnableRaisingEvents = true, IncludeSubdirectories = true,
                };
                _watchers.Add(watcher);

                // Trigger an update which will only do anything if the commit hash has changed
                watcher.Changed += (_, __) => ScheduleUpdate(true);

                // Trigger an update when asset in the project changes
                AssetModificationMonitor.AssetPostprocessComplete += OnPostprocessComplete;

                // Trigger an update when a script changes
                CompilerMonitor.Instance.EpochChanged += OnPostprocessComplete;

                // Kick off an initial status update
                ScheduleUpdate(false);

                return true;
            }

            // No git repo found
            return false;
        }

        private void OnPostprocessComplete()
        {
            ScheduleUpdate(false);
        }

        public (VcsStatus, int) GetStatus([CanBeNull] string fullPath)
        {
            var currentCache = _statusCache;

            if (string.IsNullOrEmpty(fullPath))
                return (VcsStatus.None, currentCache.Epoch);

            if (currentCache.PathStatus.TryGetValue(fullPath, out var status))
                return (status, currentCache.Epoch);

            return (VcsStatus.None, currentCache.Epoch);
        }

        private void ScheduleUpdate(bool requireCommitChange)
        {
            // Schedule an update task
            Task.Run(async () =>
            {
                // Delay a little to allow filesystem changes to buffer
                await Task.Delay(200);
                if (await Update(requireCommitChange))
                    Main.ForceRedraw();
            });
        }

        private async Task<bool> Update(bool requireCommitChange)
        {
            await _updateLock.WaitAsync();
            try
            {
                // Create a new object to contains the status for files
                var result = new Dictionary<string, VcsStatus>();

                // Get the current commit from git
                var commit = await Git("git rev-parse HEAD");

                // Early exit if a commit change was required and has not happened
                if (requireCommitChange && _statusCache.Commit == commit)
                    return false;

                // Query git for status
                var status = await Git("status --porcelain=v2");

                // Parse type 1 status messages
                // Parse type 1 status messages
                var matches = StatusPattern.Matches(status);
                foreach (Match match in matches)
                {
                    var file = match.Groups["path"].Value;
                    var indexStatus = match.Groups["status_index"].Value;
                    var wtStatus = match.Groups["status_wt"].Value;
                    var fileStatus = ParseStatus(indexStatus, wtStatus);
                    RecordStatus(result, FixPath(file), fileStatus);
                }

                // Parse type 2 status messages (renames)
                matches = StatusRenamePattern.Matches(status);
                foreach (Match match in matches)
                {
                    var file = match.Groups["path"].Value;
                    var indexStatus = match.Groups["status_index"].Value;
                    var wtStatus = match.Groups["status_wt"].Value;
                    var fileStatus = ParseStatus(indexStatus, wtStatus);
                    RecordStatus(result, FixPath(file), fileStatus | VcsStatus.Renamed);
                }

                // Parse untracked status messages
                matches = StatusUntrackedPattern.Matches(status);
                foreach (Match match in matches)
                {
                    // We consider untracked as added
                    var file = FixPath(match.Groups["path"].Value);
                    RecordStatus(result, file, VcsStatus.Added);

                    // Git status only shows the top level untracked directory, and not its contents
                    if (Directory.Exists(file))
                    {
                        // Manually add recursive folder content status
                        var entries = Directory.EnumerateFileSystemEntries(file, "*", SearchOption.AllDirectories);
                        foreach (var entry in entries)
                        {
                            if (await IsMonitoredPath(entry))
                                RecordStatus(result, entry, VcsStatus.Added, false);
                        }
                    }
                }

                // Publish the result
                _statusCache = new StatusCache(unchecked(_statusCache.Epoch + 1), commit, result);

                return true;
            }
            finally
            {
                _updateLock.Release();
            }
        }

        private VcsStatus ParseStatus(string indexStatus, string wtStatus)
        {
            var indexStatusNone = indexStatus == ".";
            var wtStatusNone = wtStatus == ".";

            // Prefer work-tree status over index status
            var type = wtStatusNone
                ? indexStatus
                : wtStatus;

            // Convert git status code to our enum
            if (!StatusConversions.TryGetValue(type, out var fileStatus))
                fileStatus = VcsStatus.None;

            // If index and work-tree status are both non-null but differ, we have a conflict
            if (!indexStatusNone && !wtStatusNone && indexStatus != wtStatus)
                fileStatus |= VcsStatus.Conflicted;

            return fileStatus;
        }
        
        private void RecordStatus([NotNull] IDictionary<string, VcsStatus> output, string file, VcsStatus status, bool setParents = true)
        {
            var fullPath = Path.IsPathRooted(file) ? file : Path.Combine(_repoPath, file);

            // Add status to current mask value
            if (!output.TryGetValue(fullPath, out var current))
                current = VcsStatus.None;

            current |= status;
            output[fullPath] = current;

            if (setParents)
            {
                // Walk up hierarchy marking all parents folders
                var dir = Path.GetDirectoryName(file);
                if (!string.IsNullOrEmpty(dir))
                    RecordStatus(output, dir, status);
            }
        }

        [CanBeNull] private static string FindRepoPath()
        {
            try
            {
                var path = FixPath(Git("rev-parse --show-toplevel").Result);
                if (Directory.Exists(path))
                    return path;
            }
            catch
            {
                // If git command line throws, we assume there is no git repo
            }

            return null;
        }

        [NotNull] private static string FixPath(string path)
        {
            path = path.Trim(Path.GetInvalidPathChars());

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                path = path.Replace('/', '\\');

            return path;
        }

        private bool IsGitDir([NotNull] string path)
        {
            return path.StartsWith(_gitFolderPath);
        }

        private async Task<bool> IsMonitoredPath([NotNull] string path)
        {
            //Ignore changes to the git dir
            if (IsGitDir(path))
                return false;

            // Ignore all files exclusded by applicable .gitignore files
            var ignored = FixPath(await Git("check-ignore \"" + path + "\""));
            if (!string.IsNullOrWhiteSpace(ignored))
                return false;

            return true;
        }

        private static Task<string> Git([NotNull] string command)
        {
            var process = new Process {
                StartInfo = {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = "git",
                    Arguments = command
                }
            };
            process.Start();

            return process.StandardOutput.ReadToEndAsync();
        }

        public void Dispose()
        {
            foreach (var watcher in _watchers)
                watcher.Dispose();

            AssetModificationMonitor.AssetPostprocessComplete -= OnPostprocessComplete;
            CompilerMonitor.Instance.EpochChanged -= OnPostprocessComplete;
        }

        private class StatusCache
        {
            public readonly int Epoch;
            public readonly string Commit;
            [NotNull] public readonly IReadOnlyDictionary<string, VcsStatus> PathStatus;

            public StatusCache(int epoch, string commit, [NotNull] IReadOnlyDictionary<string, VcsStatus> pathStatus)
            {
                Epoch = epoch;
                Commit = commit;
                PathStatus = pathStatus;
            }
        }
    }
}
