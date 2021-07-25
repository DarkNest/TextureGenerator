using UnityEngine;
using UnityEditor;

namespace TextureGenerator
{
    public class GeneratorEditor : Editor
    {
        int lastAxis;

        private void Awake()
        {
            Generator gen = target as Generator;
            gen.InitData();
            gen.UpdateRenderTexture();
            lastAxis = gen.Axis;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Generator gen = target as Generator;
            if (gen.Axis != lastAxis)
            {
                gen.InitData();
                lastAxis = gen.Axis;
            }
            if (GUILayout.Button("Save"))
            {
                gen.UpdateRenderTexture();
                gen.DoRenderTexture();
                gen.DoSaveAsset();
                AssetDatabase.Refresh();
            }
        }
    }
}
