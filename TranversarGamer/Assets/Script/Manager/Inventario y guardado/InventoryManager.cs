using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // Singleton para acceso fácil

    // Lista de los objetos que el jugador tiene actualmente
    public List<ItemData> objetosRecogidos = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AgregarItem(ItemData item)
    {
        objetosRecogidos.Add(item);
        Debug.Log($"Item añadido: {item.nombre}");
        // Aquí llamarías a una función para actualizar la UI visualmente
    }

    public void EliminarItem(ItemData item)
    {
        if (objetosRecogidos.Contains(item))
        {
            objetosRecogidos.Remove(item);
        }
    }
}