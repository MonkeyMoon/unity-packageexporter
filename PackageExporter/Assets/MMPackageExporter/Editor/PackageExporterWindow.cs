﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MM.PackageExporter
{
    public class PackageExporterWindow : EditorWindow
    {
        private AssetInfoHolder _path_holder;
        private Vector2 _scroll_position;
        private List<PackageConfigurationSave> _saves;
        private List<string> _save_names;
        private int _selected_save_id;
        private string _save_name;

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
            _saves = new List<PackageConfigurationSave>(Resources.FindObjectsOfTypeAll<PackageConfigurationSave>());
            for ( int i = _saves.Count - 1; i >= 0; --i )
            {
                if ( _saves[i].name == "" )
                {
                    _saves.RemoveAt(i);
                }
            }
            _save_names = new List<string>();
            foreach(PackageConfigurationSave pcs in _saves)
            {
                _save_names.Add(pcs.name);
            }
        }

        private void ExportPackage()
        {
            PackageConfigurationSave save = ScriptableObject.CreateInstance<PackageConfigurationSave>();
            AssetDatabase.CreateAsset(save, "Assets/" + _save_name + ".asset");
            save.SavePackageConfiguration(_path_holder);
            AssetDatabase.SaveAssets();
        }

        private void OnGUI()
        {
            using (var verticalScope = new GUILayout.VerticalScope("box"))
            {
                if (_save_names.Count != 0)
                {
                    int new_selected_save_id = EditorGUILayout.Popup(_selected_save_id, _save_names.ToArray());
                    if ( new_selected_save_id != _selected_save_id )
                    {
                        _selected_save_id = new_selected_save_id;
                        _path_holder.LoadSave(_saves[_selected_save_id]);
                        _save_name = _saves[_selected_save_id].name;
                    }
                }

                using (var horizontalScope = new GUILayout.HorizontalScope())
                {
                    _save_name = GUILayout.TextField(_save_name);
                    if ( string.IsNullOrEmpty(_save_name) == false && GUILayout.Button("save", GUILayout.Width(80)) )
                    {
                        ExportPackage();
                    }
                }
            }
            DisplayAssets();
            if (GUILayout.Button("REFRESH") == true)
            {
                RefreshContent();
            }
            if (GUILayout.Button("EXPORT PACKAGE") == true)
            {
                string save_path = EditorUtility.SaveFilePanel("Export package", "", "", "unitypackage");
                if (string.IsNullOrEmpty(save_path) == false)
                {
                    AssetDatabase.ExportPackage(_path_holder.GetSelectedAssetPaths(), save_path);
                    // TODO: Open window to freshly created package.
                }
            }
        }

        private void DisplayAssets()
        {
            if (_path_holder == null)
                RefreshContent();

            List<AssetInfo> assets = _path_holder.GetAssets();
            using (var scrollViewScope = new GUILayout.ScrollViewScope(_scroll_position, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                _scroll_position = scrollViewScope.scrollPosition;
                GUILayout.BeginVertical();
                foreach (AssetInfo ai in assets)
                {
                    if (ai.depth_level == 0)
                    {
                        DisplayAssetGroup(ai);
                    }
                }
                GUILayout.EndVertical();
            }
        }

        private void DisplayAssetGroup(AssetInfo group)
        {
            GUIStyle icon_style = new GUIStyle(EditorStyles.label);
            icon_style.padding = new RectOffset(1, 1, 1, 1);
            icon_style.margin = new RectOffset(0, 0, 2, 2);
            GUIStyle label_style = new GUIStyle(icon_style);
            label_style.margin = new RectOffset(0, 0, 4, 2);

            List<AssetInfo> child_list = group.GetChildList();
            using (var horizontalScope = new GUILayout.HorizontalScope())
            {
                GUILayout.Space(group.depth_level * 20 + 5);
                if (group.is_directory == true)
                {
                    Rect r = GUILayoutUtility.GetRect(new GUIContent("►"), EditorStyles.label, GUILayout.ExpandWidth(false));
                    group.SetFolded(!EditorGUI.Foldout(r, !group.is_folded, ""));
                    EditorGUI.showMixedValue = group.is_mixed_selection;
                    bool group_selected = group.is_selected;
                    bool selection_update = EditorGUILayout.Toggle("", group_selected, GUILayout.Width(12));
                    if (group_selected != selection_update && child_list != null)
                    {
                        group.SetSelected(selection_update);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    GUILayout.Space(19);
                    group.SetSelected(EditorGUILayout.Toggle("", group.is_selected, GUILayout.Width(12)));
                }
                GUILayout.Label(group.icon, icon_style, GUILayout.Width(18), GUILayout.MaxHeight(18));
                GUILayout.Label(group.asset_name, label_style);
            }

            if (group.is_directory == true && group.is_folded == false)
            {
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