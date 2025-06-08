using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EraserLightController : MonoBehaviour
{
    public Color lightColor = Color.white;
    public float intensity = 1f;
    public float outerAngle = 30f;
    public float range = 10f;
    [Range(0f, 64f)] public int segments = 32;
    public Transform directionTarget; // 방향 지정용
    
    private float prevAngle = -1f;
    private float prevRange = -1f;
    private int prevSegments = -1;

    private MeshFilter meshFilter;
    private MeshRenderer renderer;

    private int eraserLightPosWS = Shader.PropertyToID("_EraserLightPosWS");
    private int eraserLightDirWS = Shader.PropertyToID("_EraserLightDirWS");
    private int eraserLightRange = Shader.PropertyToID("_EraserLightRange");
    private int eraserLightOuterAngle = Shader.PropertyToID("_EraserLightOuterAngle");
    private int eraserLightColor = Shader.PropertyToID("_EraserLightColor");
    private int eraserLightIntensity = Shader.PropertyToID("_EraserLightIntensity");

    void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
        renderer = GetComponent<MeshRenderer>();
        UpdateConeMesh();
        UpdateShader();
    }

    void Update()
    {
        if (outerAngle != prevAngle || range != prevRange || segments != prevSegments)
        {
            UpdateConeMesh();
            prevAngle = outerAngle;
            prevRange = range;
            prevSegments = segments;
        }
        UpdateConeMaterial();
        UpdateShader();
    }

    void UpdateConeMesh()
    {
        float radius = Mathf.Tan(Mathf.Deg2Rad * outerAngle * 0.5f) * range;
        meshFilter.mesh = CreateConeMesh_ZForward(radius, range, segments);
    }

    Mesh CreateConeMesh_ZForward(float radius, float height, int segments)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float theta = (float)i / segments * Mathf.PI * 2f;
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            vertices[i + 1] = new Vector3(x, y, height);
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3]     = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    void UpdateConeMaterial()
    {
        if (renderer?.sharedMaterial != null)
        {
            renderer.sharedMaterial.color = lightColor;
        }
    }
    
    void UpdateShader()
    {
        var pos = transform.position;

        Vector3 dir = transform.forward;
        if (directionTarget)
        {
            dir = (directionTarget.position - pos).normalized;
        }

        Shader.SetGlobalVector(eraserLightPosWS, pos);
        Shader.SetGlobalVector(eraserLightDirWS, -dir);
        Shader.SetGlobalFloat(eraserLightRange, range);
        Shader.SetGlobalFloat(eraserLightOuterAngle, outerAngle);
        Shader.SetGlobalColor(eraserLightColor, lightColor);
        Shader.SetGlobalFloat(eraserLightIntensity, intensity);
    }
}
