using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HealthManager))]
public class HealthManagerEditor : Editor 
{
    public override void OnInspectorGUI() 
    {
        // base.OnInspectorGUI();
        DrawDefaultInspector();

        HealthManager myScript = (HealthManager)target;

        //player
        if(myScript.myCharacterType == CharacterType.Player)
        {
            myScript.HealthRecover = (float) EditorGUILayout.FloatField("Health Recover", myScript.HealthRecover);
            myScript.ToxicDamage = (float) EditorGUILayout.FloatField("Toxic Damage", myScript.ToxicDamage);
        }
    }
}
