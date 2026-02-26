using UnityEngine;
using System.Collections.Generic;

public class CameraRaycast : MonoBehaviour
{
    [Header("Configuración")]
    public List<Transform> objetivos;
    public LayerMask capaObstaculos;

    private List<TransparentObstacle> obstaculosActualmenteOcultos = new List<TransparentObstacle>();

    void Update()
    {
        List<TransparentObstacle> obstaculosDetectadosEsteFrame = new List<TransparentObstacle>();

        foreach (Transform obj in objetivos)
        {
            if (obj == null) continue;

            Vector3 direccion = obj.position - transform.position;
            float distancia = direccion.magnitude;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direccion, out hit, distancia, capaObstaculos))
            {
                TransparentObstacle obstaculo = hit.collider.gameObject.GetComponent<TransparentObstacle>();

                if (obstaculo != null && !obstaculosDetectadosEsteFrame.Contains(obstaculo))
                {
                    obstaculosDetectadosEsteFrame.Add(obstaculo);
                }
            }
        }

        // Ocultar los nuevos detectados
        foreach (TransparentObstacle obstaculo in obstaculosDetectadosEsteFrame)
        {
            if (!obstaculosActualmenteOcultos.Contains(obstaculo))
            {
                obstaculo.StartFadeOut();
                obstaculosActualmenteOcultos.Add(obstaculo);
            }
        }

        // Mostrar los que ya no están tapando
        List<TransparentObstacle> obstaculosParaMostrar = new List<TransparentObstacle>();
        foreach (TransparentObstacle obstaculo in obstaculosActualmenteOcultos)
        {
            if (!obstaculosDetectadosEsteFrame.Contains(obstaculo))
            {
                obstaculo.StartFadeIn();
                obstaculosParaMostrar.Add(obstaculo);
            }
        }

        foreach (TransparentObstacle obstaculo in obstaculosParaMostrar)
        {
            obstaculosActualmenteOcultos.Remove(obstaculo);
        }
    }
}