using UnityEngine;
using UnityEditor;
using System.IO;

namespace TextureGenerator
{
    public class TextureGenWin : EditorWindow
    {
        GeneratorType generatorType = GeneratorType.Texture2D;
        string genName = "";

        private void OnGUI()
        {
            generatorType = (GeneratorType)EditorGUILayout.EnumPopup("Texture Type: ", generatorType);
            genName = EditorGUILayout.TextField("Name:", genName);
            if (GUILayout.Button("Create"))
            {
                string fileName = genName.Replace(" ", "");
                string genPath = GeneratorSettings.GeneratorPath + "/" + fileName;
                if (!NameStrCheck(genName))
                {
                    Debug.LogError("Invalid Name:" + genName);
                    return;
                }
                if (Directory.Exists(genPath))
                {
                    Debug.LogError("Generator Already Existed: " + genPath);
                    return;
                }
                
                switch (generatorType)
                {
                    case GeneratorType.Texture2D: TempCreator.Create2DGeneratorTemp(fileName); break;
                    case GeneratorType.Texture3D: TempCreator.Create3DGeneratorTemp(fileName); break;
                    default: TempCreator.Create2DGeneratorTemp(fileName);break;
                }
            }
        }

        private bool NameStrCheck(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            char firstCh = name[0];
            if (char.IsNumber(firstCh))
                return false;
            if (char.IsWhiteSpace(firstCh))
                return false;
            return true;
        }
    }

    public class CreateAssetWin : EditorWindow
    {
        public int maxAxis = 512;
        public int axis = 128;
        public MonoScript script;

        private void OnGUI()
        {
            axis = EditorGUILayout.IntField("Axis:", axis);
            if (axis > 0 && axis < maxAxis)
            {
                if (GUILayout.Button("OK"))
                {
                    CreateTextureAsset(script, axis);
                }     
            }
            else
            {
                GUILayout.Label("Axis must be in range : 0 - " + maxAxis );
            }
        }

        static void CreateTextureAsset(MonoScript script, int axis)
        {
            string fullFilePath = AssetDatabase.GetAssetPath(script);
            string filePath = Path.GetDirectoryName(fullFilePath).Replace('\\', '/');
            string fileName = Path.GetFileName(fullFilePath).Replace(".cs", "");
            string shaderPath = filePath + "/" + fileName + ".compute";
            string assetPath = filePath + "/" + fileName + ".asset";

            ComputeShader computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(shaderPath);
            if (computeShader == null)
            {
                Debug.LogError("Can't Find Compute Shader: " + shaderPath);
                return;
            }
            Generator gen = ScriptableObject.CreateInstance(script.GetClass()) as Generator;
            gen.OutputName = fileName.Replace("Generator", "");
            gen.computeShader = computeShader;
            gen.Axis = axis;
            if (script.GetClass().BaseType == typeof(Texture2DGenerator))
            {
                gen.threadNum = new KernelThreadNum(8, 8, 1);
            }
            else if (script.GetClass().BaseType == typeof(Texture3DGenerator))
            {
                gen.threadNum = new KernelThreadNum(8, 8, 8);
                Texture3DGenerator gen3D = gen as Texture3DGenerator;
                gen3D.sliceShader = Texture3DGenerator.GetSliceShader();
            }
            AssetDatabase.CreateAsset(gen, assetPath);
            AssetDatabase.Refresh();
            Debug.Log("Create Success: " + assetPath);
        }
    }


    public class TempCreator
    {
        public static void Create2DGeneratorTemp(string name)
        {
            string scriptPath = GeneratorSettings.TempScript2DPath;
            string shaderPath = GeneratorSettings.TempShader2DPath;
            CreateTemp(scriptPath, shaderPath, name);         
        }

        public static void Create3DGeneratorTemp(string name)
        {
            string scriptPath = GeneratorSettings.TempScript3DPath;
            string shaderPath = GeneratorSettings.TempShader3DPath;
            CreateTemp(scriptPath, shaderPath, name);
        }

        private static void CreateTemp(string scriptPath, string shaderPath, string name)
        {
            if (!PathCheck(scriptPath, shaderPath))
                return;
            string outputPath = GeneratorSettings.GeneratorPath + "/" + name;
            Directory.CreateDirectory(outputPath);
            //Create Script
            string outputScriptPath = outputPath + "/" + name + ".cs";

            using (FileStream tempStream = File.OpenRead(scriptPath))
            using (StreamReader reader = new StreamReader(tempStream))
            using (FileStream file = File.Create(outputScriptPath))
            using (StreamWriter writer = new StreamWriter(file))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    writer.WriteLine(line.Replace("${ClassName}", name));
                }
            }
            //Create Compute Shader
            string outputShaderPath = outputPath + "/" + name + ".compute";
            File.Copy(shaderPath, outputShaderPath);

            AssetDatabase.Refresh();
            string loadPath = GeneratorSettings.LoadGeneratorPath + "/" + name + "/" + name + ".cs";
            Object obj = AssetDatabase.LoadAssetAtPath<MonoScript>(loadPath);
            Selection.activeObject = obj;
        }

        private static bool PathCheck(string scriptPath, string shaderPath)
        {
            if (!File.Exists(scriptPath))
            {
                Debug.LogError("Can't Find Temp File: " + scriptPath);
                return false;
            }
            if (!File.Exists(shaderPath))
            {
                Debug.LogError("Can't Find Temp File: " + shaderPath);
                return false;
            }
            return true;
        }
    }

    public enum GeneratorType
    {
        Texture2D,
        Texture3D,
    }
}
