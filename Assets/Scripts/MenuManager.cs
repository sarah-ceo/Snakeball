
namespace PERRIER_CEOUGNA
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.UI;
    using SDD.Events;

	public class MenuManager : Manager<MenuManager>
	{
        [SerializeField] GameObject CameraMenu; //camera du menu
        private Animator CameraObject;
        private Text[] labels;
        [Header("MenuManager")]

		#region Panels
		[Header("--Panels--")]

        //panels game menu
		[SerializeField] GameObject m_PanelInGameMenu;
		[SerializeField] GameObject m_PanelGameOver;

        //panels main menu
        [Header("Principal Panels optionsAndTutorial")]
        [Tooltip("The UI Panel that holds the setting")]
        [SerializeField] GameObject PanelSetting;
        [Tooltip("The UI Panel that show the tutorial")]
        [SerializeField] GameObject PanelTutorial;
        [Tooltip("The UI Panel that show the main screen")]
        [SerializeField] GameObject PanelMainScreen;
        

        [Header("under Panels Setting")]
        [Tooltip("The UI Panel that holds the CONTROLS window tab")]
        [SerializeField] GameObject PanelControls;
        [Tooltip("The UI Panel that holds the VIDEO window tab")]
        [SerializeField] GameObject PanelVideo;
        [Tooltip("The UI Panel that holds the GAME window tab")]
        [SerializeField] GameObject PanelGame;

        
        [Header("under Panels Tutorial")]
        [Tooltip("The UI Panel that holds the COMMENT JOUER window tab")]
        [SerializeField] GameObject PanelHowPlay;
        [Tooltip("The UI Panel that holds the COMMANDES window tab")]
        [SerializeField] GameObject PanelCommandes;
        [Tooltip("The UI Panel that holds the LES BONUS window tab")]
        [SerializeField] GameObject PanelBonus;
        
        

        List<GameObject> m_AllPanelsSetting; //under panels tutorials
        List<GameObject> m_AllPanelsMainMenu;
        List<GameObject> m_AllPanelsTutorial;
        List<GameObject> m_AllPanelsGameMenu;
        #endregion

        #region Events' subscription
        public override void SubscribeEvents()
		{
			base.SubscribeEvents();
		}

		public override void UnsubscribeEvents()
		{
			base.UnsubscribeEvents();
		}
		#endregion

		#region Manager implementation
		protected override IEnumerator InitCoroutine()
		{
			yield break;
		}
		#endregion

		#region Monobehaviour lifecycle
		protected override void Awake()
		{
			base.Awake();
			RegisterPanels();
		}
        protected override IEnumerator Start()
        {
            CameraObject = CameraMenu.transform.GetComponent<Animator>();
            yield break;
        }

        private void Update()
		{
			if (Input.GetButtonDown("Cancel"))
			{
				EscapeButtonHasBeenClicked();
			}
		}
		#endregion

		#region Panel Methods
		void RegisterPanels()
		{

            m_AllPanelsSetting = new List<GameObject>();
            m_AllPanelsSetting.Add(PanelControls);
            m_AllPanelsSetting.Add(PanelVideo);
            m_AllPanelsSetting.Add(PanelGame);

            m_AllPanelsMainMenu = new List<GameObject>();
            m_AllPanelsMainMenu.Add(PanelSetting);
            m_AllPanelsMainMenu.Add(PanelTutorial);
            m_AllPanelsMainMenu.Add(PanelMainScreen);

            m_AllPanelsTutorial = new List<GameObject>();
            m_AllPanelsTutorial.Add(PanelHowPlay);
            m_AllPanelsTutorial.Add(PanelCommandes);
            m_AllPanelsTutorial.Add(PanelBonus);
          
            m_AllPanelsGameMenu = new List<GameObject>();
            m_AllPanelsGameMenu.Add(m_PanelInGameMenu);
            m_AllPanelsGameMenu.Add(m_PanelGameOver);
        }

		void OpenPanel(GameObject panel, List<GameObject> panelsList)
		{
			foreach (var item in panelsList)
				if (item) item.SetActive(item == panel);
		}
		#endregion

		#region UI OnClick Events Global Buttons
		public void EscapeButtonHasBeenClicked()
		{
			EventManager.Instance.Raise(new EscapeButtonClickedEvent());
		}

		public void PlayButtonHasBeenClicked()
		{
			EventManager.Instance.Raise(new PlayButtonClickedEvent());
            CameraMenu.SetActive(false);
            
		}

		public void ResumeButtonHasBeenClicked()
		{
			EventManager.Instance.Raise(new ResumeButtonClickedEvent());
		}

        public void RetryButtonHasBeenClicked()
        {
            EventManager.Instance.Raise(new RetryButtonClickedEvent());
        }

        public void MainMenuButtonHasBeenClicked()
		{
			EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
            CameraMenu.SetActive(true);

        }

		public void QuitButtonHasBeenClicked()
		{
			EventManager.Instance.Raise(new QuitButtonClickedEvent());
		}

        public void TutorialButtonHasBeenClicked()
        {
            CameraObject.SetFloat("Animate", 1);
            OpenPanel(PanelTutorial, m_AllPanelsMainMenu);
        }

        public void SettingButtonHasBeenClicked()
        {
            CameraObject.SetFloat("Animate", 1);
            OpenPanel(PanelSetting, m_AllPanelsMainMenu);
        }

        public void ReturnButtonHasBeenClicked()
        {
            CameraObject.SetFloat("Animate", 0);
            OpenPanel(PanelMainScreen, m_AllPanelsMainMenu);
            
        }


        #endregion

        #region OnClick Events Setting and Tutorial Buttons
        public void GameButtonHasBeenClicked()
        {
            OpenPanel(PanelGame, m_AllPanelsSetting);
        }
        public void VideoButtonHasBeenClicked()
        {
            OpenPanel(PanelVideo, m_AllPanelsSetting);
        }
        public void ControlButtonHasBeenClicked()
        {
            OpenPanel(PanelControls, m_AllPanelsSetting);
        }
        public void HowPlayButtonHasBeenClicked()
        {
            OpenPanel(PanelHowPlay, m_AllPanelsTutorial);
        }
        public void CommandButtonHasBeenClicked()
        {
            OpenPanel(PanelCommandes, m_AllPanelsTutorial);
        }
        public void BonusMenuButtonHasBeenClicked()
        {
            OpenPanel(PanelBonus, m_AllPanelsTutorial);
        }
        #endregion
        #region Callbacks to GameManager events
        protected override void GameMenu(GameMenuEvent e)
		{
            OpenPanel(null, m_AllPanelsGameMenu);
            OpenPanel(PanelMainScreen, m_AllPanelsMainMenu);
        }

        protected override void GamePlay(GamePlayEvent e)
		{
            OpenPanel(null, m_AllPanelsMainMenu);
        }

		protected override void GamePause(GamePauseEvent e)
		{
			OpenPanel(m_PanelInGameMenu,m_AllPanelsGameMenu);
		}

		protected override void GameResume(GameResumeEvent e)
		{
			OpenPanel(null,m_AllPanelsGameMenu);
		}

		protected override void GameOver(GameOverEvent e)
		{
            labels = m_PanelGameOver.GetComponentsInChildren<Text>();

            foreach (Text label in labels)
            {
                if (label.name == "CurrentScore")
                    label.text = "" + e.eScore;
                if (label.name == "CurrentLevel")
                    label.text = "" + e.eLevel;
                if (label.name == "BestScore")
                    label.text = "" + e.eBestScore;
            }

            OpenPanel(m_PanelGameOver,m_AllPanelsGameMenu);
		}

        protected override void GameRetry(GameRetryEvent e)
        {
            OpenPanel(null, m_AllPanelsGameMenu);
        }
        #endregion
    }

}
