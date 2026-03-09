using UnityEngine;

public class LetreroFlotante : MonoBehaviour
{
    [Header("Configuración del Letrero")]
    public GameObject letreroUI;
    public bool mirarHaciaCamara = true;

    private Camera camaraPrincipal;
    private bool jugadorCerca = false; // Nueva variable para rastrear al jugador

    void Start()
    {
        if (letreroUI != null) letreroUI.SetActive(false);
        camaraPrincipal = Camera.main;
    }

    void Update()
    {
        // 1. Lógica de visibilidad inteligente
        if (letreroUI != null)
        {
            bool dialogoActivo = false;

            // Comprobamos si el DialogueManager existe y si se está hablando
            if (DialogueManager.Instance != null)
            {
                dialogoActivo = DialogueManager.Instance.estaHablando;
            }

            // El letrero solo se activa si el jugador está cerca Y NO hay un diálogo
            bool deberiaMostrarse = jugadorCerca && !dialogoActivo;

            if (letreroUI.activeSelf != deberiaMostrarse)
            {
                letreroUI.SetActive(deberiaMostrarse);
            }
        }

        // 2. Lógica de rotación (tu código original)
        if (mirarHaciaCamara && letreroUI != null && letreroUI.activeSelf && camaraPrincipal != null)
        {
            float rotacionY = camaraPrincipal.transform.eulerAngles.y;
            letreroUI.transform.rotation = Quaternion.Euler(0f, rotacionY, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true; // Solo marcamos que está cerca
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false; // Marcamos que se ha ido
        }
    }

    public void ApagarLetreroDefinitivamente()
    {
        jugadorCerca = false;
        if (letreroUI != null) letreroUI.SetActive(false);
        this.enabled = false;
    }
}