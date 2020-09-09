using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterFillProgress : MonoBehaviour
{
    public Item bucket;
    public Image fill;
  
  // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      fill.fillAmount = bucket.currentWaterAmount / bucket.maxWaterAmount;
    }
}
