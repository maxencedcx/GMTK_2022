using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TeamChooser : MonoBehaviour
{
    [SerializeField]
    private Team _team;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Manager.TeamManager.Instance.AssignTeam(player, this._team);
        }
    }
}
