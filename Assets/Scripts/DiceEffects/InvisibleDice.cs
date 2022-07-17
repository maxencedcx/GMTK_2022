using UnityEngine;

public class InvisibleDice : DiceEffect
{
    public InvisibleDice(Dice dice, DiceEffectData diceEffectData, InvisibleDiceData invisibleDiceData) : base(dice, diceEffectData)
    {
        this._invisibleDiceData = invisibleDiceData;
    }

    private InvisibleDiceData _invisibleDiceData;
    
    public override DiceEffectType EffectType => DiceEffectType.INVISIBLE;
    
    public override bool CanApply(DiceEffectContext diceEffectContext)
    {
        return true;
    }

    protected override void Apply(DiceEffectContext diceEffectContext)
    {
        this._dice.SetInvisibility(true);
    }

    public override void OnEffectOver()
    {
        base.OnEffectOver();
        this._dice.SetInvisibility(false);
    }
}
