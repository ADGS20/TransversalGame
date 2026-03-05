using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> objetosRecogidos = new List<ItemData>();

    // NUEVO: Guardará los IDs de los objetos del mapa que recojamos
    public List<string> objetosDestruidosMundo = new List<string>();

    [Header("Interfaz de Usuario")]
    public GameObject panelInventario;
    public GameObject prefabIconoItem;
    public Transform contenedorDeIconos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (panelInventario != null) DontDestroyOnLoad(panelInventario.transform.root.gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (panelInventario != null)
            {
                bool estadoNuevo = !panelInventario.activeSelf;
                panelInventario.SetActive(estadoNuevo);

                if (estadoNuevo == true) Time.timeScale = 0f;
                else Time.timeScale = 1f;
            }
        }
    }

    // Esta función nos dirá desde otros scripts si el inventario está abierto
    public bool EstaInventarioAbierto()
    {
        if (panelInventario != null)
        {
            return panelInventario.activeSelf;
        }
        return false;
    }

    // Esta función permite que el menú de Pausa cierre el inventario a la fuerza
    public void CerrarInventario()
    {
        if (panelInventario != null)
        {
            panelInventario.SetActive(false);
            Time.timeScale = 1f; // Reanudamos el tiempo
        }
    }

    public void AgregarItem(ItemData item)
    {
        objetosRecogidos.Add(item);
        Debug.Log($"Item añadido: {item.nombre}");

        if (prefabIconoItem != null && contenedorDeIconos != null)
        {
            GameObject nuevoIcono = Instantiate(prefabIconoItem, contenedorDeIconos);
            Image imagenUI = nuevoIcono.GetComponent<Image>();
            if (imagenUI != null && item.icono != null) imagenUI.sprite = item.icono;
        }
    }

    public void EliminarItem(ItemData item)
    {
        // Si tenemos el objeto en la lista, lo quitamos
        if (objetosRecogidos.Contains(item))
        {
            objetosRecogidos.Remove(item);
            ActualizarInterfaz(); // Llamamos a una función para refrescar los iconos
        }
    }

    // Esta función borra todos los iconos viejos y los vuelve a crear basados en lo que realmente tienes
    public void ActualizarInterfaz()
    {
        if (contenedorDeIconos == null || prefabIconoItem == null) return;

        // 1. Destruimos todos los iconos visuales actuales
        foreach (Transform hijo in contenedorDeIconos)
        {
            Destroy(hijo.gameObject);
        }

        // 2. Volvemos a crear los iconos solo de los objetos que quedan en la lista
        foreach (ItemData itemRestante in objetosRecogidos)
        {
            GameObject nuevoIcono = Instantiate(prefabIconoItem, contenedorDeIconos);
            Image imagenUI = nuevoIcono.GetComponent<Image>();
            if (imagenUI != null && itemRestante.icono != null) imagenUI.sprite = itemRestante.icono;
        }
    }
}