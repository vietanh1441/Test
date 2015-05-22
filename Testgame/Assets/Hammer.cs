using UnityEngine;
using System.Collections;

namespace Completed
{
    public class Hammer : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            StartCoroutine("Dest");
        }

        IEnumerator Dest()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other);
            if (other != null)
            {
                Debug.Log(other);
                other.SendMessage("DamageWall", 1);
            }
        }
    }
}