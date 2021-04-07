namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Propulser : MonoBehaviour
    {

        [SerializeField] float Force;

        


        private void OnTriggerEnter(Collider other)
        {
            Rigidbody RBplayer = other.GetComponent<Rigidbody>();
            RBplayer.AddForce(transform.up * Force);
            SfxManager.Instance.PlaySfx2D("propulser");
            GameManager.Instance.IncrementScore(40);
        }
    }
}