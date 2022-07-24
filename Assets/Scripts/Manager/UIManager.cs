namespace Manager
{
    using DG.Tweening;
    using RSLib.Extensions;
    using TMPro;
    using UnityEngine;

    public class UIManager : RSLib.Singleton<UIManager>
    {
        // HOW TO PLAY
        [SerializeField]
        private GameObject _howToPlayObject;
        
        // GAME TITLE
        public GameObject _gameTitleObject;

        // END GAME
        [SerializeField]
        private GameObject _endGameObject;

        [SerializeField]
        private TextMeshProUGUI _endGameText;

        private void Start()
        {
            this.SetActiveGameTitle(true);
            this.SetActiveHowToPlay(false);
            this.SetActiveEndGame(false);
        }

        #region Set Active

        public void SetActiveHowToPlay(bool active)
        {
            this._howToPlayObject.SetActive(active);
        }
        
        public void SetActiveGameTitle(bool active)
        {
            this._gameTitleObject.SetActive(active);
        }

        public void SetActiveEndGame(bool active)
        {
            this._endGameObject.transform.DOPunchScale(Vector3.one * 1.3f, 0.2f);
            this._endGameObject.SetActive(active);
        }

        #endregion

        public void DisplayEndGame(Team team)
        {
            this._endGameText.text = $"{team.ToString().ToLower().UpperFirst()} team won!";
            this._endGameText.color = team.GetTeamColor();
            
            this.SetActiveEndGame(true);
        }
    }
}