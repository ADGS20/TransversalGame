using UnityEngine;

[System.Serializable]
public class LineaDialogo
{
    public string nombrePersonaje;
    public Sprite iconoPersonaje;
    [TextArea(3, 10)]
    public string texto;
}