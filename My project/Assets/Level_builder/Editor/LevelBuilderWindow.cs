using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LevelBuilderWindow : EditorWindow
{
    private float gridSize = 1f;
    private bool builderEnabled = false;
    private GameObject selectedPrefab;
    private GameObject selectedPrefabPreview;
    [SerializeField] private GameObject prefab;
    [MenuItem("Tools/Level Builder")]
    public static void ShowWindow()
    {
        GetWindow<LevelBuilderWindow>("Level Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Builder", EditorStyles.boldLabel);

        GUILayout.Space(10);

        gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);

        GUILayout.Space(10);

        builderEnabled = EditorGUILayout.Toggle("Enable Builder", builderEnabled);
        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);
        if (GUI.changed)
        {
            SceneView.RepaintAll();
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!builderEnabled)
            return;

        DrawGrid();
        GridMouseIntereaction();
    }

    private void DrawGrid()
    {
        Handles.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        int gridRange = 20;

        for (int x = -gridRange; x <= gridRange; x++)
        {
            Vector3 start = new Vector3(x * gridSize, 0, -gridRange * gridSize);
            Vector3 end = new Vector3(x * gridSize, 0, gridRange * gridSize);

            Handles.DrawLine(start, end);
        }

        for (int z = -gridRange; z <= gridRange; z++)
        {
            Vector3 start = new Vector3(-gridRange * gridSize, 0, z * gridSize);
            Vector3 end = new Vector3(gridRange * gridSize, 0, z * gridSize);

            Handles.DrawLine(start, end);
        }
    }
    private void GridMouseIntereaction()
    {
        Event current = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        if(groundPlane.Raycast(ray, out float center))
        {
            Vector3 hitPoint = ray.GetPoint(center);
            float cellX = Mathf.Floor(hitPoint.x / gridSize) * gridSize + (gridSize * 0.5f);
            float cellZ = Mathf.Floor(hitPoint.z / gridSize) * gridSize + (gridSize * 0.5f);
            Vector3 cellCenter = new Vector3(cellX, 0, cellZ);
            Handles.color = Color.yellow;
            Handles.DrawWireCube(cellCenter, new Vector3(gridSize, 0.01f, gridSize));
            if (selectedPrefabPreview == null && selectedPrefab != null)
            {
                selectedPrefabPreview = PrefabUtility.InstantiatePrefab(selectedPrefab) as GameObject;
            }

            if (selectedPrefabPreview != null)
            {
                selectedPrefabPreview.transform.position = cellCenter;
            }
        }
    }
}