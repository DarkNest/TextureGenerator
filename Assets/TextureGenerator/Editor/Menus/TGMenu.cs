using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TextureGenerator
{
    public class TGMenu
    {
        [MenuItem("Assets/TG Tools/Create Texture Generator")]
        [MenuItem("TG Tools/Create Texture Generator")]
        public static void CreateTextureGen()
        {
            TextureGenWin win = (TextureGenWin)EditorWindow.GetWindow(typeof(TextureGenWin));
            win.Show();
        }

        [MenuItem("Assets/TG Tools/Create Texture Gen Asset", true)]
        public static bool CheckCreateAsset()
        {
            if (Selection.activeObject == null)
                return false;
            if (!(Selection.activeObject is MonoScript))
                return false;
            MonoScript script = Selection.activeObject as MonoScript;
            if (script.GetClass().BaseType == typeof(Texture2DGenerator)
                || script.GetClass().BaseType == typeof(Texture3DGenerator))
                return true;
            return false;
        }

        [MenuItem("Assets/TG Tools/Create Texture Gen Asset")]
        public static void CreatAsset()
        {
            MonoScript script = Selection.activeObject as MonoScript;
            CreateAssetWin win = (CreateAssetWin)EditorWindow.GetWindow(typeof(CreateAssetWin));
            win.script = script;
            if (script.GetClass().BaseType == typeof(Texture2DGenerator))
                win.maxAxis = GeneratorSettings.maxAxis2D;
            if (script.GetClass().BaseType == typeof(Texture3DGenerator))
                win.maxAxis = GeneratorSettings.maxAxis3D;
        }
    }
}