using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int nivelCompletado;
    public List<string> inventarioIDs; // Guardamos solo los IDs, no el objeto entero
    public List<int> decisionesTomadas; // Ej: 1 = salvó al aldeano, 0 = no lo salvó

    // Constructor por defecto (valores iniciales para partida nueva)
    public GameData()
    {
        nivelCompletado = 0;
        inventarioIDs = new List<string>();
        decisionesTomadas = new List<int>();
    }
}