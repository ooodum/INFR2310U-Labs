using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathManager))]
public class PathManagerEditor : Editor {
    [SerializeField] PathManager pathManager;
    [SerializeField] List<Waypoint> path;

    List<int> toDelete;
    Waypoint selectedPoint = null;
    bool shouldRepaint = true;

    private void OnSceneGUI() {
        path = pathManager.GetPath();
        DrawPath(path);
    }

    private void OnEnable() {
        pathManager = target as PathManager;
        toDelete = new List<int>();
    }

    public override void OnInspectorGUI() {
        this.serializedObject.Update();
        path = pathManager.GetPath();

        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Path");

        DrawPointGUI();

        if (GUILayout.Button("Add point to path")) pathManager.CreatePoint();

        EditorGUILayout.EndVertical();
        SceneView.RepaintAll();
    }

    void DrawPointGUI() {
        if (path != null && path.Count > 0) {
            for (int i = 0; i < path.Count; i++) {
                EditorGUILayout.BeginHorizontal();
                Waypoint point = path[i];

                Color color = GUI.color;
                if (selectedPoint == point) GUI.color = Color.green;

                Vector3 oldPoint = point.Position;
                Vector3 newPoint = EditorGUILayout.Vector3Field("", oldPoint);

                if (EditorGUI.EndChangeCheck()) point.Position = newPoint;

                if (GUILayout.Button("-", GUILayout.Width(25))) toDelete.Add(i);

                GUI.color = color;
                EditorGUILayout.EndHorizontal();
            }
        }
        if (toDelete.Count > 0) {
            foreach (int i in toDelete) path.RemoveAt(i);
            toDelete.Clear();
        }
    }

    public void DrawPath(List<Waypoint> path) {
        if (path == null) return;
        int index = 0;
        foreach (Waypoint point in path) {
            shouldRepaint = DrawPoint(point);
            int next = (index + 1) % path.Count;
            Waypoint nextPoint = path[next];

            DrawPathLine(point, nextPoint);
            index++;
        }
        if (shouldRepaint) Repaint();
    }

    public void DrawPathLine(Waypoint p1, Waypoint p2) {
        Color color = Handles.color;
        Handles.color = Color.grey;
        Handles.DrawLine(p1.Position, p2.Position);
        Handles.color = color;
    }

    public bool DrawPoint(Waypoint point) {
        bool isChanged = false;
        if (selectedPoint == point) {
            Color color = Handles.color;
            Handles.color = Color.green;

            EditorGUI.BeginChangeCheck();

            Vector3 oldPos = point.Position;
            Vector3 newPos = Handles.PositionHandle(oldPos, Quaternion.identity);

            float handleSize = HandleUtility.GetHandleSize(newPos);
            Handles.SphereHandleCap(-1, newPos, Quaternion.identity, .25f * handleSize, EventType.Repaint);
            if (EditorGUI.EndChangeCheck()) point.Position = newPos;
            Handles.color = color;
        }
        else {
            Vector3 currentPosition = point.Position;
            float handleSize = HandleUtility.GetHandleSize(currentPosition);
            if (Handles.Button(currentPosition, Quaternion.identity, .25f * handleSize, .25f * handleSize, Handles.SphereHandleCap)) {
                isChanged = true;
                selectedPoint = point;
            }
        }
        
        return isChanged;
    }
}