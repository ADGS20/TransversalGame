//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// <summary>
/// Zona donde se puede alternar entre el jugador principal y el compañero.
/// Funciona mediante un collider tipo trigger.
/// </summary>
public class ZonaCamPerson : MonoBehaviour
{
    [Header("Configuración")]
    public GameplayManager gameplayManager; // Referencia al gestor de gameplay

    [Header("Visual (Opcional)")]
    public Renderer indicadorVisual;        // Renderer para mostrar estado visual de la zona
    public Color colorActivo = Color.green; // Color cuando el jugador está dentro
    public Color colorInactivo = Color.white;

    void Start()
    {
        // Asegurar que el collider está configurado como trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Si no se asignó un renderer, intentar obtenerlo del mismo objeto
        if (indicadorVisual == null)
        {
            indicadorVisual = GetComponent<Renderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Activar cambio de personaje cuando el jugador entra
        if (other.CompareTag("Player"))
        {
            if (gameplayManager != null)
            {
                gameplayManager.HabilitarCambio();

                // Cambiar color del indicador visual
                if (indicadorVisual != null)
                {
                    indicadorVisual.material.color = colorActivo;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Desactivar cambio de personaje cuando el jugador sale
        if (other.CompareTag("Player"))
        {
            if (gameplayManager != null)
            {
                gameplayManager.DeshabilitarCambio();

                // Restaurar color del indicador visual
                if (indicadorVisual != null)
                {
                    indicadorVisual.material.color = colorInactivo;
                }
            }
        }
    }
}