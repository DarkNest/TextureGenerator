using UnityEngine;

namespace TextureGenerator
{
    /// <summary>
    /// Path Define
    /// </summary>
    public class GeneratorSettings
    {
        public const string TGDirPath = "TextureGenerator";
        public const string tempScript2D = "tempscript2D.txt";
        public const string tempShader2D = "tempshader2D.txt";
        public const string tempScript3D = "tempscript3D.txt";
        public const string tempShader3D = "tempshader3D.txt";
        public const string generatorPaht = "Editor/Generators";
        public const string sliceShader = "SliceShader";

        public const int maxAxis2D = 1024;
        public const int maxAxis3D = 512;

        public static string TGRootPath
        {
            get
            {
                return Application.dataPath + "/" + TGDirPath;
            }
        }

        public static string TGLoadRootPath
        {
            get
            {
                return "Assets/" + TGDirPath;
            }
        }

        public static string TempScript2DPath
        {
            get
            {
                return TGRootPath + "/Editor/Menus/Temp/" + tempScript2D;
            }
        }

        public static string TempShader2DPath
        {
            get
            {
                return TGRootPath + "/Editor/Menus/Temp/" + tempShader2D;
            }
        }

        public static string TempScript3DPath
        {
            get
            {
                return TGRootPath + "/Editor/Menus/Temp/" + tempScript3D;
            }
        }

        public static string TempShader3DPath
        {
            get
            {
                return TGRootPath + "/Editor/Menus/Temp/" + tempShader3D;
            }
        }

        public static string SliceShaderLoadPath
        {
            get
            {
                return TGLoadRootPath + "/Editor/BaseScripts/" + sliceShader + ".compute";
            }
        }

        public static string GeneratorPath
        {
            get
            {
                return TGRootPath + "/" + generatorPaht;
            }
        }

        public static string LoadGeneratorPath
        {
            get
            {
                return TGLoadRootPath + "/" + generatorPaht;
            }
        }
    }
}
