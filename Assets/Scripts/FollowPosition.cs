using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField]
    private Transform _target = null;

    private void Update()
    {
        this.transform.position = this._target.position;
    }
}
