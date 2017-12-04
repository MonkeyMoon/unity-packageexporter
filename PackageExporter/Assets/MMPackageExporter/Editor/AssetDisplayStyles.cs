using UnityEngine;
using UnityEditor;

namespace MM.PackageExporter
{
    public static class AssetDisplayStyles
    {
        public static GUIStyle icon { get; set; }
        public static GUIStyle label { get; set; }

        static AssetDisplayStyles()
        {
            // Styles ------------------------------------
            icon = new GUIStyle(EditorStyles.label);
            icon.padding = new RectOffset(1, 1, 1, 1);
            icon.margin = new RectOffset(0, 0, 2, 2);
            label = new GUIStyle(icon);
            label.margin = new RectOffset(0, 0, 4, 2);
            // -------------------------------------------
        }
    }
}