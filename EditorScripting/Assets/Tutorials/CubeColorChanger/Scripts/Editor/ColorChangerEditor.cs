using UnityEditor;
using UnityEngine;

namespace Tutorials.CubeColorChanger.Scripts.Editor
{
    [CustomEditor(typeof(ColorChanger))]
    public class ColorChangerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var colorChanger = (ColorChanger) target;

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Generate Color"))
                {
                    colorChanger.GenerateColor();
                }

                if (GUILayout.Button("Reset Color"))
                {
                    colorChanger.Reset();
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
