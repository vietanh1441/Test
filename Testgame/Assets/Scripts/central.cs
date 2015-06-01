using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.


/// <summary>
/// This is the central script for the game. This will replace Gamemanger object.
/// It will store player current health,
/// Hammer, Hoe as well as deal with player inventory.
/// 
/// For Health, everytime user use hammer and hoe, he will lose 1 hp, 1/3 everytime he dig 
/// a bomb and 5hp everytime a monster attack him.
/// When his hp become 0, he lost all item in his bag and automatically go to the next day.
/// When his hp become 7, he play a warning animation.
/// 
/// 
/// </summary>
public class central : MonoBehaviour {

    public int hp, hammer, hoe;     //The maximum stats currently. This will get added everytime player upgrade the character
    public int m_hp, m_hammer, m_hoe; //The stats of character inside the cave
    public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
    public int size;                    //maximum currently of bag
    public List<GameObject> backpack = new List<GameObject>();      //Backpack inside the mine
    public List<GameObject> inventory = new List<GameObject>();     //Inventory


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
