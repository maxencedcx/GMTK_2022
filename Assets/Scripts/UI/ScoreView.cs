using RSLib.Data;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    private Int _scoreInt;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    private void Awake()
    {
        this._scoreInt.ValueChanged += this.UpdateScoreText;
    }

    private void UpdateScoreText(Int.ValueChangedEventArgs args)
    {
        this._scoreText.text = args.New.ToString();
    }
}
