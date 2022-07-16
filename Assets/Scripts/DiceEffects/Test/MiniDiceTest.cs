using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDiceTest : MonoBehaviour
{
    [SerializeField]
    private DiceEffectData _diceEffectData = null;

    [SerializeField]
    private SizeModifierData sizeModifierData = null;

    private MiniDice _miniDice;

    private void Awake()
    {
        _miniDice = new MiniDice(this.GetComponent<Dice>(), _diceEffectData, sizeModifierData);
        _miniDice.TryApply(new DiceEffectContext());
    }

    private void Update()
    {
        if (_miniDice != null)
        {
            _miniDice.Update();
            if(_miniDice.IsOver)
            {
                _miniDice.Revert();            
                _miniDice = null;            
            }
        }
    }
}
