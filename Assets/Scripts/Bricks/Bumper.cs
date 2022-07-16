using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    [SerializeField]
    private float _bumpForce = 0f;

    [SerializeField]
    private float _bumperRadius = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null)
        {
            collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(_bumpForce, transform.position, _bumperRadius, 0f, ForceMode.Impulse);
        }

    }
}
