using RSLib.Data;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    private Int _scoreInt;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField, Range(0.1f, 3)]
    private float _punchIntensity;

    [SerializeField, Range(0.1f, 3)]
    private float _punchDuration;

    private DG.Tweening.Tween _punchEffect;

    private void Awake()
    {
        this._scoreInt.ValueChanged += this.UpdateScoreText;
    }

    private void UpdateScoreText(Int.ValueChangedEventArgs args)
    {
        this._scoreText.text = args.New.ToString();
        OnScoreUpdate();
    }

    private void OnScoreUpdate()
    {
        this._punchEffect?.Kill();
        this._punchEffect = this._scoreText.transform.DOPunchScale(Vector3.one * this._punchIntensity, this._punchDuration, 0, 0f);
    }
}
