using UnityEngine;

namespace TextureGenerator
{
    class Texture2DGeneratorEditor : GeneratorEditor
    {
        public override void OnInspectorGUI()
        {
            Generator gen = target as Generator;
            GUILayout.Label(string.Format("Axis: {0}x{0}", gen.Axis));
            base.OnInspectorGUI();
        }

        public override bool HasPreviewGUI()
        {
            Texture2DGenerator gen = target as Texture2DGenerator;
            if (gen == null)
                return false;
            if (gen.computeShader == null)
                return false;
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
            Texture2DGenerator gen = target as Texture2DGenerator;
            if (gen == null)
                return;

            gen.UpdateRenderTexture();
            gen.DoRenderTexture();
            Texture texture = gen.GetRenderTexture();
            float minBound = Mathf.Min(r.width, r.height);
            float axis = Mathf.Min(gen.Axis, minBound);
            float x = r.x + r.width / 2 - axis / 2;
            float y = r.y + r.height / 2 - axis / 2;
            Rect rect = new Rect(x, y, axis, axis);
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
        }
    }
}