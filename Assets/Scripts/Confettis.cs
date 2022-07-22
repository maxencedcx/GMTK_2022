using UnityEngine;

public class Confettis : MonoBehaviour
{
    [SerializeField]
    private Team _team = Team.NONE;
    
    [SerializeField]
    private Goal[] _goals = null;

    [SerializeField]
    private ParticleSystem[] _confettis = null;

    [SerializeField]
    private int _victoryShoots = 3;
    
    [SerializeField]
    private float _victoryShootsDelay = 0.7f;

    [SerializeField, Range(0f, 1f)]
    private float _victoryShootTrauma = 0.15f;

    private void Awake()
    {
        for (int i = 0; i < this._goals.Length; i++)
        {
            this._goals[i].GoalTriggered += OnGoalTriggered;
        }
    }

    private void OnGoalTriggered(Team team)
    {
        if (this._team == team)
        {
            return;
        }
        
        for (int i = 0; i < this._confettis.Length; i++)
        {
            this._confettis[i].Play();
        }
    }

    public void OnGameEnd()
    {
        if (this._team != Manager.GameManager.Instance.WinningTeam)
        {
            return;
        }

        this.StartCoroutine(this.VictoryCoroutine());
    }

    private System.Collections.IEnumerator VictoryCoroutine()
    {
        for (int i = 0; i < this._victoryShoots; ++i)
        {
            for (int j = 0; j < this._confettis.Length; j++)
            {
                this._confettis[j].Play();
            }
            
            Manager.GameManager.Instance.CameraShake.AddTrauma(this._victoryShootTrauma);
            yield return new WaitForSeconds(this._victoryShootsDelay);
        }
    }
}
