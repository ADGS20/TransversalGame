using UnityEngine;
using UnityEditor;

public class Create3DNoiseTexture : MonoBehaviour
{
    [MenuItem("Tools/Create 3D Noise Texture")]
    static void CreateTexture()
    {
        int size = 32;
        Texture3D texture = new Texture3D(size, size, size, TextureFormat.RGBA32, false);

        Color[] colors = new Color[size * size * size];

        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) *
                                  Mathf.PerlinNoise(y * 0.1f, z * 0.1f) *
                                  Mathf.PerlinNoise(z * 0.1f, x * 0.1f);

                    colors[x + y * size + z * size * size] = new Color(noise, noise, noise, noise);
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        AssetDatabase.CreateAsset(texture, "Assets/Noise3D.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("3D Noise Texture created at Assets/Noise3D.asset");
    }
}
