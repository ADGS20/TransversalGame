//------------Creador de este script------------//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----------------------------------------------//

using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public float velocidadEmpuje = 2.5f; // Velocidad a la que el objeto se mueve al ser empujado

    private Rigidbody rb;

    private void Awake()
    {
        // Obtener el Rigidbody del objeto
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        // Verificar si quien colisiona es el jugador
        Mov_Player3D mov = collision.transform.root.GetComponent<Mov_Player3D>();
        if (mov == null) return;

        // Dirección real del movimiento del jugador
        Vector3 dir = mov.direccionMovimiento;
        dir.y = 0f;        // Evitar movimiento vertical
        dir.Normalize();   // Normalizar para obtener dirección pura

        // Si el jugador no se está moviendo, no empuja
        if (dir.sqrMagnitude < 0.01f)
            return;

        // Aplicar velocidad en la dirección del empuje
        Vector3 vel = dir * velocidadEmpuje;
        vel.y = rb.linearVelocity.y; // Mantener velocidad vertical actual

        rb.linearVelocity = vel;
    }
}
