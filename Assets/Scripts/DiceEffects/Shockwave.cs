using RSLib.Extensions;
using UnityEngine;

public class Shockwave : DiceEffect
{
    public Shockwave(Dice dice, DiceEffectData diceEffectData, ShockwaveData shockwaveData) : base(dice, diceEffectData)
    {
        this._shockwaveData = shockwaveData;
        _colliders = new Collider[10]; 
    }

    private ShockwaveData _shockwaveData;
    private Collider[] _colliders;

    public override DiceEffectType EffectType => DiceEffectType.SHOCKWAVE;
    
    public override bool CanApply(DiceEffectContext diceEffectContext)
    {
        // TODO: Check ground.
        return true;
    }

    protected override void Apply(DiceEffectContext diceEffectContext)
    {
        ShockwaveController shockwaveController = new ShockwaveController();

        shockwaveController.Apply(_dice.transform, _shockwaveData);

        this.OnEffectApplied(diceEffectContext);
    }

    public override void OnEffectApplied(DiceEffectContext diceEffectContext)
    {
        base.OnEffectApplied(diceEffectContext);
        
        this._shockwaveData.ParticlesSpawner.SpawnParticles(_dice.transform.position);
        Manager.GameManager.Instance.CameraShake?.SetTrauma(this._shockwaveData.Trauma);

        if (this._shockwaveData.Clip != null)
        {
            RSLib.Audio.AudioManager.PlaySound(this._shockwaveData.Clip);
        }
    }
}
