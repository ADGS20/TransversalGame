using UnityEngine;

/// <summary>
/// Zona donde se puede alternar entre el jugador principal y el compañero
/// Similar a las zonas especiales en Mario & Luigi: Partners in Time
/// Versión para 3D con Collider 3D
/// </summary>
public class ZonaCamPerson : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Referencia al GameplayManager")]
    public GameplayManager gameplayManager;

    [Header("Visual (Opcional)")]
    [Tooltip("Material o renderer para indicar la zona")]
    public Renderer indicadorVisual;

    [Tooltip("Color cuando la zona está activa")]
    public Color colorActivo = Color.green;

    [Tooltip("Color cuando la zona está inactiva")]
    public Color colorInactivo = Color.white; // ← AQUÍ ESTABA EL ERROR

    void Start()
    {
        // Asegurar que el collider es trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Obtener renderer si no está asignado
        if (indicadorVisual == null)
        {
            indicadorVisual = GetComponent<Renderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el jugador principal entró en la zona
        if (other.CompareTag("Player"))
        {
            if (gameplayManager != null)
            {
                gameplayManager.HabilitarCambio();
                Debug.Log("🟢 Entraste en una zona de cambio. Presiona Tab para alternar personajes.");

                // Activar indicador visual si existe
                if (indicadorVisual != null)
                {
                    indicadorVisual.material.color = colorActivo;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verificar si el jugador principal salió de la zona
        if (other.CompareTag("Player"))
        {
            if (gameplayManager != null)
            {
                gameplayManager.DeshabilitarCambio();
                Debug.Log("⚪ Saliste de la zona de cambio.");

                // Desactivar indicador visual si existe
                if (indicadorVisual != null)
                {
                    indicadorVisual.material.color = colorInactivo;
                }
            }
        }
    }
}