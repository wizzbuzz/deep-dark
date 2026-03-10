using UnityEngine;
using UnityEditor;

    // MenuItems add items to the context menu in the editor. These are then bound to a keyboard shortcut, & standing for the ALT key
    // Lastly they are bound to a function.  

public class MoveTools : MonoBehaviour
{
        // Move X pos with ALT-W
    [MenuItem("Tools/Move X 2 &w", true)]
    private static bool ValidateMoveX() => Selection.activeGameObject != null;

    [MenuItem("Tools/Move X 2 &w")]
    private static void MoveX()
    {
        if (Selection.activeGameObject != null)
            Selection.activeGameObject.transform.Translate(Vector3.right * 2, Space.World);
    }

    // Move X neg with ALT-W
    [MenuItem("Tools/Move X -2 &s", true)]
    private static bool ValidateMoveXNeg() => Selection.activeGameObject != null;

    [MenuItem("Tools/Move X -2 &s")]
    private static void MoveXNeg()
    {
        if (Selection.activeGameObject != null)
            Selection.activeGameObject.transform.Translate(Vector3.left * 2, Space.World);
    }

    // Move Z pos with ALT-D
    [MenuItem("Tools/Move Z 2 &d", true)]
    private static bool ValidateMoveZ() => Selection.activeGameObject != null;

    [MenuItem("Tools/Move Z 2 &d")]
    private static void MoveZ()
    {
        if (Selection.activeGameObject != null)
            Selection.activeGameObject.transform.Translate(Vector3.back * 2, Space.World);
    }

    // Move Z neg with ALT-A
    [MenuItem("Tools/Move Z -2 &a", true)]
    private static bool ValidateMoveZNeg() => Selection.activeGameObject != null;

    [MenuItem("Tools/Move Z -2 &a")]
    private static void MoveZNeg()
    {
        if (Selection.activeGameObject != null)
            Selection.activeGameObject.transform.Translate(Vector3.forward * 2, Space.World);
    }
}

public class RotateTools : MonoBehaviour
{
    
    // Rotate Y with ALT-Z
    [MenuItem("Tools/Rotate Y 90° &x", true)]
    private static bool ValidateRotateZ() => Selection.activeGameObject != null;

    [MenuItem("Tools/Rotate Y 90° &x")]
    private static void RotateZ()
    {
        if (Selection.activeGameObject != null)
            Selection.activeGameObject.transform.Rotate(Vector3.forward, 90f, Space.Self);
    }
}

public class PrefabReplacer : EditorWindow
{
    GameObject prefabToUse;
    static GameObject selectedPrefab;

    [MenuItem("Tools/Open Prefab Picker &q")]
    static void OpenPicker() => GetWindow<PrefabReplacer>("Prefab Picker");

    void OnGUI()
    {
        prefabToUse = (GameObject)EditorGUILayout.ObjectField("Select Prefab", prefabToUse, typeof(GameObject), false);
        
        if (GUILayout.Button("Replace Selected") && Selection.activeGameObject != null && prefabToUse != null)
        {
            GameObject selected = Selection.activeGameObject;
            Transform parent = selected.transform.parent;
            // Copy transform properties (same as before)
            Vector3 pos = selected.transform.localPosition;
            Quaternion rot = selected.transform.localRotation;
            Vector3 scale = selected.transform.localScale;

            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToUse, parent);
            newObj.transform.SetLocalPositionAndRotation(pos, rot);
            newObj.transform.localScale = scale;

            Undo.DestroyObjectImmediate(selected);
            Selection.activeGameObject = newObj;
            
            selectedPrefab = prefabToUse;  // Remember last used
        }
        
        if (selectedPrefab != null)
            EditorGUILayout.LabelField("Last used: " + selectedPrefab.name);
    }
}


public class MapTools : MonoBehaviour
{
    



}
