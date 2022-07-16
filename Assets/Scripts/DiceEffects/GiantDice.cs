using DG.Tweening;

public class GiantDice : SizeModifier
{
    public GiantDice(Dice dice, DiceEffectData diceEffectData, SizeModifierData sizeModifierData) : base(dice, diceEffectData, sizeModifierData)
    {
    }

    public override DiceEffectType EffectType => DiceEffectType.GIANT_DICE;
}
