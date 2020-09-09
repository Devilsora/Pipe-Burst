using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    // Start is called before the first frame update

    private Image img;
    private IsometricPlayerController player;

    void Start()
    {
      img = GetComponent<Image>();
      player = FindObjectOfType<IsometricPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
      if (player.heldObject == null)
      {
        img.color = new Color(0, 0, 0, 0);
      }
      else
      {
        img.color = player.heldObject.GetComponent<SpriteRenderer>().color;

        if (player.heldObject.GetComponent<Item>().itemType == ItemType.BUCKET)
        {
          float currentWaterAmt = player.heldObject.GetComponent<Item>().currentWaterAmount;
          float maxWaterAmt = player.heldObject.GetComponent<Item>().maxWaterAmount;

          Debug.Log(player.heldObject.GetComponent<SpriteRenderer>().color);
          if (currentWaterAmt >= maxWaterAmt/2)
            img.sprite = player.heldObject.GetComponent<Item>().halfFull;
          else
          {
            img.sprite = player.heldObject.GetComponent<Item>().empty;
          }
        }
        else
        {
          img.sprite = player.heldObject.GetComponent<SpriteRenderer>().sprite;
        }
        
      }
    }
}
