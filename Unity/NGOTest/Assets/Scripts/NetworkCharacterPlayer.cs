using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class NetworkCharacterPlayer : NetworkBehaviour
{
    [SerializeField] private InputActionReference moveActionAsset;
    [SerializeField] private float moveSpeed = 4f;

    private CharacterController controller;
    private InputAction moveInput;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        controller.enabled = true;

        if (!IsOwner)
        {
            return;
        }

        transform.position = new Vector3(NetworkManager.Singleton.IsHost ? -2f : 2f, 0f, 0f);        

        moveInput = moveActionAsset.action.Clone();
        moveInput.Enable();
    }

    public override void OnNetworkDespawn()
    {
        controller.enabled = false;

        if (!IsOwner)
        {
            return;
        }
        
        moveInput?.Disable();
        moveInput?.Dispose();
        moveInput = null;
    }

    private void Update()
    {
        if (!IsOwner || !IsSpawned || moveInput is null)
        {
            return;
        }

        Vector2 input = moveInput.ReadValue<Vector2>();
        Vector3 move = new(input.x, 0f, input.y);

        MoveRequestRPC(moveSpeed * Time.deltaTime * move);
    }

    [Rpc(SendTo.Server)]
    private void MoveRequestRPC(Vector3 moveDelta)
    {
        controller.Move(moveDelta);
    }
}
