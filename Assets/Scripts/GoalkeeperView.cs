using UnityEngine;

public class GoalkeeperView : MonoBehaviour
{
    [SerializeField]
    private Animator _animator = null;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Dice>(out _))
        {
            Debug.LogError("TODO: goalkeeper stop animation");
        }
    }
}
