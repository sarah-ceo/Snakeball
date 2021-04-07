namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Doors : MonoBehaviour
    {

        Vector3 destination;
        static float timeBeforeActualize;
        static float timeRangeBroken;  //temps avant lequel on peut réemprunter un portail.
        bool valid;
        private void Awake()
        {
            timeBeforeActualize = 0;
            timeRangeBroken = 30;
            valid = true;
        }

        private void Update()
        {
            if (timeBeforeActualize < Time.time && valid)
            {
                GameObject.Find("portail1").GetComponent<Renderer>().enabled = true;
                GameObject.Find("portail2").GetComponent<Renderer>().enabled = true;
                GameObject.Find("portail3").GetComponent<Renderer>().enabled = true;
                GameObject.Find("portail4").GetComponent<Renderer>().enabled = true;

                valid = false;
            }
        }

        void OnCollisionEnter(Collision col)
        {


            if (Time.time > timeBeforeActualize)
            {
                SfxManager.Instance.PlaySfx2D("swooshsound");
                GameManager.Instance.IncrementScore(40);
                if (this.name == "Door1" || this.name == "portail1")
                {
                    destination = GameObject.Find("Door3").transform.position;
                    timeBeforeActualize = Time.time + timeRangeBroken;

                }
                else if (this.name == "Door2" || this.name == "portail2")
                {
                    destination = GameObject.Find("Door4").transform.position;
                    timeBeforeActualize = Time.time + timeRangeBroken;
                }
                else if (this.name == "Door3" || this.name == "portail3")
                {
                    destination = GameObject.Find("Door1").transform.position;
                    timeBeforeActualize = Time.time + timeRangeBroken;
                }
                else if (this.name == "Door4" || this.name == "portail4")
                {
                    destination = GameObject.Find("Door2").transform.position;
                    timeBeforeActualize = Time.time + timeRangeBroken;
                }

                col.transform.position = new Vector3(destination.x, destination.y, destination.z) + Vector3.forward * 2;

                GameObject.Find("portail1").GetComponent<Renderer>().enabled = false;
                GameObject.Find("portail2").GetComponent<Renderer>().enabled = false;
                GameObject.Find("portail3").GetComponent<Renderer>().enabled = false;
                GameObject.Find("portail4").GetComponent<Renderer>().enabled = false;
                valid = true;

            }
        }
    }
}