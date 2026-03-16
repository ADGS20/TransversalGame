using UnityEngine;
using System.Collections.Generic;

public class ZonaOndaMagica : MonoBehaviour
{
    [Header("Configuración de la Zona")]
    public float radioMaximo = 15f;
    public float segundosExpansion = 2f;

    [Header("--- CUANDO PULSAS V (NATURALEZA) ---")]
    public List<GameObject> aparecenConV;
    public List<GameObject> desaparecenConV;

    [Header("--- CUANDO PULSAS C (CORRUPCIÓN) ---")]
    public List<GameObject> aparecenConC;
    public List<GameObject> desaparecenConC;

    private bool jugadorEnZona = false;
    private bool yaSeUso = false;
    private Transform jugadorTransform;

    private float radioActual = 0f;
    private bool expandiendoV = false;
    private bool expandiendoC = false;
    private Vector3 centroOnda;

    private List<GameObject> pendientesAparecer = new List<GameObject>();
    private List<GameObject> pendientesDesaparecer = new List<GameObject>();

    void Start()
    {
        Shader.SetGlobalFloat("_RadioCreacion", 0f);
        Shader.SetGlobalFloat("_RadioCorrupcion", 0f);

        // 1. APAGAMOS LAS FÍSICAS (COLISIONES) DE LO QUE ESTÁ OCULTO
        foreach (var obj in aparecenConV) { CambiarColision(obj, false); }
        foreach (var obj in aparecenConC) { CambiarColision(obj, false); }

        // 2. ENCENDEMOS LAS FÍSICAS DE LO QUE ESTÁ VISIBLE
        foreach (var obj in desaparecenConV)
        {
            if (obj != null && !aparecenConV.Contains(obj) && !aparecenConC.Contains(obj))
                CambiarColision(obj, true);
        }
        foreach (var obj in desaparecenConC)
        {
            if (obj != null && !aparecenConV.Contains(obj) && !aparecenConC.Contains(obj))
                CambiarColision(obj, true);
        }
    }

    void Update()
    {
        if (jugadorEnZona && !yaSeUso)
        {
            if (Input.GetKeyDown(KeyCode.V)) PrepararOnda(true);
            else if (Input.GetKeyDown(KeyCode.C)) PrepararOnda(false);
        }

        if (expandiendoV)
        {
            radioActual += (radioMaximo / segundosExpansion) * Time.deltaTime;
            Shader.SetGlobalFloat("_RadioCreacion", radioActual);
            ProcesarFisicasAlTocar();
            if (radioActual >= radioMaximo) expandiendoV = false;
        }

        if (expandiendoC)
        {
            radioActual += (radioMaximo / segundosExpansion) * Time.deltaTime;
            Shader.SetGlobalFloat("_RadioCorrupcion", radioActual);
            ProcesarFisicasAlTocar();
            if (radioActual >= radioMaximo) expandiendoC = false;
        }
    }

    private void PrepararOnda(bool esV)
    {
        yaSeUso = true;
        radioActual = 0f;
        centroOnda = jugadorTransform.position;

        if (esV)
        {
            expandiendoV = true;
            Shader.SetGlobalVector("_PosOndaCreacion", centroOnda);
            pendientesAparecer = new List<GameObject>(aparecenConV);
            pendientesDesaparecer = new List<GameObject>(desaparecenConV);
        }
        else
        {
            expandiendoC = true;
            Shader.SetGlobalVector("_PosOndaCorrupcion", centroOnda);
            pendientesAparecer = new List<GameObject>(aparecenConC);
            pendientesDesaparecer = new List<GameObject>(desaparecenConC);
        }
    }

    private void ProcesarFisicasAlTocar()
    {
        // Encendemos colisiones cuando la onda pasa
        for (int i = pendientesAparecer.Count - 1; i >= 0; i--)
        {
            GameObject obj = pendientesAparecer[i];
            if (obj != null && Vector3.Distance(obj.transform.position, centroOnda) <= radioActual)
            {
                CambiarColision(obj, true);
                pendientesAparecer.RemoveAt(i);
            }
        }

        // Apagamos colisiones cuando la onda pasa
        for (int i = pendientesDesaparecer.Count - 1; i >= 0; i--)
        {
            GameObject obj = pendientesDesaparecer[i];
            if (obj != null && Vector3.Distance(obj.transform.position, centroOnda) <= radioActual)
            {
                CambiarColision(obj, false);
                pendientesDesaparecer.RemoveAt(i);
            }
        }
    }

    // Pequeña función para apagar/encender solo el componente Collider
    private void CambiarColision(GameObject obj, bool estado)
    {
        if (obj == null) return;
        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.enabled = estado;
    }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) { jugadorEnZona = true; jugadorTransform = other.transform; } }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) jugadorEnZona = false; }
}