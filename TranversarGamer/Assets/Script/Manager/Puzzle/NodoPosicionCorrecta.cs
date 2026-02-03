using UnityEngine;

public class NodoPosicionCorrecta : MonoBehaviour
{
    [Header("Nodo que depende de este tótem")]
    public NodoController nodo;

    private void OnTriggerEnter(Collider other)
    {
        if (!EsTotem(other)) return;

        nodo.enPosicionCorrecta = true;

        TotemVisual tv = other.GetComponentInParent<TotemVisual>();
        if (tv != null)
        {
            if (nodo.esNaturaleza)
                tv.ActivarNaturaleza();
            else
                tv.ActivarCorrupcion();
        }

        if (nodo.estaActivado)
            nodo.RecibirSenal();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!EsTotem(other)) return;

        nodo.enPosicionCorrecta = false;
        nodo.CortarSenal();

        TotemVisual tv = other.GetComponentInParent<TotemVisual>();
        if (tv != null)
            tv.Desactivar();
    }

    private bool EsTotem(Collider other)
    {
        if (other.CompareTag("PushableObject"))
            return true;

        if (other.GetComponentInParent<PushableObject>() != null)
            return true;

        return false;
    }
}
