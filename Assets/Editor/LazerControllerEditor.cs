using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LazerController))]
public class LazerControllerEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        // base.OnInspectorGUI();
        DrawDefaultInspector();

        LazerController myScript = (LazerController)target;

        //player
        if(myScript.myLM == LazerMode.Move)
        {
            myScript.DoorIdx = (int) EditorGUILayout.IntField("Door Index", myScript.DoorIdx);
            myScript.LazerStop = (Transform) EditorGUILayout.ObjectField("Lazer Stop", myScript.LazerStop, typeof(Transform), true);
            myScript.Speed = (Vector3) EditorGUILayout.Vector3Field("Speed", myScript.Speed);
        }
    }
}
