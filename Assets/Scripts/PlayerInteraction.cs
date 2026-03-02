using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3.5f;

    private Camera   playerCamera;
    private Openable lookingAt;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Check every frame what the player is looking at
        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            lookingAt = hit.collider.GetComponentInParent<Openable>();
        else
            lookingAt = null;

        // Press E to open/close
        if (Keyboard.current.eKey.wasPressedThisFrame && lookingAt != null)
            lookingAt.Toggle();
    }

    void OnGUI()
    {
        if (lookingAt == null) return;

        // Simple centered label near the bottom of the crosshair
        var style = new GUIStyle(GUI.skin.label);
        style.fontSize  = 26;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        float w = 280f, h = 36f;
        GUI.Label(new Rect(Screen.width / 2f - w / 2f, Screen.height / 2f + 30f, w, h),
                  "[ E ]  Open / Close", style);
    }
}
