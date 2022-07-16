using System.Linq;
using UnityEngine;

public class DiceFace : MonoBehaviour
{
    [System.Serializable]
    public struct DiceEffectMaterial
    {
        public DiceEffectType EffectType;
        public Material Material;
    }
    
    [SerializeField]
    private MeshRenderer _meshRenderer = null;

    [SerializeField]
    private Material _material = null;

    [SerializeField]
    private DiceEffectMaterial[] _diceEffectMaterials = null;
    
    [SerializeField]
    private MeshRenderer _effectTypeMaterial = null;
    
    public DiceEffectType EffectType { get; private set; }
    
    public void SetEffectType(DiceEffectType diceEffectType)
    {
        EffectType = diceEffectType;

        if (diceEffectType == DiceEffectType.NONE)
        {
            this._effectTypeMaterial.gameObject.SetActive(false);
        }
        else
        {
            this._effectTypeMaterial.material = _diceEffectMaterials.FirstOrDefault(o => o.EffectType == diceEffectType).Material;
            this._effectTypeMaterial.gameObject.SetActive(true);
        }
    }
}
