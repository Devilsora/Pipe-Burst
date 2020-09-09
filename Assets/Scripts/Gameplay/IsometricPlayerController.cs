using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum FacingDirection
{ 
  UP,
  UP_LEFT,
  LEFT,
  DOWN_LEFT,
  DOWN,
  DOWN_RIGHT,
  RIGHT,
  UP_RIGHT
}
public class IsometricPlayerController : MonoBehaviour
{
  public float speed;
  public float itemDetectionRange;
  IsometricCharacterRenderer isoRenderer;
  public GameObject itemHoldArea;
  private Rigidbody2D rb;
  private CircleCollider2D cc;
  
  public FacingDirection currDirection;
  private Dictionary<FacingDirection, Vector2> facingDirections = new Dictionary<FacingDirection, Vector2>();

  public GameObject heldObject;
  public ItemType currentObjectType;
  public bool holdingObject;
  public float dragBucketSpeed;

  public bool atWindow;
  public bool nearBox;
  public bool onBox;
  public bool ceilingCollisionIgnored;
  public bool onLadder;

  public GameObject boxStandingOn;

  public GameObject nearestWaterOpening;
  public bool nearWaterOpening;

  public GameObject floorCollider;
  public GameObject wallCollider;


  public AudioClip footsteps;
  public AudioClip tapeUse;
  public AudioClip ladderUse;
  public AudioClip boxStack;

  public AudioSource audSource;

  public float footstep_time = 0.1f;
  public float footstepCounter = 0.0f;

  public GameObject loseScreen;


  // Start is called before the first frame update
  void Awake()
    {
      isoRenderer = GetComponentInChildren<IsometricCharacterRenderer>();
      rb = GetComponent<Rigidbody2D>();
      
      cc = GetComponent<CircleCollider2D>();
      audSource = GetComponent<AudioSource>();
      audSource.volume = 0.4f;

      facingDirections.Add(FacingDirection.UP, Vector2.up);
      facingDirections.Add(FacingDirection.UP_LEFT, Vector2.up + Vector2.left);
      facingDirections.Add(FacingDirection.LEFT, Vector2.left);
      facingDirections.Add(FacingDirection.DOWN_LEFT, Vector2.down + Vector2.left);
      facingDirections.Add(FacingDirection.DOWN, Vector2.down);
      facingDirections.Add(FacingDirection.DOWN_RIGHT, Vector2.down + Vector2.right);
      facingDirections.Add(FacingDirection.RIGHT, Vector2.right);
      facingDirections.Add(FacingDirection.UP_RIGHT, Vector2.right + Vector2.up);

      floorCollider = GameObject.FindGameObjectWithTag("Floor");
      wallCollider = GameObject.FindGameObjectWithTag("WallCollider");

      Physics2D.IgnoreLayerCollision(floorCollider.layer, gameObject.layer);
  }


    public void FixedUpdate()
    {
      if (!loseScreen.activeInHierarchy)
      {
        float horizontalInput = Input.GetAxis("Horizontal") * speed;
        float verticalInput = Input.GetAxis("Vertical") * speed;

        if (holdingObject)
        {
          horizontalInput = Input.GetAxis("Horizontal") * dragBucketSpeed;
          verticalInput = Input.GetAxis("Vertical") * dragBucketSpeed;
        }

        if (!onLadder)
          rb.velocity = new Vector2(horizontalInput, verticalInput);
        else
          rb.velocity = new Vector2(0, verticalInput);

        if (rb.velocity != Vector2.zero)
        {
          footstepCounter += Time.deltaTime;
          if (footstep_time <= footstepCounter)
          {
            audSource.clip = footsteps;

            audSource.Play();
            footstepCounter = 0.0f;
          }

        }

        Vector2 direction = new Vector2(horizontalInput, verticalInput);
        FindObjectOfType<IsometricCharacterRenderer>().SetDirection(direction);

        currDirection = (FacingDirection)isoRenderer.lastDir;
      }
      
      

      //if (onBox)
      //{
      //  //raycast down and see if we're on the floor
      //  Vector2 startPos = (Vector2)transform.position;
      //  RaycastHit2D itemCheck = Physics2D.Raycast(startPos, Vector2.down, 0.5f,
      //    LayerMask.GetMask(LayerMask.LayerToName(floorCollider.layer), "Ladder"));
      //
      //  if (itemCheck.transform != null )
      //  {
      //    if (itemCheck.transform.gameObject == floorCollider)
      //    {
      //      onBox = false;
      //      transform.position = new Vector2(transform.position.x, transform.position.y - 1f);
      //      boxStandingOn.gameObject.GetComponent<BoxCollider2D>().enabled = true;
      //      boxStandingOn.transform.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Front";
      //      boxStandingOn = null;
      //    }
      //  }
      //}
      //
      //if (nearBox)
      //{
      //  //check if the player is actually trying to go on the box if their facing direction
      //  Vector2 startPos = (Vector2)transform.position + facingDirections[currDirection] * 0.5f;
      //  RaycastHit2D itemCheck = Physics2D.Raycast(startPos, facingDirections[currDirection], itemDetectionRange,
      //    LayerMask.GetMask("Default", "Ladder"));
      //
      //  if (itemCheck.transform != null)
      //  {
      //    if (itemCheck.transform.gameObject.GetComponent<Item>().itemType == ItemType.BOX)
      //    {
      //      Debug.Log("Trying to get onto box");
      //      boxStandingOn = itemCheck.transform.gameObject;
      //      itemCheck.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false;
      //      itemCheck.transform.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Items";
      //      transform.position = new Vector2(itemCheck.transform.position.x, itemCheck.transform.position.y + 1f);
      //      onBox = true;
      //    }
      //    
      //  }
      //}
    }

