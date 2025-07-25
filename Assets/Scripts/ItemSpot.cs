using UnityEngine;

public class ItemSpot : MonoBehaviour
{
    [Header("Settings")] 
    private Item item;



   
    
    public void Populate(Item item)
    {
        this.item = item;
        item.transform.SetParent(transform);
        
        item.AssignSpot(this);
    }
    
    public void Clear()
    {
        item = null;
    }
    public bool IsEmpty()
    {
        return item == null;
    }
    
}
