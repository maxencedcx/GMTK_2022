namespace Manager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class GameManager : RSLib.Singleton<GameManager>
    {
        // GAME MANAGEMENT
        public GameState State { get; set; } = GameState.LOBBY;

        // DICES SPAWN
        [SerializeField]
        private Dice _dicePrefab = null;

        [SerializeField]
        private Transform _diceSpawnPoint = null;

        [SerializeField]
        private float _diceSpawningForce = 10f;

        [SerializeField]
        private CameraShake _cameraShake = null;
        
        private List<Dice> _dices = new();

        private DiceSettings _diceSettings = new DiceSettings();
        
        // SCORE
        [SerializeField]
        private RSLib.Data.Int _blueTeamScore;
        
        [SerializeField]
        private RSLib.Data.Int _pinkTeamScore;

        [SerializeField]
        private float _restartPauseTime = 1.5f;

        // EFFECTS
        [SerializeField]
        private DiceEffectsIncompatibilitiesTable _effectsIncompatibilities = null;

        public DiceEffectsIncompatibilitiesTable EffectsIncompatibilitiesTable => this._effectsIncompatibilities;

        public CameraShake CameraShake => this._cameraShake;
        
        protected override void Awake()
        {
            base.Awake();

            this._blueTeamScore.Value = 0;
            this._pinkTeamScore.Value = 0;
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
            TeamManager.Instance.DisableTeamChoosers();
            this.State = GameState.STARTING;
            
            yield return new WaitForSeconds(3);

            this.State = GameState.RUNNING;
            this.GenerateDice(this._diceSpawnPoint.position, true);
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
            
            this.StartCoroutine(this.ScoreGoalCoroutine(triggeredGoalTeam));
        }

        private IEnumerator ScoreGoalCoroutine(Team triggeredGoalTeam)
        {
            this.State = GameState.PAUSED;
            this.DestroyAllDices();

            int playerIndex = Manager.DiceFaceChoiceManager.Instance.GetNewPlayerIndexForTeam(triggeredGoalTeam);
            Manager.DiceFaceChoiceManager.Instance.StartChoice(playerIndex);
            yield return new WaitUntil(() => this.State == GameState.RUNNING);
            
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