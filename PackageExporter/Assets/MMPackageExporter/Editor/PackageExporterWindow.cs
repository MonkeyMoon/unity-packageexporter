using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace MM.PackageExporter
{
    /// <summary>
    /// Displays the Unity Editor Window.
    /// Main entry point.
    /// </summary>
    public class PackageExporterWindow : EditorWindow
    {
        private string _plugin_path;
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

        /// <summary>
        /// Define where the Package Exporter plugin is located in the AssetDataBase and set
        /// _plugin_path variable.
        /// Used to define where the Package Configuration files will be saved.
        /// </summary>
        /// <returns>Path to Plugin folder (relative to Assets), null otherwise.</returns>
        private string FindPluginPath()
        {
            string[] assets = AssetDatabase.GetAllAssetPaths();
            foreach (string path in assets)
            {
                // Find this specific file as reference.
                if ( Path.GetFileName(path) == "PackageExporterWindow.cs" )
                {
                    // Directory.GetParent * 2 because Assets/MMPackageExporter is changed into E:\WORK\package-exporter\PackageExporter\Assets\MMPackageExporter 
                    // in case of .Parent
                    // Yup, that's weird 😒
                    return Directory.GetParent(Directory.GetParent(path).ToString()).ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// Refresh the whole content.
        /// Called in case of asset modification to always be up to date.
        /// </summary>
        public void RefreshContent()
        {
            // Look for plugin path and set it
            _plugin_path = FindPluginPath();
            _path_holder = new AssetInfoHolder();
            // Retrieve existing PackageConfigurationSave assets
            _saves = new List<PackageConfigurationSave>(Resources.FindObjectsOfTypeAll<PackageConfigurationSave>());
            for ( int i = _saves.Count - 1; i >= 0; --i )
            {
                if ( _saves[i].name == "" )
                {
                    _saves.RemoveAt(i);
                }
            }

            // Create the strings used in save selector (popup)
            _save_names = new List<string>();
            _save_names.Add("-- none --");
            foreach(PackageConfigurationSave pcs in _saves)
            {
                _save_names.Add(pcs.name);
            }
            SelectSaveByID(_selected_save_id);
        }

        /// <summary>
        /// Export the selected assets into a package.
        /// Asks the user to specify the package path before saving it.
        /// </summary>
        private void ExportPackage()
        {
            // Asks user for the package path
            string save_path = EditorUtility.SaveFilePanel("Export package", "", "", "unitypackage");
            if (string.IsNullOrEmpty(save_path) == false)
            {
                AssetDatabase.ExportPackage(_path_holder.GetSelectedAssetPaths(), save_path);
                // TODO: Open window to freshly created package.
            }
        }

        /// <summary>
        /// Save the selected assets into a PackageConfigurationSave file.
        /// Uses _save_name variable as name for the PackageConfiguration.
        /// </summary>
        /// <seealso cref="_save_name"/>
        private void SaveConfiguration()
        {
            if (string.IsNullOrEmpty(_save_name) == false)
            {
                // Create a new instance of PackageConfigurationSave
                PackageConfigurationSave save = ScriptableObject.CreateInstance<PackageConfigurationSave>();
                string asset_path = _plugin_path;
                // If plugin path is not set save configurations into Assets/PackageConfigurations
                if ( string.IsNullOrEmpty(asset_path) == true )
                {
                    asset_path = "Assets";
                }
                asset_path += "/PackageConfigurations";
                // Create folder if needed
                if (AssetDatabase.IsValidFolder(asset_path) == false )
                {
                    AssetDatabase.CreateFolder(Directory.GetParent(asset_path).ToString(), "PackageConfigurations");
                }

                AssetDatabase.CreateAsset(save, asset_path + "/" + _save_name + ".asset");
                save.SavePackageConfiguration(_path_holder);
                AssetDatabase.SaveAssets();

                RefreshContent();
                // Select freshly saved configuration
                SelectSaveByName(save.name);
            }
            else
            {
                Debug.LogWarning("Invalid file name: configuration has not been saved :S.");
            }
        }

        /// <summary>
        /// Selects a PackageConfigurationSave by its name.
        /// </summary>
        /// <param name="name">Name of the configuration to select</param>
        /// <seealso cref="SelectSaveByID(int)"/>
        private void SelectSaveByName(string name)
        {
            for ( int i = 0; i < _saves.Count; ++i )
            {
                if ( _saves[i].name.ToLower() == name.ToLower() )
                {
                    SelectSaveByID(i + 1); // +1 because ID 0 is None.
                    return;
                }
            }
        }

        /// <summary>
        /// Selects a PackageConfigurationSave by its ID.
        /// </summary>
        /// <param name="id"></param>
        private void SelectSaveByID(int id)
        {
            id = id < 0 ? 0 : id > _saves.Count ? _saves.Count : id;
            _selected_save_id = id;
            if (_selected_save_id != 0)
            {
                _path_holder.LoadSave(_saves[_selected_save_id - 1]);
                _save_name = _saves[_selected_save_id - 1].name;
            }
            else
            {
                // ID 0 is None
                // Unselect everything
                _path_holder.ClearSelection();
            }
        }

        private void OnGUI()
        {
            using (var verticalScope = new GUILayout.VerticalScope("box"))
            {
                // Save selection group ----------------------
                // If saves exist, show selection popup
                if (_save_names != null && _save_names.Count != 0)
                {
                    int new_selected_save_id = EditorGUILayout.Popup(_selected_save_id, _save_names.ToArray());
                    if ( new_selected_save_id != _selected_save_id )
                    {
                        SelectSaveByID(new_selected_save_id);
                    }
                }
                // --------------------------------------------

                // Save action group --------------------------
                using (var horizontalScope = new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("File name: ", GUILayout.Width(65));
                    _save_name = GUILayout.TextField(_save_name, GUILayout.ExpandWidth(true));
                    if ( string.IsNullOrEmpty(_save_name) == false && GUILayout.Button("save", GUILayout.Width(80)) )
                    {
                        SaveConfiguration();
                    }
                }
                // --------------------------------------------
            }

            DisplayAssets();

            //if (GUILayout.Button("REFRESH") == true)
            //{
            //    RefreshContent();
            //}
            using (var verticalScope = new GUILayout.VerticalScope("box"))
            {
                using (var horizontalScope = new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Select All") == true)
                    {
                        _path_holder.SelectAll();
                    }
                    if (GUILayout.Button("Unselect All") == true)
                    {
                        _path_holder.ClearSelection();
                    }
                }

                if (GUILayout.Button("EXPORT PACKAGE") == true)
                {
                    ExportPackage();
                }
            }
        }

        /// <summary>
        /// Displays the asset list with Toggles and File icons.
        /// </summary>
        private void DisplayAssets()
        {
            if (_path_holder == null)
                RefreshContent();

            List<AssetInfo> assets = _path_holder.GetAssetInfos();
            using (var scrollViewScope = new GUILayout.ScrollViewScope(_scroll_position, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                _scroll_position = scrollViewScope.scrollPosition;
                GUILayout.BeginVertical();
                foreach (AssetInfo ai in assets)
                {
                    // Display only level 0 assets (DisplayAssetGroup will be in charge of displaying childs).
                    if (ai.depth_level == 0) 
                    {
                        // Display specific asset and all its content.
                        DisplayAssetGroup(ai);
                    }
                }
                GUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Display a specific asset and its childs.
        /// </summary>
        /// <param name="group">AssetInfo to display.</param>
        /// <seealso cref="AssetInfo"/>
        private void DisplayAssetGroup(AssetInfo group)
        {
            List<AssetInfo> child_list = group.GetChildList();
            using (var horizontalScope = new GUILayout.HorizontalScope())
            {
                GUILayout.Space(group.depth_level * 20 + 5);
                // Display childs if current asset is a Directory.
                if (group.is_directory == true)
                {
                    // Foldout group
                    Rect r = GUILayoutUtility.GetRect(new GUIContent("►"), EditorStyles.label, GUILayout.ExpandWidth(false));
                    group.SetFolded(!EditorGUI.Foldout(r, !group.is_folded, ""));
                    // Mixed Value
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
                    // If asset is not a Directory, add space to compensate and display a simple Toggle.
                    GUILayout.Space(19);
                    group.SetSelected(EditorGUILayout.Toggle("", group.is_selected, GUILayout.Width(12)));
                }
                GUILayout.Label(group.icon, AssetDisplayStyles.icon, GUILayout.Width(18), GUILayout.MaxHeight(18));
                GUILayout.Label(group.asset_name, AssetDisplayStyles.label);
            }

            // Only display childs on Directories that are not folded.
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