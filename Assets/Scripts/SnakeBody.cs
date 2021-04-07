namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SnakeBody : MonoBehaviour
    {

        static public bool freeze;
        string bodyToFollow;

        // Use this for initialization
        private void Awake()
        {
            bodyToFollow = "SnakeBody" + (Snake.snakeSize);
            
        }
        void Start()
        {
            GameObject m_Target = GameObject.Find(bodyToFollow);
            if (!freeze)
                transform.position = new Vector3(m_Target.transform.position.x + 2.5f, m_Target.transform.position.y, m_Target.transform.position.z);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            GameObject m_Target = GameObject.Find(bodyToFollow);
            float distance = Vector3.Distance(m_Target.transform.position, transform.position);


            if (distance > 1.5f && !freeze)
            {

                transform.LookAt(m_Target.transform.position);
                transform.Translate(0.0f, 0.0f, Snake.snakeSpeed * Time.fixedDeltaTime);
            }
        }

      
    }
}