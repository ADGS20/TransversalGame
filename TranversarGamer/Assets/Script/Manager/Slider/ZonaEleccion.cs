//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

public class ZonaEleccion : MonoBehaviour
{
    // Canvas que contiene las opciones de elección
    public GameObject canvasEleccion;

    private void OnTriggerEnter(Collider other)
    {
        // Cuando el jugador entra en la zona, se muestra el menú de elección
        if (other.CompareTag("Player"))
        {
            canvasEleccion.SetActive(true);
        }
    }
}
