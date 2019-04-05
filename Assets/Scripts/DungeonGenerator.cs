using UnityEngine;
using System;
using System.Collections.Generic;      
using Random = UnityEngine.Random;      


public class DungeonGenerator : MonoBehaviour
{
    
    [Serializable]
    public class Count
    {
        public int minimum;            
        public int maximum;             

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public static int columns = 16;                                        
    public static int rows = 10;  
    public Count wallCount = new Count(10, 15);                       
    public Count foodCount = new Count(0, 2);       

    public GameObject exit;                                        
    public GameObject[] floorTiles;                               
    public GameObject[] wallTiles;                                  
    public GameObject[] foodTiles;                                  
    public GameObject[] enemyTiles;   
    public GameObject[] enemyShopTiles;  
    public GameObject[] outerWallTiles;                            
    public GameObject[] Boss;
    public GameObject[] finalBoss;
    public GameObject[] weapon1;
    public GameObject[] weapon2;
    public GameObject[] weapon3;
    public GameObject[] weapon4;
    public GameObject[] shop;
    public List<GameObject> weapon;

    public GameObject boardHolder;                                  
    public GameObject right; 
    public GameObject left; 
    public GameObject up; 
    public GameObject down;
    public static DungeonGenerator dungeonInstance = null;

    GameObject instance;
    private List<Vector3> gridPositions = new List<Vector3>();  
    private Transform newBoard;
    public  Vector2 currentPos = Vector2.zero;
    
    private List<GameObject> Board;
    public Vector2 Dir = Vector2.zero;
    private GameObject defaultWeapon;

    void Awake()
    {
        if (dungeonInstance == null)
            dungeonInstance = this;

        else if (dungeonInstance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void InitialiseList()
    {
        gridPositions.Clear();

        for (int x = 1+(int)currentPos.x*(columns+2); x < columns - 1+ (int)currentPos.x * (columns + 2); x++)
        {
            for (int y = 1+(int)currentPos.y*(rows+2); y < rows - 1+ (int)currentPos.y * (rows + 2); y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));        
            }
        }
    }

    //Lay out items to the first main board.
    public void SetupScene()
    {
        boardHolder = new GameObject();
        BoardGenerate(currentPos);
        InitialiseList();
        LayoutObjectAtBoard(wallTiles, wallCount.minimum, wallCount.maximum, boardHolder.transform);
        LayoutObjectAtBoard(foodTiles, foodCount.minimum, foodCount.maximum, boardHolder.transform);
        LayoutObjectAtBoard(enemyTiles, 2, 3, boardHolder.transform);
        // LayoutObjectAtBoard(defaultWeapon, 1, 1, boardHolder.transform);
        GetWeapon();
        GameObject weaponInstance = Instantiate(defaultWeapon, new Vector3(8,5,0), Quaternion.identity) as GameObject;
    }

    //Find the weapon we used last round by its name(saved in GameManager prefabs). Otherwise, return the default weapon instead.
    public GameObject GetWeapon()
    {
        defaultWeapon = weapon1[Random.Range(0, weapon1.Length)];
        string weaponName = GameManager.instance.weaponType;
        //Player.weaponName;
        if (weaponName!="")
        {
            if (weaponName.Length < 8) return defaultWeapon;
            int weaponType = weaponName[8] - '0';
            int weaponNum = weaponName[10] - '0' - 1;
            defaultWeapon = weapon[(weaponType - 1) * 2 + weaponNum];
        }
        return defaultWeapon;
    }

    // Lay out items every time a new board is generated.
    public void addItems(Transform t)
    {
        InitialiseList();
        LayoutObjectAtBoard(wallTiles, rows * 3, rows * 4, t);

        // Add items to left/right boards.
        if (currentPos.y == 0)
        {
            if ((int)Random.Range(0, 4) == 1)
                Shop(t);
            else if (GameManager.instance.level>9)
                finalRoom(t);
            else normalRoom(t);
        }

        // Add items to up/dowm boards
        else
        {
            if ((int)Random.Range(0, 3) >= 2)
                Shop(t);
            if ((int)Random.Range(0, 4) >= 3 && Mathf.Abs(currentPos.x) > 5)
                weaponRoom(t);
            else
                normalRoom(t);
        }
    }

