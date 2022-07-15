using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private Team _team = Team.NONE;

    [SerializeField]
    private UnityEngine.Events.UnityEvent _goalTriggered = null;

    public event System.Action GoalTriggered;
    
    public Team Team => this._team;

    [ContextMenu("On Goal Triggered")]
    private void OnGoalTriggered()
    {
        this._goalTriggered?.Invoke();
        this.GoalTriggered?.Invoke(); 
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError($"Trigger enter (team {Team}).");
        OnGoalTriggered();
    }
}
