using System;
using System.Linq;
using FancyFolders.Editor.VersionControl.Git;

namespace FancyFolders.Editor.VersionControl
{
    [Flags]
    public enum VcsStatus
    {
        None = 0,
        Added = 1,
        Modified = 2,
        Renamed = 4,
        LockedSelf = 8,
        LockedOther = 16,
        Conflicted = 32,
    }

    public class VcsStatusHandle
    {
        private readonly VcsManager _manager;
        private readonly string _fullPath;

        private int _epoch;
        private VcsStatus _status;

        public VcsStatus Status
        {
            get
            {
                if (_epoch != _manager.Epoch)
                    UpdateCache();

                return _status;
            }
        }

        public VcsStatusHandle(VcsManager manager, string fullPath)
        {
            _manager = manager;
            _fullPath = fullPath;

            UpdateCache();
        }

        private void UpdateCache()
        {
            (_status, _epoch) = _manager.GetStatus(_fullPath);
        }
    }

    public interface IVersionControlSystem
    {
        bool Initialize();

        (VcsStatus, int) GetStatus(string fullPath);

        int Epoch { get; }
    }

    public class VcsManager
    {
        public static readonly VcsManager Instance = LoadVcs();

        private readonly IVersionControlSystem _vcs;

        public int Epoch => _vcs?.Epoch ?? 0;

        private VcsManager(IVersionControlSystem vcs)
        {
            _vcs = vcs;
        }

        [NotNull] private static VcsManager LoadVcs()
        {
            var candidates = new IVersionControlSystem[]
            {
                new GitVcs(), 
            };

            var vcs = candidates.FirstOrDefault(x => x.Initialize());
            return new VcsManager(vcs);
        }

        [NotNull] public VcsStatusHandle GetStatusHandle(string fullPath)
        {
            return new VcsStatusHandle(this, fullPath);
        }

        internal (VcsStatus, int) GetStatus(string fullPath)
        {
            if (_vcs == null)
                return (VcsStatus.None, 0);
          
            // Get the status of the file itself and it's associated meta file
            var (fileStatus, fileEpoch) = _vcs.GetStatus(fullPath);
            var (metaStatus, metaEpoch) = _vcs.GetStatus(fullPath + ".meta");

            // It's possible we have got a status which is a mix of two epochs.
            // In this case return the smaller epoch so that the cache is immediately invalid, hopefully we'll get up to date data next time.
            return (fileStatus | metaStatus, Math.Min(fileEpoch, metaEpoch));
        }
    }
}
