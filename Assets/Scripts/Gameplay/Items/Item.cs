using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
  BOX,
  BUCKET,
  LADDER,
  TAPE
}

public class Item : MonoBehaviour
{

    public ItemType itemType;
    //if it can hold water and how many units
    public bool canHoldWater;
    public float maxWaterAmount;
    public float currentWaterAmount;
    public bool full;
    public bool fallenOver;
    

    public bool canLiftPlayer;                      //if it can make the player rise in height
    public bool moveable;                           //can the player move this object
    public bool canBeStackedOn;
    public bool isStacked;
    public bool isOpenable;

    public float sealTime;

    private SpriteRenderer spr;
    public Sprite halfFull;
    public Sprite empty;

    public AudioSource audSource;

    public AudioClip bucketFill;

    public bool playedTipOver;

    
    // Start is called before the first frame update
    void Awake()
    {
      spr = GetComponent<SpriteRenderer>();
      audSource = GetComponent<AudioSource>();
      if (canHoldWater)
      {
        spr.sprite = empty;

        if (currentWaterAmount >= maxWaterAmount / 2)
        {
          spr.sprite = halfFull;
        }
      }

     

    }

    // Update is called once per frame
    void Update()
    {
      if (canHoldWater)
      {
       
        if (currentWaterAmount >= maxWaterAmount / 2)
        {
          spr.sprite = halfFull;
        }
        else
        {
          spr.sprite = empty;
        }

        if (currentWaterAmount >= maxWaterAmount && !playedTipOver)
        {
         
          audSource.clip = bucketFill;
          audSource.Play();
          full = true;
          playedTipOver = true;
          fallenOver = true;
          FindObjectOfType<WaterManager>().roomWaterLevel += currentWaterAmount;
        }
      }

      if (fallenOver && canHoldWater)
      {
        transform.eulerAngles = new Vector3(0, 0, 90f);
        
      }
    }

    public void OnParticleCollision(GameObject other)
    {
      //Debug.Log("OnParticleCollision");
      if (canHoldWater)
      {
        if (other.tag == "WaterDrip" && !full)
        {
          currentWaterAmount += 0.01f;
          Debug.Log("Sending true death message");
          
        }

        if (currentWaterAmount >= maxWaterAmount)
        {
          //other.SendMessage("ToggleParticleDeath", false);
        }
      }
      else
      {
        other.SendMessage("ToggleParticleDeath", true);
        FindObjectOfType<WaterManager>().roomWaterLevel += 0.01f;
      }
    }

    
}
