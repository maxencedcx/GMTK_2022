using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class GameTitle : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _gameTitleText;

    private Tween _fadeEffect;

    [SerializeField, Range(0.1f, 5)]
    private float _fadeDuration;

    [SerializeField]
    private float _waitForFade;

    private void Start()
    {
        FadeIn();
    }

    private void FadeIn()
    {
        _fadeEffect?.Kill();
        StartCoroutine(ResetFade());
    }

    private IEnumerator ResetFade()
    {
        while (true)
        {
            _fadeEffect = this._gameTitleText.DOFade(0, _fadeDuration).SetEase(Ease.InOutSine);
            yield return _fadeEffect.WaitForCompletion();
            _fadeEffect = this._gameTitleText.DOFade(1, _fadeDuration).SetEase(Ease.InOutSine);
            yield return _fadeEffect.WaitForCompletion();

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_waitForFade);
        }
    }
}
