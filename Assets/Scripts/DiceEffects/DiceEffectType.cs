public enum DiceEffectType
{
    NONE,
    SHOCKWAVE,
    RUNNING_DICE,
    MINI_DICE,
    GIANT_DICE
}

public static class DiceEffectTypeExtensions
{
    public static bool IsIncompatibleWith(this DiceEffectType diceEffectType, DiceEffectType other)
    {
        return Manager.GameManager.Exists()
               && Manager.GameManager.Instance.EffectsIncompatibilitiesTable.AreIncompatible(diceEffectType, other);
    }
}