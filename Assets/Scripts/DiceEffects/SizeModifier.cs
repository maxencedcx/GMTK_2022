using UnityEngine;
using DG.Tweening;

public abstract class SizeModifier : DiceEffect
{
    public SizeModifier(Dice dice, DiceEffectData diceEffectData, SizeModifierData sizeModifierData) : base(dice, diceEffectData)
    {
        _sizeModifierData = sizeModifierData;
    }

    protected SizeModifierData _sizeModifierData;

    public override bool CanApply(DiceEffectContext diceEffectContext)
    {
        return !this._dice.IsTeleporting;
    }

    public override void OnEffectOver()
    {
        base.OnEffectOver();
        this.Revert();
    }

    protected override void Apply(DiceEffectContext diceEffectContext)
    {
        this._dice.transform.DOScale(_sizeModifierData.Scale, _sizeModifierData.ScaleDuration);

        if (this._sizeModifierData.Clip != null)
        {
            RSLib.Audio.AudioManager.PlaySound(this._sizeModifierData.Clip);
        }
    }

    public virtual void Revert()
    {
        this._dice.transform.DOScale(1f, _sizeModifierData.ScaleDuration);
    }
}
