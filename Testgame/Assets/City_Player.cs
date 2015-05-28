using UnityEngine;
using System.Collections;

public class City_Player : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        int horizontal = 0;  	//Used to store the horizontal move direction.
        int vertical = 0;		//Used to store the vertical move direction.
        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int)(Input.GetAxisRaw("Vertical"));
        if (horizontal != 0 || vertical != 0)
        {
            Move(horizontal, vertical);

        }
        
	}

    void Move(int up, int right)
    {

        transform.Translate(up * 3 * Time.deltaTime, right * 3 * Time.deltaTime, 0);
        float angle = Mathf.Atan2(right, up) * Mathf.Rad2Deg;
    }
}
