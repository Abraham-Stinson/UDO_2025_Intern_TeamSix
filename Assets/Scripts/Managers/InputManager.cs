using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) HandleMouseDown();
        else if (Input.GetMouseButton(0)) HandleDrag();
        else if (Input.GetMouseButtonUp(0)) HandleMouseUp();
#else
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
            HandleTouchDown(touch.position);
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            HandleTouchDrag(touch.position);
        else if (touch.phase == TouchPhase.Ended)
            HandleTouchUp();
    }
#endif
    }

    #region Mobile Input

    private void HandleTouchDown(Vector2 touchPosition)
    {
        
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 400, powerupLayer))
        {
            powerupClicked?.Invoke(hit.collider.GetComponent<Powerup>());
            return; 
        }
        if (Physics.Raycast(ray, out hit, 400))
        {
            if (hit.collider.transform.parent != null &&
                hit.collider.transform.parent.TryGetComponent(out Item item))
            {
                DeselectionCurrentItem();
                currentItem = item;
                currentItem.Select(outlineMaterial);


                StartCoroutine(SelectAndClickItem(currentItem));
                // itemClicked?.Invoke(currentItem);
                // currentItem = null;
            }
        }
    }
    
    private IEnumerator SelectAndClickItem(Item item)
    {
        yield return null;
        item.Deselect();
        itemClicked?.Invoke(item);
        currentItem = null;
    }

    private void HandleTouchDrag(Vector2 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 400))
        {
            if (hit.collider.transform.parent == null)
                return;

            if (hit.collider.transform.parent.TryGetComponent(out Item item))
            {
                DeselectionCurrentItem();
                currentItem = item;
                currentItem.Select(outlineMaterial);
            }
            else
            {
                DeselectionCurrentItem();
            }
        }
        else
        {
            DeselectionCurrentItem();
        }
    }

    private void HandleTouchUp()
    {
        if (currentItem == null)
            return;

        currentItem.Deselect();
        itemClicked?.Invoke(currentItem);
        currentItem = null;
    }

    #endregion
   
    
    private void HandleMouseDown()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 400, powerupLayer);

        if (hit.collider == null)
            return;


        //if (hit.collider.TryGetComponent(out Powerup powerup))
        powerupClicked?.Invoke(hit.collider.GetComponent<Powerup>());



    }

    private void HandleDrag()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 400);

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
