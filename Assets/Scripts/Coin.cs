namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SDD.Events;

    public class Coin : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Player"))
            {
                Destroy(gameObject);
                GameManager.Instance.FoundCoin();

                //get piece sound
                SfxManager.Instance.PlaySfx2D("getPiece");
            }
        }
    }
}