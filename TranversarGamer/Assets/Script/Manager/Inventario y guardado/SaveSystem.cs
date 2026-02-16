using UnityEngine;
using System.IO; // Necesario para manejar archivos

public class SaveSystem : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        // Ruta segura en cualquier sistema operativo (Windows, Android, iOS)
        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    public void GuardarJuego()
    {
        GameData data = new GameData();

        // 1. RECOPILAR DATOS
        // Guardamos el nivel actual (ejemplo)
        data.nivelCompletado = 5; // Aquí pondrías tu variable real de nivel

        // Guardamos el inventario convirtiendo los objetos a sus IDs
        foreach (var item in InventoryManager.Instance.objetosRecogidos)
        {
            data.inventarioIDs.Add(item.id);
        }

        // 2. CONVERTIR A JSON
        string json = JsonUtility.ToJson(data, true);

        // 3. ESCRIBIR EN DISCO
        File.WriteAllText(savePath, json);
        Debug.Log("Juego guardado en: " + savePath);
    }

    public void CargarJuego()
    {
        if (File.Exists(savePath))
        {
            // 1. LEER DEL DISCO
            string json = File.ReadAllText(savePath);

            // 2. CONVERTIR DE JSON A C#
            GameData data = JsonUtility.FromJson<GameData>(json);

            // 3. APLICAR DATOS AL JUEGO
            Debug.Log("Cargando nivel: " + data.nivelCompletado);

            // Reconstruir inventario (necesitas una base de datos de todos los items posibles para buscar por ID)
            // Esto es un paso avanzado, pero esencial: convertir ID string -> ScriptableObject
        }
        else
        {
            Debug.LogWarning("No se encontró archivo de guardado.");
        }
    }
}