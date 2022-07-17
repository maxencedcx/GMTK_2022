using RSLib.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, MainInputAction.IPlayerActions, MainInputAction.IDiceFaceChoiceActions
{
    // PHYSICS
    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _movementForceMultiplier;

    [SerializeField]
    private float _tacklingForceMultiplier;

    [FormerlySerializedAs("_collisionForceMultiplier")] [SerializeField]
    private float _diceCollisionForceMultiplier;

    [FormerlySerializedAs("_collisionYForce")] [SerializeField] [Range(0f, 1f)]
    private float _diceCollisionYForce;

    [SerializeField]
    private float _staticPlayerCollisionForceMultiplier;

    [SerializeField]
    private float _playerCollisionForceMultiplier;

    [SerializeField]
    private float _tacklingCollisionForceMultiplier;

    [SerializeField]
    private float _tackleCooldown = 0.5f;

    private bool _canTackle = true;
    
    private Vector3 _lastInputDirection = Vector3.zero;

    public bool IsStationary => this._lastInputDirection == Vector3.zero;

    private readonly HashSet<Collider> _currentCollisions = new();
    
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

    [SerializeField]
    private RuntimeAnimatorController _blueAnimator = null;

    [SerializeField]
    private RuntimeAnimatorController _pinkAnimator = null;

    [SerializeField]
    private GameObject _pinkTeamParticles = null;

    [SerializeField]
    private GameObject _blueTeamParticles = null;
    
    // VIEW
    [SerializeField]
    private RSLib.Audio.ClipProvider _tackleClip = null;
    
    [SerializeField]
    private RSLib.Audio.ClipProvider _bumpClip = null;

    [SerializeField]
    private RSLib.Audio.ClipProvider _setTeamClip = null;
    
    // GAMEPLAY
    public Team Team { get; private set; }

    public bool IsPlayerReady { get; set; } = false;

    public bool IsTackling { get; private set; } = false;

    public int PlayerIndex => this._playerInput.playerIndex;
    
    // INPUT
    [SerializeField]
    private PlayerInput _playerInput;

    private InputActionMap _playerActionMap;
    
    private InputActionMap _diceFaceChoiceActionMap;
    
    #region Unity Native Functions

    private void Awake()
    {
        this._playerActionMap = this._playerInput.actions.FindActionMap(nameof(MainInputAction.Player));
        this._diceFaceChoiceActionMap = this._playerInput.actions.FindActionMap(nameof(MainInputAction.DiceFaceChoice));
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
                this._spriteRenderer.flipX = this.Team == Team.PINK ? this._lastInputDirection.x > 0f : this._lastInputDirection.x < 0f;
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
        Vector3 collisionDirection = (collision.transform.position - this.transform.position).normalized;
        
        if (collision.gameObject.TryGetComponent<Dice>(out _))
        {
            float forceMultiplier = this._diceCollisionForceMultiplier;
            forceMultiplier *= this.IsTackling ? this._tacklingCollisionForceMultiplier : 1f;
            collisionDirection.y = Mathf.Max(0f, this._diceCollisionYForce - collisionDirection.y) ;
            collision.rigidbody.AddForce(collisionDirection * forceMultiplier, ForceMode.Impulse);
            collision.rigidbody.AddTorque(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
            
            RSLib.Audio.AudioManager.PlaySound(this._bumpClip);
        }
        else if (this._lastInputDirection != Vector3.zero
                 && collision.gameObject.TryGetComponent(out Player collidingPlayer)
                 && collidingPlayer.IsPlayerReady == false)
        {
            float forceMultiplier = collidingPlayer.IsStationary ? this._staticPlayerCollisionForceMultiplier : this._diceCollisionForceMultiplier;
            forceMultiplier *= this.IsTackling ? this._tacklingCollisionForceMultiplier : 1f;
            collision.rigidbody.AddForce(collisionDirection * forceMultiplier, ForceMode.Impulse);
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
        this._diceFaceChoiceActionMap.Disable();
    }

    public void EnableCubeChoiceInputs()
    {
        this._diceFaceChoiceActionMap.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

        if (context.performed && inputValue.magnitude > 0.4f)
        {
            this._lastInputDirection = new Vector3(inputValue.x, 0, inputValue.y);
        }
        else
        {
            this._lastInputDirection = Vector3.zero;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
    }

    public void OnTackle(InputAction.CallbackContext context)
    {
        if ((this.IsPlayerReady && Manager.GameManager.Instance.State == GameState.LOBBY)
            || !this._canTackle
            || this.IsStationary)
        {
            return;
        }

        if (context.performed)
        {
            this._rigidbody.AddForce(this._lastInputDirection * this._tacklingForceMultiplier, ForceMode.Impulse);
            this.StartCoroutine(this.TackleCooldownCoroutine());
            this.StartCoroutine(this.OnTackleCoroutine());
            
            RSLib.Audio.AudioManager.PlaySound(this._tackleClip);
        }
    }

    private IEnumerator OnTackleCoroutine()
    {
        this.IsTackling = true;
        yield return new WaitForSeconds(0.1f);
        this.IsTackling = false;
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

    public void OnValidateEffect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Manager.DiceFaceChoiceManager.Instance.ValidateChoice();
        }
    }

    public void OnSelectLeftEffect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Manager.DiceFaceChoiceManager.Instance.ChangeSelectedEffect(-1);
        }
    }

    public void OnSelectRightEffect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Manager.DiceFaceChoiceManager.Instance.ChangeSelectedEffect(1);
        }
    }

    #endregion

    public void SetTeam(Team team)
    {
        if (this.Team == team || Manager.GameManager.Instance.State != GameState.LOBBY)
        {
            return;
        }
        
        if (!this.IsPlayerReady)
        {
            this.Team = team;

            this._blueBlobShadow.SetActive(this.Team == Team.BLUE);
            this._pinkBlobShadow.SetActive(this.Team == Team.PINK);

            this._animator.runtimeAnimatorController = this.Team == Team.BLUE ? this._blueAnimator : this._pinkAnimator;

            GameObject particles = this.Team == Team.PINK ? this._pinkTeamParticles : this._blueTeamParticles;
            Instantiate(particles, this.transform.position, particles.transform.rotation);
            
            RSLib.Audio.AudioManager.PlaySound(this._setTeamClip);
        }
    }

    private System.Collections.IEnumerator TackleCooldownCoroutine()
    {
        this._canTackle = false;
        yield return RSLib.Yield.SharedYields.WaitForSeconds(this._tackleCooldown);
        this._canTackle = true;
    }
}
