using System;
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
    
    private void Awake()
    {
        this._rigidbody ??= this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Manager.GameManager.Instance.RegisterDice(this);
    }

    private void OnDestroy()
    {
        Manager.GameManager.Instance.UnregisterDice(this);
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

    [ContextMenu("Get Highest Face")]
    private DiceFace GetHighestFace()
    {
        float highestPos = float.MinValue;
        
        for (int i = 0; i < _diceFaces.Length; i++)
        {
            float posY = _diceFaces[i].transform.position.y;

            if (posY > highestPos)
            {
                highestPos = posY;
                _highestFace = _diceFaces[i];
            }
        }

        return _highestFace;
    }
    
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Dice))]
    public class DiceEditor : RSLib.EditorUtilities.ButtonProviderEditor<Dice>
    {
        protected override void DrawButtons()
        {
            DrawButton("Add Shockwave", () => Obj.AddEffect(DiceEffectType.SHOCKWAVE));
            DrawButton("Add Running Dice", () => Obj.AddEffect(DiceEffectType.RUNNING_DICE));
            DrawButton("Add Mini Dice", () => Obj.AddEffect(DiceEffectType.MINI_DICE));
            DrawButton("Add Giant Dice", () => Obj.AddEffect(DiceEffectType.GIANT_DICE));
        }
    }
#endif
}
