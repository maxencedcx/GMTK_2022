using DG.Tweening;
using RSLib.Extensions;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    [SerializeField]
    private float _anchorPosOffscreen = 200f;

    [SerializeField]
    private float _inDuration = 0.5f;
    [SerializeField]
    private Ease _inEase = Ease.InCirc;
        
    [SerializeField]
    private float _outDuration = 0.8f;
    [SerializeField]
    private Ease _outEase = Ease.OutQuint;
    
    private RectTransform _rectTransform;
    
    public void OnGameStart()
    {
        this._rectTransform.DOAnchorPosY(0f, this._inDuration)
                           .SetEase(this._inEase);
    }

    public void OnBackToLobby()
    {
        this._rectTransform.DOAnchorPosY(this._anchorPosOffscreen, this._outDuration)
                           .SetEase(this._outEase);
    }

    private void Awake()
    {
        this._rectTransform = this.transform as RectTransform;

        this._rectTransform.anchoredPosition = this._rectTransform.anchoredPosition.WithY(this._anchorPosOffscreen);
    }
}
