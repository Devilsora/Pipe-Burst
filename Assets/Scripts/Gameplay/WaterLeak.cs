using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLeak : MonoBehaviour
{
    // Start is called before the first frame update

    private ParticleSystem ps;
    public WaterManager wm;
    
    public float waterSpeed;

    public bool onCeiling;
    public bool coveredUp;

    public float coveredTime;
    public float coveredTimeCounter;

    public AudioClip dripSound;
    public AudioSource audSource;

    public SpriteRenderer parentRenderer;

    public Sprite cracked;
    public Sprite tapedOver;

  void Start()
    {
      wm = FindObjectOfType<WaterManager>();
      ps = GetComponent<ParticleSystem>();
      var emit = ps.emission;
      var rateOverTime = emit.rateOverTime;
      rateOverTime.constant = waterSpeed;
      audSource = GetComponent<AudioSource>();
      
      if(parentRenderer != null)
        parentRenderer.sprite = cracked;

    }

    // Update is called once per frame
    void Update()
    {
      if (coveredUp)
      {
        var emit = ps.emission;
        emit.enabled = false;
        coveredTimeCounter += Time.deltaTime;

        if (parentRenderer != null)
        {
          parentRenderer.sprite = tapedOver;
          if (coveredTime > 10)
          {
            Color col = new Color(67/255f, 67/255f, 67/255f, 1f);
            parentRenderer.color = col;
          }
          else
          {
            Color col = new Color(1f,1f,1f, 1f);
            parentRenderer.color = col;
          }
        }
          

        //check if better tape was used on it to color the sprite?

        if (coveredTimeCounter >= coveredTime)
        {
          emit.enabled = true;
          coveredUp = false;
          coveredTime = 0f;

          if (parentRenderer != null)
            parentRenderer.sprite = cracked;
        }
      }
    }

    public void ToggleParticleDeath(bool newStatus)
    {
      var colModule = ps.collision;
      if (newStatus == true)
      {
        colModule.minKillSpeed = 10f;
      }
      else
      {
        colModule.minKillSpeed = 0f;
      }
    }

    public void OnParticleCollision(GameObject other)
    {
      if (other.tag == "Floor")
      {
        Debug.Log("Water hit floor");
        wm.roomWaterLevel += 0.01f;
        audSource.clip = dripSound;
        audSource.Play();
        ToggleParticleDeath(true);
      }
      else
      {
        //Debug.Log("Collided with: " + other.tag + "    " + other.name);
      }
    }
}
