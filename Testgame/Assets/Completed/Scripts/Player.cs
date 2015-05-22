using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using System.Collections.Generic;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MonoBehaviour
	{

        public LayerMask blockingLayer;
        public Transform hammer;
        public Transform hoe;
        public GameObject hitbox;
        private Transform item;
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int pointsPerStamina = 10;				//Number of points to add to player Stamina points when picking up a Stamina object.
		public int pointsPerSoda = 20;				//Number of points to add to player Stamina points when picking up a soda object.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		public Text StaminaText;						//UI Text to display current player Stamina total.
		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a Stamina object.
		public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a Stamina object.
		public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
        int up = 0;  	//Used to store the horizontal move direction.
        int down = 0;		//Used to store the vertical move direction.
        bool movable = true;
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
			//Set the StaminaText to reflect the current player Stamina total.
			StaminaText.text = "Stamina: " + Stamina;
			
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
                    backpack.Add(item.gameObject);
                    //Function to put object in bag
                    Destroy(item.gameObject);
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
            else if (Input.GetButtonDown("Hammer") && !hold)
            {
                    //Debug.Log(hold);
                    Debug.Log("Hammer");
                    Instantiate(hammer, new Vector3(hitbox.transform.position.x + up, hitbox.transform.position.y + down, 0), Quaternion.identity);
                    animator.SetBool("Hammer", true);
               
            }
            else if (Input.GetButtonDown("Hoe")&& hold == false)
            {
                Debug.Log("Hoe");
                Instantiate(hoe, new Vector3(hitbox.transform.position.x + up, hitbox.transform.position.y + down, 0), Quaternion.identity);
                animator.SetBool("Hammer", true);
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
		
        void Move(int up, int right)
        {
           
            transform.Translate(up * 3 * Time.deltaTime, right * 3 * Time.deltaTime, 0);
            float angle = Mathf.Atan2(right, up) * Mathf.Rad2Deg;
            transform.GetChild(0).rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

		void Put_in()
        {

        }
		
		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Check if the tag of the trigger collided with is Exit.
			if(other.tag == "Exit")
			{
                enabled = false;
				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
				Invoke ("Restart", 0);
				
				//Disable the player object since level is over.
				
			}
			
			//Check if the tag of the trigger collided with is Stamina.
			/*else if(other.tag == "Food")
			{
				//Add pointsPerStamina to the players current Stamina total.
				//Stamina += pointsPerStamina;
				
				//Update StaminaText to represent current total and notify player that they gained points
				//StaminaText.text = "+" + pointsPerStamina + " Stamina: " + Stamina;
				
				//Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
				//SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				
				//Disable the Stamina object the player collided with.
				//other.gameObject.SetActive (false);
			}*/
			
			//Check if the tag of the trigger collided with is Soda.
			/*else if(other.tag == "Soda")
			{
				//Add pointsPerSoda to players Stamina points total
				Stamina += pointsPerSoda;
				
				//Update StaminaText to represent current total and notify player that they gained points
				StaminaText.text = "+" + pointsPerSoda + " Stamina: " + Stamina;
				
				//Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				
				//Disable the soda object the player collided with.
				other.gameObject.SetActive (false);
			}*/
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
			StaminaText.text = "-"+ loss + " Stamina: " + Stamina;
			
			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		
        void PickUp(Transform stuff)
        {


            //Create stuffs on top of the head
             item = Instantiate(stuff, new Vector3(transform.position.x, transform.position.y + 1, 0), Quaternion.identity) as Transform;
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
	}
}

