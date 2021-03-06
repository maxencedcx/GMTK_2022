using RSLib.Extensions;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private Team _team = Team.NONE;

    [SerializeField]
    private UnityEngine.Events.UnityEvent<Team> _goalTriggered = null;

    [SerializeField]
    private RSLib.Audio.ClipProvider _goalClip = null;
    
    [SerializeField]
    private RSLib.Audio.ClipProvider _applauseClip = null;

    [UnityEngine.RangeAttribute(0f, 1f)]
    public float Trauma;

    [SerializeField]
    private Transform _lightPillarHeightPivot = null;

    [SerializeField]
    private GameObject _lightPillar = null;
    
    public event System.Action<Team> GoalTriggered;
    
    public Team Team => this._team;

    [ContextMenu("On Goal Triggered")]
    private void OnGoalTriggered()
    {
        this._goalTriggered?.Invoke(this._team);
        this.GoalTriggered?.Invoke(this._team);
        Manager.GameManager.Instance.ScoreGoal(this._team);
        
        Manager.GameManager.Instance.CameraShake.AddTrauma(this.Trauma);
        RSLib.Audio.AudioManager.PlaySound(this._goalClip);
        RSLib.Audio.AudioManager.PlaySound(this._applauseClip);

        this._lightPillar.transform.SetPositionY(this._lightPillarHeightPivot.position.y);
        this._lightPillar.SetActive(true);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Dice dice))
        {
            OnGoalTriggered();
        }
    }
}
