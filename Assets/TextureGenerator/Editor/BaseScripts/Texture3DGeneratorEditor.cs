using UnityEngine;
using UnityEditor;

namespace TextureGenerator
{
    public class Texture3DGeneratorEditor : GeneratorEditor
    {
        int layer = 0;

        public override void OnInspectorGUI()
        {
            Texture3DGenerator gen = target as Texture3DGenerator;
            GUILayout.Label(string.Format("Axis: {0}x{0}x{0}", gen.Axis));
            base.OnInspectorGUI();
            layer = EditorGUILayout.IntSlider("Preview layer", layer, 0, gen.Axis - 1);
        }

        public override bool HasPreviewGUI()
        {
            Texture3DGenerator gen = target as Texture3DGenerator;
            if (gen == null)
                return false;
            if (gen.computeShader == null)
                return false;
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
            Texture3DGenerator gen = target as Texture3DGenerator;
            if (gen == null)
                return;

            gen.UpdateRenderTexture();
            gen.DoRenderTexture();
            RenderTexture texture3D = gen.GetRenderTexture();
            if (texture3D != null)
            {
                RenderTexture tex2D = gen.GetLayerRenderTex(texture3D, layer);
                float minBound = Mathf.Min(r.width, r.height);
                float axis = Mathf.Min(gen.Axis, minBound);
                float x = r.x + r.width / 2 - axis / 2;
                float y = r.y + r.height / 2 - axis / 2;
                Rect rect = new Rect(x, y, axis, axis);
                GUI.DrawTexture(rect, tex2D, ScaleMode.ScaleToFit);
            }
        }
    }
}