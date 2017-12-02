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
    }


}