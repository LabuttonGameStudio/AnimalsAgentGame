using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomMaterialAssign))]
public class RandomContainerGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RandomMaterialAssign mainScript = (RandomMaterialAssign)target;
        if (GUILayout.Button("Generate Random Colors"))
        {
            mainScript.SetRandomColors();
        }
    }
}
