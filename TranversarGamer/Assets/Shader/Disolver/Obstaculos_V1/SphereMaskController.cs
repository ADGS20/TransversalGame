using UnityEngine;

public class SphereMaskController : MonoBehaviour
{
    public GameObject esferaJugador;
    public LayerMask capaArboles;

    [Header("Configuración")]
    public float tamañoAgujero = 4f;
    public float velocidadTransicion = 8f;
    public float grosorDelRayo = 0.5f; // Evita parpadeos en modelos externos

    void Update()
    {
        // Dirección desde la cámara hacia el jugador
        Vector3 direccion = esferaJugador.transform.position - transform.position;
        float distancia = direccion.magnitude;

        // Usamos SphereCast para que el rayo tenga "cuerpo" y no falle con las hojas
        bool hayObstaculo = Physics.SphereCast(transform.position, grosorDelRayo, direccion.normalized, out RaycastHit hit, distancia, capaArboles);

        if (hayObstaculo)
        {
            // SÍ hay algo en medio: Agrandamos la esfera
            esferaJugador.transform.localScale = Vector3.Lerp(
                esferaJugador.transform.localScale,
                Vector3.one * tamañoAgujero,
                Time.deltaTime * velocidadTransicion
            );
        }
        else
        {
            // NO hay nada en medio: Encogemos a cero
            esferaJugador.transform.localScale = Vector3.Lerp(
                esferaJugador.transform.localScale,
                Vector3.zero,
                Time.deltaTime * velocidadTransicion
            );
        }
    }
}