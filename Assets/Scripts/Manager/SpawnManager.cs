using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;

public class SpawnManager : RSLib.Singleton<SpawnManager>
{
    public static int PlayerIndex = -1;
    
    [SerializeField]
    private List<Transform> _spawnPoints;

    [SerializeField]
    private float _moveDuration;

    private Tween _moveGameTitle;

    protected override void Awake()
    {
        base.Awake();
        
        InputSystem.onDeviceChange +=
            (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        // New Device.
                        break;
                    case InputDeviceChange.Disconnected:
                        // Device got unplugged.
                        break;
                    case InputDeviceChange.Reconnected:
                        // Plugged back in.
                        break;
                }
            };
    }
    
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (PlayerIndex + 1 > this._spawnPoints.Count - 1)
        {
            return;
        }

        if (PlayerIndex == -1)
        {
            StartCoroutine(MoveGameTitle());
            Manager.UIManager.Instance.SetActiveHowToPlay(true);
        }
        
        playerInput.transform.position = this._spawnPoints[++PlayerIndex].position;
    }

    private IEnumerator MoveGameTitle()
    {
        _moveGameTitle = Manager.UIManager.Instance._gameTitleObject.transform.DOMoveY(25, _moveDuration).SetEase(Ease.OutExpo);
        yield return _moveGameTitle.WaitForCompletion();
        Manager.UIManager.Instance.SetActiveGameTitle(false);
    }
}
