// Created by Nikolay Dyankov
// 31.05.2014
// Version 1.0

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

[InitializeOnLoad]
[ExecuteInEditMode]
public class ReorderMyComponents : EditorWindow {
	
	int activeButton;
	int tempButtonIndex;
	int lastButtonIndex;
	bool mouseDown;
	int[] newIndexes;
	
	[MenuItem("Window/Reorder Components")]
	private static void ShowWindow () {
		ReorderMyComponents windowHndle = (ReorderMyComponents)EditorWindow.GetWindow(typeof(ReorderMyComponents));
		windowHndle.autoRepaintOnSceneChange = true;
	}
	
	void OnInspectorUpdate() {
		Repaint();
    }
    Vector2 scrollPosition = Vector2.zero;
    void OnGUI () {	
        scrollPosition = GUILayout.BeginScrollView (scrollPosition, true, false);
		Transform currentTransform;
        if (Selection.GetFiltered(typeof(Transform), SelectionMode.Unfiltered) != null && Selection.GetFiltered(typeof(Transform), SelectionMode.Unfiltered).Length > 0)
        {
            currentTransform = (Transform)Selection.GetFiltered(typeof(Transform), SelectionMode.Unfiltered)[0];
        }
        else
        {
            currentTransform = Selection.activeTransform;
        }
		if (currentTransform != null) {
			Component[] comps = currentTransform.GetComponents<Component>();
			
			Texture2D buttonActiveBackground = new Texture2D(1, 1);
			buttonActiveBackground.SetPixel(0, 0, Color.gray);
			buttonActiveBackground.Apply();
			
			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.alignment = TextAnchor.MiddleLeft;
			buttonStyle.fixedHeight = 25;
			buttonStyle.margin = new RectOffset (4, 4, 4, 4);
			
			GUIStyle buttonDisabledStyle = new GUIStyle(GUI.skin.button);
			buttonDisabledStyle.alignment = TextAnchor.MiddleLeft;
			buttonDisabledStyle.fixedHeight = 25;
			buttonDisabledStyle.margin = new RectOffset (4, 4, 4, 4);
			buttonDisabledStyle.normal.textColor = Color.gray;
			
			GUIStyle buttonActiveStyle = new GUIStyle();
			buttonActiveStyle.alignment = TextAnchor.MiddleLeft;
			buttonActiveStyle.fixedHeight = 25;
			buttonActiveStyle.margin = new RectOffset (4, 4, 4, 4);
			buttonActiveStyle.normal.background = buttonActiveBackground;
            buttonActiveStyle.normal.textColor = Color.gray;
			
			// Check if mouse button gets pressed down and see which button is below mouse
			if (!mouseDown && Event.current.type == EventType.MouseDown) {
				activeButton = Mathf.FloorToInt(Event.current.mousePosition.y / 29);
				
				if (comps[activeButton].GetType().ToString() == "UnityEngine.Transform") {
					mouseDown = false;
				} else {
					mouseDown = true;
					lastButtonIndex = activeButton;
					
					newIndexes = new int[comps.Length];
					for (int i=0; i<comps.Length; i++) {
						newIndexes[i] = i;
					}
				}
			}
			
			// Mouse button released
			if (mouseDown && Event.current.type == EventType.MouseUp) {
                mouseDown = false;
                
                // Reorder components
                int positionsToMove = Mathf.RoundToInt(Mathf.Abs(tempButtonIndex - activeButton));
                
				if (positionsToMove > 0) {
                    int direction = (tempButtonIndex - activeButton) / Mathf.Abs(tempButtonIndex - activeButton);
				
					for (int i=0; i<positionsToMove; i++) {
						if (direction > 0) {
							UnityEditorInternal.ComponentUtility.MoveComponentDown(comps[activeButton + i]);
						}
						if (direction < 0) {
							UnityEditorInternal.ComponentUtility.MoveComponentUp(comps[activeButton - i]);
                        }
                        
                        comps = currentTransform.GetComponents<Component>();
					}
				}
            }
            
			// Draw buttons
            for (int i=0; i<comps.Length; i++) {
            	int j = i;
            	
            	if (mouseDown) {
            		j = newIndexes[i];
            	}
            
				string componentName = comps[j].GetType().ToString();
			
				GUIStyle style = buttonStyle;
				
				if (mouseDown && i == tempButtonIndex) { style = buttonActiveStyle; }
				if (componentName == "UnityEngine.Transform") { style = buttonDisabledStyle; }
				
				GUILayout.Button(componentName, style);
			}
			
			// If mouse button is down, draw the extra button
			if (mouseDown) {
				Rect buttonPosition = new Rect(Event.current.mousePosition.x - Screen.width/2, Event.current.mousePosition.y - 12.5f, Screen.width - 10, 25);
				GUI.Button(buttonPosition, comps[activeButton].GetType().ToString(), buttonStyle);
			}
			
			// Get index for the new temp button
			if (mouseDown) {
				int tmp = Mathf.FloorToInt(Event.current.mousePosition.y / 29);
				
				if (tmp > comps.Length - 1) {
					tmp = comps.Length - 1;
				}
				
				if (tmp < 0) {
					tmp = 0;
				}
				
				if (comps[tmp].GetType().ToString() != "UnityEngine.Transform") {
					tempButtonIndex = tmp;
				}
								
				if (tempButtonIndex != lastButtonIndex) {
					int temp = newIndexes[lastButtonIndex];
					newIndexes[lastButtonIndex] = newIndexes[tempButtonIndex];
					newIndexes[tempButtonIndex] = temp;
				
					lastButtonIndex = tempButtonIndex;
				}
            }
        }

        GUILayout.EndScrollView ();
    }
}
