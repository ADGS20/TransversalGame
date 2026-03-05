using UnityEngine;

public class LetreroFlotante : MonoBehaviour
{
    [Header("Configuración del Letrero")]
    [Tooltip("Arrastra aquí el Canvas o GameObject que contiene el texto.")]
    public GameObject letreroUI;

    [Tooltip("Si lo marcas, el texto siempre rotará para mirar a la cámara (Solo en el eje Y).")]
    public bool mirarHaciaCamara = true;

    private Camera camaraPrincipal;

    void Start()
    {
        if (letreroUI != null)
        {
            letreroUI.SetActive(false);
        }

        camaraPrincipal = Camera.main;
    }

    void Update()
    {
        if (mirarHaciaCamara && letreroUI != null && letreroUI.activeSelf && camaraPrincipal != null)
        {
            // 1. Obtenemos hacia dónde está mirando la cámara en el eje Y (izquierda/derecha)
            float rotacionY = camaraPrincipal.transform.eulerAngles.y;

            // 2. Le aplicamos esa rotación al letrero, pero forzamos que X y Z sean siempre 0
            letreroUI.transform.rotation = Quaternion.Euler(0f, rotacionY, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (letreroUI != null) letreroUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (letreroUI != null) letreroUI.SetActive(false);
        }
    }

    public void ApagarLetreroDefinitivamente()
    {
        if (letreroUI != null) letreroUI.SetActive(false);
        this.enabled = false;
    }
}