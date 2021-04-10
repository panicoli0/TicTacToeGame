using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyScript : MonoBehaviour
{
    public int id;
    public GameObject camera;

    private void Start()
    {
       //camera = FindObjectOfType<GameScript>().gameObject;
    }

    private void OnMouseDown()
    {
        camera.GetComponent<GameScript>().Spawn(this.gameObject, id);
    }
}
