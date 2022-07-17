public class DiceSettings
{
    public DiceSettings()
    {
        this.DiceEffectFaces = new[]
        {
            DiceEffectType.SHOCKWAVE,
            DiceEffectType.RUNNING_DICE,
            DiceEffectType.MINI_DICE,
            DiceEffectType.GIANT_DICE,
            DiceEffectType.TELEPORT,
            DiceEffectType.INVISIBLE,
        };
    }
    
    public DiceSettings(DiceEffectType[] previousDiceFaces, (DiceEffectType diceEffectType, int index) newDiceFace)
    {
        this.DiceEffectFaces = new[]
        {
            previousDiceFaces?[0] ?? DiceEffectType.NONE,
            previousDiceFaces?[1] ?? DiceEffectType.NONE,
            previousDiceFaces?[2] ?? DiceEffectType.NONE,
            previousDiceFaces?[3] ?? DiceEffectType.NONE,
            previousDiceFaces?[4] ?? DiceEffectType.NONE,
            previousDiceFaces?[5] ?? DiceEffectType.NONE,
        };
        
        this.DiceEffectFaces[newDiceFace.index] = newDiceFace.diceEffectType;
    }

    public DiceEffectType[] DiceEffectFaces;
}