    // Update is called once per frame
    void Update()
    {
      if (!loseScreen.activeInHierarchy)
      {
      if (Input.GetMouseButtonDown(0))
      {
        Debug.Log("Current facing direction: " + facingDirections[currDirection] + "  " + currDirection);
        if (holdingObject)
        {
          Debug.Log("releasing object");

          if (nearWaterOpening && currentObjectType == ItemType.TAPE)
          {
            if (!nearestWaterOpening.transform.GetChild(0).gameObject.GetComponent<WaterLeak>().coveredUp)
            {
              nearestWaterOpening.transform.GetChild(0).gameObject.GetComponent<WaterLeak>().coveredTime = heldObject.GetComponent<Item>().sealTime;
              nearestWaterOpening.transform.GetChild(0).gameObject.GetComponent<WaterLeak>().coveredUp = true;

              audSource.clip = tapeUse;
              audSource.Play();

              nearestWaterOpening = null;
              heldObject = null;
              holdingObject = false;
            }
            
          }
          else if (currentObjectType == ItemType.BOX)
          {
            Vector2 startPos = (Vector2)isoRenderer.transform.position + facingDirections[currDirection] * itemDetectionRange;
            RaycastHit2D itemCheck = Physics2D.Raycast(startPos, facingDirections[currDirection], itemDetectionRange,
              LayerMask.GetMask("Default", "Ladder"));

            if (itemCheck.transform != null)
            {
              if (itemCheck.transform.gameObject.GetComponent<Item>().itemType == ItemType.BOX)
              {
                if (itemCheck.transform.gameObject.GetComponent<Item>().isStacked)
                {
                  //get the furthest child and take that
                  int numChildren = itemCheck.transform.childCount;
                  GameObject childToTake = itemCheck.transform.GetChild(numChildren - 1).gameObject;
                  HoldObject(childToTake);

                  if (itemCheck.transform.childCount <= 0)
                  {
                    itemCheck.transform.gameObject.GetComponent<Item>().isStacked = false;
                  }
                }
                else
                {
                  itemCheck.transform.gameObject.GetComponent<Item>().isStacked = true;
                  startPos = itemCheck.transform.position;
                  heldObject.transform.parent = itemCheck.transform;
                  heldObject.transform.position = startPos;
                  heldObject.transform.position = new Vector2(heldObject.transform.position.x, heldObject.transform.position.y + (0.75f * itemCheck.transform.childCount));
                  heldObject.GetComponent<SpriteRenderer>().sortingOrder = itemCheck.transform.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
                  heldObject.SetActive(true);
                  //Instantiate(heldObject, (Vector3)startPos, Quaternion.identity).SetActive(true);
                  heldObject = null;
                  holdingObject = false;
                }


              }
            }
            else
            {
              ReleaseObject();
            }
          }
          else
          {
            ReleaseObject();
          }


        }
        else
        {
          Debug.Log("trying to see if object is in facing direction");
          //project raycast in facing direction if you aren't already holding an object, see if there's an item immediately next to you to interact with.
          //need mask that has everything but a player

          Vector2 startPos = (Vector2)transform.position + facingDirections[currDirection] * 0.5f;
          Debug.Log("Starting position: " + startPos);
          Debug.Log("Facing vector: " + facingDirections[currDirection] + "  " + currDirection);

          RaycastHit2D itemCheck = Physics2D.Raycast(startPos, facingDirections[currDirection], itemDetectionRange,
            LayerMask.GetMask("Default", "Ladder"));

          Debug.DrawLine(startPos, facingDirections[currDirection] * itemDetectionRange, Color.magenta, 2);

          if (itemCheck.transform != null)
          {
            if (itemCheck.transform.gameObject != null)
            {

              Debug.Log("item raycast to: " + itemCheck.transform.gameObject.name);
              if (itemCheck.transform.gameObject.GetComponent<Item>() != null)
              {
                if(itemCheck.transform.gameObject.GetComponent<Item>().moveable)
                  HoldObject(itemCheck.transform.gameObject);
                else if (itemCheck.transform.gameObject.GetComponent<Item>().isOpenable)
                {
                  foreach (Item i in itemCheck.transform.gameObject.GetComponent<ItemBox>().itemsInBox)
                  {
                    //spawn the items around the box before destroying the box
                    Vector2 startingPos = itemCheck.transform.position;
                    Vector2 spawnPos = startingPos + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                    Instantiate(i.gameObject, spawnPos, Quaternion.identity);
                  }

                  audSource.clip = boxStack;
                  audSource.Play();
                  Destroy(itemCheck.transform.gameObject);
                }
              }
              else
              {
                Debug.Log("Object doens't have item component");
              }
            }
            else
            {
              Debug.Log("Gameobject came back null");
            }
          }
          else
          {
            Debug.Log("No raycast");
          }
        }
      }
    }

      
    }

