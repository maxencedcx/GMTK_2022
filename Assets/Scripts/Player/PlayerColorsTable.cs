using UnityEngine;

[CreateAssetMenu(fileName = "New Player Colors Table", menuName = "GMTK/Player Colors")]
public class PlayerColorsTable : ScriptableObject
{
    [SerializeField]
    private Color[] _pinkPlayers = null;
    
    [SerializeField]
    private Color[] _bluePlayers = null;

    public Color GetPinkColorAtIndex(int index)
    {
        return this._pinkPlayers[index % this._pinkPlayers.Length];
    }
    
    public Color GetBlueColorAtIndex(int index)
    {
        return this._bluePlayers[index % this._bluePlayers.Length];
    }
}
