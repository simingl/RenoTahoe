using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

[System.Serializable]

public class csLegacyEffectEditor : EditorWindow
{
    private float Scale = 1;

    public GameObject Effect;
    public Color ShurikenSystemColor = Color.white;
    static csLegacyEffectEditor myWindow;

	[MenuItem("Window/Legacy Effect Editor")]

	public static void Init()
	{
        myWindow = EditorWindow.GetWindowWithRect<csLegacyEffectEditor>(new Rect(100, 100, 300, 220)); //set Editor Position and Size
		myWindow.title = "Editor"; 
	}

    void OnGUI()
    {
        GUILayout.Box("Legacy Effect Scale Editor", GUILayout.Width(295));
        EditorGUILayout.Space();
        Effect = (GameObject)EditorGUILayout.ObjectField("Legacy Effect", Effect, typeof(GameObject), true); 
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Scale = float.Parse(EditorGUILayout.TextField("Scale Change Value", Scale.ToString()));
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Scale Apply", GUILayout.Height(70))) 
        {
            if (Effect.GetComponent<csLegacyEffectChanger>() != null)
                Effect.GetComponent<csLegacyEffectChanger>().LegacyEffectScaleChange(Scale);
            else
            {
                Effect.AddComponent<csLegacyEffectChanger>();
                Effect.GetComponent<csLegacyEffectChanger>().LegacyEffectScaleChange(Scale);
            }
            DestroyImmediate(Effect.GetComponent<csLegacyEffectChanger>());
        }
    }

}
#endif
