using UnityEngine;

public abstract class DiceEffect
{
    public DiceEffect(Dice dice, DiceEffectData diceEffectData)
    {
        this._dice = dice;
        this._dice.EffectAdded += OnEffectAdded;
        this.DiceEffectData = diceEffectData;
    }

    protected Dice _dice;
    private float _autoApplyTimer;
    
    public abstract DiceEffectType EffectType { get; }
    
    public DiceEffectData DiceEffectData { get; }

    public float Lifetime { get; private set; }

    public bool IsOver => this.Lifetime > this.DiceEffectData.Duration;
    
    protected virtual void OnEffectAdded(DiceEffectType effectType)
    {
        if (this.EffectType.IsIncompatibleWith(effectType))
        {
            this._dice.RemoveEffect(this);
        }
    }
    
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

    public virtual void OnEffectOver()
    {
    }
    
    public void FixedUpdate()
    {
        this.Lifetime += Time.fixedDeltaTime;

        if (this.DiceEffectData.AutoApply)
        {
            this._autoApplyTimer += Time.fixedDeltaTime;
            if (this._autoApplyTimer > this.DiceEffectData.Cooldown)
            {
                this.TryApply(new DiceEffectContext() {Players = Manager.TeamManager.Instance.Players});
            }
        }
    }
}
