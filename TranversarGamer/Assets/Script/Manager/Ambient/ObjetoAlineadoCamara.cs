//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

public class ObjetoAlineadoCamara : MonoBehaviour
{
    public CameraOrbital camaraOrbital; // Referencia a la cámara orbital
    private float rotacionXFija = 30f;  // Rotación fija en el eje X para mantener inclinación constante

    void Start()
    {
        // Si no se asignó manualmente, buscar automáticamente una cámara orbital en la escena
        if (camaraOrbital == null)
        {
            camaraOrbital = Object.FindFirstObjectByType<CameraOrbital>();

            if (camaraOrbital == null)
            {
                Debug.LogWarning("No se encontró ningún objeto con CameraOrbital en la escena.");
            }
        }
    }

    void Update()
    {
        if (camaraOrbital == null) return;

        // Obtener el ángulo Y actual de la cámara orbital
        float anguloCamara = camaraOrbital.ObtenerAnguloActual();

        // Alinear el objeto con la rotación horizontal de la cámara, manteniendo X fijo
        transform.rotation = Quaternion.Euler(rotacionXFija, anguloCamara, 0f);
    }
}