namespace Manager
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TeamManager : RSLib.Singleton<TeamManager>
    {
        [SerializeField]
        private List<GameObject> _teamChoosers = new();

        [SerializeField]
        private PlayerColorsTable _playerColorsTable = null;
        
        public readonly HashSet<Player> Players = new();

        public PlayerColorsTable PlayerColorsTable => this._playerColorsTable;
        
        public bool AreAllPlayersReady => this.Players.All(p => p.IsPlayerReady);

        public bool IsThereEnoughPlayers => this.Players.Count > 1;

        public bool AreTeamsValid => this.Players.Select(p => p.Team).Distinct().Count() > 1;

        public List<Player> PinkPlayers => this.Players.Where(p => p.Team == Team.PINK)
                                                       .OrderBy(p => p.PlayerIndex)
                                                       .ToList();

        public List<Player> BluePlayers => this.Players.Where(p => p.Team == Team.BLUE)
                                                       .OrderBy(p => p.PlayerIndex)
                                                       .ToList();

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

            switch (team)
            {
                case Team.BLUE:
                    this.BluePlayers.ForEach(o => o.UpdatePlayerColor());
                    break;
                case Team.PINK:
                    this.PinkPlayers.ForEach(o => o.UpdatePlayerColor());
                    break;
            }
        }

        public void UnreadyAllPlayers()
        {
            foreach (Player player in this.Players)
            {
                player.IsPlayerReady = false;
            }
        }

        #region TeamChoosers

        public void SetActiveTeamChoosers(bool active)
        {
            for (int i = this._teamChoosers.Count - 1; i >= 0; i--)
            {
                this._teamChoosers[i].SetActive(active);
            }
        }
        
        #endregion
    }
}