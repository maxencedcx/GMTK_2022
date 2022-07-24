using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DiceEffectsInterface : MonoBehaviour
{
    [SerializeField]
    private DiceEffectInterface _diceEffectInterfacePrefab = null;

    [SerializeField]
    private RectTransform _diceEffectsContainer = null;
    
    private Dictionary<Dice, List<DiceEffectInterface>> _diceEffects = new();
    
    public void OnDiceRegistered(Dice dice)
    {
        dice.EffectAdded += OnEffectAdded;
        dice.IncompatibleEffectRemoved += OnIncompatibleEffectRemoved;
        this._diceEffects.Add(dice, new List<DiceEffectInterface>());
    }
    
    public void OnDiceUnregistered(Dice dice)
    {
        dice.EffectAdded -= OnEffectAdded;
        dice.IncompatibleEffectRemoved -= OnIncompatibleEffectRemoved;

        for (int i = this._diceEffects[dice].Count - 1; i >= 0; --i)
        {
            Destroy(this._diceEffects[dice][i].gameObject);
        }
        
        this._diceEffects.Remove(dice);
    }
    
    private void OnEffectAdded(Dice dice, DiceEffect diceEffect)
    {
        DiceEffectInterface diceEffectInterface = Instantiate(_diceEffectInterfacePrefab, this._diceEffectsContainer);
        diceEffectInterface.Init(diceEffect);
        
        this._diceEffects[dice].Add(diceEffectInterface);
    }

    private void OnIncompatibleEffectRemoved(Dice dice, DiceEffect diceEffect)
    {
        DiceEffectInterface effectInterface = this._diceEffects[dice].FirstOrDefault(o => o.DiceEffect == diceEffect);

        if (effectInterface == null)
        {
            return;
        }
        
        this._diceEffects[dice].Remove(effectInterface);
        Destroy(effectInterface.gameObject);

    }

    private void Update()
    {
        foreach (KeyValuePair<Dice, List<DiceEffectInterface>> dice in this._diceEffects)
        {
            for (int i = dice.Value.Count - 1; i >= 0; --i)
            {
                dice.Value[i].Refresh();
                if (dice.Value[i].IsOver)
                {
                    Destroy(dice.Value[i].gameObject);
                    dice.Value.RemoveAt(i);
                }
            }
        }
    }
}
