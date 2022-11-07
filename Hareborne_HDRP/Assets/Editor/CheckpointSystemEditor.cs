//Authored By Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(CheckpointSystem))]
public class CheckpointSystemEditor : Editor
{
    ///<summary>
    /// Creates buttons in the editor to create the beginning and end of the course,
    ///</summary>

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CheckpointSystem thisObject = (CheckpointSystem)target;

        GUILayout.Label("Level Creation Buttons", EditorStyles.boldLabel);        

        if (GUILayout.Button("Create Level Start"))
        {
            thisObject.CreateStart();
        }
        if (GUILayout.Button("Create Level End"))
        {
            thisObject.CreateEnd();
        }
        if (GUILayout.Button("Create New Checkpoint"))
        {
            thisObject.CreateNewCheckpoint();
        }


        GUILayout.Label("Level Removal Buttons", EditorStyles.boldLabel);
        if (GUILayout.Button("Remove Level Start"))
        {
            thisObject.RemoveStart();
        }
        if (GUILayout.Button("Remove Level End"))
        {
            thisObject.RemoveEnd();
        }

        if (GUILayout.Button("Remove Checkpoint From Start"))
        {
            thisObject.RemoveCheckpointFromStart();
        }

        if (GUILayout.Button("Remove Checkpoint From End"))
        {
            thisObject.RemoveCheckpointFromEnd();
        }

        if (GUILayout.Button("Clear Checkpoints"))
        {
            thisObject.ClearCheckpoints();
        }
    }
}
