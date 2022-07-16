using UnityEngine;

public abstract class DiceEffect
{
    public DiceEffect(Dice dice, DiceEffectData diceEffectData)
    {
        this._dice = dice;
        this.DiceEffectData = diceEffectData;
    }

    protected Dice _dice;
    private float _autoApplyTimer;
    
    public abstract DiceEffectType EffectType { get; }
    
    public DiceEffectData DiceEffectData { get; }

    public float Lifetime { get; private set; }

    public bool IsOver => this.Lifetime > this.DiceEffectData.Duration;
    
    public abstract bool CanApply(DiceEffectContext diceEffectContext);

    public bool TryApply(DiceEffectContext diceEffectContext)
    {
        if (!this.CanApply(diceEffectContext))
        {
            return false;
        }

        this.Apply(diceEffectContext);
        return true;
    }

    protected abstract void Apply(DiceEffectContext diceEffectContext);
    
    public virtual void OnEffectApplied(DiceEffectContext diceEffectContext)
    {
        this._autoApplyTimer = 0f;
    }
    
    public virtual void OnEffectStart(DiceEffectContext diceEffectContext)
    {
        if (DiceEffectData.ApplyInstantly)
        {
            this.TryApply(diceEffectContext);
        }
    }
    
    public virtual void OnLand(DiceEffectContext diceEffectContext)
    {
        if (DiceEffectData.ApplyOnLand)
        {
            this.TryApply(diceEffectContext);
        }
    }

    public void Update()
    {
        this.Lifetime += Time.deltaTime;

        if (this.DiceEffectData.AutoApply)
        {
            this._autoApplyTimer += Time.deltaTime;
            if (this._autoApplyTimer > this.DiceEffectData.Cooldown)
            {
                this.TryApply(new DiceEffectContext());
            }
        }
    }
}
