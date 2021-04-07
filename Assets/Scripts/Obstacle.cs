namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Obstacle : MonoBehaviour {

        private float JumpVerticalVelocity;

       

        private void OnTriggerEnter(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
                CameraController.IsInObstacleZone = true;
            
        }
        private void OnTriggerExit(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
                CameraController.IsInObstacleZone = false;

        }
    }
}