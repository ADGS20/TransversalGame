//------------Creador de este script------------//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----------------------------------------------//
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public float velocidadEmpuje = 2.5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        // Buscamos el script de movimiento del jugador
        Mov_Player3D mov = collision.transform.root.GetComponent<Mov_Player3D>();
        if (mov == null) return;

        // Dirección REAL de movimiento del jugador
        Vector3 dir = mov.direccionMovimiento; // ← ESTA es la clave
        dir.y = 0f;
        dir.Normalize();

        if (dir.sqrMagnitude < 0.01f)
            return; // si el jugador no se mueve, no empuja

        // Aplicamos velocidad suave en esa dirección
        Vector3 vel = dir * velocidadEmpuje;
        vel.y = rb.linearVelocity.y;

        rb.linearVelocity = vel;
    }
}