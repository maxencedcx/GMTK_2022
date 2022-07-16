using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dice : MonoBehaviour
{
    [SerializeField]
    private DiceFace[] _diceFaces = null;

    private DiceFace highestFace;

    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _initialForce;

    private void Awake()
    {
        this._rigidbody ??= this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.transform.rotation = UnityEngine.Random.rotation;
        this._rigidbody.AddForce(Vector3.forward * this._initialForce, ForceMode.Impulse);
        this._rigidbody.AddTorque(UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360), ForceMode.Impulse);
    }

    [ContextMenu("Get Highest Face")]
    private DiceFace GetHighestFace()
    {
        float highestPos = float.MinValue;
        
        for (int i = 0; i < _diceFaces.Length; i++)
        {
            float posY = _diceFaces[i].transform.position.y;

            if (posY > highestPos)
            {
                highestPos = posY;
                highestFace = _diceFaces[i];
            }
        }

        return highestFace;
    }
}
