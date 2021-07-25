using UnityEngine;
using UnityEditor;

namespace TextureGenerator
{
    public abstract class Texture2DGenerator : Generator
    {
        public sealed override void UpdateRenderTexture()
        {
            if (resultTexture == null || resultTexture.width != axis)
            {
                resultTexture = new RenderTexture(axis, axis, 0);
                resultTexture.enableRandomWrite = true;
                resultTexture.Create();
            }            
        }

        public sealed override void DoSaveAsset()
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
            Texture2D output = ConverTexture2D(resultTexture);
            string assetPath = targetPath + "/" + assetName + ".asset";
            AssetDatabase.CreateAsset(output, assetPath);
            AssetDatabase.Refresh();
            Debug.Log("Save Finish：" + assetPath);
        }
    }
}