using TextureGenerator;
using UnityEngine;
using UnityEditor;

public class Worley3D : Texture3DGenerator
{
    [Range(3, 16)]
    public int cellPerAxis = 6;

    [Range(0, 1)]
    public float bright = 1f;

    public float maxEffectDis = 500;

    public bool filp = false;

    public bool FBM = false;

    [Range(1, 10)]
    public int octaves = 1;

    public float frequency = 1.0f;
    public float lacunarity = 2.0f;
    public float amplitude = 0.5f;
    public float gain = 0.5f;


    Vector3[] points;

    ComputeBuffer pointsBuffer;

    public override void InitData()
    {
        int length = cellPerAxis * cellPerAxis * cellPerAxis;
        points = GetPointArray();
        if (pointsBuffer == null)
        {
            pointsBuffer = CreateBuffer();
        }
        else if (pointsBuffer.count != length)
        {
            pointsBuffer.Release();
            pointsBuffer = null;
            pointsBuffer = CreateBuffer();
        }
        pointsBuffer.SetData(points);
    }

    public override void DoRenderTexture()
    {
        int kernel = computeShader.FindKernel(kernelName);
        computeShader.SetTexture(kernel, "Result", resultTexture);
        computeShader.SetBuffer(kernel, "_Points", pointsBuffer);
        computeShader.SetInt("_CellPerAxis", cellPerAxis);
        computeShader.SetFloat("_CellSize", axis / (float)cellPerAxis);
        computeShader.SetInt("_Axis", axis);
        computeShader.SetBool("_Filp", filp);
        computeShader.SetFloat("_Bright", bright);
        computeShader.SetBool("_FBM", FBM);
        computeShader.SetInt("_Octaves", octaves);
        computeShader.SetFloat("_Frequency", frequency);
        computeShader.SetFloat("_Lacunarity", lacunarity);
        computeShader.SetFloat("_Amplitude", amplitude);
        computeShader.SetFloat("_Gain", gain);
        computeShader.SetFloat("_MaxEffectDis", maxEffectDis);
        computeShader.Dispatch(kernel, axis / threadNum.x, axis / threadNum.y, axis / threadNum.z);
    }

    private Vector3[] GetPointArray()
    {
        int length = cellPerAxis * cellPerAxis * cellPerAxis;
        Vector3[] points = new Vector3[length];
        float cellSize = axis / cellPerAxis;
        for (int x = 0; x < cellPerAxis; x++)
        {
            for (int y = 0; y < cellPerAxis; y++)
            {
                for (int z = 0; z < cellPerAxis; z++)
                {
                    Vector3 pos = (new Vector3(x, y, z) + new Vector3(Random.value, Random.value, Random.value)) * cellSize;
                    points[x + y * cellPerAxis + z * cellPerAxis * cellPerAxis] = pos;
                }
            }
        }
        return points;
    }

    private ComputeBuffer CreateBuffer()
    {
        int length = cellPerAxis * cellPerAxis * cellPerAxis;
        ComputeBuffer buffer = new ComputeBuffer(length, sizeof(float) * 3);
        return buffer;
    }

    public override void Dispose()
    {
        base.Dispose();
        if (pointsBuffer != null)
        {
            pointsBuffer.Release();
            pointsBuffer = null;
        }
    }
}

[CustomEditor(typeof(Worley3D))]
public class Worley3DEditor : Texture3DGeneratorEditor 
{
    int lastCellPerAxis = 0;

    private void Awake()
    {
        Worley3D gen = target as Worley3D;
        lastCellPerAxis = gen.cellPerAxis;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Worley3D gen = target as Worley3D;
        if (lastCellPerAxis != gen.cellPerAxis)
        {
            gen.InitData();
            gen.DoRenderTexture();
            lastCellPerAxis = gen.cellPerAxis;
        }

        if (GUILayout.Button("Refresh Point"))
        {
            gen.InitData();
            gen.DoRenderTexture();
        }
    }
}
