using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour {
    [HideInInspector]
    [SerializeField] List<Waypoint> path;
    [SerializeField] bool legacy;
    public GameObject prefab;
    int index = 0;
    bool forwards;

    public List<GameObject> Points;

    public List<Waypoint> GetPath() {
        if (path == null) path = new List<Waypoint>();
        return path;
    }

    public void CreatePoint() {
        Waypoint point = new Waypoint();
        path.Add(point);
    }

    public Waypoint GetNext() {
        if (legacy) {
            index = (index + 1) % path.Count;
            return path[index];
        } else {
            if (index + 1 == path.Count) forwards = false;
            if (index - 1 < 0) forwards = true;
            if (forwards) {
                index = (index + 1);
                return path[index];
            } else {
                index = (index - 1);
                return path[index];
            }
        }
        
    }

    private void Start() {
        Points = new List<GameObject>();
        forwards = true;
        int index = 0;
        foreach(Waypoint point in path) {
            GameObject go = Instantiate(prefab);
            go.transform.position = point.Position;
            Points.Add(go);
            go.name = $"{index}";
            point.Index = index;
            index++;
        }
    }

    private void Update() {
        for (int i = 0; i < path.Count; i++) {
            Waypoint point = path[i];
            GameObject go = Points[i];
            go.transform.position = point.Position;
        }
    }
}
