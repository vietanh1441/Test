using UnityEngine;
using System.Collections;

public class t_pos : MonoBehaviour {
    public int pos;
	// Use this for initialization
	void Start () {
	    
	}
	
	void OnTriggerEnter2D(Collider2D other)
    {
       // Debug.Log(other.gameObject);
        Grid grid = transform.parent.GetComponent<Grid>();
        grid.tile[pos] = other.gameObject;
    }
}
