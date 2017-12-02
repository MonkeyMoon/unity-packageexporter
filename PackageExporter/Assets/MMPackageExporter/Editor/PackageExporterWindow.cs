using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Linq;

namespace MM.PackageExporter
{
    public class AssetInfo
    {
        // TODO: enum + getter on file extension for icon display.
        public bool is_valid { get; private set; }
        public bool is_directory { get; private set; }
        public short depth_level { get; private set; }
        public string path { get; private set; }

        public AssetInfo(string asset_path)
        {
            is_valid = false;
            if (string.IsNullOrEmpty(asset_path) == false)
            {
                is_valid = true;
                is_directory = AssetDatabase.IsValidFolder(asset_path);
                depth_level = (short)asset_path.Split('/').Length;
                path = asset_path;
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
            IOrderedEnumerable<string> ordered_path = AssetDatabase.GetAllAssetPaths().OrderBy(x => x);
            foreach (string path in ordered_path)
            {
                if (assetpath_match.IsMatch(path))
                {
                    _assets.Add(new AssetInfo(path));
                }
            }
        }

        public List<AssetInfo> GetAssets()
        {
            return _assets;
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

            DisplayAssets();
        }

        private void DisplayAssets()
        {
            if (_path_holder == null)
                RefreshContent();

            List<AssetInfo> assets = _path_holder.GetAssets();
            GUILayout.BeginVertical();
            foreach ( AssetInfo ai in assets )
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(ai.path + " " + ai.depth_level);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

        }
    }
}