    //The weapon room which is likely to appear in up/down boards
    private void weaponRoom(Transform t)
    {
        LayoutObjectAtBoard(Boss, 1, 3, t);

        LayoutObjectAtBoard(weapon4, 1, 1, t);
    }

      
    private void normalRoom(Transform t)
    {
        //Lay out enemes.
        int lvl = GameManager.instance.level;
        lvl = Mathf.Clamp(lvl, 0, 10);
        LayoutObjectAtBoard(foodTiles, 0, 3, t);
        LayoutObjectAtBoard(enemyTiles, 1, GameManager.instance.level * 2 + (int)Mathf.Log(currentPos.SqrMagnitude() + 1, 2), t);
        LayoutObjectAtBoard(Boss, 0, (int)Mathf.Log(currentPos.SqrMagnitude(), 5), t);

        //Lay out the (normal) shop.
            LayoutObjectAtBoard(shop, 0, 1, t);
        //Lay out weapons 
        if (lvl >= 8)
            LayoutObjectAtBoard(weapon4, 0, 1, t);
        else if (lvl >= 4)
            LayoutObjectAtBoard(weapon3, 0, 2, t);
        else if (lvl >= 1)
            LayoutObjectAtBoard(weapon2, 0, 1, t);
  


    }


    private void finalRoom(Transform t)
    {
        LayoutObjectAtBoard(finalBoss, 0, 2, t);

    }

    //A (crazy) shop with an angry shop keeper
    private void Shop(Transform t)
    {
        LayoutObjectAtBoard(enemyShopTiles, 1, 1, t);
        LayoutObjectAtBoard(enemyTiles, 0, GameManager.instance.level, boardHolder.transform);
    }


    private void Update()
    {
        BoardUpdate();
    }



    void BoardGenerate(Vector2 dir)     {
      //generate the first board
        if(dir==Vector2.zero){

           
            for (int x = -1; x < columns + 1; x++)
            {
                for (int y = -1; y < rows + 1; y++)
                {
                    
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                    instance = Instantiate(toInstantiate, new Vector3(x, y, 0f),
                    Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder.transform);
                    if (x == -1 || x == columns || y == -1 || y == rows)
                    {
                        toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder.transform);
                    }
                    instance.transform.SetParent(boardHolder.transform);
                }
            }
        }
        //Generat left and right boards along x axis based on the player's position.
        else if (dir.y == 0)
        {             if (dir == Vector2.right)             {                 right = new GameObject("right");                 newBoard = right.transform;             }             else if (dir == Vector2.left)             {                 left = new GameObject("left");                 newBoard = left.transform;             }              for (int x = 0; x < columns+2; x++)             {                 for (int y = -1; y < rows + 1; y++)                 {
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];                     instance = Instantiate(
                        toInstantiate, new Vector3(currentPos.x * (columns+1f) + x * dir.x -(dir.x-1)*0.5f*(columns+currentPos.x)+(dir.x + 1)*0.5f*(currentPos.x-1),
                                                   currentPos.y  * (rows +2f) + y, 0f), Quaternion.identity) as GameObject;                     instance.transform.SetParent(newBoard);
                    if (x == columns +1 || y == -1 || y == rows){
                        if((x==0||x == columns + 1) &&( y == -1 || y == rows))
                        {
                            toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                        }
                       else
                            toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];

                        instance = Instantiate(toInstantiate,new Vector3(currentPos.x * (columns + 1f) + x * dir.x - (dir.x - 1) * 0.5f * (columns + currentPos.x) + (dir.x + 1) * 0.5f * (currentPos.x - 1),
                                                   currentPos.y * (rows + 2f) + y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(newBoard);
                } 
                 }             }         }

