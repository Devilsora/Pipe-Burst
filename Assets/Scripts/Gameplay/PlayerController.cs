using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private Dictionary<FacingDirection, Vector2> facingDirections = new Dictionary<FacingDirection, Vector2>();
    public GameObject heldObject;
    
    public ItemType currentObjectType;

    public float playerSpeed;
    public float itemDetectionRange;
    
    public bool onLadder;
    public bool atWindow;
    public bool nearBox;


    public float heightIncrease; //saves the amount we rose up on a box
    public float jumpHeight;
    public float jumpHorizontal;


    public bool holdingObject;
    public float dragBucketSpeed;

    public FacingDirection currentDirection;

    private Rigidbody2D mRigidbody2D;

    public bool ceilingCollisionIgnored = false;

   
  
    // Start is called before the first frame update
    void Start()
    {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        dragBucketSpeed = 0.5f * playerSpeed;

        facingDirections.Add(FacingDirection.UP, Vector2.up);
        facingDirections.Add(FacingDirection.RIGHT, Vector2.right);
        facingDirections.Add(FacingDirection.DOWN, Vector2.down);
        facingDirections.Add(FacingDirection.LEFT, Vector2.left);
    }

    public void MovePlayer()
    {

    }

    // Update is called once per frame
    void Update()
    {

      if (Input.GetKey(KeyCode.D))
      {
        transform.position = new Vector3(Time.deltaTime * playerSpeed + transform.position.x, transform.position.y, transform.position.z);
        currentDirection = FacingDirection.RIGHT;
      }
      else if (Input.GetKey(KeyCode.A))
      {
        transform.position = new Vector3(-Time.deltaTime * playerSpeed + transform.position.x, transform.position.y, transform.position.z);
        currentDirection = FacingDirection.LEFT;
      }

      else if (Input.GetKey(KeyCode.W))
      {
        transform.position = new Vector3(transform.position.x, Time.deltaTime * playerSpeed + transform.position.y, transform.position.z);
        currentDirection = FacingDirection.UP;
      }
      else if (Input.GetKey(KeyCode.S))
      {
        transform.position = new Vector3(transform.position.x, -Time.deltaTime * playerSpeed + transform.position.y, transform.position.z);
        currentDirection = FacingDirection.DOWN;
      }

      if (Input.GetKeyDown(KeyCode.Space))
      {
        Debug.Log("Trying to jump");
        Vector2 startPos = (Vector2)transform.position + facingDirections[currentDirection] * 0.5f;
        RaycastHit2D itemCheck = Physics2D.Raycast(startPos, facingDirections[currentDirection], itemDetectionRange, LayerMask.GetMask("Default"));

        if (itemCheck.transform != null)
        {
          if (itemCheck.transform.gameObject != null)
          {
            if (itemCheck.transform.gameObject.GetComponent<Item>() != null && itemCheck.transform.gameObject.GetComponent<Item>().canLiftPlayer)
            {
              Debug.Log("Near a box");
              if (nearBox)
              {
                //already on a box so add onto the height
                heightIncrease += jumpHeight;
              }
              else
              {
                heightIncrease = jumpHeight;
              }

              Debug.Log("Height increase: " + heightIncrease);
              Debug.Log("Box height: " + itemCheck.transform.localScale.y);

              switch (currentDirection)
              {
                case FacingDirection.RIGHT:
                  transform.position = new Vector3(transform.position.x + jumpHorizontal, transform.position.y + heightIncrease, transform.position.z);
                break;

                case FacingDirection.LEFT:
                  transform.position = new Vector3(transform.position.x - jumpHorizontal, transform.position.y + heightIncrease, transform.position.z);
                break;

                case FacingDirection.UP:
                  transform.position = new Vector3(transform.position.x, transform.position.y + heightIncrease, transform.position.z);
                break;

                case FacingDirection.DOWN:
                  transform.position = new Vector3(transform.position.x, transform.position.y - heightIncrease, transform.position.z);
                break;
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
      }

      if (Input.GetMouseButtonDown(0))
      {
        if (holdingObject)
        {
          Debug.Log("releasing object");
          ReleaseObject();
        }
        else
        {
          Debug.Log("trying to see if object is in facing direction");
          //project raycast in facing direction if you aren't already holding an object, see if there's an item immediately next to you to interact with.
          //need mask that has everything but a player

          Vector2 startPos = (Vector2) transform.position + facingDirections[currentDirection] * 0.5f;
          RaycastHit2D itemCheck = Physics2D.Raycast(startPos, facingDirections[currentDirection], itemDetectionRange,
            LayerMask.GetMask("Default", "Ladder"));

          if (itemCheck.transform != null)
          {
            if (itemCheck.transform.gameObject != null)
            {
              if (itemCheck.transform.gameObject.GetComponent<Item>() != null &&
                  itemCheck.transform.gameObject.GetComponent<Item>().moveable)
              {
                HoldObject(itemCheck.transform.gameObject);
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

    public void ReleaseObject()
    {
      heldObject.transform.SetParent(null);
      holdingObject = false;
    }

    public void HoldObject(GameObject obj)
    {
      ItemType objType = obj.transform.GetComponent<Item>().itemType;
      currentObjectType = objType;
      obj.transform.SetParent(transform);
      heldObject = obj.transform.gameObject;
      holdingObject = true;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
      if (onLadder && other.gameObject.layer == LayerMask.NameToLayer("Ceiling"))
      {
        Physics2D.IgnoreCollision(other.collider, gameObject.GetComponent<BoxCollider2D>());
        ceilingCollisionIgnored = true;
      }
      else
      {
        Physics2D.IgnoreCollision(other.collider, gameObject.GetComponent<BoxCollider2D>(), false);
        ceilingCollisionIgnored = false;
        if (heldObject != null)
          heldObject.transform.position = heldObject.transform.position;
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
          }

          if (heldObject != null)
            heldObject.transform.position = heldObject.transform.position;
        }
      }

      if (other.gameObject.GetComponent<Item>())
      {
        if (other.gameObject.GetComponent<Item>().canLiftPlayer)
        {
          nearBox = true;
        }
      }
        
    }

    public void OnCollisionExit2D(Collision2D other)
    {
      if (other.gameObject.tag == "Window")
        atWindow = false;

      if(other.gameObject.GetComponent<Item>())
        if (other.gameObject.GetComponent<Item>().canLiftPlayer)
        {
          nearBox = false;
          Debug.Log("Off box");
          transform.position = new Vector3(transform.position.x, transform.position.y - heightIncrease, transform.position.z);
          heightIncrease = 0.0f;
        }
          
    }
    
    public void OnCollisionStay2D(Collision2D other)
    {
      if (onLadder && other.gameObject.layer == LayerMask.NameToLayer("Ceiling"))
      {
        Physics2D.IgnoreCollision(other.collider, gameObject.GetComponent<BoxCollider2D>());
        ceilingCollisionIgnored = true;
    }
      else
      {
        Physics2D.IgnoreCollision(other.collider, gameObject.GetComponent<BoxCollider2D>(), false);
        ceilingCollisionIgnored = false;
      if (heldObject != null)
          heldObject.transform.position = heldObject.transform.position;
      }
    }

  public void OnTriggerEnter2D(Collider2D other)
    {
      if (other.transform.gameObject.layer == LayerMask.NameToLayer("Ladder"))
      {
        onLadder = true;
      }

    }

    public void OnTriggerExit2D(Collider2D other)
    {
      if (other.transform.gameObject.layer == LayerMask.NameToLayer("Ladder"))
      {
        onLadder = false;
        
      }
        
    }


    /* controls for diagonal movement as well as regular
    if (Input.GetKey(KeyCode.D))
    {
      transform.position = new Vector3(Time.deltaTime * playerSpeed + transform.position.x, transform.position.y, transform.position.z);
    }
    else if (Input.GetKey(KeyCode.A))
    {
      transform.position = new Vector3(-Time.deltaTime * playerSpeed + transform.position.x, transform.position.y, transform.position.z);
    }
  
    if (Input.GetKey(KeyCode.W))
    {
      transform.position = new Vector3(transform.position.x, Time.deltaTime * playerSpeed + transform.position.y, transform.position.z);
    }
    else if (Input.GetKey(KeyCode.S))
    {
      transform.position = new Vector3(transform.position.x, -Time.deltaTime * playerSpeed + transform.position.y, transform.position.z);
    }
    */

}
