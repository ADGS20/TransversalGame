//------------Creador de este script------------//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----------------------------------------------//

using UnityEngine;

public class StoneProjectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Si la piedra toca el suelo, se destruye
        if (collision.collider.CompareTag("Suelo"))
        {
            Destroy(gameObject);
        }
    }
}