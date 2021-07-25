using TextureGenerator;
using UnityEngine;
using UnityEditor;

public class Test3D : Texture3DGenerator
{
    public Color color = Color.white;

    public override void InitData()
    {
        //Init Your Data Here
    }

    public override void DoRenderTexture()
    {
        //Editor Your Code Here
        int kernel = computeShader.FindKernel(kernelName);
        computeShader.SetTexture(kernel, "Result", resultTexture);
        computeShader.SetInt("_axis", axis);
        computeShader.SetVector("_color", color);
        computeShader.Dispatch(kernel, axis / threadNum.x, axis / threadNum.y, axis / threadNum.z);
    }
}

[CustomEditor(typeof(Test3D))]
public class Test3DEditor : Texture3DGeneratorEditor {}
