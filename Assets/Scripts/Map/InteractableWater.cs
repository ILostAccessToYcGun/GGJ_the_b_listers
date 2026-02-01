using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(EdgeCollider2D))]
[RequireComponent(typeof(WaterTriggerHandler))]
public class InteractableWater : MonoBehaviour
{
    [Header("Mesh Generation")]
    [Range(2, 500)] public int numOfXVertices = 70;
    public float width = 10f;
    public float height = 4f;
    public Material waterMaterial;
    private const int numOfYVertices = 2;

    [Header("Gizmo")]
    public Color gizmoColor = Color.white;

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Vector3[] verticies;
    private int[] topVerticesIndex;

    private EdgeCollider2D edgeCollider;

    private void Start()
    {
        GenerateMesh();
    }

    private void Reset()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        edgeCollider.isTrigger = true;
    }

    public void ResetEdgeCollider()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();

        Vector2[] newPoints = new Vector2[2];

        Vector2 firstPoint = new Vector2(verticies[topVerticesIndex[0]].x, verticies[topVerticesIndex[0]].y);
        newPoints[0] = firstPoint;

        Vector2 secondPoint = new Vector2(verticies[topVerticesIndex[topVerticesIndex.Length - 1]].x, verticies[topVerticesIndex[topVerticesIndex.Length - 1]].y);
        newPoints[1] = secondPoint;

        edgeCollider.offset = Vector2.zero;
        edgeCollider.points = newPoints;
    }

    public void GenerateMesh()
    {
        mesh = new Mesh();

        //This adds the verticies
        verticies = new Vector3[numOfXVertices * numOfYVertices];
        topVerticesIndex = new int[numOfXVertices];
        for (int y = 0; y < numOfYVertices; y++)
        {
            for (int x = 0; x < numOfXVertices; x++)
            {
                float xPos = (x / (float)(numOfXVertices - 1)) * width - width / 2;
                float yPos = (y / (float)(numOfYVertices - 1)) * height - height / 2;
                verticies[y * numOfXVertices + x] = new Vector3(xPos, yPos, 0f);

                if (y == numOfYVertices - 1)
                {
                    topVerticesIndex[x] = y * numOfXVertices + x;
                }
            }
        }

        //This makes the triangles
        int[] triangles = new int[(numOfXVertices - 1) * (numOfYVertices - 1) * 6];
        int index = 0;

        for (int y = 0; y < numOfYVertices - 1; y++)
        {
            for (int x = 0; x < numOfXVertices - 1; x++)
            {
                int bottomLeft = y * numOfXVertices + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + numOfXVertices;
                int topRight = topLeft + 1;

                //first triangle
                triangles[index++] = bottomLeft;
                triangles[index++] = topLeft;
                triangles[index++] = bottomRight;

                //second triangle
                triangles[index++] = bottomRight;
                triangles[index++] = topLeft;
                triangles[index++] = topRight;
            }
        }

        //UV time
        Vector2[] uvs = new Vector2[verticies.Length];
        for (int i = 0; i < verticies.Length; i++)
        {
            uvs[i] = new Vector2((verticies[i].x + width / 2) / width, (verticies[i].y + height / 2) / height);
        }

        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        meshRenderer.material = waterMaterial;
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        //these two are not needed but for later if I want to mess with them
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

    }
}

[CustomEditor(typeof(InteractableWater))]
public class InteractableWaterEditor : Editor
{
    private InteractableWater water;

    private void OnEnable()
    {
        water = (InteractableWater)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        root.Add(new VisualElement { style = { height = 10 } });

        Button generateMeshButton = new Button(() => water.GenerateMesh())
        {
            text = "Generate Mesh"
        };

        root.Add(generateMeshButton);

        Button placeEdgeColliderButton = new Button(() => water.ResetEdgeCollider())
        {
            text = "Place Edge Collider"
        };

        root.Add(placeEdgeColliderButton);

        return root;
    }

    private void ChangeDimentions(ref float width, ref float height, float calculateWidthMax, float calculateHeightMax)
    {
        width = Mathf.Max(0.01f, calculateWidthMax);
        height = Mathf.Max(0.01f, calculateHeightMax);
    }

    private void OnSceneGUI()
    {
        //Drawing the wireframe box
        Handles.color = water.gizmoColor;
        Vector3 center = water.transform.position;
        Vector3 size = new Vector3(water.width, water.height, 0.01f);
        Handles.DrawWireCube(center, size);

        //Handles for width and height
        float handleSize = HandleUtility.GetHandleSize(center) * 0.01f;
        Vector3 snap = Vector3.one * 0.01f;

        //Corner handles
        Vector3[] corners = new Vector3[4];
        corners[0] = center + new Vector3(-water.width / 2, -water.height / 2, 0); //bottomLeft
        corners[1] = center + new Vector3(water.width / 2, -water.height / 2, 0); //bottomRight
        corners[2] = center + new Vector3(-water.width / 2, water.height / 2, 0); //TopLeft
        corners[3] = center + new Vector3(water.width / 2, water.height / 2, 0); //TopRight

        //handle for each corner
        EditorGUI.BeginChangeCheck();
        Vector3 newBottomLeft = Handles.FreeMoveHandle(corners[0], handleSize, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimentions(ref water.width, ref water.height, corners[1].x - newBottomLeft.x, corners[3].y - newBottomLeft.y);
            water.transform.position += new Vector3((newBottomLeft.x - corners[0].x) / 2, (newBottomLeft.y - corners[0].y) / 2, 0);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newBottomRight = Handles.FreeMoveHandle(corners[1], handleSize, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimentions(ref water.width, ref water.height, newBottomRight.x - corners[0].x, corners[3].y - newBottomRight.y);
            water.transform.position += new Vector3((newBottomRight.x - corners[1].x) / 2, (newBottomRight.y - corners[1].y) / 2, 0);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newTopLeft = Handles.FreeMoveHandle(corners[2], handleSize, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimentions(ref water.width, ref water.height, corners[3].x - newTopLeft.x, newTopLeft.y - corners[0].y);
            water.transform.position += new Vector3((newTopLeft.x - corners[2].x) / 2, (newTopLeft.y - corners[2].y) / 2, 0);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newTopRight = Handles.FreeMoveHandle(corners[3], handleSize, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimentions(ref water.width, ref water.height, newTopRight.x - corners[2].x, newTopRight.y - corners[1].y);
            water.transform.position += new Vector3((newTopRight.x - corners[3].x) / 2, (newTopRight.y - corners[3].y) / 2, 0);
        }

        //Update the mesh when handles are moved
        if (GUI.changed)
        {
            water.GenerateMesh();
        }
    }
}