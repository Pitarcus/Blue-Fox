using DG.Tweening;
using UnityEngine;

public class ChangeBridgeTransparency : MonoBehaviour
{
    public Material material;

    public MeshRenderer originalMeshRenderer;
    public GameObject alphaMeshGameObject;


    private void Start()
    {
        ResetMeshes();
        material.SetFloat("_Opacity", 1);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            originalMeshRenderer.enabled = false;
            alphaMeshGameObject.SetActive(true);

            DOVirtual.Float(1, 0.6f, 0.5f, ChangeOpacity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DOVirtual.Float(0.6f, 1f, 0.5f, ChangeOpacity).OnComplete(ResetMeshes);
        }
    }

    void ResetMeshes()
    {
        originalMeshRenderer.enabled = true;
        alphaMeshGameObject.SetActive(false);
    }

    void ChangeOpacity(float x)
    {
        material.SetFloat("_Opacity", x);
    }
}
