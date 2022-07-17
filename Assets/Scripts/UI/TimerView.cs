using RSLib.Data;
using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TimerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;
    
    [SerializeField]
    private RSLib.Data.Float _gameTimer;

    [SerializeField, Range(0.01f, 3)]
    private float _punchIntensity;

    [SerializeField, Range(0.1f, 3)]
    private float _punchDuration;
    
    [SerializeField]
    private UnityEngine.Color _finalSecColor;

    [SerializeField]
    private GameObject _goldenDiceImage = null;
    
    [SerializeField]
    private AudioSource _finalCountdownSource = null;
    
    private DG.Tweening.Tween _punchEffect;

    private int _previousSeconds;

    private void Awake()
    {
        this._gameTimer.ValueChanged += this.OnTimerValueChanged;
    }

    private void OnTimerValueChanged(Float.ValueChangedEventArgs args)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(args.New);
        this._timerText.text = Mathf.CeilToInt((float)t.TotalSeconds).ToString();

        if (t.Seconds != this._previousSeconds
            && t.TotalSeconds < 10)
        {
            OnSecondPassed();
        }

        if (Manager.GameManager.Instance.State == GameState.RUNNING
            && t.TotalSeconds < 10)
        {
            OnLastSeconds();

            if (t.TotalSeconds == 0)
            {
                this._finalCountdownSource.Stop();
            }
        }
        
        this._goldenDiceImage.SetActive(Manager.GameManager.Instance.WinningTeam == Team.NONE && Manager.GameManager.Instance.IsTimerOver);

        this._previousSeconds = t.Seconds;
    }

    private void OnSecondPassed()
    {
        this._punchEffect?.Kill();
        this._punchEffect = this._timerText.transform.DOPunchScale(Vector3.one * this._punchIntensity, this._punchDuration, 0, 0f);
    }

    private void OnLastSeconds()
    {
        this._timerText.color = _finalSecColor;

        if (!this._finalCountdownSource.isPlaying)
        {
            this._finalCountdownSource.Play();
        }
    }
}