    public void ReleaseObject()
    {
      //figure out spawn position
      Vector2 startPos = (Vector2)transform.position + facingDirections[currDirection] * (cc.radius + 0.01f)/2;
      RaycastHit2D itemCheck = Physics2D.Raycast(startPos, facingDirections[currDirection], itemDetectionRange, LayerMask.GetMask("Default", "Ladder"));

      Debug.DrawLine(startPos, facingDirections[currDirection] * itemDetectionRange, Color.magenta, 2);
      if (itemCheck.transform == null)
      {
        Instantiate(heldObject, (Vector3) startPos, Quaternion.identity).SetActive(true);
        heldObject = null;
        holdingObject = false;
      }
      else
      {
        Debug.Log("Cast onto object: " + itemCheck.transform.gameObject);
      }

      
    }

    public void HoldObject(GameObject obj)
    {
      if (heldObject == null)
      {
        ItemType objType = obj.transform.GetComponent<Item>().itemType;
        currentObjectType = objType;
        GameObject copyOfOriginal = Instantiate(obj);
        copyOfOriginal.SetActive(false);
        Destroy(obj);
        heldObject = copyOfOriginal;
        holdingObject = true;
      }
      
    }

  public FacingDirection VectorToDirection(Vector2 dir)
    {
      //get the normalized direction
      Vector2 normDir = dir.normalized;
      //calculate how many degrees one slice is
      float step = 360f / 8;
      //calculate how many degress half a slice is.
      //we need this to offset the pie, so that the North (UP) slice is aligned in the center
      float halfstep = step / 2;
      //get the angle from -180 to 180 of the direction vector relative to the Up vector.
      //this will return the angle between dir and North.
      float angle = Vector2.SignedAngle(Vector2.up, normDir);
      //add the halfslice offset
      angle += halfstep;
      //if angle is negative, then let's make it positive by adding 360 to wrap it around.
      if (angle < 0)
      {
        angle += 360;
      }
      //calculate the amount of steps required to reach this angle
      float stepCount = angle / step;
      //round it, and we have the answer!
      Debug.Log("Current facing direction: " + (FacingDirection)Mathf.FloorToInt(stepCount));
      return (FacingDirection)Mathf.FloorToInt(stepCount);
  }

