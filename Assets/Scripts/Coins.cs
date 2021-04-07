namespace PERRIER_CEOUGNA
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SDD.Events;

    public class Coins : MonoBehaviour
    {
        public GameObject coin = null;

        private void Awake()
        {
            GameManager.Instance.TotalPieces = 10;
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance.CurrentPieces == GameManager.Instance.TotalPieces)
            {
                GameManager.Instance.LevelUp();
            } 
            
        }

        public void Initialize()
        {
            Reset();
            for (int i = 0; i < GameManager.Instance.TotalPieces; i++)
            {
                GameObject oneCoin = Instantiate(coin);
                oneCoin.transform.SetParent(transform, false);
                oneCoin.name = "Coin" + (i + 1);
                float acceptableHeight = 320;
                float x = 0;
                float y = 400;
                float z = 0;
                while (y > acceptableHeight)
                {
                    x = Random.Range(100, 300);
                    z = Random.Range(100, 300);
                    y = GameObject.Find("Map").GetComponent<Terrain>().SampleHeight(new Vector3(x, 0, z));
                }
                oneCoin.transform.position = new Vector3(x, y + 1, z);
            }
        }

        public void Reset()
        {

            Coin[] coinObjects = GameObject.Find("Coins").GetComponentsInChildren<Coin>();
            foreach (Coin coinObject in coinObjects)
            {
                Destroy(coinObject.gameObject);
            }
        }
    }
}
