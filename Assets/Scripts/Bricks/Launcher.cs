using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField]
    private float _moveForce;

    private void OnTriggerStay(Collider col)
    {
        col.gameObject.GetComponent<Rigidbody>().AddForce(_moveForce, 0, 0, ForceMode.Force);
    }
}
