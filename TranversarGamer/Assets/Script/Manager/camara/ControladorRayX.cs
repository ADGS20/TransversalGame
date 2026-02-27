using UnityEngine;

public class ControladorRayX : MonoBehaviour
{
    public Transform jugador;
    public float radioTunel = 2.0f;

    void Start()
    {
        // Fijamos el tamaño una sola vez para que no baile
        transform.localScale = new Vector3(radioTunel, radioTunel, radioTunel);
    }

    void LateUpdate()
    {
        if (jugador != null)
        {
            // La esfera sigue al jugador, pero sin Raycasts que fallen
            transform.position = jugador.position;
        }
    }
}