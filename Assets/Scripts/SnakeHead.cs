namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SnakeHead : MonoBehaviour
    {
        float force;
        private void Awake()
        {
            force = 1500;
        }
        private void FixedUpdate()
        {
            if (!Snake.freeze)
            {
                GameObject.Find("SnakeBody0").transform.LookAt(GameObject.Find("Ball").transform.position);
                GameObject.Find("SnakeBody0").transform.Translate(0.0f, 0.0f, Snake.snakeSpeed * Time.fixedDeltaTime);
            }
            
        }
    

    private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                GameManager.Instance.LoseLife();
                SfxManager.Instance.PlaySfx2D("Boing");
                collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up*force);
            }

        }
    }
}
