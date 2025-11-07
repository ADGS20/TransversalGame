using UnityEngine;

public class ZonaEleccion : MonoBehaviour
{
    public GameObject canvasEleccion; // Asigna el Canvas de elecciones aquí

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el jugador tenga el tag "Player"
        {
            canvasEleccion.SetActive(true);
            // Opcional: pausa el movimiento del jugador aquí si quieres
        }
    }
}