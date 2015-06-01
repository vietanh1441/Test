using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class City_Player : MonoBehaviour {

    public GameObject inventory;
    public Text inventory_text;

	// Use this for initialization
	void Start () {
        inventory = GameObject.Find("Inventory1");
        inventory_text = GameObject.Find("Text4").GetComponent<Text>();
        inventory.SetActive(false);
        inventory_text.text = " ";
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
        if(Input.GetKeyDown(KeyCode.I))
        {
            Player_Inventory();
        }
	}

    void Player_Inventory()
    {
        if(inventory.activeSelf == true)
        {
            inventory.SetActive(false);
            inventory_text.text = " ";
        }
        else
        {
            inventory.SetActive(true);
            int[] invent = GameManager.instance.inventory.item_id;
            inventory_text.text = invent[0]+"\t\t\t\t" + invent[5]+"\t\t\t\t" +invent[10]+"\t\t\t\t" +invent[15]+"\t\t\t\t" +invent[20]+"\t\t\t\t" +invent[25]+ "\n\n\n\n" + 
                                  invent[1]+"\t\t\t\t" + invent[6]+"\t\t\t\t" +invent[11]+"\t\t\t\t" +invent[16]+"\t\t\t\t" +invent[21]+"\t\t\t\t" +invent[26]+ "\n\n\n\n" +
                                  invent[2]+"\t\t\t\t" + invent[7]+"\t\t\t\t" +invent[12]+"\t\t\t\t" +invent[17]+"\t\t\t\t" +invent[22]+"\t\t\t\t" +invent[27]+ "\n\n\n\n" +
                                  invent[3]+"\t\t\t\t" + invent[8]+"\t\t\t\t" +invent[13]+"\t\t\t\t" +invent[18]+"\t\t\t\t" +invent[23]+"\t\t\t\t" +invent[28]+ "\n\n\n\n" +
                                  invent[4]+"\t\t\t\t" + invent[9]+"\t\t\t\t" +invent[14]+"\t\t\t\t" +invent[19]+"\t\t\t\t" +invent[24]+"\t\t\t\t" +invent[29]+ "\n\n\n\n" ;
        }
    }



    void Move(int up, int right)
    {

        transform.Translate(up * 3 * Time.deltaTime, right * 3 * Time.deltaTime, 0);
        float angle = Mathf.Atan2(right, up) * Mathf.Rad2Deg;
    }
}
