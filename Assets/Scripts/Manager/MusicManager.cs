using UnityEngine;

public class MusicManager : RSLib.Singleton<MusicManager>
{
    [SerializeField]
    private RSLib.Audio.ClipProvider _lobbyMusic = null;
    
    [SerializeField]
    private RSLib.Audio.ClipProvider _gameMusic = null;

    public void PlayLobbyMusic()
    {
        RSLib.Audio.AudioManager.PlayMusic(this._lobbyMusic, RSLib.Audio.MusicTransitionsDatas.Default);
    }
    
    public void PlayGameMusic()
    {
        RSLib.Audio.AudioManager.PlayMusic(this._gameMusic, RSLib.Audio.MusicTransitionsDatas.Default);
    }

    private void Start()
    {
        this.PlayLobbyMusic();
    }
}
