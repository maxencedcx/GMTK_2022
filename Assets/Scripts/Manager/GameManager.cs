namespace Manager
{
    using System.Collections;
    using UnityEngine;

    public class GameManager : RSLib.Singleton<GameManager>
    {
        public GameState State { get; private set; } = GameState.LOBBY;

        [SerializeField]
        private GameObject _dicePrefab = null;

        [SerializeField]
        private Transform _diceSpawnPoint = null;

        [SerializeField]
        private DiceEffectsIncompatibilitiesTable _effectsIncompatibilities = null;

        public DiceEffectsIncompatibilitiesTable EffectsIncompatibilitiesTable => this._effectsIncompatibilities;
        
        public bool TryStartGame()
        {
            if (this.State != GameState.LOBBY
                || !Manager.TeamManager.Instance.IsThereEnoughPlayers
                || !Manager.TeamManager.Instance.AreAllPlayersReady
                || !Manager.TeamManager.Instance.AreTeamsValid)
            {
                return false;
            }
            
            this.StartCoroutine(this.StartGame());
            return true;
        }

        private IEnumerator StartGame()
        {
            Manager.TeamManager.Instance.DisableTeamChoosers();
            this.State = GameState.STARTING;
            yield return new WaitForSeconds(3);
            this.State = GameState.RUNNING;
            Instantiate(this._dicePrefab, this._diceSpawnPoint);
        }
    }
}