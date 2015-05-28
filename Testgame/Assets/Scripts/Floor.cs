using UnityEngine;
using System.Collections;


    public class Floor : MonoBehaviour
    {
        public bool isExit = false;
        public GameObject player;
        public Transform[] prefab;
        public Sprite dmgSprite;
        public float[] rand_num;
        private SpriteRenderer spriteRenderer;
        // Use this for initialization
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            player = GameObject.Find("Player");
        }

        // Update is called once per frame
        public void DamageFloor(int loss)
        {

            //Call the RandomizeSfx function of SoundManager to play one of two chop sounds.
           // SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

            //Set spriteRenderer to the damaged wall sprite.
            spriteRenderer.sprite = dmgSprite;


            int i = 0,j;
            //If hit points are less than or equal to zero:
            float rand = Random.Range(0f, 100f);
            if(rand < 50)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else if (rand < 55)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
                Instantiate(prefab[0], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }
            else
            {
                for (j = 1; j < rand_num.Length; j++)
                {
                    if (rand < rand_num[j])
                    {
                        i = j;
                    }
                }

                gameObject.layer = LayerMask.NameToLayer("Default");
                player.SendMessage("PickUp", prefab[i]);
            }   
                    

            
        }
    }