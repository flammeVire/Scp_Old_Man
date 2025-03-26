using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject 
{
    public string Name;
    public GameObject FloorMesh;
    public GameObject HandMesh;
    public Sprite Sprite;
}
