using UnityEngine;
using UnityEngine.UI;   
using System;
using System.Collections.Generic;      
using Random = UnityEngine.Random;      


public class Shop : MonoBehaviour
{
    public Canvas shopCanvas;
    public static bool upgrade=false;
    public static int upgradePrice=4;
    public Text priceText;
    bool isEnemy=false;
    // Start is called before the first frame update
    private void Start()
    {
        checkEnemy();
        setPrice();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            checkEnemy();
            shopCanvas.gameObject.SetActive(true);
            changePriceColor();
            priceText.text = "" + upgradePrice;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        checkEnemy();
        setPrice();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            shopCanvas.gameObject.SetActive(false);

        }
    }


    public void Upgrade()
    {
 
        if (upgradePrice <= GameManager.instance.Money)
        {
            upgrade = true;
            GameManager.instance.Money -= upgradePrice;

        }
        else 
        {
            priceText.color = new Color(0.72f, 0.22f, 0.22f);
        }
          

    }

    private void changePriceColor()
    {
        if (upgradePrice <= GameManager.instance.Money)
        {
            priceText.color = Color.white;

        }
        else priceText.color = new Color(0.72f, 0.22f, 0.22f);

    }


    private void setPrice()
    {
        if (isEnemy) 
        upgradePrice = 1111111;
        else
            upgradePrice = 4 + (GameManager.instance.level-1) * 8 ;

    }


    private void checkEnemy()
    {
        if (tag == "Enemy")
            isEnemy = true;
        else isEnemy = false;

    }

    private void Update()
    {
        checkEnemy();
        setPrice();
    }


}
