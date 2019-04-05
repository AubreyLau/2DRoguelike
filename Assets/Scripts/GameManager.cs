using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;	
using UnityEngine.UI;				



[SerializeField]
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 0.5f;						
        public float turnDelay = 0.5f; 
		public float playerHP = 100;	
        public float maxHP;  
        public int Money = 0;
        public string weaponType=null;
        public int level = 1;
        
		public static GameManager instance = null;				
		public bool playersTurn = true;

        
        public Slider healthBar;
        public Slider levelBar;
        public Slider damageBar;
        public GameObject healthbar;

        private Text gameOverText;									
		private GameObject gameOverImage;
        private Text winText;
        private GameObject winImage;
        private DungeonGenerator dungeonScript;						
							
		private List<Enemy> enemies;							
		
        private bool enemiesMoving;								
		private bool doingSetup = true;							

		
		
		//Awake is always called before any Start functions
		void Awake()
		{

        if (instance == null)
                instance = this;

            else if (instance != this)
            Destroy(gameObject);
       // loadInData();
        DontDestroyOnLoad(gameObject);
			enemies = new List<Enemy>();	
            dungeonScript = GetComponent<DungeonGenerator>();		
			InitGame();
		}


		public void InitGame()
		{
        loadInData();
        Player.level = level;
        Player.weaponName = weaponType;
        doingSetup = true;
        healthBar = GameObject.Find("healthPoint").GetComponent<Slider>();
        levelBar = GameObject.Find("level").GetComponent<Slider>();
        damageBar = GameObject.Find("damage").GetComponent<Slider>();
   
        gameOverText= GameObject.Find("GameOverText").GetComponent<Text>();
        gameOverImage= GameObject.Find("GameOverImage");
        winText = GameObject.Find("WinText").GetComponent<Text>();
        winImage = GameObject.Find("WinImage");

        HideLevelImage();
        Invoke("HideLevelImage", levelStartDelay);
		enemies.Clear();

        levelBar.maxValue =10+ level * level;
        healthBar.maxValue=80+(Mathf.Log(level, 2) + 1)*5;
        healthBar.maxValue = 100;

       
        maxHP = (int)healthBar.maxValue;
        Player.HP = maxHP;

        levelBar.value = Money;
        healthBar.value = playerHP;
        damageBar.value = Player.Damage;

        dungeonScript.SetupScene();
			
		}
		
    public void showLevelImage()
    {

        gameOverImage.SetActive(true);

    }

    public void restoreHP()
    {
        playerHP = maxHP;
        Money = 0;

    }
    void loadInData()
    {
        if (PlayerPrefs.HasKey("level"))
            level = PlayerPrefs.GetInt("level");
        
        if (PlayerPrefs.HasKey("weapon"))
            weaponType= PlayerPrefs.GetString("weapon");

    }

    public void clearData()
    {
        level = 1;
        weaponType = "";
        if (PlayerPrefs.HasKey("level"))
            PlayerPrefs.DeleteKey("level");
        if (PlayerPrefs.HasKey("weapon"))
            PlayerPrefs.DeleteKey("weapon");
        PlayerPrefs.Save();

    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.InitGame();
    }


    void HideLevelImage()
		{
            winImage.SetActive(false);
            gameOverImage.SetActive(false);			
			doingSetup = false;
		}
		

		void Update()
       {
        updateBar();
        if(playersTurn ||enemiesMoving || doingSetup)
				return;
      
		StartCoroutine (MoveEnemies ());
        }
		
		
		public void AddEnemyToList(Enemy enemy)
		{
			enemies.Add(enemy);
		}
		
		
		public void GameOver()
		{
        gameOverText.text = "YOU DIE";
        gameOverImage.SetActive(true);
        saveData();
    }

    public void win()
    {
        winText.text = "YOU WIN";

        winImage.SetActive(true);

        saveData();
    }

    public void saveData()
    {
        if (PlayerPrefs.HasKey("level"))
            PlayerPrefs.DeleteKey("level");
        if (PlayerPrefs.HasKey("weapon"))
            PlayerPrefs.DeleteKey("weapon");
        if(weaponType!=null)
        PlayerPrefs.SetString("weapon", weaponType);
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.Save();

    }


    public void updateBar(){

        Mathf.Clamp(Player.money, 0, 150);
        level = Player.level;
        Money = Player.money;

        Player.HP = Mathf.Min(maxHP, Player.HP);
        playerHP = Player.HP;



        healthBar.value = playerHP;
        levelBar.value =  Money;
        damageBar.value = Player.Damage;

        if (Shop.upgrade)
        {
      
            Player.level++;
            Player.money-= Shop.upgradePrice;
            Player.HP += 40;
            Player.baseDamage = Mathf.Log( Player.level +1, 1.8f)*2;
            Player.updatePlayerDamage();
            Shop.upgrade = false;
        }
    }

    //move enemies in sequence.
    IEnumerator MoveEnemies()
    {
      
        enemiesMoving = true;
             
        yield return new WaitForSeconds(turnDelay);

         if (enemies.Count != 0)
         {
             
              for (int i = 0; i < enemies.Count; i++)
               {

                  if (enemies[i] && enemies[i].isActiveAndEnabled)
                  {
                      enemies[i].MoveEnemy();
                  }
                  else enemies.Remove(enemies[i]);

                 }
           }
            yield return new WaitForSeconds(turnDelay);

			playersTurn = true;
			enemiesMoving = false;
		}
  


	}


