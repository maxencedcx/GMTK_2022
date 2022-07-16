using UnityEngine;

public class PlayerFaceCamera : MonoBehaviour
{
    private Transform _mainCameraTransform;

    private void Awake()
    {
        this._mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        this.transform.forward = transform.position - _mainCameraTransform.transform.position;
    }
}
