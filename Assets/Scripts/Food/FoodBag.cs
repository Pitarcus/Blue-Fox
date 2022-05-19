using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FoodBag : MonoBehaviour
{
    public MeshRenderer caughtMesh;
    public FoodCaught foodCaught;
    private Collider foodCollider;

    public FoxHealth foxHealth;
    private FoxFood foxFood;

    public GameObject bagAnimation;

    private void Start()
    {
        foodCaught.foodTriggered.AddListener(StopAnimation);
        foodCollider = GetComponent<Collider>();
        foodCollider.enabled = false;

        foxHealth.playerDeath.AddListener(SpawnFoodBag);
        foxFood = foxHealth.gameObject.GetComponent<FoxFood>();

        bagAnimation.SetActive(false);
    }

    // Called when player dies
    private void SpawnFoodBag()
    {
        bagAnimation.SetActive(true);
        caughtMesh.enabled = false;

        transform.position = foxHealth.transform.position;//new Vector3(foxHealth.transform.position.x, 0, foxHealth.transform.position.z);
        if(foxHealth.transform.position.y < 0)
            transform.DOMoveY(0, 2f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(EnableCollider);
        else
            transform.DOMoveY(10, 2f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(EnableCollider);

        foodCaught.extraValue = foxFood.GetFoodAmount();
        foxFood.ResetFoodAmount();
    }
    private void EnableCollider()
    {
        foodCollider.enabled = true;
    }

    private void StopAnimation() 
    {
        bagAnimation.SetActive(false);
        caughtMesh.enabled = true;
        foodCollider.enabled = false;
        Invoke("DespawnFoodBag", 3f);
    }

    private void DespawnFoodBag()
    {
        transform.parent = null;
        caughtMesh.enabled = false;
        bagAnimation.SetActive(false);
        transform.localScale = Vector3.one;
    }
}
