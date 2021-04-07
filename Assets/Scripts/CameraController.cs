namespace PERRIER_CEOUGNA
{
    using UnityEngine;


    public class CameraController : SimpleGameStateObserver
    {
        [SerializeField] GameObject target;
        [SerializeField] float xOffset;
        [SerializeField] float zOffset;
        [SerializeField] float yOffset;
        [SerializeField] float manualRotationCamera;
        [SerializeField] float sensivityMouseV;
        [SerializeField] float sensivityMouseH;
        [SerializeField] float HeightMaxManualCamera;
        private float distanceFloorToAdd;
        float TimeBeforeActualize;
        private Ray ray;
        public static bool IsInObstacleZone;

        private float accelerationY; //valeur représentant la vitesse d'actualisation de la position de la caméra sur l'axe y
        //une valeur basse donnera moins de précision à la caméra, sur l'axe y. Avec une valeur haute, la camera suivra bien le relief du sol
        private float precisionMax;
        private float precisionAdd;

        [SerializeField] float switchViewDistance; //distance séparatrice pour passer à une autre vue
        [SerializeField] float retryTimeSwitchView; //temps avant un check de la distance
        [SerializeField] float cameraHeight; 
        [SerializeField] float minimumHigh; //valeur permettant la comparaison entre cette valeur et la valeur de la collision du rayon 'ray' en dessous de la camera.

        void ResetCamera()
        {
            transform.position = target.transform.position + new Vector3(xOffset, yOffset, zOffset);
            accelerationY = 1;
            TimeBeforeActualize = 0;
            distanceFloorToAdd = 0;
            precisionMax = 30;
            precisionAdd = 0.1f;
            IsInObstacleZone = false;
        }

        protected override void Awake()
        {
            base.Awake();
            transform.position = target.transform.position + new Vector3(xOffset, yOffset, zOffset);
            transform.LookAt(target.transform.position);

            accelerationY = 1;
            TimeBeforeActualize = 0;
            distanceFloorToAdd = 0;
            precisionMax = 30;
            precisionAdd = 0.1f;

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ResetCamera();

        }

        private void Start()
        {
            ray = new Ray(transform.position, -transform.up);
        }

        private void Update()
        {
            if (!GameManager.Instance.IsPlaying) return;

        }

        void FixedUpdate()
        {
            ray = new Ray(transform.position, -transform.up);

            //distance camera - balle
            float distance = Vector3.Distance(Vector3.ProjectOnPlane(target.transform.position,Vector3.up), Vector3.ProjectOnPlane(transform.position,Vector3.up));
            float pointY = getPointYUnderCamera();
            distanceFloorToAdd = cameraHeight - (transform.position.y - pointY);

            //actualisation du switch mode camera éloigné - proche toutes les 'timeActualize' secondes (2 sec)
            if (Time.fixedTime > TimeBeforeActualize)
            {
                if (distance > switchViewDistance)//mode éloigné
                {
                    transform.LookAt(new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z));

                    //camera eloigné standard/mountain
                    if (pointY > minimumHigh && !IsInObstacleZone)
                    {
                        transform.Translate(0.0f, (cameraHeight + distanceFloorToAdd) *accelerationY*Time.fixedDeltaTime, distance * Time.fixedDeltaTime);
                        if(accelerationY < precisionMax) 
                            accelerationY += precisionAdd; //quand on quitte une zone d'obstacle, on redonne de la précision progressivement 
                            //en fonction du temps sur la translation en y de la caméra.
                    }

                    //camera eloigné zone obstacle
                    else
                    {
                        transform.Translate(0.0f, (cameraHeight + target.transform.position.y - transform.position.y)* Time.fixedDeltaTime, distance * Time.fixedDeltaTime);
                        accelerationY = 1;

                    }



                }
                else
                {
                    TimeBeforeActualize = Time.fixedTime + retryTimeSwitchView; //temps avant retentative de passage en mode éloigné
                }


            }
            else //mode zone proche
            {
                transform.LookAt(new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z));
                
                //Déplacement manuel de la caméra
                //avec les touches A et E
                if (Input.GetKey(KeyCode.A))
                    transform.RotateAround(target.transform.position, Vector3.up, manualRotationCamera * Time.fixedDeltaTime);
                else if(Input.GetKey(KeyCode.E))
                    transform.RotateAround(target.transform.position, Vector3.up, -manualRotationCamera * Time.fixedDeltaTime);
                else if(Input.GetKey(KeyCode.Mouse0))
                {
                    //avec la souris
                    
                    float hInput = Input.GetAxis("MouseHorizontal");
                    float vInput = Input.GetAxis("MouseVertical");
                    transform.RotateAround(target.transform.position, Vector3.up, sensivityMouseH * hInput * Time.fixedDeltaTime);
                    
                    if(transform.position.y > target.transform.position.y && transform.position.y < target.transform.position.y + HeightMaxManualCamera)
                        transform.Translate(Vector3.up * vInput * sensivityMouseV * Time.fixedDeltaTime,Space.World);
                    if (transform.position.y < target.transform.position.y && vInput >=0) //si depasse limite, on peut revenir dans la zone à ne pas dépasser
                        transform.Translate(Vector3.up * vInput * sensivityMouseV * Time.fixedDeltaTime, Space.World);
                    if (transform.position.y > target.transform.position.y && vInput <= 0)
                        transform.Translate(Vector3.up * vInput * sensivityMouseV * Time.fixedDeltaTime, Space.World);
                }

            }




        }

        private float getPointYUnderCamera()
        {
            RaycastHit hit;

            Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8);

            return hit.point.y;

        }
        protected override void GameMenu(GameMenuEvent e)
        {
        }
    }
}
