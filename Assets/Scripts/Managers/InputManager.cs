using UnityEngine;
using System;
using UnityEditor;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [Header(" Actions ")]
    public static Action<Item> itemClicked;
    public static Action<Powerup> powerupClicked;

    [Header("Settings")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private LayerMask powerupLayer;
    private Item currentItem;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {

    }


    void Update()
    {

        if (GameManager.instance.IsGame())
        {
            HandleControl();
        }
    }

    private void HandleControl()
    {
        if (Input.GetMouseButtonDown(0))
            HandleMouseDown();
        else if (Input.GetMouseButton(0))
            HandleDrag();
        else if (Input.GetMouseButtonUp(0))
            HandleMouseUp();
    }

    private void HandleMouseDown()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, powerupLayer);

        if (hit.collider == null)
            return;


        //if (hit.collider.TryGetComponent(out Powerup powerup))
        powerupClicked?.Invoke(hit.collider.GetComponent<Powerup>());



    }

    private void HandleDrag()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100);

        if (hit.collider == null)
        {
            if (currentItem != null)
                DeselectionCurrentItem();
            return;
        }

        if (hit.collider.transform.parent == null)
        {
            return;
        }
        if (!hit.collider.transform.parent.TryGetComponent(out Item item))
        {
            DeselectionCurrentItem();
            return;
        }
        Debug.Log("tıkladım " + hit.collider.name);

        DeselectionCurrentItem();

        currentItem = item;
        currentItem.Select(outlineMaterial);
    }

    private void DeselectionCurrentItem()
    {
        if (currentItem != null)
        {
            currentItem.Deselect();

            currentItem = null;
        }
    }

    private void HandleMouseUp()
    {
        if (currentItem == null)
            return;

        currentItem.Deselect();

        itemClicked?.Invoke(currentItem);
        currentItem = null;

    }
}
