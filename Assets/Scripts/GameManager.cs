namespace PERRIER_CEOUGNA
{
    using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections.Generic;
	using SDD.Events;
	using System.Linq;

	public enum GameState { gameMenu, gamePlay, gameNextLevel, gamePause, gameOver, gameVictory }

	public class GameManager : Manager<GameManager>
	{
		#region Game State
		private GameState m_GameState;
		public bool IsPlaying { get { return m_GameState == GameState.gamePlay; } }
		#endregion

		//LIVES
		#region Lives
		[Header("GameManager")]
        [SerializeField] GameObject GameDynamics;
        [SerializeField] Texture2D cursorTexture;
        [SerializeField] int m_NStartLives;
        [SerializeField] int m_NStartLevel;
        [SerializeField] int m_NStartScore;
        [SerializeField] int m_NStartCurrentPieces;


        GameObject gameDynamicInGame;

        public int NLives { get; private set; }
        void DecrementNLives(int decrement)
        {
            SetNLives(NLives - decrement);
        }

        public void SetNLives(int nLives)
        {
            NLives = nLives;
            EventManager.Instance.Raise(new GameStatisticsChangedEvent() { eBestScore = BestScore, eScore = m_Score, eLevel = Level, eNLives = NLives, eCurrentPieces = m_CurrentPieces, eTotalPieces = m_TotalPieces });
        }
        public int Level { get; set; }

        void IncrementLevel(int increment)
        {
            SetLevel(Level + increment);
        }

        void SetLevel(int level, bool raiseEvent = true)
        {
            Level = level;

            if (raiseEvent)
                EventManager.Instance.Raise(new GameStatisticsChangedEvent() { eBestScore = BestScore, eScore = m_Score, eLevel = Level, eNLives = NLives, eCurrentPieces = m_CurrentPieces, eTotalPieces = m_TotalPieces });
        }
        #endregion

        #region Pieces
        private int m_CurrentPieces;
        public int CurrentPieces
        {
            get { return m_CurrentPieces; }
            set
            {
                m_CurrentPieces = value;
            }
        }

        void IncrementCurrentPieces(int increment)
        {
            SetCurrentPieces(m_CurrentPieces + increment);
        }

        public void SetCurrentPieces(int pieces, bool raiseEvent = true)
        {
            m_CurrentPieces = pieces;

            if (raiseEvent)
                EventManager.Instance.Raise(new GameStatisticsChangedEvent() { eBestScore = BestScore, eScore = m_Score, eLevel = Level, eNLives = NLives, eCurrentPieces = m_CurrentPieces, eTotalPieces = m_TotalPieces });
        }

        private int m_TotalPieces = 10;
        public int TotalPieces
        {
            get { return m_TotalPieces; }
            set
            {
                m_TotalPieces = value;
            }
        }
        void SetTotalPieces(int pieces, bool raiseEvent = true)
        {
            m_TotalPieces = pieces;

            if (raiseEvent)
                EventManager.Instance.Raise(new GameStatisticsChangedEvent() { eBestScore = BestScore, eScore = m_Score, eLevel = Level, eNLives = NLives, eCurrentPieces = m_CurrentPieces, eTotalPieces = m_TotalPieces });
        }
        #endregion

        #region Score
        private float m_Score;
		public float Score
		{
			get { return m_Score; }
			set
			{
				m_Score = value;
				BestScore = Mathf.Max(BestScore, value);
			}
		}

		public float BestScore
		{
			get { return PlayerPrefs.GetFloat("BEST_SCORE", 0); }
			set { PlayerPrefs.SetFloat("BEST_SCORE", value); }
		}

		public void IncrementScore(float increment)
		{
			SetScore(m_Score + increment*Level);
		}

		void SetScore(float score, bool raiseEvent = true)
		{
			Score = score;

			if (raiseEvent)
				EventManager.Instance.Raise(new GameStatisticsChangedEvent() { eBestScore = BestScore, eScore = m_Score, eLevel = Level, eNLives = NLives, eCurrentPieces = m_CurrentPieces, eTotalPieces = m_TotalPieces });
		}
		#endregion

		#region Time
		void SetTimeScale(float newTimeScale)
		{
			Time.timeScale = newTimeScale;
		}
		#endregion


		#region Events' subscription
		public override void SubscribeEvents()
		{
			base.SubscribeEvents();
			
			//MainMenuManager
			EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
			EventManager.Instance.AddListener<PlayButtonClickedEvent>(PlayButtonClicked);
			EventManager.Instance.AddListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
            EventManager.Instance.AddListener<RetryButtonClickedEvent>(RetryButtonClicked);
            EventManager.Instance.AddListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
			EventManager.Instance.AddListener<QuitButtonClickedEvent>(QuitButtonClicked);

			//Score Item
			EventManager.Instance.AddListener<ScoreItemEvent>(ScoreHasBeenGained);
		}

		public override void UnsubscribeEvents()
		{
			base.UnsubscribeEvents();

			//MainMenuManager
			EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
			EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClicked);
			EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
            EventManager.Instance.RemoveListener<RetryButtonClickedEvent>(RetryButtonClicked);
            EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
			EventManager.Instance.RemoveListener<QuitButtonClickedEvent>(QuitButtonClicked);

			//Score Item
			EventManager.Instance.RemoveListener<ScoreItemEvent>(ScoreHasBeenGained);
		}
		#endregion

		#region Manager implementation
		protected override IEnumerator InitCoroutine()
		{
            Menu();
            Cursor.SetCursor(cursorTexture,Vector2.zero,CursorMode.Auto);
			yield break;
		}
		#endregion

		#region Game flow & Gameplay
		//Game initialization
		void InitNewGame(bool raiseStatsEvent = true)
		{
            SetLevel(m_NStartLevel);
            SetScore(m_NStartScore);
            SetNLives(m_NStartLives);
            SetCurrentPieces(m_NStartCurrentPieces);
            SetTotalPieces(10);


        }

        public void LoseLife()
        {
            DecrementNLives(1);
            if (NLives <= 0)
            {
                this.Over();
            }
        }

        public void FoundCoin()
        {
            IncrementScore(10);
            SetCurrentPieces(CurrentPieces + 1);

        }

        public void LevelUp()
        {
            SfxManager.Instance.PlaySfx2D("levelup");
            IncrementScore(100);
            SetLevel(Level + 1);
            SetCurrentPieces(0);
            SetTotalPieces(10 + 2 * (Level-1));
            GameObject.Find("Coins").GetComponent<Coins>().Initialize();
            GameObject.Find("Minimap Camera").GetComponent<MinimapController>().generateCoinsView();
            GameObject.Find("Snake").GetComponent<Snake>().newLevel();
        }

		#endregion

		#region Callbacks to events issued by Score items
		private void ScoreHasBeenGained(ScoreItemEvent e)
		{
			if (IsPlaying)
				IncrementScore(e.eScore);
		}
		#endregion

		#region Callbacks to Events issued by MenuManager
		private void MainMenuButtonClicked(MainMenuButtonClickedEvent e)
		{
			Menu();
		}

		private void PlayButtonClicked(PlayButtonClickedEvent e)
		{
			Play();
		}

		private void ResumeButtonClicked(ResumeButtonClickedEvent e)
		{
			Resume();
		}

        private void RetryButtonClicked(RetryButtonClickedEvent e)
        {
            Retry();
        }

        private void EscapeButtonClicked(EscapeButtonClickedEvent e)
		{
			if (IsPlaying) Pause();
		}

		private void QuitButtonClicked(QuitButtonClickedEvent e)
		{
            #if (UNITY_EDITOR)              
                UnityEditor.EditorApplication.isPlaying = false;


            #else
                Application.Quit();
            #endif

		}
        #endregion


        #region GameState methods
        private void Menu()
		{
			SetTimeScale(1);
            m_GameState = GameState.gameMenu;
            Destroy(gameDynamicInGame);
			if(MusicLoopsManager.Instance)MusicLoopsManager.Instance.PlayMusic(Constants.MENU_MUSIC);
			  EventManager.Instance.Raise(new GameMenuEvent());
		}

		private void Play()
		{
           
            EventManager.Instance.Raise(new GamePlayEvent());
			InitNewGame();
			SetTimeScale(1);
            m_GameState = GameState.gamePlay;
			if (MusicLoopsManager.Instance) MusicLoopsManager.Instance.PlayMusic(Constants.GAMEPLAY_MUSIC);

            //mise en place des objets dynamiques
            gameDynamicInGame = Instantiate(GameDynamics);
            gameDynamicInGame.name = "GameDynamics";
            gameDynamicInGame.transform.position = new Vector3(0, 0, 0);

		}

		private void Pause()
		{
			if (!IsPlaying) return;

			SetTimeScale(0);
			m_GameState = GameState.gamePause;
			EventManager.Instance.Raise(new GamePauseEvent());
		}

		private void Resume()
		{
			if (IsPlaying) return;

			SetTimeScale(1);
			m_GameState = GameState.gamePlay;
			EventManager.Instance.Raise(new GameResumeEvent());
		}

        private void Retry()
        {
            SetTimeScale(1);
            EventManager.Instance.Raise(new GameRetryEvent());
            m_GameState = GameState.gamePlay;
            Destroy(gameDynamicInGame);
            InitNewGame();

            //mise en place des objets dynamiques
            gameDynamicInGame = Instantiate(GameDynamics);
            gameDynamicInGame.name = "GameDynamics";
            gameDynamicInGame.transform.position = new Vector3(0, 0, 0);

        }

        private void Over()
		{
			SetTimeScale(0);
            m_GameState = GameState.gameOver;
			EventManager.Instance.Raise(new GameOverEvent() { eBestScore = BestScore, eScore = m_Score, eLevel = Level });

           
            if (SfxManager.Instance) SfxManager.Instance.PlaySfx2D(Constants.GAMEOVER_SFX);
		}

        private void Quit()
        {
            
        }
        #endregion
    }
}

