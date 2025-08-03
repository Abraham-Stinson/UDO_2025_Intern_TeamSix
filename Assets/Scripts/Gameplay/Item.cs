using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EItemName itemName;
    public EItemName ItemName => itemName; //this will return the item name
    
    
    [Header("Rotation Settings")]
    [SerializeField] private bool applyCustomRotationOnPlace;
    [SerializeField] private Vector3 customRotationEuler;
    public bool ApplyCustomRotationOnPlace => applyCustomRotationOnPlace;
    public Vector3 CustomRotationEuler => customRotationEuler;
    
    [Header("Placement Offset")]
    [SerializeField] private Vector3 localOffsetOnSpot;
    public Vector3 LocalOffsetOnSpot => localOffsetOnSpot;
 
    
    private ItemSpot spot;
    public ItemSpot Spot => spot;

    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;

    // Flag to track if item is selected/placed in a spot
    private bool isSelected = false;
    public bool IsSelected => isSelected;

    public EItemName GetItemName()
    {
        return itemName;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
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
    
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            foreach (EItemName name in System.Enum.GetValues(typeof(EItemName)))
            {
                if (gameObject.name.ToLower().Contains(name.ToString().ToLower()))
                {
                    itemName = name;
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,0.2f);
        
        //objenin pivot noktasını görmek ve ona göre konumlandırmak için ekledim
    }
}
