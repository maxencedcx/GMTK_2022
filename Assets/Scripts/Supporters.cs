using UnityEngine;

public class Supporters : MonoBehaviour
{
    [SerializeField]
    private Animator _animator = null;

    [SerializeField]
    private Goal[] _goals = null;

    [SerializeField]
    private float _goalAnimDuration = 3f;
    
    private void Awake()
    {
        this._animator.Play("Idle", 0, Random.value);

        for (int i = 0; i < this._goals.Length; ++i)
        {
            this._goals[i].GoalTriggered += OnGoal;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < this._goals.Length; ++i)
        {
            this._goals[i].GoalTriggered -= OnGoal;
        }
    }

    private void OnGoal(Team team)
    {
        StartCoroutine(this.GoalAnimationCoroutine());
    }

    private System.Collections.IEnumerator GoalAnimationCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.6f));
        this._animator.SetBool("Goal", true);
        yield return new WaitForSeconds(this._goalAnimDuration + Random.Range(0f, 0.3f));
        this._animator.SetBool("Goal", false);
    }
}
