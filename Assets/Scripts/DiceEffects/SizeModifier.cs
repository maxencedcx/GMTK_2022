using UnityEngine;
using DG.Tweening;

public abstract class SizeModifier : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float _scale = 0f;

    [SerializeField]
    private float _scaleDuration = 0f;

    [SerializeField, Range(0f, 1f)]
    private float _shakeTrauma = 0.4f;

    protected Rigidbody _rigidbody;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    [ContextMenu("Apply")]
    public virtual void Apply()
    {
        this.transform.DOScale(_scale, _scaleDuration);
        FindObjectOfType<CameraShake>().SetTrauma(_shakeTrauma); // TODO: Remove FindObjectOfType.
    }

    [ContextMenu("Revert")]
    public virtual void Revert()
    {
        this.transform.DOScale(1f, _scaleDuration);
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SizeModifier))]
    public class SizeModifierEditor : RSLib.EditorUtilities.ButtonProviderEditor<SizeModifier>
    {
        protected override void DrawButtons()
        {
            DrawButton("Apply", Obj.Apply);
            DrawButton("Revert", Obj.Revert);
        }
    }
#endif
}
