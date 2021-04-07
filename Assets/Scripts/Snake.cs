namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Snake : SimpleGameStateObserver
    {

        public GameObject snakeBody = null;
        public static int snakeSize;
        private float TimeBeforeActualize;
        [SerializeField] float snakeUpdateTime;
        static public bool freeze; //serpent immobilisé
        static public float timeBeforeUnfreeze;
        static public bool checkRenderer;
        public static float snakeSpeed;

        protected override void GameRetry(GameRetryEvent e)
        {
            base.GameRetry(e);

        }
        protected override void Awake()
        {
            base.Awake();
            TimeBeforeActualize = snakeUpdateTime;
            timeBeforeUnfreeze = 0;
            checkRenderer = true;
            snakeSpeed = 5f;
            snakeSize = 0;
        }

        public void Start()
        {
            snakeSpeed = 5f;
            GameObject body = Instantiate(snakeBody);

            snakeSize += 1;
            body.name = "SnakeBody" + snakeSize;
            body.transform.SetParent(transform, false);
            TimeBeforeActualize = Time.fixedTime + snakeUpdateTime;
        }

        public void FixedUpdate()
        {
          

            if (Time.fixedTime > TimeBeforeActualize && !freeze)
            {
                GameObject body = Instantiate(snakeBody);
                snakeSize += 1;
                body.name = "SnakeBody" + snakeSize;
                body.transform.SetParent(transform, false);
                TimeBeforeActualize = Time.fixedTime + snakeUpdateTime;
                snakeSpeed += GameManager.Instance.Level*0.5f;
            }

            if (Time.time > timeBeforeUnfreeze && checkRenderer)
            {
                freeze = false;
                SnakeBody.freeze = false;
                checkRenderer = false;

                //rétablir la couleur du snake
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                if (renderers != null)
                {
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        if (renderers[i].name != "SnakeEyeL" && renderers[i].name != "SnakeEyeR" && renderers[i].name != "SnakeBody0PointMap")
                            renderers[i].material.color = Color.Lerp(Color.yellow, Color.gray, 0.7f);
                    }
                }

            }
        }

        public void newLevel()
        {
            snakeSpeed = 5f;
        }

        public void ReduceSize(int nb)
        {
            for (int i = 0; i < nb; i++)
            {
                if (snakeSize >= 1)
                {
                    string bodyName = "SnakeBody" + snakeSize;
                    Destroy(GameObject.Find(bodyName));
                    snakeSize -= 1;
                }
            }
        }

        public void ReduceSpeed(float nb)
        {
            if (snakeSpeed > 5)
            {
                snakeSpeed -= 5f * nb;
                if (snakeSpeed < 5)
                    snakeSpeed = 5f;
            }
        }

        public void IncreaseSpeed(float nb)
        {
            snakeSpeed += 5f * nb;
        }
    }
    
}