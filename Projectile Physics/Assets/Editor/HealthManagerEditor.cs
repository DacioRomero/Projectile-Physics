using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(HealthManager))]
public class HealthManagerEditor : Editor
{
    MonoScript script;
    void OnEnable()
    {
        script = MonoScript.FromMonoBehaviour((HealthManager)target);
    }

    public override void OnInspectorGUI()
    {
        HealthManager hmTarget = target as HealthManager;
        script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;

        hmTarget.maxHitpoints = EditorGUILayout.FloatField("Max Hitpoints", hmTarget.maxHitpoints);
        hmTarget.hitpoints = EditorGUILayout.FloatField("Hitpoints", hmTarget.hitpoints);

        if(GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
