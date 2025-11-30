//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

public class ZonaEleccion : MonoBehaviour
{
    public GameObject canvasEleccion; // Asigna el Canvas de elecciones aquí

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvasEleccion.SetActive(true);
        }
    }
}