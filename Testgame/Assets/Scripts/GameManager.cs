using UnityEngine;
using System.Collections;

 
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{

        public class Inventory
        {
            public int[] item_id = new int[30];

            public Inventory()
            {
                for (int i = 0; i < item_id.Length; i++)
                {
                    item_id[i] = 0;
                }
            }

            public void Add(int id)
            {
                item_id[id]++;
            }

            public void Remove(int id)
            {
                item_id[id]--;
            }
        }

        public int money;

        private Vector3 MINE_FRONT = new Vector3 (-2,-2,-1);
        private Vector3 BED = new Vector3(-10, -10,-1);
        public Vector3 pawnpoint = new Vector3(-2,-2,-1);   

        public GameObject player;
        public Inventory inventory = new Inventory();
        bool die = false;
		public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
		public float turnDelay = 0.1f;							//Delay between each Player turn.
		public int playerStaminaPoints = 20;					//Starting value for Player food points.
        public int PlayerHealth = 1;
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
        public int size;
        public List<int> backpack = new List<int>();
		public int stamina;
		public int hammer = 10;
		public int hoe = 10;
        public int days = 0;
        public int max_hp, max_hammer, max_hoe;                     //pre-determine, will be reset each days. Also subject to be saved
        //public GameObject[] inventory = new GameObject[]; //The main inventory, the backpack will be added to inventory everytime the character exit.

		private Text levelText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
		private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
		private int level = 0;									//Current level number, expressed in game as "Day 1".
		private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
		private bool enemiesMoving;								//Boolean to check if enemies are moving.
		private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.

        
		
		//Awake is always called before any Start functions
		void Awake()
		{
            if(Application.loadedLevel == 2)
            {
                Application.LoadLevel(0);
            }
            money = 0;
           // player = Instantiate(player,pawnpoint, Quaternion.identity)as GameObject;
            Debug.Log("PAWNPOINT" + pawnpoint.x + pawnpoint.y);
            Debug.Log("RESTART");
            MINE_FRONT = new Vector2(-1, -1);
            BED = new Vector2(-10, -10);
            size = 10;
            max_hp = 20;
            max_hammer = 10;
            max_hoe = 40;
            days = 0;
            Reset_Tools();
			//Check if instance already exists
			if (instance == null)
				
				//if not, set instance to this
				instance = this;
			
                //Load save file

			//If instance already exists and it's not this:
			else if (instance != this)
				
				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy(gameObject);	
			
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			
			//Assign enemies to a new List of Enemy objects.
			enemies = new List<Enemy>();
			
			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();
			
			//Call the InitGame function to initialize the first level 
			//InitGame();
		}
		
		//This is called each time a scene is loaded.
		void OnLevelWasLoaded(int index)
		{
            
            Debug.Log("PAWNPOINT" + pawnpoint.x + pawnpoint.y);
            Debug.Log("RESTARTwithloaded");
           // player = GameObject.FindGameObjectWithTag("Player");
            //Reset_Tools();
            if (Application.loadedLevel == 1)
            {

                Debug.Log("UP");
                //Add one to our level number.
                level++;
                //Call InitGame to initialize our level.
                InitGame();
            }
            if(Application.loadedLevel == 0)
            {
                Instantiate(player, pawnpoint, Quaternion.identity);
                //First check the coordinate of the pawn point
                //Then pawn the player at according coordinate
                //Instantiate(player, MINE_FRONT, Quaternion.identity);
                //Then based on whether player die or what, reset accordingly
                if(die)
                {
                    Reset_when_die();
                }
                //Reset hp and add item from backpack to inventory as well as reset backpack
                else
                {
                    Reset_when_exit();
                }

            }
		}
		

        void Reset_when_die()
        {
            //When die
            //Reset hp, set backpack to 0
            ResetHp();
            backpack.Clear();
        }

        void Reset_when_exit()
        {
            //When exit
            //Add item from backpack to inventory
            for(int i = 0; i < backpack.Count; i++)
            {
                inventory.Add(backpack[i]);
                Debug.Log(inventory.item_id[0]);
                Debug.Log(inventory.item_id[1]);

            }
            //set backpack to 0
            backpack.Clear();
        }

        void Reset_Tools()
        {
            hammer = max_hammer;
            hoe = max_hoe;
        }

		//Initializes the game for each level.
		void InitGame()
		{
            die = false;
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;
			
			//Get a reference to our image LevelImage by finding it by name.
			levelImage = GameObject.Find("LevelImage");
			
			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			levelText = GameObject.Find("LevelText").GetComponent<Text>();
			
			//Set the text of levelText to the string "Day" and append the current level number.
			levelText.text = "Day " + level;
			
			//Set levelImage to active blocking player's view of the game board during setup.
			levelImage.SetActive(true);
			
			//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
			Invoke("HideLevelImage", levelStartDelay);
			
			//Clear any Enemy objects in our List to prepare for next level.
			enemies.Clear();
			
			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);
			
		}
		
		
		//Hides black image used between levels
		void HideLevelImage()
		{
			//Disable the levelImage gameObject.
			levelImage.SetActive(false);
			
			//Set doingSetup to false allowing player to move again.
			doingSetup = false;
		}
		
		//Update is called every frame.
		void Update()
		{
			
		}
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}
		
		
		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
            //Set pawnpoint to the bed
            pawnpoint = BED;
            Debug.Log(pawnpoint.x + pawnpoint.y);
			//Set levelText to display number of levels passed and game over message
			levelText.text = "Random Shit";
			
			//Enable black background image gameObject.
			levelImage.SetActive(true);
			//Reset everything
            //Invoke("ResetHp",2);
            level = 0;
            die = true;
			//Load level
            Application.LoadLevel(0);
		}
		
        //When player exit the min instead of dying
        public void Exit_Mine()
        {
            //Set pawnpoint to in front of the mine
            pawnpoint = MINE_FRONT;

            level = 0;
            Application.LoadLevel(0);
            //call reset_when_exit
        }


        public void ResetHp()
        {
            playerStaminaPoints = max_hp;
        }

        void AddMoney(int num)
        {
            money = money + num;
        }

        void LoseMoney(int num)
        {
            money = money - num;
        }
	}

