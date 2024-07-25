using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//  _._    CODE BY   _._   //
//       /| ________________
// O|===|* >________________>
//       \|
//    _._     _,-'""`-._         _
//   (,-.`._,'(       |\`-/| ___/___
//       `-.-' \ )-`( , o o) |_____|
//             `-    \`_`"'-  | - |
//                            | - |
//  MILKSHAKE BATTLECAT 2024   ``` 
/// <summary>
/// Adds a button in the inspector to quickly toggle localScale of RectTransforms
/// </summary>
[CustomEditor(typeof(RectTransform))]
public class ScaleTogglerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the target object
        RectTransform rectTransform = (RectTransform)target;

        // Add a space before the custom button
        EditorGUILayout.Space();

        // Create the toggle button
        if (GUILayout.Button("Toggle Scale"))
        {
            ToggleScale(rectTransform);
        }
    }

    private void ToggleScale(RectTransform rectTransform)
    {
        if (rectTransform.localScale == Vector3.one)
        {
            rectTransform.localScale = Vector3.zero;
        }
        else
        {
            rectTransform.localScale = Vector3.one;
        }
    }
} // end ScaleTogglerEditor
//                    ^           ^
//                   /  \______ /  \
//                  /  ^          ^ \
//                 |                 |
//                |       "    "     |
//             -- |  _.--._   _.--._  | --
//       (@_  ===(= .      '        . =) ===
//  _     ) \_____\  '. :   <->  :'.  /_______________________
// (_)@8@8{}<__________|\____Y____/|___________________________>
//        )_/  milk  \       -     /  battle
//       (@   shake    `----------`    cat
