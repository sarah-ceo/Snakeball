namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Bonus : MonoBehaviour
    {

        [SerializeField] string m_nameAuraOrSkin;
        private List<string> auraList;
        private Player player;

        static private int speedToAdd;
        static private int jumpToAdd;
        static public float timeBonusJumpToAdd; //temps d'effet de l'aura à rajouter
        static public float timeBonusSpeedToAdd; //temps d'effet de l'aura à rajouter
        static public float timeBonusBombToAdd; //temps d'effet de l'aura à rajouter

        static private float distanceBombEffect; //distance maximale de l'effet de l'explosion de la bombe.
        static public float timeFreezingSnake;

        private void Awake()
        {
            speedToAdd = 15;
            jumpToAdd = 200;
            timeBonusJumpToAdd = 30.0f;
            timeBonusSpeedToAdd = 30.0f;
            timeBonusBombToAdd = 20.0f;
            distanceBombEffect = 8f;
            timeFreezingSnake = 20f;
        }

        private void OnTriggerEnter(Collider other)
        {
            //récupération des caractéristiques du joueur et du snake
            Player player = other.GetComponent<Player>();
            GameObject snakeObj = GameObject.FindWithTag("Snake");

            //récupération des auras du player
            auraList = player.auraList;

            if (player != null && checkCumulableBonus())
            {
                SfxManager.Instance.PlaySfx2D("bonusGet");
                //cas spéciaux : si le bonus, c'est le coeur
                if (m_nameAuraOrSkin == "Heart")
                {
                    GameManager.Instance.IncrementScore(500);

                    //ajout d'une vie au compteur
                    GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                    gameManager.SetNLives(gameManager.NLives + 1);

                    //Activation du whiteParticle
                    GameObject.Find("WhiteParticles").GetComponent<ParticleSystem>().Play();

                    //suppression de l'objet bonus
                    Destroy(gameObject);
                    return;
                }

                //cas spéciaux : si le bonus, c'est le speedreducer
                if (m_nameAuraOrSkin == "SpeedReducer")
                {
                    GameManager.Instance.IncrementScore(50);
                    Snake snake = snakeObj.GetComponent<Snake>();
                    snake.ReduceSpeed(1);

                    //Activation du whiteParticle
                    GameObject.Find("WhiteParticles").GetComponent<ParticleSystem>().Play();

                    //suppression de l'objet bonus
                    Destroy(gameObject);
                    return;
                }

                //cas spéciaux : si le bonus, c'est le sizereducer
                if (m_nameAuraOrSkin == "SizeReducer")
                {
                    GameManager.Instance.IncrementScore(50);
                    Snake snake = snakeObj.GetComponent<Snake>();
                    snake.ReduceSize(1);

                    //Activation du whiteParticle
                    GameObject.Find("WhiteParticles").GetComponent<ParticleSystem>().Play();

                    //suppression de l'objet bonus
                    Destroy(gameObject);
                    return;
                }

                //cas spéciaux : si le bonus, c'est un coffre à pièces
                if (m_nameAuraOrSkin == "GoldChest")
                {
                    GameManager.Instance.IncrementScore(150);
                    GameManager gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
                    int rangeMaxToAdd = gamemanager.TotalPieces - gamemanager.CurrentPieces;
                    int rangeMinToAdd = gamemanager.TotalPieces / 2;

                    if (gamemanager.CurrentPieces > gamemanager.TotalPieces / 2)
                        rangeMinToAdd = gamemanager.CurrentPieces / 2;

                    gamemanager.SetCurrentPieces(gamemanager.CurrentPieces + Random.Range(rangeMinToAdd, rangeMaxToAdd));

                    //Activation du whiteParticle
                    GameObject.Find("WhiteParticles").GetComponent<ParticleSystem>().Play();

                    //suppression de l'objet bonus
                    Destroy(gameObject);
                    return;
                }

                //ajout du nom de l'aura dans la liste
                auraList.Add(m_nameAuraOrSkin);


                GameObject aura = GameObject.Find(m_nameAuraOrSkin);

                //activation des renderer
                Renderer[] renderers = aura.GetComponentsInChildren<Renderer>();

                if (renderers != null)
                {
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.enabled = true;
                    }
                }

                //Activation du whiteParticle
                GameObject bonusGettingAura = GameObject.Find("WhiteParticles");
                bonusGettingAura.GetComponent<ParticleSystem>().Play();

                //activation des particules systèmes
                ParticleSystem particles = aura.GetComponent<ParticleSystem>();
                if (particles != null)
                    aura.GetComponent<ParticleSystem>().Play();



                //affectation des bonus
                if (m_nameAuraOrSkin == "SpeedAura")
                {
                    GameManager.Instance.IncrementScore(150);
                    player.m_BallSpeed += speedToAdd;
                    player.timerBonusSpeed = Time.time + timeBonusSpeedToAdd;
                }
                else if (m_nameAuraOrSkin == "SuperJumpAura")
                {
                    GameManager.Instance.IncrementScore(150);
                    player.m_JumpVerticalVelocity += jumpToAdd;
                    player.rightsToMoveNotGrounded = true;
                    player.nbJumps = 2;
                    player.timerBonusJump = Time.time + timeBonusJumpToAdd;
                }
                else if (m_nameAuraOrSkin == "BombSkin")
                {
                    GameManager.Instance.IncrementScore(150);
                    player.timerBonusJump = Time.time + timeBonusBombToAdd;
                }


                //suppression de l'objet bonus
                Destroy(gameObject);
            }

        }

        private bool checkCumulableBonus()
        {
            //si bonus déjà possédé ou actif
            if (auraList.Contains(m_nameAuraOrSkin))
                return false;

            //si bonus non cumulable déjà actif/possédé et que le bonus qui est trigger est non cumulable
            if ((auraList.Contains("BombSkin") || auraList.Contains("SpikerSkin") || auraList.Contains("InvincibleSkin")) && (m_nameAuraOrSkin == "BombSkin" || m_nameAuraOrSkin == "SpikerSkin" || m_nameAuraOrSkin == "InvincibleSkin"))
                return false;

            return true;
        }

        static public void StopAura(string auraName)
        {
            //récupération des caractéristiques du joueur
            Player player = GameObject.Find("Ball").GetComponent<Player>();

            if (player != null)
            {
                GameObject aura = GameObject.Find(auraName);

                //cas Spécial : Explosion de la bombe
                if (auraName == "BombSkin")
                {
                    GameObject.Find("BigExplosionEffect").GetComponent<ParticleSystem>().Play();
                    SfxManager.Instance.PlaySfx2D("explosion");
                    Snake snake = GameObject.FindWithTag("Snake").GetComponent<Snake>();
                    GameObject snakeHead = GameObject.Find("SnakeBody0");

                    float distance = Vector3.Distance(player.transform.position, snakeHead.transform.position);
                    if (distance < distanceBombEffect)
                    {
                        GameManager.Instance.IncrementScore(300);
                        snake.ReduceSize(5);
                        Snake.freeze = true;
                        SnakeBody.freeze = true;
                        Snake.checkRenderer = true;

                        //changer couleur du snake
                        Renderer[] renderersSnake = snake.GetComponentsInChildren<Renderer>();
                        if (renderersSnake != null)
                        {
                           
                            foreach (Renderer renderer in renderersSnake)
                            {
                                //On n'applique pas le changement de couleur au point associé à la minimap et aux yeux du snake
                                if(renderer.name != "SnakeEyeL" && renderer.name != "SnakeEyeR" && renderer.name != "SnakeBody0PointMap")
                                    renderer.material.color = Color.red;
                            }
                        }

                        Snake.timeBeforeUnfreeze = Time.time + timeFreezingSnake;

                    }

                }

                //désactivation des renderer
                Renderer[] renderers = aura.GetComponentsInChildren<Renderer>();

                if (renderers != null)
                {
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.enabled = false;
                    }
                }

                //désactivation des particules systèmes
                ParticleSystem particles = aura.GetComponent<ParticleSystem>();
                if (particles != null)
                    aura.GetComponent<ParticleSystem>().Stop();

            }
            else
            {
                Debug.LogError("Problem in StopAura, player object is null");
            }
        }
    }
}