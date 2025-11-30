//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//


using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Asset de acciones")]
    public InputActionAsset inputActions;  // Arrastra tu InputSystem_Actions aquí

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    public Vector2 Movimiento { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Saltar { get; private set; }

    void Awake()
    {
        // Buscar el Action Map "Player"
        var playerMap = inputActions.FindActionMap("Player", true);

        moveAction = playerMap.FindAction("Move", true);
        lookAction = playerMap.FindAction("Look", true);
        jumpAction = playerMap.FindAction("Jump", true);

        playerMap.Enable();
    }

    void OnDestroy()
    {
        if (inputActions != null)
            inputActions.Disable();
    }

    void Update()
    {
        // Leer valores de las Actions (teclado + mando a la vez)
        Movimiento = moveAction.ReadValue<Vector2>();
        Look = lookAction.ReadValue<Vector2>();
        Saltar = jumpAction.WasPerformedThisFrame();
    }
}