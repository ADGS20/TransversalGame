using UnityEngine;

[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string id;           // Identificador único (ej: "pocion_salud")
    public string nombre;       // Nombre para mostrar
    public Sprite icono;        // Imagen para la UI
    public bool esAcumulable;   // ¿Se pueden tener varios en el mismo slot?
}