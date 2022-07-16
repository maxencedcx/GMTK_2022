using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;

    private void Update()
    {
        this._timerText.text = "00:00";
    }
}
