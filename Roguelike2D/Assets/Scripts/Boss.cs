using UnityEngine;
using System.Collections;

public class Boss : Enemy
{

    public GameObject droptEnemy;
    public GameObject attackEffect;
    public float intensity=1f;
    float t=0;
    protected override void Start()
    {
        stepDelay = 2;
        GameManager.instance.AddEnemyToList(this);
        transform.localScale =1.8f * Vector3.one * intensity;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
        this.hp += 5*GameManager.instance.level* intensity * intensity;
        this.playerDamage += (int)(10 * intensity);
      //  GameObject explode = Instantiate(attackEffect, transform.position + Vector3.right, Quaternion.identity) as GameObject;
      //  explode.transform.SetParent(transform);

    }


    public override void MoveEnemy()
    {
      
        if (this == null)     
         return;
        
        Vector3 dir = target.transform.position - transform.position;
        if (dir.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            transform.GetChild(1).localPosition = Vector3.right;
        }
        else{
            GetComponent<SpriteRenderer>().flipX = false;
            transform.GetChild(1).localPosition = Vector3.left;
        }
            
        AttemptMove<Player>((int)dir.x, (int)dir.y);

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
     
        if (other.tag == "Wall")
        {
            other.gameObject.SetActive(false);
        }
    }


    protected override void AttemptMove<T>(int xDir, int yDir)
    {
       
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null){
            if ( (Mathf.Abs(yDir) < 1) && ((Mathf.Abs(xDir) <2))){
                OnCantMove(hitComponent);
                transform.Find("explosion").GetComponent<Animator>().SetTrigger("explode");
            }
            else Move(2, 0, out hit);
             
            
        }
    }




    public override void hit(float loss)
    {

        hp -= loss;

        if (hp <= 0)
        {
            
            GameObject enemyRight = Instantiate(droptEnemy, target.position + Vector3.right, Quaternion.identity) as GameObject;
            GameObject enemyLeft= Instantiate(droptEnemy, target.position + Vector3.left, Quaternion.identity) as GameObject;
            GameObject enemyUp = Instantiate(droptEnemy, target.position +Vector3.up, Quaternion.identity) as GameObject;
            GameObject enemyDown= Instantiate(droptEnemy, target.position + Vector3.down, Quaternion.identity) as GameObject;

            GameObject randomItem = item[Random.Range(0, item.Length)];
            GameObject coin = Instantiate(randomItem, target.position + Vector3.up*intensity*intensity, Quaternion.identity) as GameObject;

            if (coin.tag != "Exit")
                coin.transform.localScale = Vector3.one * 3;
            else
            {
                coin.GetComponent<Animator>().SetTrigger("open");
            }

            if (coin.tag=="coin")
            coin.tag = "superCoin";
           
            gameObject.SetActive(false);

        }

    }



    private void Update()
    {
        t += Time.deltaTime;
        if (intensity != 1)
        {

            moveTime = (Mathf.Sin(t)+1)*0.3f;

        }
    }

}







