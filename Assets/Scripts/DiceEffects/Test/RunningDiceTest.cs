using RSLib.Extensions;
using UnityEngine;

public class RunningDiceTest : MonoBehaviour
{
    [SerializeField]
    private Transform[] _players = null;

    [SerializeField]
    private DiceEffectData _diceEffectData = null;
    
    [SerializeField]
    private RunningDiceData _runningDiceData = null;

    private RunningDice _runningDice;
    
    private Vector3 _refFleeDirectionSmoothing;
    private Vector3 _fleeDirection;

    private void Awake()
    {
        this._runningDice = new RunningDice(this.GetComponent<Dice>(), _diceEffectData, this._runningDiceData);
    }

    // private void Update()
    // {
    //     DiceEffectContext context = new()
    //     {
    //         Players = this._players
    //     };
    //     
    //     Vector3 currentFleeDirection = this._runningDice.GetFleeDirection(context);
    //     this._fleeDirection = Vector3.SmoothDamp(this._fleeDirection, currentFleeDirection, ref this._refFleeDirectionSmoothing, this._runningDiceData.FleeDirectionSmoothing);
    //     this._fleeDirection.Normalize();
    //     
    //     this._runningDice.Update();
    //     if (this._runningDice.IsOver)
    //     {
    //         this._runningDice = null;
    //     }
    // }

    private void OnDrawGizmosSelected()
    {
        if (this._runningDiceData != null)
        {
            Vector3 position = this.transform.position;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(position, this._runningDiceData.DetectionRange);
            Gizmos.DrawLine(position, position + this._fleeDirection);
        }
    }
}
