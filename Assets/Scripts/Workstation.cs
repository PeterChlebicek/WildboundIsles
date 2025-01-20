using UnityEngine;

public class Workstation : MonoBehaviour
{
    [Header("Crafting UI")]
    [SerializeField] private GameObject craftingUI; // Odkaz na pr�zdn� objekt Crafting (UI)
    [SerializeField] private PlayerCam playerCam; // Odkaz na skript kamery
    [SerializeField] private MonoBehaviour playerController; // Skript, kter� ��d� pohyb hr��e
    [SerializeField] private Animator crosshair; // Odkaz na crosshair anim�tor

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f; // Maxim�ln� vzd�lenost pro interakci
    [SerializeField] private KeyCode interactKey = KeyCode.E; // Kl�vesa pro interakci
    [SerializeField] private LayerMask interactMask; // Vrstva pro interakci

    private Camera playerCamera;
    private bool isCraftingActive = false;

    private void Start()
    {
        playerCamera = Camera.main; // Najde hlavn� kameru
        if (craftingUI != null)
        {
            craftingUI.SetActive(false); // Skr�t UI na za��tku
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactMask))
        {
            if (hit.collider.gameObject == gameObject)
            {
                ToggleCraftingUI();
            }
        }
    }

    private void ToggleCraftingUI()
    {
        isCraftingActive = !isCraftingActive;
        if (craftingUI != null)
        {
            craftingUI.SetActive(isCraftingActive);

            // Ovl�d�n� kamery a kurzoru
            if (playerCam != null)
            {
                playerCam.LockMovement(isCraftingActive);
            }

            // Ovl�d�n� pohybu hr��e
            if (playerController != null)
            {
                playerController.enabled = !isCraftingActive;
            }

            // Ovl�d�n� crosshairu
            if (crosshair != null)
            {
                crosshair.SetBool("IsCrafting", isCraftingActive);
            }

            Debug.Log($"Crafting UI {(isCraftingActive ? "opened" : "closed")}");
        }
    }
}
