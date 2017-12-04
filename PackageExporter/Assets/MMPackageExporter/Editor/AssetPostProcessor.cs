using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MM.PackageExporter
{
    /// <summary>
    /// Refreshes PackageExporterWindow content when an Asset is modified.
    /// </summary>
    class AssetPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            PackageExporterWindow exporter_window = (PackageExporterWindow)EditorWindow.GetWindow(typeof(PackageExporterWindow));
            if (exporter_window != null)
            {
                exporter_window.RefreshContent();
            }
        }
    }
}
