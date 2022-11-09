using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class PatternWindow : EditorWindow {

    public Vector2[] pattern = new Vector2[9];
    public Cube_TNT currentTNT;
    
    private VisualElement root;

    private bool isUpdated;
    
    [MenuItem("Tools/Pattern")]
    public static void ShowWindow() {
        PatternWindow wnd = GetWindow<PatternWindow>();
        wnd.titleContent = new GUIContent("PatternWindow");
        wnd.maxSize = new Vector2(350, 500);
        wnd.minSize = wnd.maxSize;
    }

    public void CreateGUI() {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Amaury/PatternWindow/PatternWindow.uxml");
        root = visualTree.Instantiate();
        
        rootVisualElement.Add(root);
        
        for (int i = 0; i < root.Children().ToList()[1].childCount; i++) {
            if (i != 4) {
                Button currentButton = (Button)root.Children().ToList()[1].Children().ToList()[i].Children().ToList()[0];
                currentButton.parent.style.backgroundColor = new Color(0,255,0,0f);

                currentButton.clicked += () => { 
                    currentButton.parent.style.backgroundColor = currentButton.parent.style.backgroundColor.value.a == 0
                        ? new Color(0, 255, 0, 1)
                        : new Color(0, 255, 0, 0);
                
                };

                if (i < pattern.Length) {
                    if (pattern[i] != Vector2.zero) 
                        currentButton.parent.style.backgroundColor = new Color(0, 255, 0, 1f);
                }
            }
        }

        ((Button)root.Children().ToList()[0]).clicked += SavePattern;
    }

    private void Update() {
        if (!isUpdated) {

            for (int i = 0; i < root.Children().ToList()[1].childCount; i++) {
                if (i != 4 && i < pattern.Length) {
                    Button currentButton = (Button)root.Children().ToList()[1].Children().ToList()[i].Children().ToList()[0];

                    if (pattern[i] != Vector2.zero)
                        currentButton.parent.style.backgroundColor = new Color(0, 255, 0, 1f);
                }

                isUpdated = true;
            }
        }
    }


    private void SavePattern() {
        pattern = new Vector2[9];
        
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                Button currentButton = (Button)root.Children().ToList()[1].Children().ToList()[j + 3 * i].Children().ToList()[0];
                pattern[j + 3 * i] = currentButton.parent.style.backgroundColor.value.a == 0 ? Vector2.zero : CalculateVector(j + 3 * i);
            }
        }

        currentTNT.pattern = pattern;
        
        PrefabUtility.SavePrefabAsset(currentTNT.gameObject,out bool result);
    }

    private Vector2 CalculateVector(int position) {

        switch (position) {
            case 0:
                return new Vector2(-1,1);
            
            case 1:
                return new Vector2(0,1);
            
            case 2:
                return new Vector2(1, 1);

            case 3:
                return new Vector2(-1,0);
            
            case 4:
                return Vector2.zero;

            case 5:
                return new Vector2(1,0);
            
            case 6:
                return new Vector2(-1,-1);
            
            case 7:
                return new Vector2(0,-1);
            
            case 8:
                return new Vector2(1,-1);
            
            default:
                return Vector2.zero;
        }
    }

}