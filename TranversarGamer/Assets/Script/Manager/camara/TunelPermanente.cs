using UnityEngine;

public class TunelPermanente : MonoBehaviour
{
    public Transform jugador;    // Arrastra aquí a tu personaje
    public float tamañoAgujero = 4f;

    void Start()
    {
        // Fijamos el tamaño del túnel para siempre al empezar
        transform.localScale = new Vector3(tamañoAgujero, tamañoAgujero, tamañoAgujero);
    }

    void LateUpdate()
    {
        if (jugador != null)
        {
            // La esfera te sigue a todos lados
            transform.position = jugador.position;
        }
    }
}