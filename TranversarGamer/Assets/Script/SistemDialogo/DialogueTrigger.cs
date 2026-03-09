using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public LineaDialogo[] dialogo;
    public bool activadoAlInicio = false;
    public bool unSoloUso = true;

    private bool yaUsado = false;

    void Start()
    {
        if (activadoAlInicio) Iniciar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaUsado)
        {
            Iniciar();
        }
    }

    void Iniciar()
    {
        DialogueManager.Instance.IniciarDialogo(dialogo);
        if (unSoloUso) yaUsado = true;
    }
}