//------------Creador de este script------------//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----------------------------------------------//

using UnityEngine;

public class BreakableBranch : MonoBehaviour
{
    [Header("Visual de la rama")]
    public GameObject ramaEntera; // Modelo de la rama intacta
    public GameObject ramaRota;   // Modelo de la rama rota

    [Header("Objeto pesado existente en la escena")]
    public Rigidbody objetoPesado; // Objeto que caerá al romper la rama

    [Header("Opciones")]
    public float fuerzaCaida = -5f;      // Fuerza aplicada hacia abajo al objeto pesado
    public float tiempoDestruirRama = 2f; // Tiempo antes de destruir la rama rota

    private bool rota = false; // Evita romper la rama más de una vez

    private void Start()
    {
        // Al inicio, el objeto pesado está bloqueado
        if (objetoPesado != null)
        {
            objetoPesado.useGravity = false;
            objetoPesado.isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Evitar múltiples activaciones
        if (rota) return;

        // Si la rama es golpeada por un proyectil, se rompe
        if (collision.gameObject.CompareTag("Projectile"))
        {
            RomperRama();
        }
    }

    void RomperRama()
    {
        rota = true;

        // Cambiar visual de rama entera a rota
        if (ramaEntera != null)
            ramaEntera.SetActive(false);

        if (ramaRota != null)
            ramaRota.SetActive(true);

        // Liberar el objeto pesado
        if (objetoPesado != null)
        {
            objetoPesado.isKinematic = false;
            objetoPesado.useGravity = true;

            // Aplicar impulso hacia abajo
            objetoPesado.AddForce(Vector3.up * fuerzaCaida, ForceMode.Impulse);
        }

        // Destruir la rama después de un tiempo
        if (tiempoDestruirRama > 0)
            Destroy(gameObject, tiempoDestruirRama);
    }
}