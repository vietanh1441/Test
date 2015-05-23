using UnityEngine;
using System.Collections;

namespace Completed
{
public class messages : MonoBehaviour {

	public float time = 2f;

	// Use this for initialization
	void Start () {
		StartCoroutine ("Dest");
	}

	IEnumerator Dest()
	{
		yield return new WaitForSeconds (time);
			Destroy (gameObject);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
}