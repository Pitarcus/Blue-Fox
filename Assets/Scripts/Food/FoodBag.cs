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
    private Vector3 newPosition;

    private void Start()
    {
        foodCaught.foodTriggered.AddListener(StopAnimation);
        foodCollider = GetComponent<Collider>();
        foodCollider.enabled = false;

        foxHealth.playerDeath.AddListener(SpawnFoodBag);
        foxHealth.playerRespawned.AddListener(ShowFoodBag);

        foxFood = foxHealth.gameObject.GetComponent<FoxFood>();

        bagAnimation.SetActive(false);
    }

    private void OnDisable()
    {
        foxHealth.playerDeath.RemoveListener(SpawnFoodBag);
        foxHealth.playerRespawned.RemoveListener(ShowFoodBag);
    }

    private void ShowFoodBag()
    {
        bagAnimation.SetActive(true);
        caughtMesh.enabled = false;

        transform.localScale = Vector3.one;
        transform.position = newPosition;
    }
    // Called when player dies.
    // If player has more than 0 food, the bag should spawn. If a bag is already alive it should disappear.
    private void SpawnFoodBag()
    {
        if (foxFood.GetFoodAmount() > 0)
        {

            bagAnimation.SetActive(true);
            caughtMesh.enabled = false;

            Vector3 lastGroundPosition = foxHealth.lastGroundPosition;
            Vector3 deathPosition = foxHealth.transform.position;
            Vector3 intermediatePoint = Vector3.Lerp(lastGroundPosition, deathPosition, 0.5f);

            transform.localScale = Vector3.one;
            transform.position = foxHealth.transform.position;//new Vector3(foxHealth.transform.position.x, 0, foxHealth.transform.position.z);
            if (foxHealth.transform.position.y < 0)
            {
                newPosition = new Vector3(intermediatePoint.x, lastGroundPosition.y + 4f, intermediatePoint.z);
                transform.DOMoveY(foxHealth.lastGroundPosition.y + 4f, 2f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(EnableCollider);
            }
            else
            {
                newPosition = new Vector3(intermediatePoint.x, 10, intermediatePoint.z);
                transform.DOMoveY(10, 2f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(EnableCollider);
            }

            foodCaught.extraValue = foxFood.GetFoodAmount();
            foxFood.ResetFoodAmount();
        }
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
        StartCoroutine("DespawnFoodBag");
    }

    private IEnumerator DespawnFoodBag()
    {
        yield return new WaitForSecondsRealtime(3f);
        transform.parent = null;
        caughtMesh.enabled = false;
        bagAnimation.SetActive(false);
        transform.localScale = Vector3.one;
    }
}
