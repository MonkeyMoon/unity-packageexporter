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

        private List<AssetInfo> _childs;

        public AssetInfo(string asset_path)
        {
            is_valid = false;
            if (string.IsNullOrEmpty(asset_path) == false)
            {
                is_valid = true;
                path = asset_path;
                depth_level = (short)(path.Split('/').Length - 2); // -2 then Assets/asset will be 0 : root;
                is_directory = AssetDatabase.IsValidFolder(path);
                if (is_directory == true)
                {
                    _childs = new List<AssetInfo>();
                }
            }
        }

        public void AddChild(AssetInfo child)
        {
            if ( child != null && is_directory == true && child != this )
            {
                _childs.Add(child);
            }
        }

        public List<AssetInfo> GetChildList()
        {
            return _childs;
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

            Regex child_match;
            foreach (AssetInfo assetinfo in _assets)
            {
                if ( assetinfo.is_directory == true )
                {
                    child_match = new Regex(@"^" + assetinfo.path + ".+");
                    foreach (AssetInfo child in _assets)
                    {
                        if ( child_match.IsMatch(child.path) == true && child.depth_level == assetinfo.depth_level + 1 )
                        {
                            assetinfo.AddChild(child);
                        }
                    }
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
        private IEnumerable<AssetInfo> child_enumerator;

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
                if (ai.depth_level == 0)
                {
                    DisplayAssetGroup(ai);
                }
            }
            GUILayout.EndVertical();
        }

        private void DisplayAssetGroup(AssetInfo group)
        {
            string padding = "";
            for (int i = 0; i < group.depth_level; ++i )
            {
                padding += "\t";
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label(padding + group.path + " " + group.depth_level);
            GUILayout.EndHorizontal();
            if (group.is_directory == true)
            {
                List<AssetInfo> child_list = group.GetChildList();
                if (child_list != null)
                {
                    foreach (AssetInfo child in child_list)
                    {
                        DisplayAssetGroup(child);
                    }
                }
            }
        }
    }
}