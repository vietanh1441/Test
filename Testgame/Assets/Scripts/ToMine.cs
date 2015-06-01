using UnityEngine;
using System.Collections;

public class ToMine : MonoBehaviour {
    GameObject screen;
	// Use this for initialization
	void Start () {
        screen = GameObject.FindGameObjectWithTag("Screen");
	}
	
	void OnTriggerEnter2D(Collider2D other)
    {
        screen.SendMessage("Start_Fade");
        if(other.tag == "Player")
        {
            Debug.Log("YNOT");
            StartCoroutine("Load_lv");
        }
    }

    IEnumerator Load_lv()
    {
        yield return new WaitForSeconds(1);
        Application.LoadLevel("Main");
    }
}
