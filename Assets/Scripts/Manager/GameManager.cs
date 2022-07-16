namespace Manager
{
    using UnityEngine;

    public class GameManager : RSLib.Singleton<GameManager>
    {
        public GameState State { get; private set; } = GameState.LOBBY;

        [SerializeField]
        private GameObject _dicePrefab;

        [SerializeField]
        private Transform _diceSpawnPoint;
        
        public bool TryStartGame()
        {
            if (this.State != GameState.LOBBY
                || !Manager.TeamManager.Instance.IsThereEnoughPlayers
                || !Manager.TeamManager.Instance.AreAllPlayersReady)
            {
                return false;
            }

            Debug.Log($"launch game!");
            this.State = GameState.RUNNING;
            Instantiate(this._dicePrefab, this._diceSpawnPoint);
            Manager.TeamManager.Instance.DisableTeamChoosers();
            return true;
        }
    }
}