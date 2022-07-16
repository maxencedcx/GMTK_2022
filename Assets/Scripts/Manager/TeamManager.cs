namespace Manager
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TeamManager : RSLib.Singleton<TeamManager>
    {
        [SerializeField]
        private List<GameObject> _teamChoosers = new();

        public readonly HashSet<Player> Players = new();

        public bool AreAllPlayersReady => this.Players.All(p => p.IsPlayerReady);

        public bool IsThereEnoughPlayers => this.Players.Count > 1;

        public bool AreTeamsValid => this.Players.Select(p => p.Team).Distinct().Count() > 1;

        public List<Player> PinkPlayers => this.Players.Where(p => p.Team == Team.PINK).ToList();

        public List<Player> BluePlayers => this.Players.Where(p => p.Team == Team.BLUE).ToList();

        public void RegisterPlayer(Player player)
        {
            if (this.Players.Contains(player))
            {
                return;
            }
            
            this.Players.Add(player);
        }
        
        public void AssignTeam(Player player, Team team)
        {
            player.SetTeam(team);
        }

        #region TeamChoosers

        public void DisableTeamChoosers()
        {
            for (int i = this._teamChoosers.Count - 1; i >= 0; i--)
            {
                this._teamChoosers[i].SetActive(false);
            }
        }
        
        public void EnableTeamChoosers()
        {
            for (int i = this._teamChoosers.Count - 1; i >= 0; i--)
            {
                this._teamChoosers[i].SetActive(true);
            }
        }

        #endregion
    }
}