using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    using System;
    using System.Linq;
    using Random = UnityEngine.Random;

    public class DiceFaceChoiceManager : RSLib.Singleton<DiceFaceChoiceManager>
    {
        [SerializeField]
        private GameObject DiceChoiceParent;
        
        [SerializeField]
        private Transform Dice;

        private readonly Dictionary<Team, List<int>> _playerChoiceIndexes = new Dictionary<Team, List<int>>()
        {
            {Team.BLUE, new List<int>()},
            {Team.PINK, new List<int>()},
        };

        private Player _currentPlayerChoosing;
        
        public DiceEffectType SelectedDiceEffectType { get; private set; }

        public void RotateCube(Vector3 direction)
        {
            this.Dice.Rotate(direction, 90, Space.World);
        }

        public void ValidateChoice()
        {
            Manager.GameManager.Instance.AddDiceFace((this.SelectedDiceEffectType, Random.Range(0, 6)));
            this.DiceChoiceParent.SetActive(false);
            Manager.GameManager.Instance.State = GameState.RUNNING;
            this._currentPlayerChoosing.DisableCubeChoiceInputs();
            this.EnableAllPlayerInputs();
        }

        public void ChangeSelectedEffect(int direction)
        {
            this.SelectedDiceEffectType = (DiceEffectType)Mathf.Clamp((int)this.SelectedDiceEffectType + direction, (int)DiceEffectType.NONE + 1,Enum.GetValues(typeof(DiceEffectType)).Length);
            Debug.Log($"chose {this.SelectedDiceEffectType}");
        }

        public void DisableAllPlayerInputs()
        {
            HashSet<Player> players = Manager.TeamManager.Instance.Players;
        
            foreach (Player player in players)
            {
                player.DisablePlayerInputs();
            }
        }
    
        public void EnableAllPlayerInputs()
        {
            HashSet<Player> players = Manager.TeamManager.Instance.Players;
        
            foreach (Player player in players)
            {
                player.EnablePlayerInputs();
            }
        }

        public int GetNewPlayerIndexForTeam(Team team)
        {
            List<Player> players;
            
            switch (team)
            {
                case Team.BLUE:
                    players = Manager.TeamManager.Instance.BluePlayers;
                    break;
                case Team.PINK:
                    players = Manager.TeamManager.Instance.PinkPlayers;
                    break;
                default:
                    return 0;
            }
            
            Player player = players.FirstOrDefault(p => !this._playerChoiceIndexes[team].Contains(p.PlayerIndex));

            if (player == null)
            {
                this._playerChoiceIndexes[team].Clear();
                player = players.First();
            }
                    
            this._playerChoiceIndexes[team].Add(player.PlayerIndex);
            return player.PlayerIndex;
        }

        public void StartChoice(int playerIndex)
        {
            this.DisableAllPlayerInputs();
            this._currentPlayerChoosing = TeamManager.Instance.Players.First(p => p.PlayerIndex == playerIndex);
            this._currentPlayerChoosing.EnableCubeChoiceInputs();
            this.DiceChoiceParent.SetActive(true);
        }
    }
}
