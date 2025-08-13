using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Avatars
{
    public Sprite avatars;
    public bool isUnlock;
    public bool isHadPrice;
    public int price;
}

public class AvatarManager : MonoBehaviour
{
    [Header("Avatar Data")]
    [SerializeField] private Avatars[] avatars;
    [SerializeField] private GameObject avatarPrefab;
    [SerializeField] private Transform avatarParent;

    [Header("Avatar Display")]
    [SerializeField] private GameObject[] avatarGO; // Game objects that display the selected avatar

    [Header("Settings")]
    public int selectedAvatarIndex;

    // Save system keys
    private const string SELECTED_AVATAR_KEY = "SelectedAvatarIndex";
    private const string AVATAR_UNLOCK_PREFIX = "AvatarUnlocked_";

    private void Awake()
    {
        // Set avatar parent if not assigned
        if (avatarParent == null)
            avatarParent = transform;
    }

    void Start()
    {
        LoadAvatarData();
        CreateAvatars();
        ApplySelectedAvatar();
    }

    /// <summary>
    /// Loads saved avatar data from PlayerPrefs
    /// </summary>
    private void LoadAvatarData()
    {
        // Load selected avatar index
        selectedAvatarIndex = PlayerPrefs.GetInt(SELECTED_AVATAR_KEY, 0);

        // Ensure selected avatar index is valid
        if (selectedAvatarIndex >= avatars.Length)
            selectedAvatarIndex = 0;

        // Load unlock status for each avatar
        for (int i = 0; i < avatars.Length; i++)
        {
            // First avatar is always unlocked by default
            if (i == 0)
            {
                avatars[i].isUnlock = true;
            }
            else
            {
                avatars[i].isUnlock = PlayerPrefs.GetInt(AVATAR_UNLOCK_PREFIX + i, 0) == 1;
            }
        }

        Debug.Log($"Avatar data loaded. Selected avatar: {selectedAvatarIndex}");
    }

    /// <summary>
    /// Saves avatar data to PlayerPrefs
    /// </summary>
    private void SaveAvatarData()
    {
        // Save selected avatar index
        PlayerPrefs.SetInt(SELECTED_AVATAR_KEY, selectedAvatarIndex);

        // Save unlock status for each avatar
        for (int i = 0; i < avatars.Length; i++)
        {
            if (i > 0) // Skip first avatar as it's always unlocked
            {
                PlayerPrefs.SetInt(AVATAR_UNLOCK_PREFIX + i, avatars[i].isUnlock ? 1 : 0);
            }
        }

        PlayerPrefs.Save();
        Debug.Log($"Avatar data saved. Selected avatar: {selectedAvatarIndex}");
    }

    /// <summary>
    /// Creates avatar selection UI elements
    /// </summary>
    public void CreateAvatars()
    {
        // Clear existing avatars if any
        foreach (Transform child in avatarParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < avatars.Length; i++)
        {
            GameObject avatarGameObject = Instantiate(avatarPrefab, avatarParent);

            // Get components
            var avatarSprite = avatarGameObject.transform.Find("AvatarSelectionButon")?.GetComponent<Image>();
            var avatarButton = avatarGameObject.transform.Find("AvatarSelectionButon")?.GetComponent<Button>();
            var avatarTransparentBackground = avatarGameObject.transform.Find("AvatarBackground")?.gameObject;
            var avatarPrice = avatarGameObject.transform.Find("AvatarPrice")?.GetComponent<TextMeshProUGUI>();

            // Set avatar sprite
            if (avatarSprite != null && avatars[i].avatars != null)
            {
                avatarSprite.sprite = avatars[i].avatars;
            }

            // Configure based on unlock status
            if (!avatars[i].isUnlock)
            {
                // Locked avatar
                if (avatarTransparentBackground != null)
                {
                    avatarTransparentBackground.SetActive(true);
                    avatarPrice.gameObject.SetActive(true);
                }


                if (avatarPrice != null)
                    avatarPrice.text = avatars[i].price.ToString();

                if (avatarButton != null)
                {
                    int avatarIndex = i; // Capture the index for the lambda
                    avatarButton.onClick.AddListener(() => BuyAvatar(avatarIndex));
                }
            }
            else
            {
                // Unlocked avatar
                if (avatarTransparentBackground != null)
                {
                    avatarTransparentBackground.SetActive(false);
                    avatarPrice.gameObject.SetActive(false);
                }

                if (avatarPrice != null)
                {
                    avatarPrice.gameObject.SetActive(false);
                    avatarPrice.gameObject.SetActive(false);
                }

                if (avatarButton != null)
                {
                    int avatarIndex = i; // Capture the index for the lambda
                    avatarButton.onClick.AddListener(() => SelectAvatar(avatarIndex));
                }
            }
        }
    }

