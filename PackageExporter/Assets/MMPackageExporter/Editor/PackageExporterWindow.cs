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
            List<AssetInfo> child_list = group.GetChildList();
            GUILayout.BeginHorizontal();
            GUILayout.Space(group.depth_level * 20 + 5);
            if (group.is_directory == true)
            {
                EditorGUI.showMixedValue = group.is_mixed_selection;
                bool group_selected = group.is_selected;
                bool selection_update = EditorGUILayout.ToggleLeft(group.asset_name, group_selected);
                if ( group_selected != selection_update && child_list != null )
                {
                    group.SetSelected(selection_update);
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                group.SetSelected(EditorGUILayout.ToggleLeft(group.asset_name, group.is_selected));
            }
            GUILayout.EndHorizontal();

            if (group.is_directory == true)
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