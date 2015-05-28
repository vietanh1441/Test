using UnityEngine;
using System.Collections;


/// <summary>
/// This is a data structure for each tile of a grid game.
/// 
/// How this works:
/// 
/// a tile will have 8 childrens that send message to it what is the according object 
/// this tile will store it in a dictionary
/// </summary>
public class Grid : MonoBehaviour {

    public Transform[] prefab;
    public int points;
    public bool worth;
    public bool go;
    public int time;
    public bool exit;
    public Sprite[] dmgSprite;
    public Sprite bmbSprite;
    public GameObject[] tile = new GameObject[9];
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        //First, init the worth by using random
        //if worth = 
        // 0 = exit
        go = false;
        points = 0;
        time = 0;
        exit = false;
        worth =false;  //will need to add a function to determine probabilities
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Invoke("Calculate", 2); //Wait for 2 seconds to make sure that everything is init
    }

    void Calculate()
    {
        int i;
        for(i = 0; i < 9; i++)
        {
            if(tile[i] != null)
            {
                if (Get_Points(tile[i]))
                    points++;
            }
        }
        go = true;
    }

    public void DamageFloor()
    {
        if (go)
        {
            if (exit)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
                Instantiate(prefab[0], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }
            else if (worth)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
                Debug.Log("BOMB!");
                GameObject.FindGameObjectWithTag("Player").SendMessage("GetBombed");
                spriteRenderer.sprite = bmbSprite;
            }
            else
            {
                time++;
                gameObject.layer = LayerMask.NameToLayer("Default");
                int i;
                Debug.Log(points);
                if (time == 1)
                {
                    if (points == 0)
                    {
                        StartCoroutine("SendStuff");
                    }
                    spriteRenderer.sprite = dmgSprite[points];
                }
            }
        }
    }

    IEnumerator SendStuff()
    {
        yield return new WaitForSeconds(0);
        int i;
        for (i = 0; i < 9; i++)
        {
            if (tile[i] != null)
            {
                tile[i].SendMessage("DamageFloor");
            }
        }
       
    }

    bool Get_Points(GameObject other)
    {
        Grid grid = other.GetComponent<Grid>();
        return grid.worth;
    }

	
	// Update is called once per frame
	void Update () {
	}
}
