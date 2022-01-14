using UnityEditor;
using UnityEngine;

namespace Tutorials.SphereSizeChanger.Scripts.Editor
{
	
    [CustomEditor(typeof(HeartBeat))]
    public class HeartBeatEditor : UnityEditor.Editor
    {
	    public override void OnInspectorGUI()
	    {
		    base.OnInspectorGUI();

		    HeartBeat heartBeat = (HeartBeat) target;
		    GUILayout.Label("Oscillates around a base size.");
		    heartBeat.baseSize = EditorGUILayout.Slider("BaseSize", heartBeat.baseSize, .1f, 2f);
		    heartBeat.transform.localScale = Vector3.one * heartBeat.baseSize;
	    }
    }
    
}