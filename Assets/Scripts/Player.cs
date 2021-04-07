namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]

    public class Player : MonoBehaviour
    {
        [SerializeField] float m_AccelerationBall; //intensité de la force appliqué lors d'un mouvement
        [SerializeField] public float m_BallSpeed; //vitesse maximum de la balle
        [SerializeField] float m_AdjustGravity;
        [SerializeField] LayerMask m_GroundedLayerMask;
        [SerializeField] public float m_JumpVerticalVelocity;
        [SerializeField] GameObject m_mainCamera;

        //variables concernant le déplacement de la balle
        bool m_isGrounded;

        private Rigidbody m_Rigidbody;

        //variables concernant les bonus appliqués sur la balle
        public List<string> auraList;
        public bool rightsToMoveNotGrounded;
        private int cptJump;
        private float timeBeforeJumpAgain;
        static private float timeBeforeJumpAgainAdd;
        public int nbJumps;

        //timers d'aura
        public float timerBonusSpeed;
        public float timerBonusJump;
        public float timerBonusBomb;


        private void Awake()
        {
            auraList = new List<string>();
            m_Rigidbody = GetComponent<Rigidbody>();

            cptJump = 0;
            timeBeforeJumpAgain = 0.0f;
            timeBeforeJumpAgainAdd = 0.5f;
            nbJumps = 1;

            timerBonusJump = 0;
            timerBonusSpeed = 0;
            timerBonusBomb = 0;


        }

        private void Update()
        {
            //Arreter les auras 
            if (auraList.Contains("SpeedAura") && Time.time > timerBonusSpeed)
            {
                m_BallSpeed = 15;
                Bonus.StopAura("SpeedAura");
                auraList.Remove("SpeedAura");

            }
            if (auraList.Contains("SuperJumpAura") && Time.time > timerBonusJump)
            {
                nbJumps = 1;
                m_JumpVerticalVelocity = 700;
                rightsToMoveNotGrounded = false;
                Bonus.StopAura("SuperJumpAura");
                auraList.Remove("SuperJumpAura");
            }

            if (auraList.Contains("BombSkin") && (Time.time > timerBonusJump || Input.GetKeyDown(KeyCode.B)))
            {
                Bonus.StopAura("BombSkin");
                auraList.Remove("BombSkin");
            }

        }

        private void FixedUpdate()
        {

            //remise du compteur de jump à 0
            if (m_isGrounded)
                cptJump = 0;

            //lire les valeurs des touches haut bas gauche et droite.
            float hInput = Input.GetAxis("Horizontal");
            float vInput = Input.GetAxis("Vertical");

            //mouvement de la balle en fonction de la direction (projeté sur l'axe y) vers laquelle regarde la caméra.
            if (rightsToMoveNotGrounded)
            {
                if (m_Rigidbody.velocity.magnitude <= m_BallSpeed)
                {
                    Vector3 dir = Vector3.ProjectOnPlane((transform.position - m_mainCamera.transform.position), Vector3.up).normalized;
                    Vector3 dirRotate = new Vector3(dir.z, 0, -dir.x);
                    m_Rigidbody.AddForce(dirRotate * hInput * m_AccelerationBall);
                    m_Rigidbody.AddForce(dir * vInput * m_AccelerationBall);
                }
            }
            else
            {
                if (m_Rigidbody.velocity.magnitude <= m_BallSpeed && m_isGrounded)
                {
                    Vector3 dir = Vector3.ProjectOnPlane((transform.position - m_mainCamera.transform.position), Vector3.up).normalized;
                    Vector3 dirRotate = new Vector3(dir.z, 0, -dir.x);
                    m_Rigidbody.AddForce(dirRotate * hInput * m_AccelerationBall);
                    m_Rigidbody.AddForce(dir * vInput * m_AccelerationBall);
                }
            }

            //gravité appliqué à la balle en plus de celle de unity.
            m_Rigidbody.AddForce(new Vector3(0, m_AdjustGravity, 0));

            //Saut
            if (Input.GetButton("Jump") && cptJump < nbJumps && Time.fixedTime > timeBeforeJumpAgain)
            {
                m_Rigidbody.AddForce(Vector3.up * m_JumpVerticalVelocity);
                cptJump++;
                m_isGrounded = false;
                SfxManager.Instance.PlaySfx2D("jump");
                timeBeforeJumpAgain = Time.fixedTime + timeBeforeJumpAgainAdd;
            }
        }



        private void OnCollisionEnter(Collision collision)
        {
            //si y'a contact avec le sol
            if ((m_GroundedLayerMask.value & (1 << collision.gameObject.layer)) != 0)
            {
                if ((collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") || Vector3.Distance(collision.contacts[0].point, transform.position) < .05f))
                    m_isGrounded = true;

            }
        }


        private void OnCollisionExit(Collision collision)
        {
            //si on perd un contact avec le sol (et qu'il y'avait un seul contact)
            if (((m_GroundedLayerMask.value & (1 << collision.gameObject.layer)) != 0) && collision.contacts.Length == 1)
                m_isGrounded = false;
        }
    }
}