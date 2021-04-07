namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SDD.Events;

    public class BonusSpawn : Manager<BonusSpawn>
    {
        [SerializeField] GameObject minimapPoint;
        [SerializeField] GameObject bonus1;
        [SerializeField] GameObject bonus2;
        [SerializeField] GameObject bonusRare1;
        [SerializeField] GameObject bonusRare2;
        [SerializeField] GameObject bonusContainerConstant;
        [SerializeField] GameObject bonusContainerOneTime;
        private int level;
        private int pointMapHeight; //position y des points map des bonus

        [SerializeField] int nbBonusMax;

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


        protected override void Awake()
        {
            base.Awake();
            level = 1;
            pointMapHeight = 330;
        }

        protected override void GameStatisticsChanged(GameStatisticsChangedEvent e)
        {
            if (level != e.eLevel)
            {
                level = e.eLevel;
                Reset();
                Initialize();
            }
           
        }
        protected override void GameRetry(GameRetryEvent e)
        {
            level = 1;
            Reset(true);
            Initialize(true);

        }
        protected override void GamePlay(GamePlayEvent e)
        {
            level = 1;
            Initialize(true);
        }

        protected override void GameMenu(GameMenuEvent e)
        {
            Reset(true);
        }

        public void Initialize(bool withOneTimeSpawns = false)
        {
            //génération des bonus random
            for (int i = 0; i < nbBonusMax; i++)
            {
                int choixTypeBonus = Random.Range(0, 12);
                GameObject oneStandardBonus = null;
                if (choixTypeBonus == 0)
                    oneStandardBonus = Instantiate(bonusRare1);
                else if (choixTypeBonus == 1)
                    oneStandardBonus = Instantiate(bonusRare2);
                else
                {
                    int choixBonus = Random.Range(0, 2);
                    if (choixBonus == 0)
                        oneStandardBonus = Instantiate(bonus1);
                    else if (choixBonus == 1)
                        oneStandardBonus = Instantiate(bonus2);
                }



                oneStandardBonus.transform.SetParent(GameObject.Find("randomSpawn").transform, false);
                oneStandardBonus.name = "BonusStd" + (i + 1);
                float acceptableHeight = 320;
                float x = 0;
                float y = 400;
                float z = 0;
                while (y > acceptableHeight)
                {
                    x = Random.Range(100, 300);
                    z = Random.Range(100, 300);
                    y = GameObject.Find("Map").GetComponent<Terrain>().SampleHeight(new Vector3(x, 0, z));
                }
                oneStandardBonus.transform.position = new Vector3(x, y + 1, z);
            }

            //génération des bonus rares 
            GameObject bonusRaresContainer = Instantiate(bonusContainerConstant);
            bonusRaresContainer.transform.SetParent(GameObject.Find("BonusContainer").transform, false);
            bonusRaresContainer.name = "constantSpawn";

            if(withOneTimeSpawns)
            {
                //génération des bonus rares 
                GameObject bonusOneTimeContainer = Instantiate(bonusContainerOneTime);
                bonusOneTimeContainer.transform.SetParent(GameObject.Find("BonusContainer").transform, false);
                bonusOneTimeContainer.name = "oneTimeSpawn";
            }

            //mise à jour de la map

            generateBonusView();
        }

        public void Reset(bool withOneTimeSpawns = false)
        {
            Bonus[] bonusObjects = GameObject.Find("randomSpawn").GetComponentsInChildren<Bonus>();
            foreach (Bonus bonusObject in bonusObjects)
            {
                Destroy(bonusObject.gameObject);
            }

            //suppression bonus rares
            Destroy(GameObject.Find("constantSpawn"));

            if(withOneTimeSpawns)
                Destroy(GameObject.Find("oneTimeSpawn"));

        }

        public void generateBonusView()
        {

            //placement bonus sur la carte (on n'affiche pas les bonus rares)
            Bonus[] bonusTab = FindObjectsOfType<Bonus>();
            foreach (Bonus bonus in bonusTab)
            {
                //si le bonus n'est pas à un point de spawn constant(et donc si ce n'est pas un bonus rare)
                if (bonus.transform.parent.name == "randomSpawn") 
                {
                    GameObject point = Instantiate(minimapPoint);
                    point.name = bonus.name + "PointMap";
                    point.transform.position = new Vector3(bonus.transform.position.x, pointMapHeight, bonus.transform.position.z);
                    point.transform.SetParent(bonus.transform, true);
                    point.GetComponent<Renderer>().material.color = Color.yellow;

                }
            }
        }
    }
}
