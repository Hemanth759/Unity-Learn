using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tutorials.CustomWindows.Scripts.Editor
{
    public class ColorChangerWindow : EditorWindow
    {
        private Color _color;
        
        [MenuItem("Window/ColorChanger")]
        private static void ShowWindow()
        {
            var window = GetWindow<ColorChangerWindow>();
            window.titleContent = new GUIContent("ColorChanger");
            window.Show();
        }

        private void OnGUI()
        {
            _color = EditorGUILayout.ColorField("Color", _color);
            if (GUILayout.Button("Colorize!"))
            {
                Selection.gameObjects.Select(o => o.GetComponent<Renderer>()).Where(renderer => renderer != null)
                    .ToList().ForEach(renderer => renderer.sharedMaterial.color = _color);
            }
        }
    }
}