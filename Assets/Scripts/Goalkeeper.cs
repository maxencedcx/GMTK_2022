using RSLib.Extensions;
using UnityEngine;

public class Goalkeeper : MonoBehaviour
{
    [SerializeField]
    private float _angle = 180f;

    [SerializeField]
    private float _speed = 10f;
    
    private float _currentAngle;
    private int _direction = 1;
    private float _initEulerAnglesY;

    private void Awake()
    {
        this._initEulerAnglesY = this.transform.localEulerAngles.y;
    }

    private void Update()
    {
        this._currentAngle += this._speed * Time.deltaTime * this._direction;

        bool limitExceed = false;
        if (this._currentAngle > _angle * 0.5f)
        {
            this._currentAngle = _angle * 0.5f;
            limitExceed = true;
        }
        else if (this._currentAngle < -_angle * 0.5f)
        {
            this._currentAngle = -_angle * 0.5f;
            limitExceed = true;
        }
        
        if (limitExceed)
        {
            this._direction = -this._direction;
        }
        
        this.transform.SetEulerAnglesY(this._currentAngle + this._initEulerAnglesY);
    }
}
