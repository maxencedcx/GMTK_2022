using RSLib.Extensions;
using UnityEngine;

public class Shockwave : DiceEffect
{
    public Shockwave(Dice dice, DiceEffectData diceEffectData, ShockwaveData shockwaveData) : base(dice, diceEffectData)
    {
        this._shockwaveData = shockwaveData;
        _colliders = new Collider[10]; // TODO: Replace with some static max number of players.
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
        Transform diceTransform = _dice.transform;
        Vector3 dicePosition = diceTransform.position;
        
        int collidersCount = Physics.OverlapSphereNonAlloc(dicePosition, this._shockwaveData.Range, this._colliders, this._shockwaveData.LayerMask);
        if (collidersCount > 0)
        {
            for (int i = 0; i < collidersCount; ++i)
            {
                Collider target = _colliders[i];

                if (target.transform == diceTransform)
                {
                    continue;
                }
                
                if (!target.TryGetComponent(out Rigidbody targetRigidbody))
                {
                    continue;
                }

                Vector3 targetPosition = target.transform.position;
                Vector3 direction = (targetPosition - dicePosition).normalized;
                float distance = Vector3.Distance(dicePosition, targetPosition);
                float force = RSLib.Maths.Maths.Normalize(distance, 0f, _shockwaveData.Range, _shockwaveData.ForceMinMax.y, _shockwaveData.ForceMinMax.x);
                
                targetRigidbody.AddForce(direction.WithZ(0f) * force, ForceMode.Impulse);
            }
        }

        this.OnEffectApplied(diceEffectContext);
    }

    public override void OnEffectApplied(DiceEffectContext diceEffectContext)
    {
        base.OnEffectApplied(diceEffectContext);
        
        this._shockwaveData.ParticlesSpawner.SpawnParticles(_dice.transform.position);
        UnityEngine.Object.FindObjectOfType<CameraShake>().SetTrauma(this._shockwaveData.Trauma); // TODO: Remove FindObjectOfType.
    }
}
