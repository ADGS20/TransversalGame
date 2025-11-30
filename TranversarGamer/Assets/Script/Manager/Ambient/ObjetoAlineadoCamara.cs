//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

public class ObjetoAlineadoCamara : MonoBehaviour
{
    public CameraOrbital camaraOrbital; // Opcional: puedes arrastrarlo manualmente
    private float rotacionXFija = 30f; // Rotación fija en el eje X

    void Start()
    {
        // Si no se asignó manualmente, busca automáticamente
        if (camaraOrbital == null)
        {
            camaraOrbital = Object.FindFirstObjectByType<CameraOrbital>();

            if (camaraOrbital == null)
            {
                Debug.LogWarning("No se encontró ningún objeto con el script CameraOrbital en la escena.");
            }
        }
    }

    void Update()
    {
        if (camaraOrbital == null) return;

        float anguloCamara = camaraOrbital.ObtenerAnguloActual();

        // Alinea el objeto con el eje Y de la cámara, manteniendo X fijo
        transform.rotation = Quaternion.Euler(rotacionXFija, anguloCamara, 0f);
    }
}