using RSLib.Extensions;
using UnityEngine;

public class RunningDice : DiceEffect
{
    public RunningDice(Dice dice, DiceEffectData diceEffectData, RunningDiceData runningDiceData) : base(dice, diceEffectData)
    {
        this._runningDiceData = runningDiceData;
    }
    
    private RunningDiceData _runningDiceData;

    public override DiceEffectType EffectType => DiceEffectType.RUNNING_DICE;

    public override bool CanApply(DiceEffectContext diceEffectContext)
    {
        return true;
    }

    protected override void Apply(DiceEffectContext diceEffectContext)
    {
        this._dice.Rigidbody.AddForce(this.GetFleeDirection(diceEffectContext) * this._runningDiceData.FleeDirectionForce, ForceMode.Impulse);
    }

    public Vector3 GetFleeDirection(DiceEffectContext diceEffectContext)
    {
        Vector3 toPlayers = Vector3.zero;

        foreach (Player player in diceEffectContext.Players)
        {
            Vector3 toPlayer = player.transform.position.WithY(0f) - this._dice.transform.position.WithY(0f);

            if (toPlayer.sqrMagnitude > this._runningDiceData.DetectionRangeSqr)
            {
                continue;
            }

            float distancePercentage = toPlayer.magnitude / this._runningDiceData.DetectionRange;
            toPlayers += toPlayer * (1f - distancePercentage); // The closer the player is, the more influence it has on flee direction.
        }

        return -toPlayers.normalized;
    }
}
