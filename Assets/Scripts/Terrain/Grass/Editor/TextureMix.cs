#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamePlus.Urp.Terrain.Grass.Editor
{
    public class TextureMix : EditorWindow
    {

        [MenuItem("Assets/TextureMix Tool")]
        static void ShowTextureMixToolsWindow()
        {
            EditorWindow.GetWindow(typeof(TextureMix));
        }

        private static readonly int _R_ID = Shader.PropertyToID("_R");
        private static readonly int _G_ID = Shader.PropertyToID("_G");
        private static readonly int _B_ID = Shader.PropertyToID("_B");
        private static readonly int _A_ID = Shader.PropertyToID("_A");
        private static readonly int _DEFAULT_VALUE = Shader.PropertyToID("default_value");
        private static readonly int _VALUE_SWITCH = Shader.PropertyToID("value_switch");

        private Texture2D _r;
        private Texture2D _g;
        private Texture2D _b;
        private Texture2D _a;

        private Vector4 _default = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        private Vector4 _defaultSwitch = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        private Vector2 _resolution = new Vector2(512, 512);
        private int _width = 512;
        private int _height = 512;

        private bool _useSRGB = false;

        private String _savePath = "Temp.tga";

        private Material _material;


        private Texture2D _out;

        TextureMix()
        {
            this.titleContent = new GUIContent("TextureMix Tool");
            
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            _r = (Texture2D)EditorGUILayout.ObjectField(
                "R Channel", _r, typeof(Texture2D), false);
            _g = (Texture2D)EditorGUILayout.ObjectField(
                "G Channel", _g, typeof(Texture2D), false);
            _b = (Texture2D)EditorGUILayout.ObjectField(
                "B Channel", _b, typeof(Texture2D), false);
            _a = (Texture2D)EditorGUILayout.ObjectField(
                "A Channel", _a, typeof(Texture2D), false);

            _default = EditorGUILayout.Vector4Field("Default Value", _default);

            GUILayout.Space(20);
            _useSRGB = EditorGUILayout.Toggle("Use sRGB", _useSRGB);

            GUILayout.Space(20);
            EditorGUILayout.PrefixLabel("Resolution: ");
            _width = EditorGUILayout.IntField("Width: ", _width, GUILayout.MinWidth(100));
            _height = EditorGUILayout.IntField("Height: ", _height, GUILayout.MinWidth(100));
            
            GUILayout.Space(20);
            _out = (Texture2D)EditorGUILayout.ObjectField("Override", _out, typeof(Texture2D), false);
            
            if (_out == null)
            {
                EditorGUILayout.PrefixLabel("SavePath: ");
                _savePath = EditorGUILayout.TextField("", _savePath);    
            }
            
            
            GUILayout.EndVertical();
            
            if (GUILayout.Button("Save"))
            {
                Save();
            }
        }
        
        void Save()
        {
            if (_material == null)
            {
                _material = CoreUtils.CreateEngineMaterial("[Hidden]Editor/MixTexture");
            }
            Texture2D merged = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            RenderTextureDescriptor desc = new RenderTextureDescriptor(_width, _height, RenderTextureFormat.ARGB32, 0)
                {
                    sRGB = _useSRGB,
                    useMipMap = false
                };

            RenderTexture rt = RenderTexture.GetTemporary(desc);

            _defaultSwitch.Set(_r == null ? 0f: 1f, _g == null ? 0f: 1f, _b == null ? 0f: 1f, _a == null ? 0f: 1f);
            _material.SetTexture(_R_ID, _r);
            _material.SetTexture(_G_ID, _g);
            _material.SetTexture(_B_ID, _b);
            _material.SetTexture(_A_ID, _a);
            _material.SetVector(_DEFAULT_VALUE, _default);
            _material.SetVector(_VALUE_SWITCH, _defaultSwitch);
            Graphics.Blit(merged, rt, _material);
            merged.ReadPixels(new Rect(0, 0, merged.width, merged.height), 0, 0);
            merged.Apply();
            
            byte[] bytes = merged.EncodeToTGA();
            var path = "Assets/" + _savePath;
            path = Path.ChangeExtension(path, ".tga");
            if (_out != null)
            {
                path = AssetDatabase.GetAssetPath(_out);
            }
            File.WriteAllBytes(path, bytes);

            RenderTexture.ReleaseTemporary(rt);
            
            AssetDatabase.Refresh();
            
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.sRGBTexture = _useSRGB;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Bilinear;
                importer.wrapMode = TextureWrapMode.Clamp;
                
                importer.ClearPlatformTextureSettings("Android");
                importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings()
                {
                    overridden = true,
                    name = "Android",
                    format = TextureImporterFormat.RGBA32,
                    textureCompression = TextureImporterCompression.Compressed,
                    crunchedCompression = true,
                    allowsAlphaSplitting = true,
                    maxTextureSize = 2048,
                    resizeAlgorithm = TextureResizeAlgorithm.Mitchell,
                    androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
                });
                
                importer.SaveAndReimport();
            }

            _out = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            
            AssetDatabase.Refresh();
        }

    }
}
#endif