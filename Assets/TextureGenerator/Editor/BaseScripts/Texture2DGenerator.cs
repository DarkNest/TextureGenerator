using UnityEngine;
using UnityEditor;
using System.IO;

namespace TextureGenerator
{
    public abstract class Texture2DGenerator : Generator
    {
        public bool savePNG = true;

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
            string assetName = OutputName;
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogWarning("Create texture failed, Check asset Name");
                return;
            }
            Texture2D output = ConverTexture2D(resultTexture);
            if (savePNG)
                SavePNG(output);
            else
                SaveAsset(output);
            
            AssetDatabase.Refresh();
        }

        void SavePNG(Texture2D tex)
        {
            string assetName = OutputName;
            string assetPath = GeneratorSettings.TGOutputFullPath + "/" + assetName + ".png";
            byte[] data = tex.EncodeToPNG();
            using (FileStream file = File.Open(assetPath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(file))
            {
                writer.Write(data);
                file.Close();
            }
            Debug.Log("Save Finish：" + assetPath);
        }

        void SaveAsset(Texture2D tex)
        {
            string assetName = OutputName;
            string assetPath = GeneratorSettings.TGOutputPath + "/" + assetName + ".asset";
            AssetDatabase.CreateAsset(tex, assetPath);
            AssetDatabase.Refresh();
            Debug.Log("Save Finish：" + assetPath);
        }
    }
}