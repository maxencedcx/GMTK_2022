using UnityEngine;

[UnityEngine.CreateAssetMenu(fileName = "New Running Dice Data", menuName = "GMTK/Running Dice Data")]
public class RunningDiceData : ScriptableObject
{
    [SerializeField, Min(1f)]
    private float _detectionRange = 5f;

    [SerializeField, Min(0f)]
    private float _fleeDirectionSmoothing = 0f;

    [SerializeField, Min(0f)]
    private float _fleeDirectionForce = 0f;

    public float DetectionRange => this._detectionRange;
    public float DetectionRangeSqr => this.DetectionRange * this.DetectionRange;

    public float FleeDirectionSmoothing => this._fleeDirectionSmoothing;

    public float FleeDirectionForce => this._fleeDirectionForce;
}
