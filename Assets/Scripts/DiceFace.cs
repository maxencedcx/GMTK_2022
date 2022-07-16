using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceFace : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _meshRenderer = null;

    [SerializeField]
    private Material _material = null;

    public DiceEffectType EffectType { get; private set; }
    
    public void SetEffectType(DiceEffectType diceEffectType)
    {
        EffectType = diceEffectType;
    }
}
