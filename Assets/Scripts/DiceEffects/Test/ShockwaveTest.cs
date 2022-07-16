using UnityEngine;

public class ShockwaveTest : MonoBehaviour
{
    [SerializeField]
    private DiceEffectData _shockwaveEffectData = null;
    
    [SerializeField]
    private ShockwaveData _shockwaveData = null;

    [SerializeField]
    private LayerMask _layerMask = 0;
    
    private Shockwave _shockwave;
    
    public void ApplyShockwave()
    {
        this._shockwave.TryApply(new DiceEffectContext());
    }

    private void Awake()
    {
        this._shockwave = new Shockwave(this.GetComponent<Dice>(), this._shockwaveEffectData, this._shockwaveData);
    }

    private void Start()
    {
        this._shockwave.OnEffectStart(new DiceEffectContext());
    }

    private void Update()
    {
        this._shockwave?.Update();
        
        if (this._shockwave is {IsOver: true})
        {
            this._shockwave = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (this._shockwaveData != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, this._shockwaveData.Range);
        }
    }

    private void DebugOnLand()
    {
        this._shockwave.OnLand(new DiceEffectContext());
    }
    
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ShockwaveTest))]
    public class ShockwaveEditor : RSLib.EditorUtilities.ButtonProviderEditor<ShockwaveTest>
    {
        protected override void DrawButtons()
        {
            DrawButton("Apply", Obj.ApplyShockwave);
            DrawButton("On Land", Obj.DebugOnLand);
        }
    }
#endif
}
