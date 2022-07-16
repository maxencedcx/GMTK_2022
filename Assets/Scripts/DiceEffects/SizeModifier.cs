using UnityEngine;
using DG.Tweening;

public abstract class SizeModifier : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float _scale = 0f;

    [SerializeField]
    private float _scaleDuration = 0f;

    [ContextMenu("Apply")]
    protected virtual void Apply() => this.transform.DOScale(_scale, _scaleDuration);

    [ContextMenu("Revert")]
    protected void Revert() => this.transform.DOScale(1f, _scaleDuration);
}
