using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para usar el Dropdown moderno
using UnityEngine.UI; // Para usar el Toggle

public class Configuraciones : MonoBehaviour
{
    public TMP_Dropdown dropdownResoluciones;
    public Toggle togglePantallaCompleta;
    Resolution[] resoluciones;

    void Start()
    {
        // 1. Ver qué resoluciones aguanta tu monitor
        resoluciones = Screen.resolutions;
        dropdownResoluciones.ClearOptions();

        List<string> opciones = new List<string>();
        int resolucionActual = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string nombreOpcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(nombreOpcion);

            // Saber cuál es la resolución que tienes puesta ahora mismo
            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
            }
        }

        // 2. Llenar la lista del menú
        dropdownResoluciones.AddOptions(opciones);
        dropdownResoluciones.value = resolucionActual;
        dropdownResoluciones.RefreshShownValue();

        // 3. Ver si ya estás en pantalla completa o no
        togglePantallaCompleta.isOn = Screen.fullScreen;
    }

    // Función para cambiar la resolución
    public void CambiarResolucion(int indice)
    {
        Resolution res = resoluciones[indice];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    // Función para poner o quitar pantalla completa
    public void CambiarPantallaCompleta(bool estaActivado)
    {
        Screen.fullScreen = estaActivado;
    }
}