using UnityEngine.Events;
using UnityEngine;
using DG.Tweening;

public class FoodCaught : MonoBehaviour
{
    [Header("Assign in Editor")]
    public Mesh newMesh;
    private FoxFood foxFood;

    [Header("Parameters")]
    public bool changesMesh;
    //public bool hasAnimation;
    public bool unlocked = false;
    public bool destroyAfter = true;
    public bool instaGet = true;
    public int extraValue = 0;
    public float objectScale = 1f;


    public UnityEvent foodTriggered;
    private MeshFilter mesh;
    private Transform fox;
    private Collider col;

    private void OnEnable()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foxFood = player.GetComponent<FoxFood>();

        if (unlocked)
            Destroy(this.gameObject);

        mesh = GetComponent<MeshFilter>();
        col = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && instaGet)
        {
            FoodCaughtFunction(other.transform);
        }
    }

    public void FoodCaughtFunction( Transform foxTransform)
    {
        foodTriggered.Invoke();

        foxFood.IncreaseFoodAmount(extraValue);

        // Animate
        transform.DOScale(Vector3.zero, 0.2f).OnComplete(ShowInHead);
        fox = foxTransform;
        col.enabled = false;
    }

    public void StartAnimation(Transform foxTransform) 
    {
        transform.DOScale(Vector3.zero, 0.2f).OnComplete(ShowInHead);
        fox = foxTransform;
    }
    void ShowInHead() 
    {
        if (changesMesh)
            mesh.mesh = newMesh;

        transform.parent = fox;
        transform.localPosition = new Vector3(0, 18, 4);
        transform.DOScale(Vector3.one * objectScale * 1.2f,  0.2f).SetEase(Ease.InCubic);
        Invoke("AnimateOut", 1.5f);
    }

    void AnimateOut() 
    {
        if(destroyAfter)
            Destroy(gameObject, 1f);
        transform.DOLocalMoveY(25, 0.6f).SetEase(Ease.InBounce);
        transform.DOScale(0,0.6f).SetEase(Ease.InCubic);
    }

}
