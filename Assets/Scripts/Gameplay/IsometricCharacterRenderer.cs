using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCharacterRenderer : MonoBehaviour
{
  public static readonly string[] staticDirs =
    {"Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE"};

  public static readonly string[] runDirs =
    {"Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE"};

  private Animator animator;
  public int lastDir;

    // Start is called before the first frame update
    void Awake()
    {
      animator = GetComponent<Animator>();
    }

    public void Update()
    {
      
    }

    public void SetDirection(Vector2 dir)
    {
      string[] directionArr = null;

      if (dir.magnitude < 0.01f)
      {
        //standing still, use static
        directionArr = staticDirs;
      }
      else
      {
        directionArr = runDirs;
        lastDir = DirectionToIndex(dir);
      }

      animator.Play(directionArr[lastDir]);
    }

  public static int DirectionToIndex(Vector2 dir)
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
      return Mathf.FloorToInt(stepCount);
    }

  




}
