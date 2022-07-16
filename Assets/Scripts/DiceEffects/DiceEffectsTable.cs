using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dice Effects Table", menuName = "GMTK/Dice Effects Table")]
public class DiceEffectsTable : ScriptableObject
{
    [Header("SHOCKWAVE")]
    public DiceEffectData _shockwaveEffectData;
    public ShockwaveData _shockwaveData;
    
    [Header("RUNNING DICE")]
    public DiceEffectData _runningDiceEffectData;
    public RunningDiceData _runningDiceData;
    
    [Header("MINI DICE")]
    public DiceEffectData _miniDiceEffectData;
    public SizeModifierData _miniDiceData;
    
    [Header("GIANT DICE")]
    public DiceEffectData _giantDiceEffectData;
    public SizeModifierData _giantDiceData;
}
