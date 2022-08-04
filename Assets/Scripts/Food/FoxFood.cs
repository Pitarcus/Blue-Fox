using UnityEngine;
using UnityEngine.Events;

public class FoxFood : MonoBehaviour
{
    public static int foodAmount = 0;
    public UnityEvent<int> foodChanged;

    public FMODUnity.EventReference foodObtainedEvent;

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

        if(amount > 0)
            FMODUnity.RuntimeManager.PlayOneShot(foodObtainedEvent);
    }
}
