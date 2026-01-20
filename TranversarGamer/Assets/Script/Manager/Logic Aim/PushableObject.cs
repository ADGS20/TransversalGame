using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public float fuerzaEmpuje = 8f;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null) return;

            // Dirección del empuje = hacia donde camina el jugador
            Vector3 direccion = collision.collider.transform.forward;

            rb.AddForce(direccion * fuerzaEmpuje, ForceMode.Force);
        }
    }
}

