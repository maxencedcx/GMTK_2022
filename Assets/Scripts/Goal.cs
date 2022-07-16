using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private Team _team = Team.NONE;

    [SerializeField]
    private UnityEngine.Events.UnityEvent<Team> _goalTriggered = null;

    public event System.Action<Team> GoalTriggered;
    
    public Team Team => this._team;

    [ContextMenu("On Goal Triggered")]
    private void OnGoalTriggered()
    {
        this._goalTriggered?.Invoke(this._team);
        this.GoalTriggered?.Invoke(this._team);
        Manager.GameManager.Instance.ScoreGoal(this._team);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Dice dice))
        {
            OnGoalTriggered();
        }
    }
}
