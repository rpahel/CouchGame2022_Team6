using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Absorption))]
public class AbsorptionEditor : Editor {

    private Absorption instance;

    private void OnEnable() {
        instance = (Absorption)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Init Mesh"))
        {
            if(instance.drawSquare)
                instance.DrawSquare();
            if (instance.drawCustomShape)
                instance.DrawCustomShape();
        }
    }
}
