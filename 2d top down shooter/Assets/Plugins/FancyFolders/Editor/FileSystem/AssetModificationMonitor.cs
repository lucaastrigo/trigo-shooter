using System;
using UnityEditor;

namespace FancyFolders.Editor.FileSystem
{
    public class AssetModificationMonitor
        : AssetPostprocessor
    {
        public static event Action<string> AssetCreatedOrModified;
        public static event Action<string> AssetDeleted;
        public static event Action<string, string> AssetMoved;
        public static event Action AssetPostprocessComplete;

        // ReSharper disable ParameterTypeCanBeEnumerable.Global (Justification: Must be exactly this signature for unity to invoke it)
        public static void OnPostprocessAllAssets([NotNull] string[] importedAssets, [NotNull] string[] deletedAssets, [NotNull] string[] movedAssets, [NotNull] string[] movedFromAssetPaths)
        // ReSharper restore ParameterTypeCanBeEnumerable.Global
        {
            var somethingHappened = false;

            // A new file was created or an existing file was changed
            foreach (var assetPath in importedAssets)
            {
                if (AssetCreatedOrModified != null)
                {
                    somethingHappened = true;
                    AssetCreatedOrModified(assetPath);
                }
                else
                    break;
            }

            // A file was deleted (this is not triggered for file moves)
            foreach (var assetPath in deletedAssets)
            {
                if (AssetDeleted != null)
                {
                    somethingHappened = true;
                    AssetDeleted(assetPath);
                }
                else
                    break;
            }

            // A file was moved
            for (var i = 0; i < movedAssets.Length; i++)
            {
                if (AssetMoved != null)
                {
                    somethingHappened = true;
                    AssetMoved(movedFromAssetPaths[i], movedAssets[i]);
                }
                else
                    break;
            }

            // Something happened
            if (somethingHappened && AssetPostprocessComplete != null)
                AssetPostprocessComplete();
        }
    }
}
