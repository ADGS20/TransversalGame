using UnityEngine;

/// <summary>
/// Zona donde se puede alternar entre el jugador principal y el compañero
/// Similar a las zonas especiales en Mario & Luigi: Partners in Time
/// </summary>
public class ZonaCamPerson : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Referencia al GameplayManager")]
    public GameplayManager gameplayManager;

    [Header("Visual (Opcional)")]
    [Tooltip("Sprite o efecto visual para indicar la zona")]
    public SpriteRenderer indicadorVisual;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el jugador principal entró en la zona
        if (collision.CompareTag("Player"))
        {
            if (gameplayManager != null)
            {
                gameplayManager.HabilitarCambio();
                Debug.Log("Entraste en una zona de cambio. Presiona Tab para alternar personajes.");

                // Activar indicador visual si existe
                if (indicadorVisual != null)
                {
                    indicadorVisual.color = Color.green;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Verificar si el jugador principal salió de la zona
        if (collision.CompareTag("Player"))
        {
            if (gameplayManager != null)
            {
                gameplayManager.DeshabilitarCambio();
                Debug.Log("Saliste de la zona de cambio.");

                // Desactivar indicador visual si existe
                if (indicadorVisual != null)
                {
                    indicadorVisual.color = Color.white;
                }
            }
        }
    }
}