using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MM.PackageExporter
{
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
                if (assetinfo.is_directory == true)
                {
                    child_match = new Regex(@"^" + assetinfo.path + ".+");
                    foreach (AssetInfo child in _assets)
                    {
                        if (child_match.IsMatch(child.path) == true && child.depth_level == assetinfo.depth_level + 1)
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

        public void LoadSave(PackageConfigurationSave package_config_save)
        {
            foreach (AssetInfo assetinfo in _assets)
            {
                assetinfo.SetSelected(false);
                foreach ( string path in package_config_save.asset_path_list)
                {
                    if ( assetinfo.path == path )
                    {
                        assetinfo.SetSelected(true);
                        break;
                    }
                }
            }
        }

        public string[] GetSelectedAssetPaths()
        {
            List<string> selected_assets = new List<string>();
            foreach ( AssetInfo asset_info in _assets )
            {
                if ( asset_info.is_directory == false && asset_info.is_selected == true )
                {
                    selected_assets.Add(asset_info.path);
                }
            }
            return selected_assets.ToArray();
        }
    }


}