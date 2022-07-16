using RSLib.Extensions;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _mine;

    public void NewMine()
    {
        Instantiate(_mine, transform.position.AddY(0.5f), _mine.transform.rotation, transform);
    }
}
