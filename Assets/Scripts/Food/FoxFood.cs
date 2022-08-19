using UnityEngine;
using UnityEngine.Events;

public class FoxFood : MonoBehaviour
{
    public static int foodAmount = 0;
    public UnityEvent<int> foodChanged;

    public FMODUnity.EventReference foodObtainedEvent;
    public FMODUnity.EventReference strawberryObtainedEvent;

    [HideInInspector]
    public int potentialStrawberries = 0;

    private void Start()
    {
        IncreaseFoodAmount(0);
    }

    public int GetFoodAmount() 
    {
        return foodAmount;
    }

    public void ResetFoodAmount()
    {
        foodAmount = 0;
        foodChanged.Invoke(foodAmount);
    }

    public void IncreaseFoodAmount(int amount) 
    {
        
        foodAmount += amount;
        foodChanged.Invoke(foodAmount);
        if (amount != 0)
        {
            if (amount < 5)
                FMODUnity.RuntimeManager.PlayOneShot(foodObtainedEvent);
            else // Change for strawberry sound
                FMODUnity.RuntimeManager.PlayOneShot(strawberryObtainedEvent);
        }
    }

    public void DecreaseFoodAmount()
    {
        foodAmount--;
        foodChanged.Invoke(foodAmount);
    }
}
