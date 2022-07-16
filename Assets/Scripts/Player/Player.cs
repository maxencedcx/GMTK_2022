using RSLib.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = System.Random;

public class Player : MonoBehaviour, MainInputAction.IPlayerActions
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
    
    // GAMEPLAY
    public Team Team { get; private set; }

    public bool IsPlayerReady { get; private set; } = false;
    

    #region Unity Native Functions

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
            Manager.GameManager.Instance.TryStartGame();
        }
    }

    #endregion

    public void SetTeam(Team team)
    {
        if (!this.IsPlayerReady)
        {
            this._rigidbody.velocity = Vector3.zero;
            this.Team = team;
            this._meshRenderer.material.SetColor("_Color", team.GetTeamColor());
        }
    }
}
