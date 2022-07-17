using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : RSLib.Singleton<SpawnManager>
{
    public static int PlayerIndex = -1;
    
    [SerializeField]
    private List<Transform> _spawnPoints;

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
            Manager.UIManager.Instance.SetActiveGameTitle(false);
            Manager.UIManager.Instance.SetActiveHowToPlay(true);
        }
        
        playerInput.transform.position = this._spawnPoints[++PlayerIndex].position;
    }
}
