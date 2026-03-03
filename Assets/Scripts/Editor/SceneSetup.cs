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

    // ── Creates a fully-wired Player if the scene doesn't already have one ──
    static void EnsurePlayer()
    {
        if (GameObject.Find("Player") != null)
        {
            Debug.Log("Player already exists — skipping creation.");
            return;
        }

        // ── Player root ──────────────────────────────────────────────
        var player = new GameObject("Player");
        Undo.RegisterCreatedObjectUndo(player, "Create Player");

        // Just inside the front door (door at Z = -7, facing north)
        player.transform.position = new Vector3(0f, 1.5f, -5f);

        var cc    = player.AddComponent<CharacterController>();
        cc.height = 1.75f;
        cc.radius = 0.4f;
        cc.center = Vector3.zero;   // FirstPersonController adjusts this every frame

        player.AddComponent<FirstPersonController>();
        player.AddComponent<PlayerInteraction>();

        // ── Camera child ─────────────────────────────────────────────
        var camGO = new GameObject("PlayerCamera");
        Undo.RegisterCreatedObjectUndo(camGO, "Create PlayerCamera");
        camGO.transform.SetParent(player.transform, false);
        camGO.transform.localPosition = new Vector3(0f, 0.725f, 0f);   // eye height

        var cam           = camGO.AddComponent<Camera>();
        cam.nearClipPlane = 0.05f;
        cam.fieldOfView   = 75f;
        camGO.AddComponent<AudioListener>();

        // Remove the default Main Camera so there's only one AudioListener
        var oldCam = GameObject.FindWithTag("MainCamera");
        if (oldCam != null && oldCam != camGO)
            Undo.DestroyObjectImmediate(oldCam);

        Debug.Log("Player created at the front door.");
    }
}
