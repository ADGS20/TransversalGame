using System.Collections.Generic;
using UnityEngine;

public class NodoController : MonoBehaviour
{
    [Header("Tipo de Nodo")]
    public bool esNaturaleza = true;

    [Header("Alcance")]
    public float alcanceBase = 5f;
    public float alcanceExtraConCriatura = 3f;

    [Header("Conexiones posibles")]
    public List<NodoController> nodosConectables = new List<NodoController>();

    [Header("Colores")]
    public Color grisNoAlcanza = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color grisNormal = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color grisConCriatura = new Color(0.75f, 0.75f, 0.75f, 1f);
    public Color colorNaturaleza = new Color(0.2f, 1f, 0.2f);
    public Color colorCorrupcion = new Color(0.7f, 0f, 0.7f);
    public float intensidadBrillo = 3f;

    [Header("Curva de raíz")]
    public int puntosPorLinea = 9;
    public float desviacionLateralMin = 0.4f;
    public float desviacionLateralMax = 1.2f;
    public float extraDesvioObstaculo = 0.8f;
    public LayerMask obstaculos;

    [Header("Estado del nodo")]
    public bool estaActivado = false;
    public bool enPosicionCorrecta = false;

    [Header("Restricciones")]
    public bool requiereTotemParaActivar = false;

    private bool jugadorCerca = false;
    private bool criaturaEncima = false;
    private NodoController nodoConectado = null;

    private class ConexionData
    {
        public LineRenderer lr;
        public float lateralSign;
        public float lateralOffset;
    }

    private Dictionary<NodoController, ConexionData> conexiones = new Dictionary<NodoController, ConexionData>();

    private void Start()
    {
        if (puntosPorLinea < 3) puntosPorLinea = 3;

        foreach (NodoController objetivo in nodosConectables)
        {
            if (objetivo == null) continue;

            GameObject lineaGO = new GameObject($"Linea_{name}_to_{objetivo.name}");
            lineaGO.transform.SetParent(transform);

            LineRenderer lr = lineaGO.AddComponent<LineRenderer>();
            lr.positionCount = puntosPorLinea;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.widthMultiplier = 0.08f;
            lr.useWorldSpace = true;

            ConexionData data = new ConexionData();
            data.lr = lr;
            data.lateralSign = Random.value < 0.5f ? -1f : 1f;
            data.lateralOffset = Random.Range(desviacionLateralMin, desviacionLateralMax);

            conexiones[objetivo] = data;
        }
    }

    private void Update()
    {
        float alcanceActual = criaturaEncima ? alcanceBase + alcanceExtraConCriatura : alcanceBase;

        foreach (var kvp in conexiones)
        {
            NodoController objetivo = kvp.Key;
            ConexionData data = kvp.Value;
            LineRenderer lr = data.lr;

            Vector3 p0 = transform.position + Vector3.up * 0.15f;
            Vector3 pEnd = objetivo.transform.position + Vector3.up * 0.15f;

            Vector3 dir = (pEnd - p0).normalized;
            float distancia = Vector3.Distance(p0, pEnd);

            Vector3 lateral = Vector3.Cross(dir, Vector3.up).normalized * data.lateralSign;

            float desvio = data.lateralOffset;
            if (Physics.Raycast(p0, dir, distancia, obstaculos))
                desvio += extraDesvioObstaculo;

            for (int i = 0; i < puntosPorLinea; i++)
            {
                float t = i / (float)(puntosPorLinea - 1);
                Vector3 basePos = Vector3.Lerp(p0, pEnd, t);

                float curvaLateral = Mathf.Sin(t * Mathf.PI) * desvio;

                Vector3 pos = basePos;
                pos += lateral * curvaLateral;

                lr.SetPosition(i, pos);
            }

            // -----------------------------
            // BLOQUE VISUAL FINAL
            // -----------------------------
            if (estaActivado)
            {
                Color c = esNaturaleza ? colorNaturaleza : colorCorrupcion;
                c *= intensidadBrillo;
                lr.startColor = c;
                lr.endColor = c;
            }
            else
            {
                if (distancia <= alcanceActual)
                {
                    Color c = criaturaEncima ? grisConCriatura : grisNormal;
                    lr.startColor = c;
                    lr.endColor = c;
                }
                else
                {
                    lr.startColor = grisNoAlcanza;
                    lr.endColor = grisNoAlcanza;
                }
            }
            // -----------------------------
        }

        if (jugadorCerca && Input.GetKeyDown(KeyCode.F))
            IntentarConectar();
    }

    public void IntentarConectar()
    {
        // Si requiere tótem y no está en posición → NO se activa
        if (requiereTotemParaActivar && !enPosicionCorrecta)
        {
            Debug.Log($"[{name}] No puede activarse: falta tótem.");
            return;
        }

        float alcanceActual = criaturaEncima ? alcanceBase + alcanceExtraConCriatura : alcanceBase;

        foreach (NodoController objetivo in nodosConectables)
        {
            float distancia = Vector3.Distance(transform.position, objetivo.transform.position);

            if (distancia <= alcanceActual)
            {
                nodoConectado = objetivo;
                estaActivado = true;
                objetivo.RecibirSenal();
                return;
            }
        }
    }

    public void RecibirSenal()
    {
        if (estaActivado) return;

        if (requiereTotemParaActivar && !enPosicionCorrecta)
        {
            estaActivado = false;
            return;
        }

        estaActivado = true;

        if (nodoConectado != null)
            nodoConectado.RecibirSenal();
    }

    public void CortarSenal()
    {
        if (!estaActivado) return;

        estaActivado = false;

        if (nodoConectado != null)
            nodoConectado.CortarSenal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Mov_Player3D>() != null)
            jugadorCerca = true;

        if (other.CompareTag("Criatura"))
            criaturaEncima = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Mov_Player3D>() != null)
            jugadorCerca = false;

        if (other.CompareTag("Criatura"))
            criaturaEncima = false;
    }
}
