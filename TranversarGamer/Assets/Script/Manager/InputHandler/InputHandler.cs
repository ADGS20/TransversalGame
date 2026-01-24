//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Asset de acciones")]
    public InputActionAsset inputActions;  // Asset del nuevo Input System

    private InputAction moveAction;        // Acción de movimiento
    private InputAction lookAction;        // Acción de mirar
    private InputAction jumpAction;        // Acción de salto

    public Vector2 Movimiento { get; private set; } // Movimiento leído del input
    public Vector2 Look { get; private set; }       // Dirección de mirada
    public bool Saltar { get; private set; }        // Indica si se presionó salto

    void Awake()
    {
        // Buscar el Action Map "Player"
        var playerMap = inputActions.FindActionMap("Player", true);

        // Obtener acciones individuales
        moveAction = playerMap.FindAction("Move", true);
        lookAction = playerMap.FindAction("Look", true);
        jumpAction = playerMap.FindAction("Jump", true);

        // Activar el mapa de acciones
        playerMap.Enable();
    }

    void OnDestroy()
    {
        // Desactivar acciones al destruir el objeto
        if (inputActions != null)
            inputActions.Disable();
    }

    void Update()
    {
        // Leer valores del input cada frame
        Movimiento = moveAction.ReadValue<Vector2>();
        Look = lookAction.ReadValue<Vector2>();
        Saltar = jumpAction.WasPerformedThisFrame();
    }
}