  public void OnCollisionEnter2D(Collision2D other)
  {
    if (other != null)
    {
      Debug.Log("Other object: " + other.gameObject);
    }
    else
    {
      Debug.Log("Other collider not found");
    }

    if (onLadder && (other.gameObject.layer == LayerMask.NameToLayer("WorldCol") || other.gameObject.layer == LayerMask.NameToLayer("TilemapFloor")))
    {
      Debug.Log("Ignoring collision on enter");
      ceilingCollisionIgnored = true;
    }
    else
    {
      Debug.Log("On ladder: " + onLadder);
      Debug.Log("intersection layer: " + LayerMask.LayerToName(other.gameObject.layer));
      Debug.Log("Enabling collision on enter");
      ceilingCollisionIgnored = false;
      
    }

    if (other.gameObject.tag == "Window")
    {
      atWindow = true;
      if (holdingObject)
      {
        if (currentObjectType == ItemType.BUCKET)
        {
          Debug.Log("Emptying bucket");
          heldObject.GetComponent<Item>().currentWaterAmount = 0.0f;
          heldObject.GetComponent<Item>().fallenOver = false;
          heldObject.GetComponent<Item>().full = false;
        }

        if (heldObject != null)
          heldObject.transform.position = heldObject.transform.position;
      }
    }

    if (other.gameObject.GetComponent<Item>())
    {
      if (other.gameObject.GetComponent<Item>().canLiftPlayer && other.gameObject.GetComponent<Item>().isStacked == false)
      {
        nearBox = true;
      }
    }
  }

  //public void OnCollisionStay2D(Collision2D other)
  //{
  //  if (onLadder && other.gameObject.layer == LayerMask.NameToLayer("WorldCol") || other.gameObject.layer == LayerMask.NameToLayer("TilemapFloor"))
  //  {
  //    Debug.Log("Ignoring collision in stay");
  //    ceilingCollisionIgnored = true;
  //  }
  //  else
  //  {
  //    Debug.Log("On ladder: " + onLadder);
  //    Debug.Log("intersection layer: " + LayerMask.LayerToName(other.gameObject.layer));
  //    Debug.Log("Enabling collision in stay");
  //    ceilingCollisionIgnored = false;
  //    
  //  }
  //}

  public void OnCollisionExit2D(Collision2D other)
  {
    if (onLadder && other.gameObject.layer == LayerMask.NameToLayer("WorldCol") || other.gameObject.layer == LayerMask.NameToLayer("TilemapFloor"))
    {
      Debug.Log("Ignoring collision in stay");
      ceilingCollisionIgnored = true;
    }
    else
    {
      Debug.Log("On ladder: " + onLadder);
      Debug.Log("intersection layer: " + LayerMask.LayerToName(other.gameObject.layer));
      Debug.Log("Enabling collision in stay");
      ceilingCollisionIgnored = false;
    }

    if (other.gameObject.GetComponent<Item>())
    {
      if (other.gameObject.GetComponent<Item>().canLiftPlayer && other.gameObject.GetComponent<Item>().isStacked == false)
      {
        nearBox = false;
      }
    }
  }


  public void OnTriggerEnter2D(Collider2D other)
  {
    if (other.transform.gameObject.layer == LayerMask.NameToLayer("Ladder"))
    {
      onLadder = true;
      audSource.clip = ladderUse;
      audSource.Play();
      Debug.Log("Got on ladder");
      Debug.Log("Trigger entered ladder");
    }

    if (other.transform.gameObject.tag == "Window")
    {
      Debug.Log("Reached window");
      if (currentObjectType == ItemType.BUCKET)
      {
        Debug.Log("Emptying bucket");
        heldObject.GetComponent<Item>().currentWaterAmount = 0.0f;
      }
      else
      {
        Debug.Log("Not holding a bucket");
      }
    }
    
    if (other.transform.gameObject.tag == "WaterOpening")
    {
      Debug.Log("Near a water opening");
      nearWaterOpening = true;
      nearestWaterOpening = other.gameObject;
    }
    else
    {
      Debug.Log("Trigger entered: " + other.tag);
      Debug.Log("Trigger entered: " + LayerMask.LayerToName(other.gameObject.layer));
    }

  }

  public void OnTriggerExit2D(Collider2D other)
  {
    if (other.transform.gameObject.layer == LayerMask.NameToLayer("Ladder"))
    {
      onLadder = false;
      Debug.Log("Got off ladder");
    }
    else if (other.transform.gameObject.tag == "WaterOpening")
    {
      nearWaterOpening = false;
      nearestWaterOpening = null;

    }

  }


}
