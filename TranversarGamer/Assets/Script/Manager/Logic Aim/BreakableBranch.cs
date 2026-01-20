using UnityEngine;

public class BreakableBranch : MonoBehaviour
{
    [Header("Visual de la rama")]
    public GameObject ramaEntera;   // Rama normal
    public GameObject ramaRota;     // Rama rota (opcional)

    [Header("Objeto pesado que caerá")]
    public GameObject objetoPesadoPrefab; // Prefab del tronco/roca
    public Transform puntoCaida;          // Empty donde aparecerá

    [Header("Opciones")]
    public float fuerzaCaida = -5f;       // Impulso inicial hacia abajo
    public float tiempoDestruirRama = 2f; // Limpieza opcional

    private bool rota = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (rota) return;

        // Detectar impacto de la piedra
        if (collision.gameObject.CompareTag("Projectile"))
        {
            RomperRama();
        }
    }

    void RomperRama()
    {
        rota = true;

        // Cambiar visual
        if (ramaEntera != null)
            ramaEntera.SetActive(false);

        if (ramaRota != null)
            ramaRota.SetActive(true);

        // Instanciar el objeto pesado
        if (objetoPesadoPrefab != null && puntoCaida != null)
        {
            GameObject obj = Instantiate(objetoPesadoPrefab, puntoCaida.position, Quaternion.identity);

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.AddForce(Vector3.up * fuerzaCaida, ForceMode.Impulse);
            }
        }

        // Destruir rama si quieres limpiar
        if (tiempoDestruirRama > 0)
            Destroy(gameObject, tiempoDestruirRama);
    }
}

