//MAPNAV Navigation ToolKit v.1.3.4
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GetDistance))]
public class GetDistInspector : Editor {
	private SerializedObject getDist;
	private SerializedProperty
			waypoints,
			totalDistance,
			milesDist,
			metersDist,
			feetDist,
			renderPath,
			pathColor,
			pathWidth,
			updateRate;


	private void OnEnable(){
 		getDist = new SerializedObject(target);
		waypoints =  getDist.FindProperty ("waypoints");
		renderPath =  getDist.FindProperty ("renderPath");
		pathColor =  getDist.FindProperty ("pathColor");
		pathWidth =  getDist.FindProperty ("pathWidth");
		totalDistance = getDist.FindProperty("totalDistance");
		milesDist = getDist.FindProperty("milesDist");
		metersDist = getDist.FindProperty("metersDist");
		feetDist = getDist.FindProperty("feetDist");
		updateRate =  getDist.FindProperty ("updateRate");
	}

	public override void OnInspectorGUI () {
		getDist.Update();
		EditorGUILayout.Space ();
		EditorGUILayout.PropertyField(waypoints, true);
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField("Total Distance (Km):",totalDistance.floatValue.ToString());
		EditorGUILayout.LabelField("Total Distance (miles):",milesDist.floatValue.ToString());
		EditorGUILayout.LabelField("Total Distance (meters):",metersDist.floatValue.ToString());
		EditorGUILayout.LabelField("Total Distance (feet):",feetDist.floatValue.ToString());
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(updateRate,new GUIContent("Update/Refresh Rate (s)"));
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(renderPath,new GUIContent("Render Path"));
		if (renderPath.boolValue) {
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(pathColor,new GUIContent("Path Color"));
			EditorGUILayout.PropertyField(pathWidth,new GUIContent("Path Width"));
			EditorGUI.indentLevel--;
		}
		getDist.ApplyModifiedProperties ();
	}	
}