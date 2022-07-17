using Manager;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dice : MonoBehaviour
{
    [SerializeField]
    private DiceFace[] _diceFaces = null;

    [SerializeField]
    private Rigidbody _rigidbody;

    public Rigidbody Rigidbody => this._rigidbody;

    [SerializeField]
    private DiceEffectsTable _diceEffectsTable = null;

    private DiceFace _highestFace;

    private System.Collections.Generic.List<DiceEffect> _activeEffects = new();

    public event System.Action<DiceEffectType> EffectAdded;

    private float _stationarySince = -1;

    [SerializeField] [Range(0.1f, 2f)]
    private float _triggerFaceEffectTiming;

    [HideInInspector]
    public bool IsTeleporting;
    
    public bool IsStationary => this._stationarySince > 0f;

    public bool ShouldTriggerEffect => this.IsStationary && !this._triggeredLastStationaryEffect && Time.time >= this._stationarySince + this._triggerFaceEffectTiming;

    private bool _triggeredLastStationaryEffect = false;
    
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
        
        diceEffect.OnEffectStart(new DiceEffectContext());
        
        this.EffectAdded?.Invoke(diceEffectType);
        this._activeEffects.Add(diceEffect);
    }

    public void RemoveEffect(DiceEffect diceEffect)
    {
        this._activeEffects.Remove(diceEffect);
    }

    public void SetInvisibility(bool state)
    {
        for (int i = 0; i < this._diceFaces.Length; ++i)
        {
            this._diceFaces[i].SetInvisible(state);
        }
    }
    
    private void Awake()
    {
        this._rigidbody ??= this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GameManager.Instance.RegisterDice(this);
    }

    private void OnDestroy()
    {
        if (GameManager.Exists())
        {
            GameManager.Instance.UnregisterDice(this);
        }
    }

    private void Update()
    {
        for (int i = this._activeEffects.Count - 1; i >= 0; --i)
        {
            DiceEffect activeEffect = this._activeEffects[i];
            
            activeEffect.Update();
            if (activeEffect.IsOver)
            {
                activeEffect.OnEffectOver();
                this.RemoveEffect(activeEffect);
            }
        }
    }

    private void FixedUpdate()
    {
        if (this._rigidbody.velocity != Vector3.zero)
        {
            this._stationarySince = -1;
        }
        else if (!this.IsStationary)
        {
            this._triggeredLastStationaryEffect = false;
            this._stationarySince = Time.time;
        }

        if (this.ShouldTriggerEffect)
        {
            DiceEffectType effectType = this.GetHighestFace().EffectType;
            if (effectType != DiceEffectType.NONE)
            {
                Debug.Log($"triggering {effectType}");
                this._triggeredLastStationaryEffect = true;
                this.AddEffect(effectType);
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
