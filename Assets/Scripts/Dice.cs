using System.Linq;
using Manager;
using RSLib;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dice : MonoBehaviour
{
    // PHYSICS
    [SerializeField]
    private Rigidbody _rigidbody;

    public Rigidbody Rigidbody => this._rigidbody;

    private float _stationarySince = -1;

    [SerializeField]
    private float _stationaryAllowance = 0f;

    // DICE FACES
    [SerializeField]
    private DiceFace[] _diceFaces = null;

    [SerializeField]
    private DiceEffectsTable _diceEffectsTable = null;

    private DiceFace _highestFace;

    // EFFECTS
    private System.Collections.Generic.List<DiceEffect> _activeEffects = new();

    public event System.Action<Dice, DiceEffect> EffectAdded;
    
    public event System.Action<Dice, DiceEffect> IncompatibleEffectRemoved;

    [SerializeField] [Range(0.1f, 2f)]
    private float _triggerFaceEffectTiming;

    [SerializeField]
    private GameObject _blobShadow = null;
    
    [SerializeField]
    private RSLib.Audio.ClipProvider _collisionClip = null;
    
    [SerializeField]
    private RSLib.Audio.ClipProvider _applyEffectClip = null;
    
    [HideInInspector]
    public bool IsTeleporting;
    
    private bool _triggeredLastStationaryEffect = false;

    private DiceFace _lastTriggeredDiceFace = null; 
    
    public bool IsStationary => this._stationarySince > 0f;

    public bool ShouldTriggerEffect => this.IsStationary
                                       && !this._triggeredLastStationaryEffect
                                       && Time.time >= this._stationarySince + this._triggerFaceEffectTiming;

    public void AddEffect(DiceEffectType diceEffectType)
    {
        DiceEffect diceEffect;
        
        switch (diceEffectType)
        {
            case DiceEffectType.SHOCKWAVE:
                diceEffect = new Shockwave(this, this._diceEffectsTable._shockwaveEffectData, this._diceEffectsTable._shockwaveData);
                break;
            case DiceEffectType.RUNNING_DICE:
                diceEffect = new RunningDice(this, this._diceEffectsTable._runningDiceEffectData, this._diceEffectsTable._runningDiceData);
                break;
            case DiceEffectType.MINI_DICE:
                diceEffect = new MiniDice(this, this._diceEffectsTable._miniDiceEffectData, this._diceEffectsTable._miniDiceData);
                break;
            case DiceEffectType.GIANT_DICE:
                diceEffect = new GiantDice(this, this._diceEffectsTable._giantDiceEffectData, this._diceEffectsTable._giantDiceData);
                break;
            case DiceEffectType.TELEPORT:
                diceEffect = new TeleportingDice(this, this._diceEffectsTable._teleportingDiceEffectData, this._diceEffectsTable._teleportingDiceData);
                break;
            case DiceEffectType.INVISIBLE:
                diceEffect = new InvisibleDice(this, this._diceEffectsTable._invisibleDiceEffectData, this._diceEffectsTable._invisibleDiceData);
                break;
            case DiceEffectType.NONE:
            default:
                Debug.LogError($"Unhandled effect type {diceEffectType}!");
                diceEffect = null;
                return;
        }
        
        diceEffect.OnEffectStart(new DiceEffectContext {Players = Manager.TeamManager.Instance.Players});
        
        this.EffectAdded?.Invoke(this, diceEffect);
        this._activeEffects.Add(diceEffect);
    }

    public void RemoveEffect(DiceEffect diceEffect)
    {
        this._activeEffects.Remove(diceEffect);
    }

    public void RemoveIncompatibleEffect(DiceEffect diceEffect)
    {
        this.IncompatibleEffectRemoved?.Invoke(this, diceEffect);
        RemoveEffect(diceEffect);
    }

    public void SetInvisibility(bool state)
    {
        for (int i = 0; i < this._diceFaces.Length; ++i)
        {
            this._diceFaces[i].SetInvisible(state);
        }
        
        this._blobShadow.SetActive(!state);
    }
    
    private void Awake()
    {
        this._rigidbody ??= this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GameManager.Instance.RegisterDice(this);
        CameraManager.Instance.RegisterTarget(this.transform);
    }

    private void OnDestroy()
    {
        if (GameManager.Exists())
        {
            GameManager.Instance.UnregisterDice(this);
        }

        if (CameraManager.Exists())
        {
            CameraManager.Instance.UnregisterTarget(this.transform);
        }
    }
    
    private void FixedUpdate()
    {
        if (this._rigidbody.velocity.magnitude > (Vector3.one * this._stationaryAllowance * 2.5f).magnitude)
        {
            this._stationarySince = -1;
        }
        else if (!this.IsStationary && this._rigidbody.velocity.magnitude <= (Vector3.one * this._stationaryAllowance).magnitude)
        {
            this._triggeredLastStationaryEffect = this.GetHighestFace() == this._lastTriggeredDiceFace;
            this._stationarySince = Time.time;
        }
                
        for (int i = this._activeEffects.Count - 1; i >= 0; --i)
        {
            DiceEffect activeEffect = this._activeEffects[i];
            
            activeEffect.FixedUpdate();
            if (activeEffect.IsOver)
            {
                activeEffect.OnEffectOver();
                this.RemoveEffect(activeEffect);
            }
        }

        if (this.ShouldTriggerEffect)
        {
            DiceFace diceFace = this.GetHighestFace();

            if (diceFace.EffectType != DiceEffectType.NONE
                && (diceFace.EffectType != DiceEffectType.TELEPORT
                    || this._activeEffects.All(o => o.EffectType != DiceEffectType.TELEPORT)))
            {
                this._lastTriggeredDiceFace = diceFace;
                this._triggeredLastStationaryEffect = true;
                this.AddEffect(diceFace.EffectType);
                
                RSLib.Audio.AudioManager.PlaySound(_applyEffectClip);
            }
        }
    }

    public void ApplySettings(DiceSettings diceSettings)
    {
        if (diceSettings == null)
        {
            return;
        }
        
        for (int i = 0; i < diceSettings.DiceEffectFaces.Length; i++)
        {
            this._diceFaces[i].SetEffectType(diceSettings.DiceEffectFaces[i]);
        }
    }

    [ContextMenu("Get Highest Face")]
    private DiceFace GetHighestFace()
    {
        float highestPos = float.MinValue;
        
        for (int i = 0; i < this._diceFaces.Length; i++)
        {
            float posY = this._diceFaces[i].transform.position.y;

            if (posY > highestPos)
            {
                highestPos = posY;
                this._highestFace = this._diceFaces[i];
            }
        }

        return this._highestFace;
    }

    private void OnCollisionEnter(Collision collision)
    {
        RSLib.Audio.AudioManager.PlaySound(_collisionClip);
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Dice))]
    public class DiceEditor : RSLib.EditorUtilities.ButtonProviderEditor<Dice>
    {
        protected override void DrawButtons()
        {
            this.DrawButton("Add Shockwave", () => this.Obj.AddEffect(DiceEffectType.SHOCKWAVE));
            this.DrawButton("Add Running Dice", () => this.Obj.AddEffect(DiceEffectType.RUNNING_DICE));
            this.DrawButton("Add Mini Dice", () => this.Obj.AddEffect(DiceEffectType.MINI_DICE));
            this.DrawButton("Add Giant Dice", () => this.Obj.AddEffect(DiceEffectType.GIANT_DICE));
            this.DrawButton("Add Teleport", () => this.Obj.AddEffect(DiceEffectType.TELEPORT));
            this.DrawButton("Add Invisible", () => this.Obj.AddEffect(DiceEffectType.INVISIBLE));
        }
    }
#endif
}
