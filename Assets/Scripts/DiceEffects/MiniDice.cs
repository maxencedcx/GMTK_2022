using DG.Tweening;

public class MiniDice : SizeModifier
{
    public MiniDice(Dice dice, DiceEffectData diceEffectData, SizeModifierData sizeModifierData) : base(dice, diceEffectData, sizeModifierData)
    {
    }

    public override DiceEffectType EffectType => DiceEffectType.MINI_DICE;
}
