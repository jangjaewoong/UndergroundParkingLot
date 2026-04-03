using UnityEditor;
using UnityEngine;

public class MoveSelectedObjects : EditorWindow
{
    private Vector3 moveOffset = Vector3.zero;

    [MenuItem("Tools/Move Selected Objects")]
    static void ShowWindow()
    {
        GetWindow<MoveSelectedObjects>("Move Selected");
    }

    void OnGUI()
    {
        GUILayout.Label("각 오브젝트를 현재 위치에서 상대 이동", EditorStyles.boldLabel);
        moveOffset = EditorGUILayout.Vector3Field("이동 거리", moveOffset);

        if (GUILayout.Button("이동 적용"))
        {
            foreach (var obj in Selection.gameObjects)
            {
                Undo.RecordObject(obj.transform, "Move Selected Objects");
                obj.transform.position += moveOffset;
            }
        }
    }
}