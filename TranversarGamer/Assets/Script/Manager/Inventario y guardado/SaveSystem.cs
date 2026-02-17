using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // NUEVO: Necesario para leer el Nivel actual

public class SaveSystem : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    public Transform jugador;
    public CameraOrbital camaraOrbital;

    [Header("Feedback Visual")]
    [Tooltip("Arrastra aquí el texto o panel que dirá 'Partida Guardada'")]
    public GameObject cartelGuardado;

    // Aquí guardaremos las decisiones temporalmente mientras jugamos
    public List<int> decisionesActuales = new List<int>();

    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");

        // Nos aseguramos de que el cartel empiece apagado al abrir el juego
        if (cartelGuardado != null) cartelGuardado.SetActive(false);
    }

    private void Start()
    {
        CargarJuego();
    }

    public void GuardarJuego()
    {
        GameData data = new GameData();

        // 1. Guardar Nivel Dinámico (Lee el número de la escena actual)
        data.nivelCompletado = SceneManager.GetActiveScene().buildIndex;

        // 2. Guardar Inventario y Objetos destruidos
        foreach (var item in InventoryManager.Instance.objetosRecogidos)
        {
            data.inventarioIDs.Add(item.id);
        }
        data.objetosDestruidosMundo = new List<string>(InventoryManager.Instance.objetosDestruidosMundo);

        // 3. Guardar Posición del Jugador
        if (jugador != null)
        {
            data.posicionX = jugador.position.x;
            data.posicionY = jugador.position.y;
            data.posicionZ = jugador.position.z;
        }

        // 4. Guardar Lógica de la Cámara
        if (camaraOrbital != null)
        {
            data.camaraAngulo = camaraOrbital.ObtenerAngulo();
            data.camaraZoom = camaraOrbital.ObtenerZoom();
        }

        // 5. Guardar Decisiones (Misiones, cofres, jefes)
        data.decisionesTomadas = new List<int>(decisionesActuales);

        // ESCRIBIR EN EL DISCO
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Partida guardada en el Nivel: " + data.nivelCompletado);

        // --- FEEDBACK VISUAL PARA EL JUGADOR ---
        if (cartelGuardado != null)
        {
            cartelGuardado.SetActive(true); // Encendemos el mensaje
            Invoke("OcultarCartel", 2f);    // Lo apagamos después de 2 segundos
        }
    }

    private void OcultarCartel()
    {
        if (cartelGuardado != null) cartelGuardado.SetActive(false);
    }

    public void CargarJuego()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            // Cargar inventario y objetos destruidos
            InventoryManager.Instance.objetosRecogidos.Clear();
            foreach (Transform hijo in InventoryManager.Instance.contenedorDeIconos) Destroy(hijo.gameObject);
            InventoryManager.Instance.objetosDestruidosMundo = new List<string>(data.objetosDestruidosMundo);

            ItemData[] todosLosItemsPosibles = Resources.LoadAll<ItemData>("Items");
            foreach (string idGuardado in data.inventarioIDs)
            {
                ItemData itemEncontrado = System.Array.Find(todosLosItemsPosibles, item => item.id == idGuardado);
                if (itemEncontrado != null) InventoryManager.Instance.AgregarItem(itemEncontrado);
            }

            // Cargar posición
            if (jugador != null)
            {
                CharacterController cc = jugador.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                jugador.position = new Vector3(data.posicionX, data.posicionY, data.posicionZ);
                Physics.SyncTransforms();
                if (cc != null) cc.enabled = true;
            }

            // Cargar cámara
            if (camaraOrbital != null)
            {
                camaraOrbital.CargarDatosCamara(data.camaraAngulo, data.camaraZoom);
            }

            // Cargar decisiones
            decisionesActuales = new List<int>(data.decisionesTomadas);

            Debug.Log("Partida Cargada automáticamente. Nivel actual: " + data.nivelCompletado);
        }
    }

    public void NuevaPartida()
    {
        if (File.Exists(savePath)) File.Delete(savePath);
        InventoryManager.Instance.objetosRecogidos.Clear();
        InventoryManager.Instance.objetosDestruidosMundo.Clear();
        foreach (Transform hijo in InventoryManager.Instance.contenedorDeIconos) Destroy(hijo.gameObject);

        decisionesActuales.Clear(); // Limpiamos las decisiones

        if (jugador != null)
        {
            CharacterController cc = jugador.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            jugador.position = new Vector3(0f, 0f, 0f);
            Physics.SyncTransforms();
            if (cc != null) cc.enabled = true;
        }

        if (camaraOrbital != null) camaraOrbital.CargarDatosCamara(0f, 0.5f);
    }

    // --- NUEVAS HERRAMIENTAS PARA MISIONES / EVENTOS ---

    // Llama a esto cuando el jugador haga algo importante (ej: matar al jefe 1)
    public void GuardarDecision(int idDecision)
    {
        if (!decisionesActuales.Contains(idDecision))
        {
            decisionesActuales.Add(idDecision);
        }
    }

    // Llama a esto para preguntar si ya hizo algo (ej: ¿ya habló con el rey?)
    public bool YaTomoDecision(int idDecision)
    {
        return decisionesActuales.Contains(idDecision);
    }
}