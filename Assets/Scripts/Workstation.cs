using UnityEngine;

public class Workstation : MonoBehaviour
{
    [Header("Crafting UI")]
    [SerializeField] private GameObject craftingUI; // Odkaz na prázdný objekt Crafting (UI)
    [SerializeField] private PlayerCam playerCam; // Odkaz na skript kamery
    [SerializeField] private MonoBehaviour playerController; // Skript, který øídí pohyb hráèe
    [SerializeField] private Animator crosshair; // Odkaz na crosshair animátor

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f; // Maximální vzdálenost pro interakci
    [SerializeField] private KeyCode interactKey = KeyCode.E; // Klávesa pro interakci
    [SerializeField] private LayerMask interactMask; // Vrstva pro interakci

    private Camera playerCamera;
    private bool isCraftingActive = false;

    private void Start()
    {
        playerCamera = Camera.main; // Najde hlavní kameru
        if (craftingUI != null)
        {
            craftingUI.SetActive(false); // Skrýt UI na zaèátku
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

            // Ovládání kamery a kurzoru
            if (playerCam != null)
            {
                playerCam.LockMovement(isCraftingActive);
            }

            // Ovládání pohybu hráèe
            if (playerController != null)
            {
                playerController.enabled = !isCraftingActive;
            }

            // Ovládání crosshairu
            if (crosshair != null)
            {
                crosshair.SetBool("IsCrafting", isCraftingActive);
            }

            Debug.Log($"Crafting UI {(isCraftingActive ? "opened" : "closed")}");
        }
    }
}
