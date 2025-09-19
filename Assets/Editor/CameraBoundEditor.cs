using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraBounds))]
public class CameraBoundsEditor : Editor
{
    private CameraBounds cb;
    private bool addingLine;
    private Vector2 pendingStart;

    private void OnEnable()
    {
        cb = (CameraBounds)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Hold Ctrl (Cmd on Mac) and click in the Scene view to add lines.", MessageType.Info);

        if (GUILayout.Button("Clear All Lines"))
        {
            Undo.RecordObject(cb, "Clear Camera Bounds");
            cb.lines = new CameraBounds.Line[0];
            EditorUtility.SetDirty(cb);
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        if (cb.lines == null) return;

        Handles.color = Color.yellow;

        // Draw and move existing lines
        for (int i = 0; i < cb.lines.Length; i++)
        {
            var line = cb.lines[i];
            Handles.DrawLine(line.start, line.end);

            EditorGUI.BeginChangeCheck();
            Vector3 newStart = Handles.PositionHandle(line.start, Quaternion.identity);
            Vector3 newEnd = Handles.PositionHandle(line.end, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(cb, "Move Bound Point");
                line.start = newStart;
                line.end = newEnd;
                cb.lines[i] = line;
                EditorUtility.SetDirty(cb);
                SceneView.RepaintAll();
            }
        }

        HandleSceneClick();
    }

    private void HandleSceneClick()
    {
        Event e = Event.current;

        // Only respond if Ctrl or Cmd is held
        if ((e.control || e.command) && e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 clickPos = ray.origin;

            if (!addingLine)
            {
                // first point
                pendingStart = clickPos;
                addingLine = true;
            }
            else
            {
                // second point, create line
                AddLine(pendingStart, clickPos);
                addingLine = false;
            }

            e.Use();
        }

        // Draw a preview from pendingStart to mouse position if we're in the middle of a line
        if (addingLine)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 currentMouse = ray.origin;
            Handles.color = Color.green;
            Handles.DrawLine(pendingStart, currentMouse);
            SceneView.RepaintAll();
        }
    }

    private void AddLine(Vector2 start, Vector2 end)
    {
        Undo.RecordObject(cb, "Add Camera Bound Line");
        var newLines = new CameraBounds.Line[(cb.lines?.Length ?? 0) + 1];
        if (cb.lines != null) cb.lines.CopyTo(newLines, 0);
        newLines[newLines.Length - 1] = new CameraBounds.Line { start = start, end = end };
        cb.lines = newLines;
        EditorUtility.SetDirty(cb);
        SceneView.RepaintAll();
    }
}
