using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using System.Collections.Generic;


	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MonoBehaviour
	{

        public LayerMask blockingLayer;
		public Transform msg_fbp;
        public Transform hammer;
        public Transform hoe;
        public GameObject hitbox;
        private Transform item;
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int pointsPerStamina = 10;				//Number of points to add to player Stamina points when picking up a Stamina object.
		public int pointsPerSoda = 20;				//Number of points to add to player Stamina points when picking up a soda object.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		public Text StaminaText;						//UI Text to display current player Stamina total.
		public Text BackpackText;
		public Text HammerText;
		public Text HoeText;
		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a Stamina object.
		public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a Stamina object.
		public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
        int up = 0;  	//Used to store the horizontal move direction.
        int down = 0;		//Used to store the vertical move direction.
        //bool movable = true;
        bool go = true;
		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int food;							//Used to store player Stamina points total during level.
        private int Stamina;
        private int Health;
        private bool hold;
		private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
        private BoxCollider2D boxCollider; 		//The BoxCollider2D component attached to this object.
        private Rigidbody2D rb2D;				//The Rigidbody2D component attached to this object.
        private float inverseMoveTime;			//Used to make movement more efficient.
        private int size;
        private List<GameObject> backpack;
		private int stamina_pts;
		private int hammer_pts;
		private int hoe_pts;
		private bool exit = false;
		
		//Start overrides the Start function of MovingObject
		 void Start ()
		{
            hold = false;
			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();
            hitbox = GameObject.Find("Hitbox");
			//Get the current Stamina point total stored in GameManager.instance between levels.
			Stamina = GameManager.instance.playerStaminaPoints;
            Health = GameManager.instance.PlayerHealth;
            size = GameManager.instance.size;
            backpack = GameManager.instance.backpack;
			stamina_pts = GameManager.instance.stamina;
			hammer_pts = GameManager.instance.hammer;
			hoe_pts = GameManager.instance.hoe;

			//Set the StaminaText to reflect the current player Stamina total.
			StaminaText.text = ": " + Stamina;
			BackpackText.text = ": " + backpack.Count + "/" + size; 
			HammerText.text = ": " + hammer_pts;
			HoeText.text = ": " + hoe_pts;

			//Call the Start function of the MovingObject base class.
            //Get a component reference to this object's BoxCollider2D
            boxCollider = GetComponent<BoxCollider2D>();

            //Get a component reference to this object's Rigidbody2D
            rb2D = GetComponent<Rigidbody2D>();

            //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
           // inverseMoveTime = 1f / moveTime;
		}
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local Stamina total in the GameManager so it can be re-loaded in next level.
			GameManager.instance.playerStaminaPoints = Stamina;
			GameManager.instance.hammer = hammer_pts;
			GameManager.instance.hoe = hoe_pts;
			GameManager.instance.backpack = backpack;

		}
		
		
		private void Update ()
		{
            transform.Translate(0,-0.024f*Time.deltaTime,0);
            animator.SetBool("Hammer", false);
			//If it's not the player's turn, exit the function.
			if(!GameManager.instance.playersTurn) return;

            int horizontal = 0;  	//Used to store the horizontal move direction.
            int vertical = 0;		//Used to store the vertical move direction.
			
			//Check if we are running either in the Unity editor or in a standalone build.
			#if UNITY_STANDALONE || UNITY_WEBPLAYER
            if (hold)
            {
                if (Input.GetButtonDown("yes"))
                {

                    Put_in();
                    
                    hold = false;
                }
                else if (Input.GetButtonDown("no"))
                {
                    Debug.Log("No");
                    //Function to throw away
                    Destroy(item.gameObject);
                    hold = false;
                }
                
            }
			else if (exit)
			{
				if (Input.GetButtonDown("yes"))
				{
					
					Do_exit();
					
					exit = false;
				}
				else if (Input.GetButtonDown("no"))
				{
					Do_not_exit();

					exit = false;
				}
			}
            else if (Input.GetButtonDown("Hammer") && !hold)
            {
				Hammer();
            }
            else if (Input.GetButtonDown("Hoe")&& hold == false)
            {
				Hoe();
            }
            else
            {
                //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
                horizontal = (int)(Input.GetAxisRaw("Horizontal"));

                //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
                vertical = (int)(Input.GetAxisRaw("Vertical"));

                //Store up or down here
                if (horizontal != 0)
                {
                    up = horizontal;
                    down = 0;
                }
                else if (vertical != 0)
                {
                    up = 0;
                    down = vertical;
                }
                //Debug.Log(go);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
                //Check if we have a non-zero value for horizontal or vertical
                if (horizontal != 0 || vertical != 0)
                {
                    Move(horizontal, vertical);
                    
                }
            }
		}
		



		void Hammer()
		{
			if(hammer_pts > 0)
			{
				LoseStamina(1);
				//Debug.Log(hold);
				Debug.Log("Hammer");
				Instantiate(hammer, new Vector3(hitbox.transform.position.x + up, hitbox.transform.position.y + down, 0), Quaternion.identity);
				hammer_pts--;
				HammerText.text = ": " + hammer_pts;
				animator.SetBool("Hammer", true);
			}
			else 
			{
				DisplayText(msg_fbp);
			}
		}

		void Hoe()
		{
			if(hoe_pts>0)
			{
                
				LoseStamina(1);
				Debug.Log("Hoe");
				Instantiate(hoe, new Vector3(hitbox.transform.position.x + up, hitbox.transform.position.y + down, 0), Quaternion.identity);
				hoe_pts--;
				HoeText.text = ": " + hoe_pts;
				animator.SetBool("Hammer", true);
			}
			else
			{
				DisplayText(msg_fbp);
			}
		}



        void Move(int up, int right)
        {
           
            transform.Translate(up * 3 * Time.deltaTime, right * 3 * Time.deltaTime, 0);
            float angle = Mathf.Atan2(right, up) * Mathf.Rad2Deg;
            transform.GetChild(0).rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }




		void DisplayText(Transform msg_pf)
		{
			Transform clone;
			clone = Instantiate(msg_pf, new Vector3(transform.position.x, transform.position.y + 1f, -5), Quaternion.identity)as Transform;
			clone.parent = transform;
		}



		void Put_in()
        {
			if(backpack.Count<size)
			{
				backpack.Add(item.gameObject);
				Destroy(item.gameObject);
				BackpackText.text = ": " + backpack.Count + "/" + size; 
			}

			else
			{
				DisplayText(msg_fbp);
			}

			//Function to put object in bag

        }
		
		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Check if the tag of the trigger collided with is Exit.
			if (other.tag == "Exit") {
				//First, create a bool, then make the update to check if yes or no, if yes then do the exit things, otherwise, move the character to the opposite side
				exit = true;

				//enabled = false;
				//Destroy (other);
				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
				//Invoke ("Restart", 0);
				
				//Disable the player object since level is over.
				
			}
			
		}

		void Do_exit()
		{
			enabled = false;
			Destroy (GameObject.FindWithTag("Exit"));
			//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
			Invoke ("Restart", 0);
		}


		void Do_not_exit()
		{
			//make the character go back opposite direction

			Move(-1*up, -1*down);
		}

		//Restart reloads the scene when called.
		private void Restart ()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game.
			Application.LoadLevel (Application.loadedLevel);
		}
		
		
		//LoseStamina is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void LoseStamina (int loss)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			animator.SetTrigger ("playerHit");
			
			//Subtract lost Stamina points from the players total.
			Stamina -= loss;
			
			//Update the Stamina display with the new total.
			StaminaText.text = ": " + Stamina;
			
			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		
        void PickUp(Transform stuff)
        {


            //Create stuffs on top of the head
             item = Instantiate(stuff, new Vector3(transform.position.x, transform.position.y + 0.9f, -5), Quaternion.identity) as Transform;
            //Choose to keep it or throw it
            item.parent = transform;
            hold = true;
            Debug.Log(hold);
            //
        }


		//CheckIfGameOver checks if the player is out of Stamina points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if Stamina point total is less than or equal to zero.
			/*if (Stamina <= 0) 
			{
				//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
				SoundManager.instance.PlaySingle (gameOverSound);
				
				//Stop the background music.
				SoundManager.instance.musicSource.Stop();
				
				//Call the GameOver function of GameManager.
				GameManager.instance.GameOver ();
			}*/
		}

        //Grid will call this function when Player dig a bomb
        void GetBombed()
        {
            int hp = (int)(Stamina / 3);
            if(hp < 5)
            {
                hp = 5;
            }
            Stamina -= hp;

            //Update the Stamina display with the new total.
            StaminaText.text = ": " + Stamina;
        }

        //

	}


