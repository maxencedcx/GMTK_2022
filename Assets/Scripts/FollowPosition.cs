using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField]
    private Transform _target = null;
    
    [SerializeField]
    private Vector3 offset = Vector3.zero;

    private void Update()
    {
        this.transform.position = this._target.position + this.offset;
    }
}
