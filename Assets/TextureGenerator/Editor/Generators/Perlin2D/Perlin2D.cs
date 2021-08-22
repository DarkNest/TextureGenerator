using UnityEditor;
using UnityEngine;
using TextureGenerator;

public class Perlin2D : Texture2DGenerator
{
    [Range(1, 100), HideInInspector]
    private int gridNum = 5;

    private Vector4 randomSeed = Vector4.zero;

    public bool isCicle = false;
    //////////////FBM
    public bool FBM = false;
    [Range(1, 10)]
    public int octaves = 1;
    public float frequency = 1.0f;
    public float lacunarity = 2.0f;
    public float amplitude = 0.5f;
    public float gain = 0.5f;

    public int GridNum
    {
        get
        {
            return gridNum;
        }
        set
        {
            if (value <= 0)
                value = 1;
            gridNum = value;
        }
    }

    public override void InitData()
    {
        RefreshData();
    }

    public override void DoRenderTexture()
    {           
        int kenerl = computeShader.FindKernel(kernelName);
        computeShader.SetTexture(kenerl, "Result", resultTexture);
        computeShader.SetFloat("_axis", axis);
        computeShader.SetInt("_GridNum", GridNum);
        computeShader.SetFloat("_GridSize", axis/(float)GridNum);
        computeShader.SetVector("_RandomSeed", randomSeed);
        computeShader.SetBool("_IsCicle", isCicle);
        computeShader.SetBool("_FBM", FBM);
        computeShader.SetInt("_Octaves", octaves);
        computeShader.SetFloat("_Frequency", frequency);
        computeShader.SetFloat("_Lacunarity", lacunarity);
        computeShader.SetFloat("_Amplitude", amplitude);
        computeShader.SetFloat("_Gain", gain);
        computeShader.Dispatch(kenerl, axis/threadNum.x, axis/threadNum.y, axis/threadNum.z);        
    }

    public void RefreshData()
    {
        randomSeed = new Vector4(Random.value, Random.value, Random.value, Random.value);
    }
}

[CustomEditor(typeof(Perlin2D))]
class Perlin2DEditor : Texture2DGeneratorEditor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Perlin2D gen = target as Perlin2D;
        gen.GridNum = EditorGUILayout.IntField("Grid Num", gen.GridNum);
        if (GUILayout.Button("Refresh"))
        {
            gen.RefreshData();
        }
    }
}
