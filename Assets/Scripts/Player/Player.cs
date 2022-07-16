using RSLib.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, MainInputAction.IPlayerActions, MainInputAction.ICubeChoiceActions
{
    // PHYSICS
    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _movementForceMultiplier;

    [SerializeField]
    private float _collisionForceMultiplier;

    [SerializeField] [Range(0f, 1f)]
    private float _collisionYForce;

    private Vector3 _lastInputDirection = Vector3.zero;

    private readonly List<Collider> _currentCollisions = new();
    
    // VIEW
    [SerializeField]
    private MeshRenderer _meshRenderer;

    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;
    
    [SerializeField]
    private Animator _animator = null;

    [SerializeField]
    private GameObject _blueBlobShadow = null;

    [SerializeField]
    private GameObject _pinkBlobShadow = null;
    
    // GAMEPLAY
    public Team Team { get; private set; }

    public bool IsPlayerReady { get; private set; } = false;

    public int PlayerIndex => this._playerInput.playerIndex;
    
    // INPUT
    [SerializeField]
    private PlayerInput _playerInput;

    private InputActionMap _playerActionMap;
    
    private InputActionMap _cubeChoiceActionMap;
    

    #region Unity Native Functions

    private void Awake()
    {
        this._playerActionMap = this._playerInput.actions.FindActionMap("Player");
        this._cubeChoiceActionMap = this._playerInput.actions.FindActionMap("CubeChoice");
    }

    private void Start()
    {
        Manager.TeamManager.Instance.RegisterPlayer(this);
    }

    private void FixedUpdate()
    {
        if (this.IsPlayerReady && Manager.GameManager.Instance.State == GameState.LOBBY)
        {
            return;
        }
        
        if (this._lastInputDirection != Vector3.zero)
        {
            this._rigidbody.AddForce(this._lastInputDirection * this._movementForceMultiplier, ForceMode.Acceleration);
            this._animator.SetBool("Running", true);

            if (this._lastInputDirection.x != 0f)
            {
                this._spriteRenderer.flipX = this._lastInputDirection.x < 0f;
            }
        }
        else
        {
            this._animator.SetBool("Running", false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (this._currentCollisions.Contains(collision.collider))
        {
            return;
        }
        
        this._currentCollisions.Add(collision.collider);
        if (collision.gameObject.TryGetComponent<Dice>(out _))
        {
            Vector3 direction = (collision.transform.position - this.transform.position).normalized;
            direction.y = Mathf.Max(0f, this._collisionYForce - direction.y) ;
            collision.rigidbody.AddForce(direction * this._collisionForceMultiplier, ForceMode.Impulse);
            collision.rigidbody.AddTorque(UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360));
        }
    }

    private void OnCollisionExit(Collision other)
    {
        this._currentCollisions.Remove(other.collider);
    }

    #endregion

    #region Input Handler

    public void DisablePlayerInputs()
    {
        this._playerActionMap.Disable();
    }

    public void EnablePlayerInputs()
    {
        this._playerActionMap.Enable();
    }

    public void DisableCubeChoiceInputs()
    {
        this._cubeChoiceActionMap.Disable();
    }

    public void EnableCubeChoiceInputs()
    {
        this._cubeChoiceActionMap.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();
        this._lastInputDirection = new Vector3(inputValue.x, 0, inputValue.y);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
    }

    public void OnTackle(InputAction.CallbackContext context)
    {
        if (this.IsPlayerReady && Manager.GameManager.Instance.State == GameState.LOBBY)
        {
            return;
        }

        if (context.performed)
        {
            this._rigidbody.AddForce(this._lastInputDirection * this._movementForceMultiplier / 3, ForceMode.Impulse);
            this._rigidbody.velocity = this._rigidbody.velocity.ClampAll(-this._movementForceMultiplier, this._movementForceMultiplier);
        }
    }

    public void OnPlayerReady(InputAction.CallbackContext context)
    {
        if (Manager.GameManager.Instance.State != GameState.LOBBY)
        {
            return;
        }
        
        if (context.performed)
        {
            this.IsPlayerReady = this.Team != Team.NONE && !this.IsPlayerReady;
            if (this.IsPlayerReady)
            {
                // TODO: Player can still dash and not change its team.
                this._rigidbody.NullifyMovement();
                this._animator.SetBool("Running", false);
            }
                
            Manager.GameManager.Instance.TryStartGame();
        }
    }
    
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 rotationDirection = context.ReadValue<Vector2>();
            rotationDirection.y = rotationDirection.x != 0 ? 0 : rotationDirection.y;
            Manager.DiceFaceChoiceManager.Instance.RotateCube(rotationDirection);
        }
    }

    public void OnChangeEffect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int direction = context.ReadValue<int>();
            Manager.DiceFaceChoiceManager.Instance.ChangeSelectedEffect(direction);
            Debug.Log($"OnChangeEffect");
        }
    }

    public void OnValidateEffect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Manager.DiceFaceChoiceManager.Instance.ValidateChoice();
        }
    }

    #endregion

    public void SetTeam(Team team)
    {
        if (!this.IsPlayerReady)
        {
            this.Team = team;

            this._blueBlobShadow.SetActive(this.Team == Team.BLUE);
            this._pinkBlobShadow.SetActive(this.Team == Team.PINK);
        }
    }
}
