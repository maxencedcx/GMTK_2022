using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dice : MonoBehaviour
{
    [SerializeField]
    private DiceFace[] _diceFaces = null;

    private DiceFace _highestFace;

    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _initialForce;

    [SerializeField]
    private DiceEffectsTable _diceEffectsTable = null;
    
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
            case DiceEffectType.GIANT_DICE:
            case DiceEffectType.NONE:
            default:
                Debug.LogError($"Unhandled effect type {diceEffectType}!");
                diceEffect = null;
                break;
        }
        
        EffectAdded?.Invoke(diceEffectType);
    }
    
    private void Awake()
    {
        this._rigidbody ??= this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.transform.rotation = UnityEngine.Random.rotation;
        this._rigidbody.AddForce(Vector3.forward * this._initialForce, ForceMode.Impulse);
        this._rigidbody.AddTorque(UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360), ForceMode.Impulse);
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
}
