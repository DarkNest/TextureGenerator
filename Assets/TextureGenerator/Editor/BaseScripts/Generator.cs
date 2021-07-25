using System;
using UnityEngine;

namespace TextureGenerator
{
    public abstract class Generator : ScriptableObject, IDisposable
    {
        [Tooltip("生成文件名")]
        public string OutputName;

        public string kernelName = "CSMain";
        public ComputeShader computeShader;
        
        protected int axis = 128;

        public int Axis
        {
            set
            {
                axis = value;
                InitData();
            }
            get
            {
                return axis;
            }
        }

        [SerializeField]
        public KernelThreadNum threadNum;

        protected RenderTexture resultTexture;

        public virtual void InitData() { }

        public abstract void UpdateRenderTexture();

        public abstract void DoRenderTexture();

        public abstract void DoSaveAsset();

        public RenderTexture GetRenderTexture()
        {
            return resultTexture;
        }

        protected Texture2D ConverTexture2D(RenderTexture renderTexture)
        {
            Texture2D texture = new Texture2D(axis, axis);            
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0,0,axis,axis), 0, 0);
            texture.Apply();
            return texture;
        }

        public virtual void Dispose()
        {
            if (resultTexture != null)
            {
                if (RenderTexture.active == resultTexture)
                    RenderTexture.active = null;
                resultTexture.Release();
            }
            resultTexture = null;
        }
    }

    [Serializable]
    public struct KernelThreadNum
    {
        [Range(1, 256)]
        public int x;
        [Range(1, 256)]
        public int y;
        [Range(1, 256)]
        public int z;

        public KernelThreadNum(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
