using RSLib.Extensions;
using UnityEngine;

public class RunningDice : MonoBehaviour
{
    [SerializeField]
    private Transform[] _players = null;

    [SerializeField, Min(1f)]
    private float _detectionRange = 5f;

    [SerializeField, Min(0f)]
    private float _fleeDirectionSmoothing = 0f;

    private Vector3 _refFleeDirectionSmoothing;
    private Vector3 _fleeDirection;
    
    private Vector3 GetFleeDirection()
    {
        Vector3 toPlayers = Vector3.zero;

        for (int i = this._players.Length - 1; i >= 0; --i)
        {
            Transform player = this._players[i];
            Vector3 toPlayer = player.position.WithY(0f) - this.transform.position.WithY(0f);

            if (toPlayer.sqrMagnitude > this._detectionRange * this._detectionRange)
            {
                continue;
            }

            float distancePercentage = toPlayer.magnitude / this._detectionRange;
            toPlayers += toPlayer * (1f - distancePercentage); // The closer the player is, the more influence it has on flee direction.
        }

        return -toPlayers.normalized;
    }
    
    private void Update()
    {
        this._fleeDirection = Vector3.SmoothDamp(this._fleeDirection, this.GetFleeDirection(), ref this._refFleeDirectionSmoothing, this._fleeDirectionSmoothing);
        this._fleeDirection.Normalize();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 position = this.transform.position;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(position, this._detectionRange);
        Gizmos.DrawLine(position, position + this._fleeDirection);
    }
}
