using DG.Tweening;
using RSLib.Extensions;
using UnityEngine;

public class TeleportingDice : DiceEffect
{
    public TeleportingDice(Dice dice, DiceEffectData diceEffectData, TeleportingDiceData teleportingDiceData) : base(dice, diceEffectData)
    {
        this._teleportingDiceData = teleportingDiceData;
    }

    private TeleportingDiceData _teleportingDiceData;

    public override DiceEffectType EffectType => DiceEffectType.TELEPORT;
    
    public override bool CanApply(DiceEffectContext diceEffectContext)
    {
        return true;
    }

    protected override void Apply(DiceEffectContext diceEffectContext)
    {
        this._dice.StartCoroutine(this.TeleportCoroutine(diceEffectContext));
        this.OnEffectApplied(diceEffectContext);
    }

    private System.Collections.IEnumerator TeleportCoroutine(DiceEffectContext diceEffectContext)
    {
        this._dice.IsTeleporting = true;
        
        this._teleportingDiceData.ParticlesSpawnerOnPrepare.SpawnParticles(this._dice.transform.position);

        Transform target = DiceTeleportTargets.Instance.GetTarget();
        Vector3 scale = this._dice.transform.localScale;

        Tween tween = this._dice.transform.DOScale(Vector3.zero, this._teleportingDiceData.ScaleTweenDuration);
        yield return tween.WaitForCompletion();
        
        if (this._teleportingDiceData.NullifyMovement)
        {
            this._dice.Rigidbody.NullifyMovement();
        }

        this._dice.transform.position = target.position;
        
        tween = this._dice.transform.DOScale(scale, this._teleportingDiceData.ScaleTweenDuration);
        Manager.GameManager.Instance.CameraShake.AddTrauma(this._teleportingDiceData.Trauma);
        this._teleportingDiceData.ParticlesSpawnerOnTeleported.SpawnParticles(target.position);
        yield return tween.WaitForCompletion();
        
        this._dice.IsTeleporting = false;
    }
}