    /// <summary>
    /// Buys an avatar with money
    /// </summary>
    public void BuyAvatar(int avatarIndex)
    {
        if (avatarIndex < 0 || avatarIndex >= avatars.Length)
            return;

        if (avatars[avatarIndex].isUnlock)
            return;

        // Check if player has enough money
        if (MoneyManager.instance.money < avatars[avatarIndex].price)
        {
            SectionAndLevelUI.Instance.WarningMesageUI("money");
            Debug.Log("Not enough money to buy this avatar!");
            return;
        }

        // Buy the avatar
        MoneyManager.instance.ReduceMoney(avatars[avatarIndex].price);
        avatars[avatarIndex].isUnlock = true;

        // Save the data
        SaveAvatarData();

        // Select the newly bought avatar
        SelectAvatar(avatarIndex);

        // Refresh the UI
        CreateAvatars();

        Debug.Log($"Avatar {avatarIndex} purchased successfully!");
    }

    /// <summary>
    /// Selects an avatar
    /// </summary>
    public void SelectAvatar(int avatarIndex)
    {
        if (avatarIndex < 0 || avatarIndex >= avatars.Length)
            return;

        if (!avatars[avatarIndex].isUnlock)
        {
            Debug.Log("Cannot select locked avatar!");
            return;
        }

        selectedAvatarIndex = avatarIndex;
        SaveAvatarData();
        ApplySelectedAvatar();

        Debug.Log($"Avatar {avatarIndex} selected!");
    }

    /// <summary>
    /// Applies the selected avatar to all avatar display objects
    /// </summary>
    private void ApplySelectedAvatar()
    {
        if (selectedAvatarIndex < 0 || selectedAvatarIndex >= avatars.Length)
            return;

        Sprite selectedSprite = avatars[selectedAvatarIndex].avatars;

        foreach (var avatarGO in this.avatarGO)
        {
            if (avatarGO != null)
            {
                // Try to get Image component first
                Image imageComponent = avatarGO.GetComponent<Image>();
                if (imageComponent != null)
                {
                    imageComponent.sprite = selectedSprite;
                }
                else
                {
                    // Try to get SpriteRenderer if it's a 3D object
                    SpriteRenderer spriteRenderer = avatarGO.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sprite = selectedSprite;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the currently selected avatar sprite
    /// </summary>
    public Sprite GetSelectedAvatarSprite()
    {
        if (selectedAvatarIndex >= 0 && selectedAvatarIndex < avatars.Length)
            return avatars[selectedAvatarIndex].avatars;
        return null;
    }

    /// <summary>
    /// Gets the unlock status of an avatar
    /// </summary>
    public bool IsAvatarUnlocked(int avatarIndex)
    {
        if (avatarIndex >= 0 && avatarIndex < avatars.Length)
            return avatars[avatarIndex].isUnlock;
        return false;
    }

    /// <summary>
    /// Resets all avatar data (for testing purposes)
    /// </summary>
    [ContextMenu("Reset Avatar Data")]
    public void ResetAvatarData()
    {
        // Reset unlock status (keep first avatar unlocked)
        for (int i = 1; i < avatars.Length; i++)
        {
            avatars[i].isUnlock = false;
        }

        // Reset selected avatar
        selectedAvatarIndex = 0;

        // Save the reset data
        SaveAvatarData();

        // Refresh UI
        CreateAvatars();
        ApplySelectedAvatar();

        Debug.Log("Avatar data reset!");
    }
}
