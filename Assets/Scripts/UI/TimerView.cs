using RSLib.Data;
using System;
using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;
    
    [SerializeField]
    private RSLib.Data.Float _gameTimer;

    private void Awake()
    {
        this._gameTimer.ValueChanged += this.OnTimerValueChanged;
    }

    private void OnTimerValueChanged(Float.ValueChangedEventArgs args)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(args.New);
        this._timerText.text = $"{t:mm\\:ss}";
    }
}
