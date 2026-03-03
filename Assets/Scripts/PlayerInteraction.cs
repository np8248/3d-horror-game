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
        if (playerCamera == null)
            Debug.LogError("PlayerInteraction: no Camera found as a child of " + gameObject.name);
    }

    void Update()
    {
        if (Keyboard.current == null || playerCamera == null) return;

        // Scan ALL hits along the ray so decorative geometry doesn't block doors
        var ray  = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        var hits = Physics.RaycastAll(ray, interactRange);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        lookingAt = null;
        foreach (var h in hits)
        {
            var o = h.collider.GetComponentInParent<Openable>();
            if (o != null) { lookingAt = o; break; }
        }

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
