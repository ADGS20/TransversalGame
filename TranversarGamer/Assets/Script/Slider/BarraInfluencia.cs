using UnityEngine;
using UnityEngine.UI;

public class BarraInfluencia : MonoBehaviour
{
    public Slider slider; // Vertical, -100 a 100

    // Imágenes del handle (superpuestas)
    public Image handleCorrompido;
    public Image handleNeutral;
    public Image handleLuminoso;

    void Update()
    {
        float valor = slider.value; // -100 a 100

        // Reset alphas
        handleCorrompido.color = new Color(1, 1, 1, 0);
        handleNeutral.color = new Color(1, 1, 1, 0);
        handleLuminoso.color = new Color(1, 1, 1, 0);

        if (valor < -50)
        {
            // Crossfade entre corrompido y neutral
            float t = Mathf.InverseLerp(-100, -50, valor);
            handleCorrompido.color = new Color(1, 1, 1, 1 - t);
            handleNeutral.color = new Color(1, 1, 1, t);
        }
        else if (valor > 50)
        {
            // Crossfade entre neutral y luminoso
            float t = Mathf.InverseLerp(50, 100, valor);
            handleNeutral.color = new Color(1, 1, 1, 1 - t);
            handleLuminoso.color = new Color(1, 1, 1, t);
        }
        else
        {
            // Solo neutral visible
            handleNeutral.color = new Color(1, 1, 1, 1);
        }
    }
}