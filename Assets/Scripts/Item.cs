using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EItemName itemName;
    public EItemName ItemName => itemName; //this will return the item name

    private ItemSpot spot;
    public ItemSpot Spot => spot;

    public EItemName GetItemName()
    {
        return itemName;
    }

    
    [Header("Elements")] 
    [SerializeField] private Renderer renderer;
    [SerializeField] private Collider collider;

    private Material baseMaterial;

    private void Awake()
    {
        baseMaterial = renderer.material;
    }

    public void AssignSpot(ItemSpot spot)
        => this.spot = spot;
    public void DisableShadows()
    {
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void DisablePhysics()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        collider.enabled = false;
    }


    public void Select(Material outlineMaterial)
    {
        renderer.materials = new Material[2] {baseMaterial,outlineMaterial};
    }

    public void Deselect()
    {
        renderer.materials = new Material[] { baseMaterial };
    }
}
