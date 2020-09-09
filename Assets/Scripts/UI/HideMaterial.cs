using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideMaterial : MonoBehaviour
{
  private TilemapRenderer tilemapRenderer;

    // Start is called before the first frame update
    void Awake()
    {
      tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    // Update is called once per frame
    void Start()
    {
      tilemapRenderer.enabled = false;
    }
}
