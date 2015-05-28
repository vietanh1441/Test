using UnityEngine;
using System.Collections;

public class ToMine : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Player")
        {
            Application.LoadLevel("Main");
        }
    }
}
