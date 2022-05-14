using UnityEngine;

public class SphereTracker : MonoBehaviour
{
    public Transform sphere;
    public new Renderer renderer;
    public float sphereRadius;

    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        propBlock.SetVector("_SpherePos", sphere.position);
        propBlock.SetFloat("_SphereRadius", sphereRadius);
        renderer.SetPropertyBlock(propBlock);
    }
}
