using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    //spawn water leak prefab - number of prefabs out dictate how quickly room fills up
    public List<GameObject> waterLeakPrefabs;
    public List<GameObject> waterLeaks = new List<GameObject>();
    public float roomWaterLevel;
    public float roomWaterLevelMAX;
    public float waterLevelPercent;
    

    public float leakSpawnPercent;
    public float leakSpawnTime;
    public float leakSpawnTimeCounter;
    public GameObject waterEffect;

    public float boxSpawnPercent;
    public List<GameObject> itemsThatCanSpawn;
    public Dictionary<float, List<Item>> itemSpawnTable = new Dictionary<float, List<Item>>();
    

    public GameObject lootBox;
    public ParticleSystem.MinMaxCurve spawn_Curve_high;
    public ParticleSystem.MinMaxCurve spawn_Curve_low;
    public float max_bucket_fill = 5f;



    public Vector3 minWaterEffectPos = new Vector3(-1f, -5.61f, 11.46f);
    public Vector3 maxWaterEffectPosition = new Vector3(-1f, -1.2f, 11.46f);

    private float startX = -1f;
    private float startZ = 11.46f;


    public AudioSource audSource;
    public GameObject BGM;
    public bool soundEffectOn;
    public AudioClip leakSpawn;

    public GameObject loseScreen;

  //as water level rises, might be more likely to make more leaks?

  // Start is called before the first frame update
    void Start()
    {
      audSource = GetComponent<AudioSource>();
      SpriteRenderer newC = waterEffect.GetComponent<SpriteRenderer>();
      Color col = newC.color;
      col.a = 0f;
      newC.color = col;

      //load up spawn table
      //1st item is empty, 2nd item is tape, 3rd item is bucket, 4th item is tape and bucket, 5th is better tape == wrench, 5th is all 3?
      itemSpawnTable.Add(10f, new List<Item>());
      itemSpawnTable.Add(30f, new List<Item>(){itemsThatCanSpawn[0].GetComponent<Item>()});
      itemSpawnTable.Add(40f, new List<Item>() { itemsThatCanSpawn[1].GetComponent<Item>() });
      itemSpawnTable.Add(50f, new List<Item>() {itemsThatCanSpawn[0].GetComponent<Item>(), itemsThatCanSpawn[1].GetComponent<Item>() });
      itemSpawnTable.Add(70f, new List<Item>(){itemsThatCanSpawn[2].GetComponent<Item>() });
      itemSpawnTable.Add(90f, new List<Item>() { itemsThatCanSpawn[0].GetComponent<Item>(), itemsThatCanSpawn[1].GetComponent<Item>(), itemsThatCanSpawn[2].GetComponent<Item>()});
    }

    public void MakeItemBox()
    {
      float randomVal = Random.Range(0f, 100f);
      Debug.Log("Box random val: " + randomVal);
      List<Item> objectList = new List<Item>();
      itemsThatCanSpawn[1].GetComponent<Item>().maxWaterAmount = Random.Range(2f, max_bucket_fill);
      //find first value that this is less than
      foreach (KeyValuePair<float,List<Item>> spawnTableEntry in itemSpawnTable)
      {
        if (randomVal <= spawnTableEntry.Key)
        {
          objectList = spawnTableEntry.Value;
          break;
        }
      }

      float x_val = Random.Range(-6.5f, 6.5f);

      float y_Val = Random.Range(spawn_Curve_low.Evaluate(x_val), spawn_Curve_high.Evaluate(x_val));

      Vector2 newPos = new Vector2(x_val, y_Val);

      lootBox.GetComponent<ItemBox>().itemsInBox = objectList;
      Instantiate(lootBox, newPos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
    //until 25% of the way there, water effect alpha is based on that percentage and will max out at 128
    //after 25% it'll start moving up as the water level rises

      waterLevelPercent = roomWaterLevel / roomWaterLevelMAX;

      if (waterLevelPercent <= 0.25f)
      {
        SpriteRenderer spr = waterEffect.GetComponent<SpriteRenderer>();
        Color newC = spr.color;
        newC.a = waterLevelPercent * 2f;
        spr.color = newC;
        //Debug.Log("new Water effect color: " + waterEffect.GetComponent<SpriteRenderer>().color);
      }
      else
      {
        Debug.Log("Moving the water effect");
        waterEffect.transform.position = Vector3.Lerp(minWaterEffectPos, maxWaterEffectPosition, waterLevelPercent);
      }

      if (waterLevelPercent >= 75.0f && !soundEffectOn)
      {
        BGM.GetComponent<AudioLowPassFilter>().enabled = true;
        soundEffectOn = true;
      }

      leakSpawnTimeCounter += Time.deltaTime;
      

      if (leakSpawnTimeCounter >= leakSpawnTime && waterLevelPercent < 100f)
      {
        float randomVal = Random.Range(0, 100);
        Vector3 spawnPos = Vector3.zero;
        float spawnX = 0.0f;
        float spawnY = 0.0f;
      
        if (randomVal < leakSpawnPercent)
        {
          int index = Random.Range(0, 2);
          
          
          GameObject newLeak = waterLeakPrefabs[index];
          switch (index)
          {
            //ceiling leak
            case 0:
               spawnX = Random.Range(-6.5f, 6.5f);
               spawnY = 5.5f;
               spawnPos = new Vector3(spawnX, spawnY, 0);
               newLeak.GetComponent<WaterLeak>().onCeiling = true;
              break;

            //left wall leak
            case 1:
              spawnX = Random.Range(-5.88f, -0.5f);
              spawnY = 0.4646f * spawnX + 4.228f;
              spawnY += Random.Range(0.1f, 0.3f);
              spawnPos = new Vector3(spawnX, spawnY, 0);
              newLeak.GetComponentInChildren<WaterLeak>().onCeiling = false;
            break;

            //right wall leak
            case 2:
              spawnX = 8.5f;
              spawnY = Random.Range(-3f, 3f);
              spawnPos = new Vector3(spawnX, spawnY, 0);
              newLeak.GetComponentInChildren<WaterLeak>().onCeiling = false;
            break;
          }



          audSource.clip = leakSpawn;
          audSource.Play();
          Instantiate(newLeak, spawnPos, Quaternion.identity);
          MakeItemBox();
          waterLeaks.Add(newLeak);
        }

        leakSpawnTimeCounter = 0.0f;
      }
      
        //decide whether to spawn a new leak or not


        //if we reach max water level we lose the game
        if (roomWaterLevel >= roomWaterLevelMAX)
        {
          //reload the scene
          loseScreen.SetActive(true);
        }
    }
}
