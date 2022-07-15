using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private RSLib.Shake.ShakeSettings _shakeSettings = RSLib.Shake.ShakeSettings.Default;

    private RSLib.Shake _shake;
    
    public void AddTrauma(float trauma)
    {
        this._shake.AddTrauma(trauma);
    }

    public void SetTrauma(float trauma)
    {
        this._shake.SetTrauma(trauma);
    }

    private void Awake()
    {
        this._shake = new RSLib.Shake(this._shakeSettings);
    }

    private void Update()
    {
        (Vector3 pos, Quaternion rot)? shake = this._shake.Evaluate(transform);
        if (shake.HasValue)
        {
            this.transform.position = shake.Value.pos;
            this.transform.rotation = shake.Value.rot;
        }
        else
        {
            this.transform.position = Vector3.zero;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(CameraShake))]
    public class CameraShakeEditor : RSLib.EditorUtilities.ButtonProviderEditor<CameraShake>
    {
        protected override void DrawButtons()
        {
            DrawButton("Trauma 1", () => Obj.SetTrauma(1f));
        }
    }
#endif
}
