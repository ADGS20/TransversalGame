//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;
using UnityEngine.UI;

public class BarraInfluencia : MonoBehaviour
{
    public Slider slider; // Slider vertical cuyo rango va de -100 a 100

    // Imágenes superpuestas del handle, cada una representa un estado visual distinto
    public Image handleCorrompido;
    public Image handleNeutral;
    public Image handleLuminoso;

    void Update()
    {
        // Valor actual del slider, usado para determinar qué imagen mostrar
        float valor = slider.value; // Rango esperado: -100 a 100

        // Reiniciar la transparencia de todas las imágenes
        handleCorrompido.color = new Color(1, 1, 1, 0);
        handleNeutral.color = new Color(1, 1, 1, 0);
        handleLuminoso.color = new Color(1, 1, 1, 0);

        // Si el valor está en la zona negativa fuerte
        if (valor < -50)
        {
            // Interpolación entre la imagen corrompida y la neutral
            float t = Mathf.InverseLerp(-100, -50, valor);

            // A mayor valor, más se desvanece la imagen corrompida y aparece la neutral
            handleCorrompido.color = new Color(1, 1, 1, 1 - t);
            handleNeutral.color = new Color(1, 1, 1, t);
        }
        // Si el valor está en la zona positiva fuerte
        else if (valor > 50)
        {
            // Interpolación entre la imagen neutral y la luminosa
            float t = Mathf.InverseLerp(50, 100, valor);

            // A mayor valor, más se desvanece la neutral y aparece la luminosa
            handleNeutral.color = new Color(1, 1, 1, 1 - t);
            handleLuminoso.color = new Color(1, 1, 1, t);
        }
        else
        {
            // En la zona central solo se muestra la imagen neutral
            handleNeutral.color = new Color(1, 1, 1, 1);
        }
    }
}
