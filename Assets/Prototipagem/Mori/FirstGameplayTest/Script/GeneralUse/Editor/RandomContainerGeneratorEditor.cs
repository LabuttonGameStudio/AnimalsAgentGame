using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomContainerGenerator))]
public class RandomContainerGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RandomContainerGenerator mainScript = (RandomContainerGenerator)target;
        if (GUILayout.Button("Generate Random Colors"))
        {
            mainScript.SetRandomColors();
        }
    }
}
