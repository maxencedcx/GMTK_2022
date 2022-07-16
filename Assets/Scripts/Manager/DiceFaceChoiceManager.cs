using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    using System.Linq;

    public class DiceFaceChoiceManager : RSLib.Singleton<DiceFaceChoiceManager>
    {
        [SerializeField]
        private GameObject DiceChoiceParent;
        
        [SerializeField]
        private Transform Dice;

        private Player _currentPlayerChoosing;

        public void RotateCube(Vector3 direction)
        {
            this.Dice.Rotate(direction, 90, Space.World);
        }

        public void ValidateChoice()
        {
            Manager.GameManager.Instance.AddDiceFace((DiceEffectType.SHOCKWAVE, Random.Range(0, 6)));
            this.DiceChoiceParent.SetActive(false);
            Manager.GameManager.Instance.State = GameState.RUNNING;
            this._currentPlayerChoosing.DisableCubeChoiceInputs();
            this.EnableAllPlayerInputs();
        }

        public void ChangeSelectedEffect(int direction)
        {
            
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

        public void StartChoice(int playerIndex)
        {
            this.DisableAllPlayerInputs();
            this._currentPlayerChoosing = TeamManager.Instance.Players.First(p => p.PlayerIndex == playerIndex);
            this._currentPlayerChoosing.EnableCubeChoiceInputs();
            this.DiceChoiceParent.SetActive(true);
        }
    }
}
