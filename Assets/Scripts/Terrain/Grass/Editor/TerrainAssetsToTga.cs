using System.IO;
using UnityEditor;
using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Editor
{
    public class TerrainAssetsToTga
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Convert to TGA")]
        private static void ConvertToTGA()
        {
            // 获取所选的 asset 文件
            var selection = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(selection);

            // 加载 asset 文件
            var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            // 将 asset 转换为 TGA 格式
            var bytes = asset.EncodeToTGA();

            // 保存 TGA 文件
            var tgaPath = Path.ChangeExtension(path, ".tga");
            File.WriteAllBytes(tgaPath, bytes);
            
            // 刷新 AssetDatabase
            AssetDatabase.Refresh();
            
            // 导入 TGA 文件
            var importer = AssetImporter.GetAtPath(tgaPath) as TextureImporter;
            if (importer != null)
            {
                importer.sRGBTexture = false;
                importer.SaveAndReimport();
            }

            // 刷新 AssetDatabase
            AssetDatabase.Refresh();
        }
#endif
    }
}
