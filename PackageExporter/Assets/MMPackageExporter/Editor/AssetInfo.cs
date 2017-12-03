using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MM.PackageExporter
{
    public class AssetInfo
    {
        public bool is_valid { get; private set; }
        public bool is_directory { get; private set; }
        public short depth_level { get; private set; }
        public string path { get; private set; }
        public string asset_name { get; private set; }
        public bool is_folded { get; private set; }
        public Texture2D icon { get; private set; }

        public bool is_selected
        { 
            get
            {
                if (is_directory == true)
                {
                    if (_childs == null)
                        return false;
                    foreach (AssetInfo child in _childs)
                    {
                        if (child.is_selected == false)
                            return false;
                    }
                    return true;
                }
                else
                    return _selected;
            }
        }

        public bool is_mixed_selection
        {
            get
            {
                if (is_directory == true)
                {
                    if (_childs == null)
                        return false;
                    bool selection = is_selected;
                    foreach (AssetInfo child in _childs)
                    {
                        if (child.is_selected != is_selected || child.is_mixed_selection == true)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private bool _selected;
        private List<AssetInfo> _childs;

        public AssetInfo(string asset_path)
        {
            is_valid = false;
            if (string.IsNullOrEmpty(asset_path) == false)
            {
                is_valid = true;
                path = asset_path;
                asset_name = Path.GetFileName(path);
                depth_level = (short)(path.Split('/').Length - 2); // -2 then Assets/asset will be 0 : root;
                icon = AssetPreview.GetMiniThumbnail(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
                is_directory = AssetDatabase.IsValidFolder(path);
                if (is_directory == true)
                {
                    _childs = new List<AssetInfo>();
                }
                _selected = false;
            }
        }

        public void AddChild(AssetInfo child)
        {
            if (child != null && is_directory == true && child != this)
            {
                _childs.Add(child);
            }
        }

        public List<AssetInfo> GetChildList()
        {
            return _childs;
        }

        public void SetSelected(bool selection_value)
        {
            if (is_directory == true)
            {
                if ( _childs != null )
                {
                    foreach ( AssetInfo child in _childs )
                    {
                        child.SetSelected(selection_value);
                    }
                }
            }
            else
                _selected = selection_value;
        }

        public void SetFolded(bool folded)
        {
            if ( is_directory == true )
            {
                is_folded = folded;
            }
        }
    }
}