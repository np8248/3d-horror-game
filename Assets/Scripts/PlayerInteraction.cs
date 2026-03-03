using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3.5f;

    private Camera     playerCamera;
    private Openable   lookingAt;
    private Openable[] allOpenables;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
            Debug.LogError("PlayerInteraction: no Camera found as a child of " + gameObject.name);

        // Cache every door/cabinet in the scene at startup
        allOpenables = Object.FindObjectsOfType<Openable>();
        if (allOpenables.Length == 0)
            Debug.LogWarning("PlayerInteraction: no Openable objects found — " +
                             "run Tools > Generate House then Tools > Furnish Rooms.");
    }

    void Update()
    {
        if (Keyboard.current == null || playerCamera == null) return;

        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        // --- Pass 1: precise raycast (works when nothing blocks the line of sight) ---
        lookingAt = null;
        var hits = Physics.RaycastAll(ray, interactRange);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (var h in hits)
        {
            var o = h.collider.GetComponentInParent<Openable>();
            if (o != null) { lookingAt = o; break; }
        }

        // --- Pass 2: dot-product fallback (works even when geometry blocks the ray) ---
        if (lookingAt == null)
        {
            float bestDot = 0.7f;   // cos ~45 ° — must be roughly looking at the door
            foreach (var o in allOpenables)
            {
                if (o == null) continue;
                Vector3 toObj = o.transform.position - ray.origin;
                if (toObj.magnitude > interactRange) continue;
                float dot = Vector3.Dot(ray.direction, toObj.normalized);
                if (dot > bestDot) { bestDot = dot; lookingAt = o; }
            }
        }

        if (Keyboard.current.eKey.wasPressedThisFrame && lookingAt != null)
            lookingAt.Toggle();
    }

    void OnGUI()
    {
        if (lookingAt == null) return;

        var style = new GUIStyle(GUI.skin.label);
        style.fontSize  = 26;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        float w = 280f, h = 36f;
        GUI.Label(new Rect(Screen.width / 2f - w / 2f, Screen.height / 2f + 30f, w, h),
                  "Press E to open", style);
    }
}
