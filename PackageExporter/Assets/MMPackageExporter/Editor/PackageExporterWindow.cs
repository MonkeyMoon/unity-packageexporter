using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace MM.PackageExporter
{
    public class AssetInfo
    {
        // TODO: enum + getter on file extension for icon display.
        public bool is_valid { get; private set; }
        public bool is_directory { get; private set; }

        public AssetInfo(string asset_path)
        {
            is_valid = false;
            if (string.IsNullOrEmpty(asset_path) == false)
            {
                is_valid = true;
                is_directory = AssetDatabase.IsValidFolder(asset_path);
            }
        }
    }

    public class AssetInfoHolder
    {
        private List<AssetInfo> _assets;

        public AssetInfoHolder()
        {
            _assets = new List<AssetInfo>();
            Regex assetpath_match = new Regex(@"^Assets\/");
            string[] asset_paths = AssetDatabase.GetAllAssetPaths();
            foreach (string path in asset_paths)
            {
                if (assetpath_match.IsMatch(path))
                {
                    _assets.Add(new AssetInfo(path));
                }
            }
        }
    }

    public class PackageExporterWindow : EditorWindow
    {
        private AssetInfoHolder _path_holder;

        [MenuItem("Window/Monkey Moon/Package Exporter")]
        private static void ShowWindow()
        {
            PackageExporterWindow window = (PackageExporterWindow)EditorWindow.GetWindow(typeof(PackageExporterWindow));
            window.Show();
            window.RefreshContent();
        }

        public void RefreshContent()
        {
            _path_holder = new AssetInfoHolder();
        }

        private void ExportPackage()
        {

        }

        private void OnGUI()
        {
            if ( GUILayout.Button("REFRESH") == true )
            {
                RefreshContent();
            }
        }
    }
}