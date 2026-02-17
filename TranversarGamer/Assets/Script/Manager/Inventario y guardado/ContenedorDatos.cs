using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int nivelCompletado;
    public List<string> inventarioIDs;
    public List<int> decisionesTomadas;

    public float posicionX;
    public float posicionY;
    public float posicionZ;

    public List<string> objetosDestruidosMundo;

    // --- NUEVO: Variables para guardar la lógica de la cámara ---
    public float camaraAngulo;
    public float camaraZoom;

    public GameData()
    {
        nivelCompletado = 0;
        inventarioIDs = new List<string>();
        decisionesTomadas = new List<int>();

        posicionX = 0f;
        posicionY = 0f;
        posicionZ = 0f;

        objetosDestruidosMundo = new List<string>();

        // Valores por defecto para una Nueva Partida
        camaraAngulo = 0f;
        camaraZoom = 0.5f;
    }
}