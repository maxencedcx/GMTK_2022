using RSLib.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour, MainInputAction.IPlayerActions, MainInputAction.IDiceFaceChoiceActions
{
    [Header("PHYSICS")]
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
    [Header("GENERAL")]
    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;
    [SerializeField]
    private Animator _animator = null;
    [SerializeField]
    private RuntimeAnimatorController _blueAnimator = null;
    [SerializeField]
    private RuntimeAnimatorController _pinkAnimator = null;
    
    [Header("BLOB SHADOW")]
    [SerializeField]
    private MeshRenderer[] _teamRelatedRenderers = null;
    [SerializeField]
    private Transform[] _circles = null;
    [SerializeField]
    private GameObject[] _arrows = null;

    [Header("FEEDBACK")]
    [SerializeField]
    private GameObject _tackleParticles = null;
    [SerializeField]
    private GameObject _pinkTeamParticles = null;
    [SerializeField]
    private GameObject _blueTeamParticles = null;
    [SerializeField]
    private GameObject _diceHitParticles = null;

    [Header("WORLD UI")]
    [SerializeField]
    private TextMeshProUGUI _readyText = null;
    [SerializeField]
    private TextMeshProUGUI _indexPlayer;
    
    [Header("AUDIO")]
    [SerializeField]
    private RSLib.Audio.ClipProvider _tackleClip = null;
    [SerializeField]
    private RSLib.Audio.ClipProvider _tackleHitClip = null;
    [SerializeField]
    private RSLib.Audio.ClipProvider _bumpClip = null;
    [SerializeField]
    private RSLib.Audio.ClipProvider _setTeamClip = null;

    // GAMEPLAY
    public Team Team { get; private set; }

    private bool _isPlayerReady = false;

    public bool IsPlayerReady
    {
        get => this._isPlayerReady;
        set
        {
            this._readyText.gameObject.SetActive(value);
            this._isPlayerReady = value;
            this._rigidbody.isKinematic = value;
        }
    }

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
        this._readyText.gameObject.SetActive(false);
    }

    private void Start()
    {
        Manager.TeamManager.Instance.RegisterPlayer(this);
        Manager.CameraManager.Instance.RegisterTarget(this.transform);
        DisplayPlayerIndex();
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
            
            for (int i = 0; i < this._circles.Length; ++i)
            {
                this._circles[i].LookAt(transform.position + this._lastInputDirection);
                this._circles[i].SetEulerAnglesX(90f);
            }
            
            if (this._lastInputDirection.x != 0f && !IsTackling)
            {
                this._spriteRenderer.flipX = this.Team == Team.PINK ? this._lastInputDirection.x > 0f : this._lastInputDirection.x < 0f;
            }
            
            for (int i = 0; i < this._arrows.Length; ++i)
            {
                this._arrows[i].SetActive(true);
            }
        }
        else
        {
            this._animator.SetBool("Running", false);
            for (int i = 0; i < this._arrows.Length; ++i)
            {
                this._arrows[i].SetActive(false);
            }
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
            Instantiate(_diceHitParticles, collision.GetContact(0).point + this._rigidbody.velocity.normalized, this._diceHitParticles.transform.rotation);
        }
        else if (this._lastInputDirection != Vector3.zero
                 && collision.gameObject.TryGetComponent(out Player collidingPlayer)
                 && collidingPlayer.IsPlayerReady == false)
        {
            float forceMultiplier = collidingPlayer.IsStationary ? this._staticPlayerCollisionForceMultiplier : this._diceCollisionForceMultiplier;
            forceMultiplier *= this.IsTackling ? this._tacklingCollisionForceMultiplier : 1f;
            collision.rigidbody.AddForce(collisionDirection * forceMultiplier, ForceMode.Impulse);
        }
        
        RSLib.Audio.AudioManager.PlaySound(this._tackleHitClip);
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
        Instantiate(this._tackleParticles, transform.position, this._tackleParticles.transform.rotation);
        Manager.GameManager.Instance.CameraShake.AddTrauma(0.15f);
        
        this._animator.SetBool("Tackling", true);
        this._animator.SetTrigger("Dash");

        this.IsTackling = true;
        yield return new WaitForSeconds(0.175f);
        
        this._animator.SetBool("Tackling", false);
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
                
                for (int i = 0; i < this._arrows.Length; ++i)
                {
                    this._arrows[i].SetActive(false);
                }
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
            
            this.UpdatePlayerColor();
            
            this.DisplayPlayerIndex();
            
            this._animator.runtimeAnimatorController = this.Team == Team.BLUE ? this._blueAnimator : this._pinkAnimator;

            GameObject particles = this.Team == Team.PINK ? this._pinkTeamParticles : this._blueTeamParticles;
            Instantiate(particles, this.transform.position, particles.transform.rotation);
            
            RSLib.Audio.AudioManager.PlaySound(this._setTeamClip);
        }
    }

    public void OnGameEnd()
    {
        this._rigidbody.NullifyMovement();
        this.DisablePlayerInputs();
        
        if (Manager.GameManager.Instance.WinningTeam == this.Team)
        {
            this._animator.SetTrigger("Victory");
        }
        else
        {
            this._animator.SetTrigger("Defeat");
        }
    }

    public void OnGameEndSequenceOver()
    {
        this.EnablePlayerInputs();
        this._animator.SetTrigger("Idle");
    }
    
    private System.Collections.IEnumerator TackleCooldownCoroutine()
    {
        this._canTackle = false;
        yield return RSLib.Yield.SharedYields.WaitForSeconds(this._tackleCooldown);
        this._canTackle = true;
    }

    private void DisplayPlayerIndex()
    {
        int newPlayerIndex = this.PlayerIndex + 1;
        _indexPlayer.text = "P" + newPlayerIndex;
    }

    public void UpdatePlayerColor()
    {
        int teamIndex = this.Team == Team.BLUE ? Manager.TeamManager.Instance.BluePlayers.IndexOf(this) : Manager.TeamManager.Instance.PinkPlayers.IndexOf(this);
        Color color = this.Team == Team.BLUE ? Manager.TeamManager.Instance.PlayerColorsTable.GetBlueColorAtIndex(teamIndex) : Manager.TeamManager.Instance.PlayerColorsTable.GetPinkColorAtIndex(teamIndex);

        this._readyText.color = color;
        this._indexPlayer.color = color;
        
        for (int i = this._teamRelatedRenderers.Length - 1; i >= 0; --i)
        {
            this._teamRelatedRenderers[i].material.SetColor("_Color", color);
        }
    }
}
