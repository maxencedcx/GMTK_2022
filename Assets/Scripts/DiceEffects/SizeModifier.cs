using UnityEngine;
using DG.Tweening;

public abstract class SizeModifier : DiceEffect
{
    public SizeModifier(Dice dice, DiceEffectData diceEffectData, SizeModifierData sizeModifierData) : base(dice, diceEffectData)
    {
        _sizeModifierData = sizeModifierData;
    }

    //[SerializeField, Range(0f, 1f)]
    //private float _shakeTrauma = 0.4f;

    protected SizeModifierData _sizeModifierData;
    //protected Rigidbody _rigidbody;

    public override bool CanApply(DiceEffectContext diceEffectContext)
    {
        return true;
    }

    protected override void Apply(DiceEffectContext diceEffectContext)
    {
        this._dice.transform.DOScale(_sizeModifierData.Scale, _sizeModifierData.ScaleDuration);
        //findobjectoftype<camerashake>().settrauma(_shaketrauma); // todo: remove findobjectoftype.
    }

    //private void Start()
    //{
    //    _rigidbody = GetComponent<Rigidbody>();
    //}

    //[ContextMenu("Apply")]
    //public virtual void Apply()
    //{
    //    this.transform.DOScale(_scale, _scaleDuration);
    //    FindObjectOfType<CameraShake>().SetTrauma(_shakeTrauma); // TODO: Remove FindObjectOfType.
    //}

    public virtual void Revert()
    {
        this._dice.transform.DOScale(1f, _sizeModifierData.ScaleDuration);
    }


    //#if UNITY_EDITOR
    //    [UnityEditor.CustomEditor(typeof(SizeModifier))]
    //    public class SizeModifierEditor : RSLib.EditorUtilities.ButtonProviderEditor<SizeModifier>
    //    {
    //        protected override void DrawButtons()
    //        {
    //            DrawButton("Apply", Obj.Apply);
    //            DrawButton("Revert", Obj.Revert);
    //        }
    //    }
    //#endif
}
