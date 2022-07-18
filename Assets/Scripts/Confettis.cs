using UnityEngine;

public class Confettis : MonoBehaviour
{
    [SerializeField]
    private Team _team = Team.NONE;
    
    [SerializeField]
    private Goal[] _goals = null;

    [SerializeField]
    private ParticleSystem[] _confettis = null;
    
    private void Awake()
    {
        for (int i = 0; i < this._goals.Length; i++)
        {
            this._goals[i].GoalTriggered += OnGoalTriggered;
        }
    }

    private void OnGoalTriggered(Team team)
    {
        if (this._team != team)
        {
            return;
        }
        
        for (int i = 0; i < this._confettis.Length; i++)
        {
            this._confettis[i].Play();
        }
    }
}
