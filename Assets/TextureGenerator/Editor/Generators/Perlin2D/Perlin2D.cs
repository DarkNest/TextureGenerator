using UnityEditor;
using UnityEngine;
using TextureGenerator;

public class Perlin2D : Texture2DGenerator
{
    [Range(1, 100), HideInInspector]
    private int gridNum = 5;

    private ComputeBuffer valueBuffer;

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
            if (value > axis / 2)
                value = axis / 2;
            if (value != gridNum && gridNum > 0)
            {
                InitBufferData();
            }
            gridNum = value;
        }
    }

    private int bufferCount
    {
        get
        {
            int num = gridNum + 1;
            return num * num;
        }
    }

    public override void InitData()
    {
        InitBufferData();
    }

    public override void DoRenderTexture()
    {           
        int kenerl = computeShader.FindKernel(kernelName);
        computeShader.SetTexture(kenerl, "Result", resultTexture);
        computeShader.SetInt("_axis", axis);
        computeShader.SetBuffer(kenerl, "_PointVals", valueBuffer);
        computeShader.SetInt("_GridNum", GridNum);
        computeShader.Dispatch(kenerl, axis/threadNum.x, axis/threadNum.y, axis/threadNum.z);        
    }

    private void InitBufferData()
    {
        int count = bufferCount;
        if(valueBuffer == null)
        {            
            valueBuffer = new ComputeBuffer(count, sizeof(float));
        }
        else if(valueBuffer.count != count)
        {
            valueBuffer.Release();
            valueBuffer = null;
            valueBuffer = new ComputeBuffer(count, sizeof(float));
        }
        
        float[] values = GetPointsValue();
        valueBuffer.SetData(values);
    }

    private float[] GetPointsValue()
    {
        float[] ret = new float[bufferCount];
        for(int i = 0; i < ret.Length; i++)
        {
            ret[i] = Random.value;
        }
        return ret;
    }

    public void Refresh()
    {
        InitBufferData();
    }

    public override void Dispose()
    {
        base.Dispose();
        if (valueBuffer != null)
        {
            valueBuffer.Release();
            valueBuffer = null;
        }
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

        if ( GUILayout.Button("Refresh"))
        {
            gen.Refresh();
            gen.DoRenderTexture();
        }
    }
}
