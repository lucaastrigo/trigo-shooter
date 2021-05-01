using System;
using System.Collections.Generic;
using System.IO;
using FancyFolders.Editor.FileSystem;
using UnityEditor;
using UnityEditor.Compilation;

namespace FancyFolders.Editor.Compiler
{
    public class CompilerMonitor
    {
        public static CompilerMonitor Instance { get; } = new CompilerMonitor();

        public event Action EpochChanged;

        public uint Epoch { get; private set; }

        private readonly HashSet<AssetGuid> _errors = new HashSet<AssetGuid>();

        public void CompileComplete([NotNull] IEnumerable<CompilerMessage> messages)
        {
            //Increment epoch to blast all caches of compiler data
            unchecked { Epoch++; }

            _errors.Clear();

            //Mark file GUIDs with an error
            foreach (var compilerMessage in messages)
            {
                if (compilerMessage.type == CompilerMessageType.Warning)
                    continue;

                var fileGuid = AssetDatabase.AssetPathToGUID(compilerMessage.file);
                if (fileGuid == null)
                    continue;

                //Mark this file
                _errors.Add(new AssetGuid(fileGuid));

                //Walk up hierarchy marking all parents folders
                var dir = Path.GetDirectoryName(compilerMessage.file);
                while (!string.IsNullOrEmpty(dir))
                {
                    var dirGuid = AssetDatabase.AssetPathToGUID(dir);
                    if (dirGuid != null)
                        _errors.Add(new AssetGuid(dirGuid));

                    dir = Path.GetDirectoryName(dir);
                }
            }

            if (EpochChanged != null)
                EpochChanged();
        }

        public bool HasError(AssetGuid guid)
        {
            return _errors.Contains(guid);
        }
    }
}