        //Generat boards along y axis.
        else
        {
            if (dir == Vector2.up)             {                 up = new GameObject("up");                 newBoard = up.transform;             }             else if(dir == Vector2.down)             {                 down = new GameObject("down");                 newBoard = down.transform;             }             int X = (int)currentPos.x;
            int Y = (int)currentPos.y;             for (int x = -1; x < columns + 1; x++)             {                 for (int y = 0; y < rows +2; y++)                 {                     GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];                     instance = Instantiate(toInstantiate, new Vector3(X * (columns +2f ) + x, 
                      Y * (rows + 1f) + y * dir.y - (dir.y - 1) * 0.5f * (rows + Y) + (dir.y + 1) * 0.5f * (Y - 1),0f), Quaternion.identity) as GameObject;                     instance.transform.SetParent(newBoard);

                    if (x==-1||x == columns || y == rows+1){
                        if ((y==0||y == rows + 1 )&& (x == -1 || x == columns))
                        {
                            toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                        }
                        else
                            toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                        instance = Instantiate(toInstantiate, new Vector3(X * (columns + 2f) + x,
                         Y * (rows + 1f) + y * dir.y - (dir.y - 1) * 0.5f * (rows + Y) + (dir.y + 1) * 0.5f * (Y - 1), 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(newBoard);
                } 
                 }             }
        }      }


      void BoardUpdate()
    {
        // Generate the leftboard
        if (Player.stepCounter.x > columns-1)
        {
            if (Dir.x < 0)
            {              
                if(Player.stepCounter.x==2*columns+2) 
                    currentPos.x += 2;                               
                else return;                               
            }
            else
            {
                currentPos.x++;
                if(right){ boardHolder = right; }                  
           
            }
            if(left)
            Destroy(left.gameObject);       
            left = boardHolder;
            BoardGenerate(Vector2.right);
            Player.stepCounter.x = -2;
            Dir.x = 1;
            addItems(right.transform);
            cleanUpBoard();
            cleanDownBoard();
        }

        // Generate the rightboard
        if (Player.stepCounter.x < 0)
        {
            if (Dir.x > 0)
            {
                if (Player.stepCounter.x == - 3-columns)
                    currentPos.x -= 2;                       
                else return;                
            }
            else
            {
                currentPos.x--;
                if (left) boardHolder = left;             
            }
            if(right)
            Destroy(right.gameObject);
            right = boardHolder;
            BoardGenerate(Vector2.left);
            Player.stepCounter.x = columns + 1;
            Dir.x = -1;
            addItems(left.transform);
            cleanUpBoard();
            cleanDownBoard();
        }

        // Generate the up/down board.
        if (Player.stepCounter.y > rows - 1)
        {
            currentPos.y = 1;
            if (up) return;

            BoardGenerate(Vector2.up);
            addItems(up.transform);
            currentPos.y = 0;
        }

        else if (Player.stepCounter.y < 0)
        {
            currentPos.y = -1;
            if (down) return;

            BoardGenerate(Vector2.down);
            addItems(down.transform);
            currentPos.y = 0;

        }
        else currentPos.y = 0;
    }


    //Get current position by the player's step counter.
    private Vector2 getCurrentPos()
    {
        Vector2 pos=Vector2.zero;
        pos.x = (int)Math.Round(Player.playerPos.x / columns);
        if (Player.playerPos.x < 0)
        {
            pos.x = (int)(Player.playerPos.x / columns) - 1;
        }
        return pos;
    }


    Vector3 RandomPosition()
    {
       int Index = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[Index]; 
         gridPositions.RemoveAt(Index);
        if(randomPosition==new Vector3(columns*0.5f,rows*0.5f,0)){
            return RandomPosition();
        }
          else return randomPosition;
    }


    // Lay out items at random
    void LayoutObjectAtBoard(GameObject[] tileArray, int minimum, int maximum, Transform board)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition() ;
            //+ new Vector3(currentPos.x * (columns + 2), currentPos.y * (rows + 2));
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            GameObject instance =
                Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;
            instance.transform.SetParent(board);

        }
    }


    private void cleanUpBoard()
    {
        if (up)
        {
            Destroy(up);
            up = null;
        }    
    }
    private void cleanDownBoard()
    {
        if (down)
        {
            Destroy(down);
            down = null;
        }
    }



}

