using UnityEngine;

public class GuardadoZonaUmbral : MonoBehaviour
{
    [Header("Conexión")]
    public HabilidadShaderController controladorZona;

    [Header("Identificador Único")]
    [Tooltip("Pon un nombre distinto para cada zona del juego, ej: 'Puerta_Bosque_1'")]
    public string idUnicoZona = "Zona_01";

    private bool zonaCompletada = false;

    void Start()
    {
        // 1. Buscamos tu script original que ya está en este mismo objeto
        if (controladorZona == null) controladorZona = GetComponent<HabilidadShaderController>();

        // 2. Leemos la memoria: ¿Ya pasamos por aquí antes?
        if (PlayerPrefs.GetInt(idUnicoZona + "_Completada", 0) == 1)
        {
            zonaCompletada = true;
            AplicarEstadoPermanente(); // Mostramos todo de golpe
        }
    }

    void Update()
    {
        // 3. Si no hemos pasado por aquí, vigilamos si pulsas la tecla V o C
        if (!zonaCompletada && controladorZona != null)
        {
            bool magiaFueUsada = false;

            // Revisamos si tu script original activó algún objeto
            if (controladorZona.caminosCuracion.Count > 0 && controladorZona.caminosCuracion[0].activeSelf)
                magiaFueUsada = true;

            if (controladorZona.caminosCorrupcion.Count > 0 && !controladorZona.caminosCorrupcion[0].activeSelf)
                magiaFueUsada = true;

            // Si detectamos que se activaron, lo anotamos en el Libro de Memoria
            if (magiaFueUsada)
            {
                GuardarPartida();
            }
        }
    }

    private void GuardarPartida()
    {
        zonaCompletada = true;
        PlayerPrefs.SetInt(idUnicoZona + "_Completada", 1);
        PlayerPrefs.Save();
    }

    private void AplicarEstadoPermanente()
    {
        // Forzamos a que los objetos se vean sin hacer la animación de la onda
        foreach (GameObject obj in controladorZona.caminosCuracion)
        {
            if (obj != null) obj.SetActive(true);
        }

        foreach (GameObject obj in controladorZona.caminosCorrupcion)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}