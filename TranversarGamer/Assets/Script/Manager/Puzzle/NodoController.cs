//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using System.Collections.Generic;
using UnityEngine;

public class NodoController : MonoBehaviour
{
    // ---------------------------
    // CONFIGURACIÓN DEL NODO
    // ---------------------------

    [Header("Tipo de Nodo")]
    public bool esNaturaleza = true;
    // Determina si el nodo pertenece al lado de la naturaleza o la corrupción.

    [Header("Alcance")]
    public float alcanceBase = 5f;
    // Distancia máxima para conectar sin criatura encima.
    public float alcanceExtraConCriatura = 3f;
    // Alcance adicional cuando una criatura está sobre el nodo.

    [Header("Conexiones posibles")]
    public List<NodoController> nodosConectables = new List<NodoController>();
    // Lista de nodos a los que este nodo puede conectarse.

    [Header("Colores")]
    public Color grisNoAlcanza = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color grisNormal = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color grisConCriatura = new Color(0.75f, 0.75f, 0.75f, 1f);
    public Color colorNaturaleza = new Color(0.2f, 1f, 0.2f);
    public Color colorCorrupcion = new Color(0.7f, 0f, 0.7f);
    public float intensidadBrillo = 3f;
    // Colores usados para representar estados visuales de las conexiones.

    [Header("Curva de raíz")]
    public int puntosPorLinea = 9;
    // Cantidad de puntos para interpolar la curva de la línea.
    public float desviacionLateralMin = 0.4f;
    public float desviacionLateralMax = 1.2f;
    public float extraDesvioObstaculo = 0.8f;
    public LayerMask obstaculos;
    // Parámetros para generar curvas orgánicas y evitar obstáculos.

    [Header("Estado del nodo")]
    public bool estaActivado = false;
    // Indica si el nodo está recibiendo señal.
    public bool enPosicionCorrecta = false;
    // Solo nodos colocados correctamente pueden transmitir señal.

    private bool jugadorCerca = false;
    private bool criaturaEncima = false;
    private NodoController nodoConectado = null;
    // Estados dinámicos del nodo.

    // Datos internos para cada conexión visual
    private class ConexionData
    {
        public LineRenderer lr;
        public float lateralSign;
        public float lateralOffset;
    }

    private Dictionary<NodoController, ConexionData> conexiones = new Dictionary<NodoController, ConexionData>();


    private void Start()
    {
        // Asegurar un mínimo de puntos para la curva
        if (puntosPorLinea < 3) puntosPorLinea = 3;

        // Crear un LineRenderer por cada nodo conectable
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

            // Configuración aleatoria para dar variación orgánica
            ConexionData data = new ConexionData();
            data.lr = lr;
            data.lateralSign = Random.value < 0.5f ? -1f : 1f;
            data.lateralOffset = Random.Range(desviacionLateralMin, desviacionLateralMax);

            conexiones[objetivo] = data;
        }
    }


    private void Update()
    {
        // Determinar alcance según si hay criatura encima
        float alcanceActual = criaturaEncima ? alcanceBase + alcanceExtraConCriatura : alcanceBase;

        // Actualizar cada línea visual
        foreach (var kvp in conexiones)
        {
            NodoController objetivo = kvp.Key;
            ConexionData data = kvp.Value;
            LineRenderer lr = data.lr;

            // Puntos inicial y final elevados ligeramente
            Vector3 p0 = transform.position + Vector3.up * 0.15f;
            Vector3 pEnd = objetivo.transform.position + Vector3.up * 0.15f;

            Vector3 dir = (pEnd - p0).normalized;
            float distancia = Vector3.Distance(p0, pEnd);

            // Dirección lateral para la curva
            Vector3 lateral = Vector3.Cross(dir, Vector3.up).normalized * data.lateralSign;

            float desvio = data.lateralOffset;

            // Si hay obstáculo, aumentar desviación
            if (Physics.Raycast(p0, dir, distancia, obstaculos))
                desvio += extraDesvioObstaculo;

            // Generar puntos de la curva
            for (int i = 0; i < puntosPorLinea; i++)
            {
                float t = i / (float)(puntosPorLinea - 1);
                Vector3 basePos = Vector3.Lerp(p0, pEnd, t);

                float curvaLateral = Mathf.Sin(t * Mathf.PI) * desvio;

                Vector3 pos = basePos + lateral * curvaLateral;
                lr.SetPosition(i, pos);
            }

            // Si estaba conectado pero se salió del alcance, cortar señal
            if (nodoConectado == objetivo && distancia > alcanceActual)
            {
                nodoConectado = null;
                CortarSenal();
            }

            // Colorear según estado
            if (nodoConectado == objetivo)
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
        }

        // Intentar conectar si el jugador está cerca y presiona F
        if (jugadorCerca && Input.GetKeyDown(KeyCode.F))
            IntentarConectar();
    }


    public void IntentarConectar()
    {
        // Intento de conexión manual por parte del jugador
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
        // Evitar bucles infinitos
        if (estaActivado) return;

        // Solo nodos bien colocados pueden activarse
        if (!enPosicionCorrecta)
        {
            estaActivado = false;
            return;
        }

        estaActivado = true;

        // Propagar señal si hay un nodo conectado
        if (nodoConectado != null)
            nodoConectado.RecibirSenal();
    }


    public void CortarSenal()
    {
        if (!estaActivado) return;

        estaActivado = false;

        // Propagar corte hacia adelante
        if (nodoConectado != null)
            nodoConectado.CortarSenal();
    }


    private void OnTriggerEnter(Collider other)
    {
        // Detectar jugador
        if (other.GetComponent<Mov_Player3D>() != null)
            jugadorCerca = true;

        // Detectar criatura encima
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
