//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//


using UnityEngine;
using UnityEngine.UI;

public class EleccionInfluencias : MonoBehaviour
{
    public SliderAnimado sliderAnimado;
    public GameObject canvasEleccion;
    public Mov_Player3D scriptMovimientoJugador;
   

    private void OnEnable()
    {
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = false;
    }

    private void OnDisable()
    {
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = true;
    }

    public void EleccionBuena()
    {
        sliderAnimado.SumarValor(30);
        Debug.Log($"Elección: BUENA (+30). Valor objetivo: {sliderAnimado.valorObjetivo}");
        canvasEleccion.SetActive(false);
    }

    public void EleccionMala()
    {
        sliderAnimado.SumarValor(-30);
        Debug.Log($"Elección: MALA (-30). Valor objetivo: {sliderAnimado.valorObjetivo}");
        canvasEleccion.SetActive(false);
    }
}