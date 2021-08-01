using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace TextureGenerator
{
    public abstract class Texture3DGenerator : Generator
    {        
        public ComputeShader sliceShader;
        private const string sliceKernelName = "CSMain";

        private KernelThreadNum sliceThread = new KernelThreadNum(8, 8, 1);        

        sealed public override void UpdateRenderTexture()
        {            
            if (resultTexture == null || resultTexture.width != axis)
            {
                resultTexture = new RenderTexture(axis, axis, 0, RenderTextureFormat.ARGB32);
                resultTexture.enableRandomWrite = true;
                resultTexture.dimension = TextureDimension.Tex3D;
                resultTexture.volumeDepth = axis;
                resultTexture.Create();
            }
        }

        sealed public override void DoSaveAsset()
        {
            if (resultTexture == null)
                return;
            string targetPath = "Assets/TextureGenerator/Output";
            string assetName = OutputName;
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogWarning("Create texture failed, Check asset Name");
                return;
            }
            Texture3D output = ConverToTex3D(resultTexture);            
            string assetPath = targetPath + "/" + assetName + ".asset";
            AssetDatabase.CreateAsset(output, assetPath);
            AssetDatabase.Refresh();
            Debug.Log("保存资源成功：" + assetPath);
        }

        private Texture3D ConverToTex3D(RenderTexture renderTexture)
        {
            Texture3D output = new Texture3D(axis, axis, axis, TextureFormat.ARGB32, false);
            output.filterMode = FilterMode.Trilinear;
            Color[] colorArray = new Color[axis * axis * axis];
            for(int layer = 0; layer < axis; layer++)
            {
                Texture2D layerTex = GetLayerTex2D(renderTexture, layer);
                for(int x = 0; x < axis; x++)
                {
                    for(int y = 0;y < axis; y++)
                    {
                        colorArray[x + y * axis + layer * axis * axis] = layerTex.GetPixel(x,y);
                    }
                }
            }
            output.SetPixels(colorArray);
            output.Apply();
            return output;
        }

        public RenderTexture GetLayerRenderTex(RenderTexture renderTexture, int layer)
        {
            if (sliceShader == null)
            {
                sliceShader = GetSliceShader();
                return null;
            }
            RenderTexture layerTexRW = new RenderTexture(axis, axis, 0, RenderTextureFormat.ARGB32);
            layerTexRW.enableRandomWrite = true;
            layerTexRW.Create();
            int kenel = sliceShader.FindKernel(sliceKernelName);
            sliceShader.SetTexture(kenel, "Input", renderTexture);
            sliceShader.SetTexture(kenel, "Output", layerTexRW);
            sliceShader.SetInt("_layer", layer);
            sliceShader.Dispatch(kenel, axis / sliceThread.x, axis / sliceThread.y, axis / sliceThread.z);
            return layerTexRW;
        }

        public Texture2D GetLayerTex2D(RenderTexture renderTexture, int layer)
        {
            RenderTexture layerTexRW = GetLayerRenderTex(renderTexture, layer);
            Texture2D layerTex = ConverTexture2D(layerTexRW);
            return layerTex;
        }

        public static ComputeShader GetSliceShader()
        {
            string path = GeneratorSettings.SliceShaderLoadPath;
            ComputeShader sliceShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(path);
            if (sliceShader == null)
                Debug.LogError("Can't Find Computer Shader: " + path);
            return sliceShader;
        }
    }
}