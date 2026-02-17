using UnityEngine;

public class ItemFisico : MonoBehaviour
{
    [Header("Identificador Único en el Mapa")]
    [Tooltip("Pon un nombre único para ESTE objeto exacto, ej: pocion_bosque_1")]
    public string idUnicoMundo;

    [Header("Datos del Objeto")]
    public ItemData itemQueOtorga;
    public bool recogerAutomaticamente = true;

    private bool jugadorCerca = false;

    private void Start()
    {
        // NUEVO: Le damos a Unity 0.1 segundos para que el SaveSystem termine de cargar la partida
        Invoke("ComprobarSiYaSeRecogio", 0.1f);
    }

    private void ComprobarSiYaSeRecogio()
    {
        // Ahora sí preguntamos si este objeto ya estaba en la lista de destruidos
        if (InventoryManager.Instance != null && InventoryManager.Instance.objetosDestruidosMundo.Contains(idUnicoMundo))
        {
            Destroy(gameObject); // Ya se recogió en el pasado, nos autodestruimos
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (recogerAutomaticamente) Recoger();
            else
            {
                jugadorCerca = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = false;
    }

    private void Update()
    {
        if (!recogerAutomaticamente && jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Recoger();
        }
    }

    private void Recoger()
    {
        InventoryManager.Instance.AgregarItem(itemQueOtorga);

        // Guardamos que ESTE objeto del mapa ya no existe
        if (!string.IsNullOrEmpty(idUnicoMundo))
        {
            InventoryManager.Instance.objetosDestruidosMundo.Add(idUnicoMundo);
        }

        Destroy(gameObject);
    }
}