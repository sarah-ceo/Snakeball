namespace PERRIER_CEOUGNA
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using SDD.Events;

	public class HudManager : Manager<HudManager>
	{

        [Header("HudManager")]
        #region Labels & Values
        [SerializeField] GameObject PanelHUD;
        [SerializeField] GameObject LifeImage;
        private Text[] labels;
        #endregion

        #region Manager implementation
        protected override IEnumerator InitCoroutine()
		{
            yield break;
		}
        #endregion

        protected override void Awake()
        {
            base.Awake();

        }

        #region Callbacks to GameManager events
        protected override void GameStatisticsChanged(GameStatisticsChangedEvent e)
		{
            labels  = PanelHUD.GetComponentsInChildren<Text>();
            
            foreach(Text label in labels)
            {
                if(label.name == "CurrentPieces")
                    label.text = e.eCurrentPieces + " / " + e.eTotalPieces;
                if (label.name == "CurrentScore")
                    label.text = "" + e.eScore;
                if (label.name == "CurrentLevel")
                    label.text = "" + e.eLevel;
                if (label.name == "BestScore")
                    label.text = "" + e.eBestScore;
            }

            //mettre l'affichage des vies à jour
            GameObject containerImg = GameObject.FindGameObjectWithTag("containerImg"); 

            Image[] imgTab = null;
            imgTab = containerImg.GetComponentsInChildren<Image>();
            foreach (Image img in imgTab)
            {
                Destroy(img.gameObject);
            }

            for(int i = 0; i<e.eNLives; i++)
            {
                GameObject lifeImg = Instantiate(LifeImage);
                lifeImg.name = "lifeImg" + i;
                lifeImg.transform.position = new Vector3(-75-i*20, -75, 0);
                lifeImg.transform.SetParent(containerImg.transform, false);
                
            }
        }

        protected override void GameMenu(GameMenuEvent e)
        {
            base.GameMenu(e);
            PanelHUD.SetActive(false);
        }

        protected override void GamePlay(GamePlayEvent e)
        {
            base.GamePlay(e);
            PanelHUD.SetActive(true);
        }

        protected override void GameOver(GameOverEvent e)
        {
            base.GameOver(e);
            PanelHUD.SetActive(false);
        }
        protected override void GameRetry(GameRetryEvent e)
        {
            base.GameRetry(e);
            PanelHUD.SetActive(true);

        }
        #endregion

    }
}