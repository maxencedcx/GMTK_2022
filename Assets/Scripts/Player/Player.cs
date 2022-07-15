using RSLib.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _forceMultiplier;

    private Vector2 _lastInputDirection = Vector2.zero;
    
    private MainInputAction _mainInputAction;

    private void Awake()
    {
        this._mainInputAction = new MainInputAction();
        this._mainInputAction.Player.Move.performed += this.Move;
        this._mainInputAction.Player.Tackle.performed += this.Tackle;
        this._mainInputAction.Enable();
        
        InputSystem.onDeviceChange +=
            (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        // New Device.
                        break;
                    case InputDeviceChange.Disconnected:
                        // Device got unplugged.
                        break;
                    case InputDeviceChange.Reconnected:
                        // Plugged back in.
                        break;
                }
            };
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        this._lastInputDirection = ctx.ReadValue<Vector2>();
    }

    private void Tackle(InputAction.CallbackContext ctx)
    {
        this._rigidbody.AddForce(this._lastInputDirection * this._forceMultiplier / 3, ForceMode.Impulse);
        this._rigidbody.velocity = this._rigidbody.velocity.ClampAll(-this._forceMultiplier, this._forceMultiplier);
    }

    void FixedUpdate()
    {
        Vector2 moveInput = this._mainInputAction.Player.Move.ReadValue<Vector2>();
        if (moveInput != Vector2.zero)
        {
            this._rigidbody.AddForce(new Vector3(moveInput.x, 0, moveInput.y) * this._forceMultiplier, ForceMode.Acceleration);
        }
    }
}
