using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MM.PackageExporter
{
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
            foreach (AssetInfo ai in assets)
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
                    bool selection_update = EditorGUILayout.Toggle("", group_selected, GUILayout.Width(12), GUILayout.Height(12));
                    if (group_selected != selection_update && child_list != null)
                    {
                        group.SetSelected(selection_update);
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    GUILayout.Space(19);
                    group.SetSelected(EditorGUILayout.Toggle("", group.is_selected, GUILayout.Width(12), GUILayout.Height(12)));
                }
//                GUI.DrawTexture();
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