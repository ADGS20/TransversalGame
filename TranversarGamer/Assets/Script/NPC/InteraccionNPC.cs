using UnityEngine;

public class InteraccionNPC : MonoBehaviour
{
    [Header("Configuración del Evento")]
    [Tooltip("El objeto que quieres que aparezca o desaparezca")]
    public GameObject objetoAActivar;

    [Tooltip("Marca la casilla si quieres que se ACTIVE. Desmárcala si quieres que se DESACTIVE.")]
    public bool activarObjeto = true;

    [Header("Requisito de Inventario")]
    [Tooltip("Arrastra aquí el ScriptableObject (ItemData) que el jugador necesita tener.")]
    public ItemData itemRequerido;

    [Header("Controles")]
    [Tooltip("La tecla que el jugador debe pulsar para interactuar.")]
    public KeyCode teclaInteraccion = KeyCode.R; // <-- AQUÍ ESTÁ LA NUEVA VARIABLE

    private bool jugadorCerca = false;

    void Update()
    {
        // Solo permite interactuar si el jugador está cerca Y NO hay un diálogo activo
        bool puedeInteractuar = jugadorCerca && (DialogueManager.Instance == null || !DialogueManager.Instance.estaHablando);

        if (puedeInteractuar && Input.GetKeyDown(teclaInteraccion))
        {
            IntentarInteraccion();
        }
    }

    void IntentarInteraccion()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.objetosRecogidos.Contains(itemRequerido))
        {
            Debug.Log($"¡Tienes el ítem: {itemRequerido.nombre}! Ejecutando acción...");

            if (objetoAActivar != null)
            {
                objetoAActivar.SetActive(activarObjeto);
            }

            InventoryManager.Instance.EliminarItem(itemRequerido);
        }
        else
        {
            Debug.Log($"Te falta el ítem: {(itemRequerido != null ? itemRequerido.nombre : "Ninguno asignado")}. No ocurre nada.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            // El mensaje de consola también se adapta a la tecla que elijas
            Debug.Log($"Presiona '{teclaInteraccion.ToString()}' para interactuar con el animal.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}