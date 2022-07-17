namespace Manager
{
    using DG.Tweening;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Random = UnityEngine.Random;

    public class GameManager : RSLib.Singleton<GameManager>
    {
        // GAME MANAGEMENT
        public GameState State { get; set; } = GameState.LOBBY;

        [SerializeField]
        private float GameDurationInSeconds = 180;

        // TIMER
        [SerializeField]
        private RSLib.Data.Float _gameTimer;

        public bool IsTimerOver => this._gameTimer.Value <= 0f; 

        [SerializeField]
        private GameObject CountdownObject;

        [SerializeField]
        private TextMeshProUGUI CountdownText;

        // DICES SPAWN
        [SerializeField]
        private Dice _dicePrefab = null;

        [SerializeField]
        private Transform _diceSpawnPoint = null;

        [SerializeField]
        private float _diceSpawningForce = 10f;

        [SerializeField]
        private CameraShake _cameraShake = null;

        [SerializeField]
        private Material _circleMaterial;

        private List<Dice> _dices = new();

        private DiceSettings _diceSettings = new DiceSettings();
        
        // SCORE
        [SerializeField]
        private RSLib.Data.Int _blueTeamScore;
        
        [SerializeField]
        private RSLib.Data.Int _pinkTeamScore;

        [SerializeField]
        private float _restartPauseTime = 1.5f;
        
        public Team WinningTeam => (this._blueTeamScore.Value - this._pinkTeamScore.Value) switch
        {
            > 0 => Team.BLUE,
            < 0 => Team.PINK,
            _ => Team.NONE
        };

        // EFFECTS
        [SerializeField]
        private DiceEffectsIncompatibilitiesTable _effectsIncompatibilities = null;

        public DiceEffectsIncompatibilitiesTable EffectsIncompatibilitiesTable => this._effectsIncompatibilities;

        public CameraShake CameraShake => this._cameraShake;
        
        // REFS
        
        [SerializeField]
        private PlayerInputManager _playerInputManager;
        
        protected override void Awake()
        {
            base.Awake();

            this._blueTeamScore.Value = 0;
            this._pinkTeamScore.Value = 0;
            this._gameTimer.Value = this.GameDurationInSeconds * 1000;
            this._circleMaterial.color = new Color32(126, 15, 0, 255);
        }

        private void Update()
        {
            if (this._gameTimer > 0f && this.State == GameState.RUNNING)
            {
                this._gameTimer.Value -= Time.deltaTime * 1000;

                if (this._gameTimer.Value <= 0f
                    && this._blueTeamScore.Value != this._pinkTeamScore.Value)
                {
                    this.EndGame();
                }
            }
            
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                RSLib.Helpers.QuitPlatformDependent();
            }
        }

        #region Game Management

        public bool TryStartGame()
        {
            if (this.State != GameState.LOBBY
                || !TeamManager.Instance.IsThereEnoughPlayers
                || !TeamManager.Instance.AreAllPlayersReady
                || !TeamManager.Instance.AreTeamsValid)
            {
                return false;
            }
            
            this.StartCoroutine(this.StartGame());
            return true;
        }

        private IEnumerator StartGame()
        {
            this._playerInputManager.DisableJoining();
            Manager.UIManager.Instance.SetActiveHowToPlay(false);
            Manager.UIManager.Instance.SetActiveEndGame(false);
            TeamManager.Instance.SetActiveTeamChoosers(false);
            TeamManager.Instance.UnreadyAllPlayers();
            this.State = GameState.STARTING;
            
            MusicManager.Instance.PlayGameMusic();
            
            yield return this.CountDownCoroutine(3);

            this.State = GameState.RUNNING;
            this.GenerateDice(this._diceSpawnPoint.position, true);
        }

        private IEnumerator CountDownCoroutine(int seconds)
        {
            this.CountdownObject.SetActive(true);
            
            while (seconds > 0)
            {
                this.CountdownText.text = seconds.ToString();
                this.CountdownText.transform.localScale = Vector3.one;
                this.CountdownText.transform.DOScale(Vector3.zero, 1).SetEase(Ease.InQuint);
                yield return new WaitForSeconds(1);
                seconds -= 1;
            }

            this.CountdownObject.SetActive(false);
        }

        private void EndGame()
        {
            Manager.UIManager.Instance.DisplayEndGame(this.WinningTeam);
            this.DestroyAllDices();
            this.State = GameState.LOBBY;
            this._playerInputManager.EnableJoining();
            TeamManager.Instance.SetActiveTeamChoosers(true);
            this._gameTimer.Value = this.GameDurationInSeconds * 1000;

            this._blueTeamScore.Value = 0;
            this._pinkTeamScore.Value = 0;
            
            MusicManager.Instance.PlayLobbyMusic();
        }

        #endregion

        #region Goal Management

        public void ScoreGoal(Team triggeredGoalTeam)
        {
            switch (triggeredGoalTeam)
            {
                case Team.BLUE:
                    this._pinkTeamScore += 1;
                    break;
                case Team.PINK:
                    this._blueTeamScore += 1;
                    break;
            }

            ChangeColorCircles();

            if (this.IsTimerOver)
            {
                this.EndGame();
            }
            else
            {
                this.StartCoroutine(this.ScoreGoalCoroutine(triggeredGoalTeam));
            }
        }

        private void ChangeColorCircles()
        {
            Color nextColor = this.WinningTeam != Team.NONE ? this.WinningTeam.GetTeamColor() : new Color32(126, 15, 0, 255);
            _circleMaterial.DOColor(nextColor, 0.5f);
        }


        private IEnumerator ScoreGoalCoroutine(Team triggeredGoalTeam)
        {
            this.DestroyAllDices();

            // this.State = GameState.PAUSED;
            // int playerIndex = Manager.DiceFaceChoiceManager.Instance.GetNewPlayerIndexForTeam(triggeredGoalTeam);
            // Manager.DiceFaceChoiceManager.Instance.StartChoice(playerIndex);
            // yield return new WaitUntil(() => this.State == GameState.RUNNING);
            
            yield return new WaitForSeconds(this._restartPauseTime);
            this.GenerateDice(this._diceSpawnPoint.position, true);
        }

        #endregion

        #region Dice Management

        private Dice GenerateDice(Vector3 position, bool applySpawningForce)
        {
            Dice dice = Instantiate(this._dicePrefab, position, Quaternion.identity, this.transform);

            if (applySpawningForce)
            {
                dice.transform.rotation = Random.rotation;
                dice.Rigidbody.AddForce(Vector3.forward * this._diceSpawningForce, ForceMode.Impulse);
                dice.Rigidbody.AddTorque(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360), ForceMode.Impulse);
            }
            
            dice.ApplySettings(this._diceSettings);
            return dice;
        }

        public void AddDiceFace((DiceEffectType diceEffectType, int index) newDiceFace)
        {
            this._diceSettings = new DiceSettings(this._diceSettings.DiceEffectFaces, newDiceFace);
        }

        public void DestroyAllDices()
        {
            for (int i = this._dices.Count - 1; i >= 0; i--)
            {
                Destroy(this._dices[i].gameObject);
            }
        }

        public void RegisterDice(Dice dice)
        {
            this._dices.Add(dice);
        }

        public void UnregisterDice(Dice dice)
        {
            this._dices.Remove(dice);
        }

        #endregion
    }
}