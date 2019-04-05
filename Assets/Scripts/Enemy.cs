using UnityEngine;
using System.Collections;
using System.Collections.Generic;       
using UnityEngine.UI;


public class Enemy : MovingObject
	{
		public int playerDamage;
        public static int stepDelay = 1;
        public static int stepCounter = 0;
		public AudioClip attackSound1;						
		public AudioClip attackSound2;						
		
		
		private Animator animator=null;		
        public Transform target;							
		private bool skipMove;								//Boolean to determine whether or not enemy should skip a turn or move this turn.
        private bool canAttack;
        public float hp;
        public GameObject[] item;
      
    protected override void Start ()
		{
			GameManager.instance.AddEnemyToList (this);
            if(GetComponent<Animator>()!=null)
			animator = GetComponent<Animator> ();
			target = GameObject.FindGameObjectWithTag ("Player").transform;
          //  hp = (int)Mathf.Log(GameManager.instance.level + 8, 2)*4+10;
            base.Start ();
		}

  /*  IEnumerator enemiesAttack()
    {
        canAttack = true;
        yield return new WaitForSeconds(3);
    }*/

    protected override void AttemptMove <T> (int xDir, int yDir)
		{
			if(skipMove)
			{
				skipMove = false;
				return;
				
			}
        if(stepDelay!=stepCounter){
            stepCounter++;
            return;
        }
        else{
            stepCounter = 0;
        }

        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;

        T hitPlayer = hit.transform.GetComponent<T>();
        if (!canMove && hitPlayer != null)
        {
            if ((Mathf.Abs(yDir) < 2) && ((Mathf.Abs(xDir) < 2)))
            {
                OnCantMove(hitPlayer);
            }
        }
    }
		
		
    public virtual void MoveEnemy ()
		{
			int xDir = 0;
			int yDir = 0;
        if(this==null){
            return;
        }
        if(Mathf.Abs (target.transform.position.x - transform.position.x) < float.Epsilon)          	
            yDir = target.transform.position.y > transform.position.y ? 1 : -1;
		else
            xDir = target.transform.position.x > transform.position.x ? 1 : -1;
        
            if (xDir > 1)
            GetComponent<SpriteRenderer>().flipX = true;
            else if(xDir<0){
            GetComponent<SpriteRenderer>().flipX = false;
        }
        
        AttemptMove <Player> (xDir, yDir);
		}
		
		
		protected override void OnCantMove <T> (T component)
		{
        		Player hitPlayer = component as Player;
			    hitPlayer.LoseHp(playerDamage);
           
                if (animator)
                animator.SetTrigger("enemyAttack");
        
			    SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}




    public virtual void hit(float loss)
    {

            hp -= loss;

        if (hp <= 0)
        {
            GameObject randomItem = item[Random.Range(0, item.Length)];
            GameObject c;
            //If the enemy is shop keeper, remove the enemy tag of the shop.
            if (transform.parent&&transform.parent.tag == "Enemy")
            {
                transform.parent.tag = "Untagged";             
                c = Instantiate(randomItem, this.transform.position + Vector3.one*2, Quaternion.identity) as GameObject;
            }
            else
                 c = Instantiate(randomItem, this.transform.position + Vector3.one, Quaternion.identity) as GameObject;

        
            gameObject.SetActive(false);
        }                     
   
    }




    }





	

