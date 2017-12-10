using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace MM.PackageExporter
{
    /// <summary>
    /// Contains all the AssetInfo of current project.
    /// </summary>
    public class AssetInfoHolder
    {
        private List<AssetInfo> _assets;
        private List<AssetInfo> _first_row;

        public AssetInfoHolder()
        {
            _assets = new List<AssetInfo>();
            _first_row = new List<AssetInfo>();
            // Get all asset paths contain in the AssetDatabase and keep only the ones that match the 
            // Regular expression.
            Regex assetpath_match = new Regex(@"(Assets\\.*)$");
            Match match;
            string[] dir_paths = Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories);
            string[] file_paths = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            string[] paths = new string[dir_paths.Length + file_paths.Length];
            dir_paths.CopyTo(paths, 0);
            file_paths.CopyTo(paths, dir_paths.Length);

            foreach (string path in paths) 
            {
                match = assetpath_match.Match(path);
                if ( match.Success == true && Path.GetExtension(path).ToString().ToLower() != ".meta" )
                {
                    _assets.Add(new AssetInfo(match.Groups[1].ToString()));
                }
            }

            // For every kept asset, find and attach their childs
            // ex: MMPackageExporter will contain MMPackageExporter/FileA.cs and MMPackageExporter/FileB.cs 
            //     as childs
            Regex child_match;
            foreach (AssetInfo assetinfo in _assets)
            {
                if (assetinfo.is_directory == true)
                {
                    child_match = new Regex(@"^" + assetinfo.path + ".+");
                    foreach( AssetInfo child in _assets)
                    {
                        if (child_match.IsMatch(child.path) == true && child.depth_level == assetinfo.depth_level + 1)
                        {
                            assetinfo.AddChild(child);
                        }
                    }
                    if ( assetinfo.depth_level == 0 )
                    {
                        _first_row.Add(assetinfo);
                    }
                }
            }
        }

        public List<AssetInfo> GetAssetInfos()
        {
            return _assets;
        }

        public List<AssetInfo> GetFirstRowInfos()
        {
            return _first_row;
        }

        /// <summary>
        /// Load a give PackageConfigurationSave and apply the according selections.
        /// </summary>
        /// <param name="package_config_save">Package Configuration to load.</param>
        /// <seealso cref="PackageConfigurationSave"/>
        /// <remarks>If a file has been saved in a PackageConfigurationSave but doesn't exist anymore
        /// it will be ignored.</remarks>
        public void LoadSave(PackageConfigurationSave package_config_save)
        {
            if (package_config_save == null || package_config_save.asset_path_list == null)
                return;
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

        /// <summary>
        /// Get an array of all selected assets path.
        /// </summary>
        /// <returns>An array of string containing the path of selected assets</returns>
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

        /// <summary>
        /// Unselect every previously selected asset.
        /// </summary>
        public void ClearSelection()
        {
            foreach (AssetInfo asset_info in _assets )
            {
                asset_info.SetSelected(false);
            }
        }

        /// <summary>
        /// Select all the assets.
        /// </summary>
        public void SelectAll()
        {
            foreach (AssetInfo asset_info in _assets)
            {
                asset_info.SetSelected(true);
            }
        }
    }


}