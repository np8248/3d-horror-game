using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates a standalone Unity scene for each room so two people can
/// work on different rooms without git conflicts.
/// Run: Tools → Create Room Scenes
/// </summary>
public static class RoomSceneBuilder
{
    const float H = 3.5f;   // room height — must match HouseGenerator
    const float T = 0.3f;   // wall thickness

    [MenuItem("Tools/Create Room Scenes")]
    public static void CreateRoomScenes()
    {
        // Remember the scene that's currently open so we can return to it
        string originalPath = SceneManager.GetActiveScene().path;

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        CreateScene("Kitchen",  "Assets/Scenes/Kitchen.unity",
                     1f,  8f,  1f, 7f, RoomBuilder.BuildKitchen);
        CreateScene("Hallway",  "Assets/Scenes/Hallway.unity",
                    -8f,  8f, -7f, 1f, RoomBuilder.BuildHallway);
        CreateScene("CageRoom", "Assets/Scenes/CageRoom.unity",
                    -8f,  1f,  1f, 7f, RoomBuilder.BuildCageRoom);

        AssetDatabase.Refresh();

        // Reopen the scene that was active before we started
        if (!string.IsNullOrEmpty(originalPath))
            EditorSceneManager.OpenScene(originalPath);

        EditorUtility.DisplayDialog(
            "Room Scenes Created",
            "Saved to Assets/Scenes/:\n  Kitchen.unity\n  Hallway.unity\n  CageRoom.unity",
            "OK");
    }

    // ── helpers ──────────────────────────────────────────────────────

    static void CreateScene(string roomName, string path,
                            float x0, float x1, float z0, float z1,
                            System.Action<Transform> buildRoom)
    {
        // NewSceneSetup.DefaultGameObjects adds a camera + directional light
        var scene = EditorSceneManager.NewScene(
            NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        float cx = (x0 + x1) / 2f;
        float cz = (z0 + z1) / 2f;
        float w  = x1 - x0;
        float d  = z1 - z0;

        // "House" root — same name / coords as the main scene so RoomBuilder works unchanged
        var house = new GameObject("House");

        // Minimal room shell: floor + ceiling + 4 solid walls
        // Uses the same world coords as HouseGenerator so furniture positions match
        Wall("Floor",   house.transform, cx, -0.1f,      cz, w + T, 0.2f, d + T);
        Wall("Ceiling", house.transform, cx,  H + 0.1f,  cz, w + T, 0.2f, d + T);
        Wall("Wall_N",  house.transform, cx,  H / 2f,    z1, w + T, H,    T);
        Wall("Wall_S",  house.transform, cx,  H / 2f,    z0, w + T, H,    T);
        Wall("Wall_E",  house.transform, x1,  H / 2f,    cz, T,     H,    d);
        Wall("Wall_W",  house.transform, x0,  H / 2f,    cz, T,     H,    d);

        // Room furniture
        buildRoom(house.transform);

        EditorSceneManager.SaveScene(scene, path);
    }

    static void Wall(string name, Transform parent,
                     float x, float y, float z, float sx, float sy, float sz)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(x, y, z);
        go.transform.localScale    = new Vector3(sx, sy, sz);
    }
}
