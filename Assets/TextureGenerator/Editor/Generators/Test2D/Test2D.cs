using UnityEditor;
using UnityEngine;
using TextureGenerator;

public class Test2D : Texture2DGenerator
{
    public Color color = Color.white;
    public Vector2 offset = Vector2.zero;

    public override void InitData()
    {
        //Init Your Data Here
    }

    public override void DoRenderTexture()
    {   
        //Editor Your Code Here
        int kenerl = computeShader.FindKernel(kernelName);
        computeShader.SetTexture(kenerl, "Result", resultTexture);
        computeShader.SetVector("_color", new Vector4(color.r, color.g, color.b, color.a));
        computeShader.SetVector("_offset", offset);
        computeShader.SetInt("_axis", axis);
        computeShader.Dispatch(kenerl, axis/threadNum.x, axis/threadNum.y, axis/threadNum.z);        
    }
}

[CustomEditor(typeof(Test2D))]
class Test2DEditor : Texture2DGeneratorEditor { }
