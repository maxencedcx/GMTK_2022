using RSLib.Extensions;
using UnityEngine;

public class PlayerFaceCamera : MonoBehaviour
{
    public enum Axis
    {
        FORWARD,
        UP,
        RIGHT
    }

    [SerializeField]
    private Axis _axis = Axis.FORWARD;

    [SerializeField]
    private bool _negate = false;

    private Transform _mainCameraTransform;

    private void Awake()
    {
        this._mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        switch (this._axis)
        {
            case Axis.FORWARD:
                this.transform.forward = transform.position - _mainCameraTransform.transform.position;
                if (this._negate)
                {
                    this.transform.forward = -this.transform.forward;
                }
                break;
            case Axis.UP:
                this.transform.up = transform.position - _mainCameraTransform.transform.position;
                if (this._negate)
                {
                    this.transform.up = -this.transform.up;
                }
                break;
            case Axis.RIGHT:
                this.transform.right = transform.position - _mainCameraTransform.transform.position;
                if (this._negate)
                {
                    this.transform.right = -this.transform.right;
                }
                break;
        }
    }
}
