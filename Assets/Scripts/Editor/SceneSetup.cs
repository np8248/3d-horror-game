using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Tools → Build Full Scene
/// One-click setup: generates the house, furnishes all rooms (with
/// interactive doors), and creates the Player if one doesn't exist.
/// Open SampleScene (or any scene), run this, then press Play.
/// </summary>
public static class SceneSetup
{
    [MenuItem("Tools/Build Full Scene")]
    public static void BuildFullScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        // 1 — House walls, floor, ceiling, yard
        HouseGenerator.GenerateHouse();

        // 2 — Kitchen, CageRoom, and Hallway furniture + Openable doors
        RoomBuilder.FurnishRooms();

        // 3 — Player (skipped if one already exists)
        EnsurePlayer();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Full scene built! Save (Ctrl+S / Cmd+S) then press Play.");
    }

    // ── Wires up the Player — creates it if missing, fixes it if incomplete ──
    static void EnsurePlayer()
    {
        // Find or create the root Player object
        var player = GameObject.Find("Player");
        if (player == null)
        {
            player = new GameObject("Player");
            Undo.RegisterCreatedObjectUndo(player, "Create Player");
            player.transform.position = new Vector3(0f, 1.5f, -5f);
            Debug.Log("Player created at the front door.");
        }
        else
        {
            Debug.Log("Player found — adding any missing components.");
        }

        // ── CharacterController ──────────────────────────────────────
        var cc = player.GetComponent<CharacterController>();
        if (cc == null) cc = Undo.AddComponent<CharacterController>(player);
        cc.height = 1.75f;
        cc.radius = 0.4f;
        cc.center = Vector3.zero;

        // ── Movement & interaction scripts ───────────────────────────
        if (player.GetComponent<FirstPersonController>() == null)
            Undo.AddComponent<FirstPersonController>(player);

        if (player.GetComponent<PlayerInteraction>() == null)
            Undo.AddComponent<PlayerInteraction>(player);

        // ── Camera child ─────────────────────────────────────────────
        // Reuse an existing Camera child if there is one
        var cam = player.GetComponentInChildren<Camera>();
        if (cam == null)
        {
            var camGO = new GameObject("PlayerCamera");
            Undo.RegisterCreatedObjectUndo(camGO, "Create PlayerCamera");
            camGO.transform.SetParent(player.transform, false);
            camGO.transform.localPosition = new Vector3(0f, 0.725f, 0f);

            cam               = camGO.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            cam.fieldOfView   = 75f;

            if (camGO.GetComponent<AudioListener>() == null)
                camGO.AddComponent<AudioListener>();
        }

        // Remove the default Main Camera so there's only one AudioListener
        var oldCam = GameObject.FindWithTag("MainCamera");
        if (oldCam != null && oldCam != cam.gameObject)
            Undo.DestroyObjectImmediate(oldCam);
    }
}
