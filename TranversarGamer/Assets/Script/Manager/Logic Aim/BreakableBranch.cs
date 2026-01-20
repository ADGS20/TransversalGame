//------------Creador de este script------------//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----------------------------------------------//
using UnityEngine;

public class BreakableBranch : MonoBehaviour
{
    [Header("Visual de la rama")]
    public GameObject ramaEntera;
    public GameObject ramaRota;

    [Header("Objeto pesado existente en la escena")]
    public Rigidbody objetoPesado;

    [Header("Opciones")]
    public float fuerzaCaida = -5f;
    public float tiempoDestruirRama = 2f;

    private bool rota = false;

    private void Start()
    {
        // 🔒 Al inicio, el objeto pesado está bloqueado
        if (objetoPesado != null)
        {
            objetoPesado.useGravity = false;
            objetoPesado.isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rota) return;

        if (collision.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("💥 Rama golpeada por la piedra");
            RomperRama();
        }
    }

    void RomperRama()
    {
        rota = true;

        if (ramaEntera != null)
            ramaEntera.SetActive(false);

        if (ramaRota != null)
            ramaRota.SetActive(true);

        if (objetoPesado != null)
        {
            // 🔓 Activar físicas del objeto pesado
            objetoPesado.isKinematic = false;
            objetoPesado.useGravity = true;

            // 💨 Aplicar impulso hacia abajo
            objetoPesado.AddForce(Vector3.up * fuerzaCaida, ForceMode.Impulse);

            Debug.Log("🪵 Objeto pesado liberado");
        }

        if (tiempoDestruirRama > 0)
            Destroy(gameObject, tiempoDestruirRama);
    }
}
