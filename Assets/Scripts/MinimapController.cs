namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MinimapController : MonoBehaviour
    {

        [SerializeField] GameObject target;
        [SerializeField] float xOffset;
        [SerializeField] float yOffset;
        [SerializeField] float zOffset;
        //[SerializeField] float m_ModelHeightGenerated; //hauteur des boules représentant un point sur la map
        [SerializeField] GameObject minimapPoint;
        private List<GameObject> minimapPoints = new List<GameObject>();
       

        // Use this for initialization
        void Start()
        {
            transform.position = target.transform.position + new Vector3(xOffset, yOffset, zOffset);
            transform.LookAt(target.transform.position);
            generateCoinsView();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = target.transform.position + new Vector3(xOffset, yOffset, zOffset);
            transform.LookAt(target.transform.position);
            transform.rotation = Quaternion.Euler(90f, GameObject.Find("Ball Camera").transform.eulerAngles.y, 0f);
            radarPositions();
        }

        public void generateCoinsView()
        {
            //placement des pièces sur la carte
            Coin[] coinsTab = FindObjectsOfType<Coin>();
            foreach (Coin coin in coinsTab)
            {
                GameObject point = Instantiate(minimapPoint);
                point.name = coin.name + "PointMap";
                point.transform.position = new Vector3(coin.transform.position.x, target.transform.position.y, coin.transform.position.z);
                point.transform.SetParent(coin.transform, false);
                point.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                point.GetComponent<Renderer>().material.color = Color.cyan;
                minimapPoints.Add(point);
            }
        }

        private void radarPositions()
        {

            minimapPoints.RemoveAll(x => x == null);
            foreach (GameObject point in minimapPoints)
            {
                Vector3 parentPosition = new Vector3(point.transform.parent.transform.position.x, transform.position.y - 10, point.transform.parent.transform.position.z);
                if (Vector3.Distance(target.transform.position, parentPosition) > 49)
                {
                    Vector3 direction = (parentPosition - target.transform.position).normalized;
                    point.transform.position = target.transform.position + direction * 49f;
                } else
                {
                    point.transform.position = new Vector3(parentPosition.x, target.transform.position.y, parentPosition.z);
                }
            }
        }
    }
}