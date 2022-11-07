using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


[CustomEditor(typeof(TNT))]
public class TNTEditor : Editor {
    
    private TNT instance;
    private void OnEnable() {
        instance = (TNT)target;
    }

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        if (GUILayout.Button("Pattern")) {
            PatternWindow window = EditorWindow.GetWindow<PatternWindow>();
            window.currentTNT = instance;
            window.pattern = instance.pattern;
            window.Show();
        }
       
        
    }
}
