using UnityEngine;
using UnityEditor;

namespace MM.PackageExporter
{
    public static class AssetDisplayStyles
    {
        public static GUIStyle label { get; set; }

        static AssetDisplayStyles()
        {
            // Styles ------------------------------------
            label = new GUIStyle(EditorStyles.label);
            label.alignment = TextAnchor.MiddleLeft;
            // -------------------------------------------
        }
    }
}