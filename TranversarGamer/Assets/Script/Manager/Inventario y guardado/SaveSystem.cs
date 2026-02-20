using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveSystem : MonoBehaviour
{
    [Header("Referencias (Se autocompletan solas)")]
    public Transform jugador;
    public CameraOrbital camaraOrbital;
    public GameObject cartelGuardado;

    public List<int> decisionesActuales = new List<int>();
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        if (cartelGuardado != null) cartelGuardado.SetActive(false);
    }

    // --- NUEVO: ESTO DETECTA CUANDO CAMBIAS DE ESCENA ---
    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena; // Nos suscribimos al evento
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena; // Nos desuscribimos para evitar errores
    }

    // Esta función se ejecuta AUTOMÁTICAMENTE cada vez que entras a un nivel nuevo
    void AlCargarEscena(Scene scene, LoadSceneMode mode)
    {
        // Solo buscamos si no estamos en el menú principal (asumiendo que menú es índice 0)
        if (scene.buildIndex != 0)
        {
            BuscarReferencias();
        }

        // Si había una carga pendiente (la "nota" del menú), cargamos ahora
        if (PlayerPrefs.GetInt("CargarPartidaPendiente", 0) == 1)
        {
            PlayerPrefs.SetInt("CargarPartidaPendiente", 0);
            CargarJuego();
        }
    }

    // --- LA MAGIA: EL SABUESO ---
    public void BuscarReferencias()
    {
        // 1. Buscar al Jugador por su ETIQUETA (Tag)
        GameObject jugadorEncontrado = GameObject.FindGameObjectWithTag("Player");
        if (jugadorEncontrado != null)
        {
            jugador = jugadorEncontrado.transform;
        }

        // 2. Buscar la Cámara Orbital (NUEVO MÉTODO UNITY 6)
        camaraOrbital = FindFirstObjectByType<CameraOrbital>();

        // 3. (Opcional) Buscar el Cartel de Guardado si se perdió
        if (cartelGuardado == null)
        {
            GameObject cartel = GameObject.Find("TextoGuardado");
            if (cartel != null) cartelGuardado = cartel;
        }
    }
    // -----------------------------------------------------

    private void Start()
    {
        // Ejecutamos la búsqueda también al arrancar por si acaso
        BuscarReferencias();
    }

    public void GuardarJuego()
    {
        // Asegurarnos de tener referencias antes de guardar
        if (jugador == null) BuscarReferencias();

        GameData data = new GameData();
        data.nivelCompletado = SceneManager.GetActiveScene().buildIndex;

        if (InventoryManager.Instance != null)
        {
            foreach (var item in InventoryManager.Instance.objetosRecogidos)
            {
                data.inventarioIDs.Add(item.id);
            }
            data.objetosDestruidosMundo = new List<string>(InventoryManager.Instance.objetosDestruidosMundo);
        }

        if (jugador != null)
        {
            data.posicionX = jugador.position.x;
            data.posicionY = jugador.position.y;
            data.posicionZ = jugador.position.z;
        }

        if (camaraOrbital != null)
        {
            data.camaraAngulo = camaraOrbital.ObtenerAngulo();
            data.camaraZoom = camaraOrbital.ObtenerZoom();
        }

        data.decisionesTomadas = new List<int>(decisionesActuales);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Partida guardada en Nivel: " + data.nivelCompletado);

        if (cartelGuardado != null)
        {
            cartelGuardado.SetActive(true);
            Invoke("OcultarCartel", 2f);
        }

        if (cartelGuardado != null)
        {
            cartelGuardado.SetActive(true);
            // Usamos una Corrutina para ignorar la pausa del juego
            StartCoroutine(ApagarCartelRealtime());
        }

    }

    private IEnumerator ApagarCartelRealtime()
    {
        // Espera 2 segundos de la vida real, aunque el juego esté en Pausa
        yield return new WaitForSecondsRealtime(2f);
        if (cartelGuardado != null) cartelGuardado.SetActive(false);
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

            // Lógica de cambio de escena si estamos en el menú
            if (SceneManager.GetActiveScene().buildIndex != data.nivelCompletado)
            {
                PlayerPrefs.SetInt("CargarPartidaPendiente", 1);
                SceneManager.LoadScene(data.nivelCompletado);
                return;
            }

            // --- APLICAR DATOS ---
            // Nos aseguramos DE NUEVO de que tenemos al jugador localizado
            if (jugador == null) BuscarReferencias();

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.objetosRecogidos.Clear();
                foreach (Transform hijo in InventoryManager.Instance.contenedorDeIconos) Destroy(hijo.gameObject);
                InventoryManager.Instance.objetosDestruidosMundo = new List<string>(data.objetosDestruidosMundo);

                ItemData[] todosLosItemsPosibles = Resources.LoadAll<ItemData>("Items");
                foreach (string idGuardado in data.inventarioIDs)
                {
                    ItemData itemEncontrado = System.Array.Find(todosLosItemsPosibles, item => item.id == idGuardado);
                    if (itemEncontrado != null) InventoryManager.Instance.AgregarItem(itemEncontrado);
                }
            }

            if (jugador != null)
            {
                CharacterController cc = jugador.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                jugador.position = new Vector3(data.posicionX, data.posicionY, data.posicionZ);
                Physics.SyncTransforms();
                if (cc != null) cc.enabled = true;
            }

            if (camaraOrbital != null) camaraOrbital.CargarDatosCamara(data.camaraAngulo, data.camaraZoom);
            decisionesActuales = new List<int>(data.decisionesTomadas);

            Debug.Log("Partida Cargada. Referencias reconectadas.");
        }
    }

    public void NuevaPartida()
    {
        if (File.Exists(savePath)) File.Delete(savePath);

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.objetosRecogidos.Clear();
            InventoryManager.Instance.objetosDestruidosMundo.Clear();
            foreach (Transform hijo in InventoryManager.Instance.contenedorDeIconos) Destroy(hijo.gameObject);
        }
        decisionesActuales.Clear();

        // Si estamos en el menú, cargar nivel 1
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            // Si ya estamos jugando, reseteamos posición
            if (jugador == null) BuscarReferencias();
            if (jugador != null)
            {
                CharacterController cc = jugador.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                jugador.position = Vector3.zero;
                if (cc != null) cc.enabled = true;
            }
        }
    }
}