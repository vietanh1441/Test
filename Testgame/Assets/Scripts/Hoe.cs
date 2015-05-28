using UnityEngine;
using System.Collections;

    public class Hoe : MonoBehaviour
    {
        public GameObject gameManager;
        public bool init;
        void Start()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager");
            init = gameManager.GetComponent<BoardManager>().init;
            StartCoroutine("Dest");
        }

        IEnumerator Dest()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
        // Update is called once per frame
        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other);
            if (other != null)
            {
                Grid grid = other.GetComponent<Grid>();
                if(!init)
                {
                    grid.worth = false;
                    gameManager.SendMessage("Init");
                    gameManager.GetComponent<BoardManager>().init = true;
                }
                Debug.Log(other);
                other.SendMessage("DamageFloor");
            }
        }
    }