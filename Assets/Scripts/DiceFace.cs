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
    private DiceEffectMaterial[] _diceEffectMaterials = null;
    
    [SerializeField]
    private MeshRenderer _faceRenderer = null;
    
    [SerializeField]
    private MeshRenderer _effectTypeMaterial = null;

    [SerializeField]
    private Material _invisibleMaterial = null;

    private Material _initMaterial;
    
    public DiceEffectType EffectType { get; private set; }

    private void Awake()
    {
        this._initMaterial = this._faceRenderer.material;
    }

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

    public void SetInvisible(bool state)
    {
        this._faceRenderer.material = state ? this._invisibleMaterial : this._initMaterial;
        this._effectTypeMaterial.gameObject.SetActive(!state);
    }
}
