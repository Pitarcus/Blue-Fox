using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxFood : MonoBehaviour
{
    public int foodAmount = 0;

    public void IncreaseFoodAmount(int amount) 
    {
        foodAmount += amount;
    }
}
