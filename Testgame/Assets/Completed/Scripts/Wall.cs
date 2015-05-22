using UnityEngine;
using System.Collections;

namespace Completed
{
	public class Wall : MonoBehaviour
	{
        public bool isExit = false;

        public float[] rand_num;
        public GameObject player;
        public Transform[] prefab;
		public AudioClip chopSound1;				//1 of 2 audio clips that play when the wall is attacked by the player.
		public AudioClip chopSound2;				//2 of 2 audio clips that play when the wall is attacked by the player.
		public Sprite dmgSprite;					//Alternate sprite to display after Wall has been attacked by player.
		public int hp = 3;							//hit points for the wall.
        bool damaged = false;
		
		private SpriteRenderer spriteRenderer;		//Store a component reference to the attached SpriteRenderer.
		
		
		void Awake ()
		{
			//Get a component reference to the SpriteRenderer.
			spriteRenderer = GetComponent<SpriteRenderer> ();
            player = GameObject.Find("Player");
		}
		
		
		//DamageWall is called when the player attacks a wall.
		public void DamageWall (int loss)
		{
            
			//Call the RandomizeSfx function of SoundManager to play one of two chop sounds.
			SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
			
			//Set spriteRenderer to the damaged wall sprite.
			spriteRenderer.sprite = dmgSprite;
			
			//Subtract loss from hit point total.
			hp -= loss;
			
			//If hit points are less than or equal to zero:
			if(hp <= 0)
            {
                int i = 0, j;
                //If hit points are less than or equal to zero:
                float rand = Random.Range(0f, 100f);
                Debug.Log(rand);
                for (j = 0; j < rand_num.Length; j++)
                {
                    if (rand < rand_num[j])
                    {
                        i = j;
                        break;
                    }
                }
				//Disable the gameObject.
				gameObject.SetActive (false);
                //Send to player the prefab
                player.SendMessage("PickUp", prefab[i]);
                
                }
		}
	}
}
