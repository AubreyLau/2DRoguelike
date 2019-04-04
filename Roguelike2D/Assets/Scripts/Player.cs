using UnityEngine;
using System.Collections;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;



	public class Player : MovingObject
	{
        public static int level;
		public int pointsPerFood = 10;				
		public int pointsPerSoda = 20;				
        public static int pointsPerCoin = 2;
        public static float Damage = 0;	
        public static float HP=100;   
        public static int money = 0;
        public static float baseDamage = 1;
        public static int attackRange = 0;               
        public static int attackDelay = 0; 
        public static Vector2 stepCounter;
        public static Vector3 playerPos;
		public Text hpText;		
        public Text moneyText; 	
        public Text lvlText;			
		public AudioClip moveSound1;				
		public AudioClip moveSound2;				
		public AudioClip eatSound1;				
		public AudioClip eatSound2;					
		public AudioClip drinkSound1;				
		public AudioClip drinkSound2;				
		public AudioClip gameOverSound;             
        public AudioClip attackSound1;                      
        public AudioClip attackSound2;
        public static bool usingPotion = false;

        public static bool weaponEquipped = false;
        public static string weaponName;
        private bool weaponFlipped = false;
   
        private Animator animator;                 //Used to store a reference to the Player's animator component.
        private Rigidbody2D r;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif
    protected override void Start ()
		{    
            initPlayer();
			base.Start ();
		}


       private void initPlayer(){

        transform.position = new Vector3((int)(DungeonGenerator.columns * 0.5), (int)(DungeonGenerator.rows * 0.5), 0);
        stepCounter = new Vector2(transform.position.x, transform.position.y);
        animator = GetComponent<Animator>();
        GameManager.instance.restoreHP();
        baseDamage =  Mathf.Log(GameManager.instance.level + 1, 1.8f)*2;
    }

    private void OnDisable()
    {
        GameManager.instance.restoreHP();
    }

    public static void updatePlayerDamage()
    {    
        Damage = weapons.damage + baseDamage;
    }

		
		private void Update ()
		{
            showText();

        if (usingPotion)
        {
              HP = GameManager.instance.maxHP;
              StartCoroutine(usePotion());
        }
       
            playerPos = transform.position;
			if(!GameManager.instance.playersTurn) return;			
			int x = 0;  	
			int y = 0;		
			
#if UNITY_STANDALONE || UNITY_WEBPLAYER
						
			x = (int) (Input.GetAxisRaw ("Horizontal"));
			y = (int) (Input.GetAxisRaw ("Vertical"));
       

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			
			if (Input.touchCount > 0)
			{
			 Touch myTouch = Input.touches[0];
			
				if (myTouch.phase == TouchPhase.Began)
				{
					touchOrigin = myTouch.position;
				}
					else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{

                    	Vector2 touchEnd = myTouch.position;
						float a = touchEnd.x - touchOrigin.x;
						 float b = touchEnd.y - touchOrigin.y;
						touchOrigin.x = -1;
					
					if (Mathf.Abs(a) > Mathf.Abs(b))
						x = a > 0 ? 1 : -1;
					else
						y = b > 0 ? 1 : -1;
				}
			}

#endif

        if (x != 0)
            y = 0;
        
        flip(x);


        if (x != 0 || y != 0)
			{
       
            AttemptMove<Wall>(x, y);
            AttemptMove<Enemy>(x, y);
            RaycastHit2D hit;
            bool canMove = Move(x, y, out hit);
            if (canMove)
            {
                stepCounter += new Vector2(x, y);
                SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            }

        }    

		}
    private void showText(){
        
      //  hpText.text =  " HP: " + HP;
        moneyText.text = "" +money;
        lvlText.text = "" + level;

    }
       protected override void AttemptMove <T> (int xDir, int yDir)
		{
        base.AttemptMove <T> (xDir, yDir);


        CheckIfGameOver ();
			GameManager.instance.playersTurn = false;
		}
		
		
		protected override void OnCantMove <T> (T component)
		{
       
        if (weaponEquipped){

            attackAnimation();
            if (component.tag == "Enemy")
            {
                Enemy hitEnemy = component as Enemy;
                hitEnemy.hit(Damage);
                SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);

            }  
            else if (component.tag == "Wall")
            {
              

                Wall hitWall = component as Wall;
                hitWall.DamageWall(2);
            }
       }      
       
		}
        

    private void OnTriggerEnter2D (Collider2D other)
		{
			if(other.tag == "Exit")
			{
               Invoke ("YouWin",1.0f);
			}
				
			else if(other.tag == "Food")
			{
				HP += pointsPerFood;			
				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
                animator.SetTrigger("playerJump");
				other.gameObject.SetActive (false);
			}
        else if (other.tag == "Heart")
        {
            RectTransform rect = GameManager.instance.healthBar.GetComponent<RectTransform>();
            if (GameManager.instance.maxHP > 250) return;
            GameManager.instance.healthBar.maxValue *=1.8f;
            GameManager.instance.maxHP *= 1.8f;
            rect.sizeDelta *= new Vector2(1.8f, 1f);
            HP += pointsPerFood;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            animator.SetTrigger("playerJump");
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Potion")
        {
            transform.localScale = Vector3.one * 1.6f;
            playerSpotlight.intensity = 1.8f;
            HP += pointsPerFood;

            useItem(other.transform);
            usingPotion = true;
        }

        else if(other.tag == "Soda")
			{
				HP += pointsPerSoda;
            useItem(other.transform);
        }
        else if (other.tag == "coin")
        {
            money += pointsPerCoin;
            useItem(other.transform);
        }
        else if (other.tag == "superCoin")
        {
            money += pointsPerCoin*10;
            useItem(other.transform);
        }
        else if (other.tag == "weapon")
        {
            if (weaponEquipped)
            takeOffWeapon();
            other.transform.SetParent(this.transform);
            weaponEquipped = true;
            initWeapon();
        }

		}


    void takeOffWeapon()
    {
      
        transform.GetChild(2).GetComponent<SpriteRenderer>().flipX = weaponFlipped;
        transform.GetChild(2).transform.position = transform.position + Vector3.one*1.5f;
            transform.GetChild(2).SetParent(null);       
    }
    void initWeapon()
    {
    
        weaponName = transform.GetChild(2).name;
        GameManager.instance.weaponType = weaponName;
        weapons.equipt(weaponName[8] - '0', weaponName[10] - '0');  // get the type and range of the weapon by its name
        Damage = weapons.damage + baseDamage; 
        weaponFlipped = transform.GetChild(2).GetComponent<SpriteRenderer>().flipX; 

    }


    public void Restart ()
		{
        GameManager.instance.saveData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        SoundManager.instance.musicSource.Play();
       
        restore();
        enabled = false;
    }

    public void YouWin()
    {

        GameManager.instance.clearData();
        GameManager.instance.win();
        restore();
    }


    public void NewGame()
    {
        GameManager.instance.clearData();
        Restart();
    }
    public void Quit()
    {
        GameManager.instance.saveData();
        restore();
        Application.Quit();
    }

    public void restore()
    {
        stepCounter = Vector2.zero;
        DungeonGenerator.dungeonInstance.currentPos = Vector2.zero;
        DungeonGenerator.dungeonInstance.Dir = Vector2.zero;
        HP = GameManager.instance.maxHP;
        money = 0;
        weaponEquipped = false;
        weapons.damage = 0;
        Damage = baseDamage;
    }

    IEnumerator usePotion()
    {
    
        yield return new WaitForSeconds(20);
        usingPotion = false;
        transform.localScale = Vector3.one;
        playerSpotlight.intensity = 1;

    }
    public void LoseHp (int loss)
		{
		animator.SetTrigger ("playerHit");
        animator.SetTrigger("playerJump");
			
			HP -= loss;

			CheckIfGameOver ();
		}
		
		
		private void CheckIfGameOver ()
		{
			
        if (HP <= 0) 
			{
				SoundManager.instance.PlaySingle (gameOverSound);
				SoundManager.instance.musicSource.Stop();
				GameManager.instance.GameOver();
			}
		}


    private void attackAnimation()
    {

        animator.SetTrigger("playerJump");
        animator.SetTrigger("playerChop");
        transform.transform.GetChild(2).GetComponent<Animator>().SetTrigger("attack");

    }
    private void useItem(Transform item)
    {
        SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
        animator.SetTrigger("playerJump");
        item.gameObject.SetActive(false);

    }

    public void flip(int x)
    {
        if (x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            if(weaponEquipped){
                transform.GetChild(2).GetComponent<SpriteRenderer>().flipX = !weaponFlipped;
                transform.GetChild(2).transform.localPosition = new Vector3(-0.45f, -0.04f, 0);
            }
           
        }
        else if (x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            if (weaponEquipped)
            {
                transform.GetChild(2).GetComponent<SpriteRenderer>().flipX = weaponFlipped;
                transform.GetChild(2).transform.localPosition = new Vector3(0.45f, -0.04f, 0);
            }
        }
    }



	}

