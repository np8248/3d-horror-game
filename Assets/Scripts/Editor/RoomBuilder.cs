using UnityEngine;
using UnityEditor;

public static class RoomBuilder
{
    // These match the values in HouseGenerator
    // House: X = -8 to 8, Z = -7 to 7
    // Interior divider (horizontal) at Z = 1
    // Interior divider (vertical)   at X = 1  (back half only)
    //
    // Rooms:
    //   Front hallway  : X -8..8,  Z -7..1
    //   Cage room      : X -8..1,  Z  1..7
    //   Kitchen        : X  1..8,  Z  1..7  (7 units wide)

    [MenuItem("Tools/Furnish Rooms")]
    public static void FurnishRooms()
    {
        var house = GameObject.Find("House");
        if (house == null)
        {
            Debug.LogError("House not found! Run Tools > Generate House first.");
            return;
        }

        // Remove old room furniture if regenerating
        DestroyChild(house, "Kitchen");
        DestroyChild(house, "CageRoom");
        DestroyChild(house, "Hallway");

        BuildKitchen(house.transform);
        BuildCageRoom(house.transform);
        BuildHallway(house.transform);

        Debug.Log("Rooms built!");
    }

    // ── KITCHEN (X: 1..8, Z: 1..7) — 7 units wide ───────────────────
    public static void BuildKitchen(Transform house)
    {
        var root = Child("Kitchen", house);

        // ── Counters (L-shape: north wall + east wall) ──
        Box("Counter_N",     root, 4.5f,  0.45f, 6.55f, 6.5f, 0.9f,  0.55f);
        Box("Counter_N_Top", root, 4.5f,  0.92f, 6.42f, 6.5f, 0.06f, 0.8f);

        Box("Counter_E",     root, 7.62f, 0.45f, 3.8f,  0.55f, 0.9f,  4.8f);
        Box("Counter_E_Top", root, 7.48f, 0.92f, 3.8f,  0.8f,  0.06f, 4.8f);

        // Sink basin on north counter (slightly inset)
        Box("Sink_Basin", root, 3.5f, 0.90f, 6.48f, 0.7f, 0.05f, 0.4f);
        Box("Sink_Faucet",root, 3.5f, 1.05f, 6.6f,  0.05f, 0.15f, 0.05f);

        // ── Upper wall cabinets ──
        // North wall cabinets (above counter, stop before fridge corner)
        Box("Cab_N_1", root, 2.2f, 1.85f, 6.8f, 1.8f, 0.7f, 0.35f);
        Box("Cab_N_2", root, 4.2f, 1.85f, 6.8f, 1.8f, 0.7f, 0.35f);
        Box("Cab_N_3", root, 6.0f, 1.85f, 6.8f, 1.4f, 0.7f, 0.35f);

        // East wall cabinets (above east counter)
        Box("Cab_E_1", root, 7.82f, 1.85f, 5.5f, 0.35f, 0.7f, 1.6f);
        Box("Cab_E_2", root, 7.82f, 1.85f, 3.5f, 0.35f, 0.7f, 1.6f);

        // ── Cabinet doors (pivot-based so they swing open) ──
        // North wall: double-leaf doors — left leaf +90°, right leaf -90°, both open together
        DoubleCabDoor("CabDoor_N1", root, 1.35f, 1.85f, 6.63f, 1.7f, 0.65f, 0.04f);
        DoubleCabDoor("CabDoor_N2", root, 3.35f, 1.85f, 6.63f, 1.7f, 0.65f, 0.04f);
        DoubleCabDoor("CabDoor_N3", root, 5.35f, 1.85f, 6.63f, 1.3f, 0.65f, 0.04f);
        // East wall: hinge on north edge, swings toward kitchen (90)
        SwingDoor("CabDoor_E1", root, new Vector3(7.65f, 1.85f, 6.25f), new Vector3(0f, 0f, -0.75f), new Vector3(0.04f, 0.65f, 1.5f),   90f);
        SwingDoor("CabDoor_E2", root, new Vector3(7.65f, 1.85f, 4.25f), new Vector3(0f, 0f, -0.75f), new Vector3(0.04f, 0.65f, 1.5f),   90f);

        // ── Fridge (north-east corner) ──
        Box("Fridge_Body",   root, 7.25f, 1.0f,  1.9f,  0.85f, 2.0f, 0.75f);
        Box("Fridge_Handle", root, 6.80f, 1.3f,  1.7f,  0.02f, 0.25f, 0.04f);
        // Fridge doors: hinge on south edge, swing out toward kitchen (-90)
        SwingDoor("Fridge_Door_T", root, new Vector3(6.82f, 1.35f, 1.575f), new Vector3(0f, 0f, 0.325f), new Vector3(0.04f, 0.8f, 0.65f), -90f);
        SwingDoor("Fridge_Door_B", root, new Vector3(6.82f, 0.45f, 1.575f), new Vector3(0f, 0f, 0.325f), new Vector3(0.04f, 0.7f, 0.65f), -90f);

        // ── Dining table — centered in open floor space ──
        Box("Table",    root, 4.0f, 0.40f, 3.8f, 2.0f, 0.07f, 1.1f);
        Box("Table_L1", root, 3.1f, 0.20f, 3.3f, 0.09f, 0.40f, 0.09f);
        Box("Table_L2", root, 4.9f, 0.20f, 3.3f, 0.09f, 0.40f, 0.09f);
        Box("Table_L3", root, 3.1f, 0.20f, 4.3f, 0.09f, 0.40f, 0.09f);
        Box("Table_L4", root, 4.9f, 0.20f, 4.3f, 0.09f, 0.40f, 0.09f);

        // Four chairs around the table
        Chair("Chair_N",  root, 4.0f, 4.75f, 180f);
        Chair("Chair_S",  root, 4.0f, 2.85f,   0f);
        Chair("Chair_W",  root, 2.7f, 3.8f,   90f);
        Chair("Chair_E",  root, 5.3f, 3.8f,  270f);
    }

    static void Chair(string name, Transform parent, float x, float z, float rotY)
    {
        var r = Child(name, parent);
        r.localPosition = new Vector3(x, 0, z);
        r.localRotation = Quaternion.Euler(0, rotY, 0);
        Box("Seat",    r, 0f,    0.28f,  0f,    0.4f,  0.05f, 0.4f);
        Box("Back",    r, 0f,    0.55f,  0.18f, 0.4f,  0.5f,  0.05f);
        Box("Leg_FL",  r, -0.18f, 0.14f, -0.18f, 0.05f, 0.28f, 0.05f);
        Box("Leg_FR",  r,  0.18f, 0.14f, -0.18f, 0.05f, 0.28f, 0.05f);
        Box("Leg_BL",  r, -0.18f, 0.14f,  0.18f, 0.05f, 0.28f, 0.05f);
        Box("Leg_BR",  r,  0.18f, 0.14f,  0.18f, 0.05f, 0.28f, 0.05f);
    }

    // ── CAGE ROOM (X: -8..3, Z: 1..7) ────────────────────────────────
    public static void BuildCageRoom(Transform house)
    {
        var root = Child("CageRoom", house);

        // Cage sits in the middle of the room
        // Cage bounds: X -5.5..-1.5,  Z 2..6  (4 wide, 4 deep)
        float x0 = -5.5f, x1 = -1.5f;
        float z0 = 2.0f,  z1 = 6.0f;
        float barH    = 2.1f;
        float barW    = 0.07f;
        float spacing = 0.45f;
        float railY   = barH;
        float midY    = barH / 2f;

        // Top rails
        Rail("Rail_Top_N", root, (x0+x1)/2f, railY, z1, x1-x0+barW, barW, barW);
        Rail("Rail_Top_S", root, (x0+x1)/2f, railY, z0, x1-x0+barW, barW, barW);
        Rail("Rail_Top_W", root, x0, railY, (z0+z1)/2f, barW, barW, z1-z0);
        Rail("Rail_Top_E", root, x1, railY, (z0+z1)/2f, barW, barW, z1-z0);

        // Bottom rails
        Rail("Rail_Bot_N", root, (x0+x1)/2f, 0.04f, z1, x1-x0+barW, barW, barW);
        Rail("Rail_Bot_S", root, (x0+x1)/2f, 0.04f, z0, x1-x0+barW, barW, barW);
        Rail("Rail_Bot_W", root, x0, 0.04f, (z0+z1)/2f, barW, barW, z1-z0);
        Rail("Rail_Bot_E", root, x1, 0.04f, (z0+z1)/2f, barW, barW, z1-z0);

        // Door gap on the south face (centered)
        float doorCX   = (x0 + x1) / 2f;
        float doorHalf = 0.5f;

        // Vertical bars — north, west, east (solid), south (with door gap)
        for (float x = x0; x <= x1 + 0.01f; x += spacing)
        {
            Rail("BarN_" + x.ToString("F1"), root, x, midY, z1, barW, barH, barW);
            Rail("BarS_" + x.ToString("F1"), root, x, midY, z0, barW, barH, barW); // south solid too; door is on west face
        }
        for (float z = z0; z <= z1 + 0.01f; z += spacing)
        {
            Rail("BarW_" + z.ToString("F1"), root, x0, midY, z, barW, barH, barW);
            // East face: leave door gap near south end
            float crawlW = 1.5f;
            bool inDoorGap = (z >= z0 && z <= z0 + crawlW);
            if (!inDoorGap)
                Rail("BarE_" + z.ToString("F1"), root, x1, midY, z, barW, barH, barW);
        }

        float crawlW2 = 1.5f;
        // Door frame sides
        Rail("DoorFrame_Top",   root, x1, barH,      z0 + crawlW2 / 2f, barW, barW, crawlW2);
        Rail("DoorFrame_Side1", root, x1, barH / 2f, z0,                 barW, barH, barW);
        Rail("DoorFrame_Side2", root, x1, barH / 2f, z0 + crawlW2,       barW, barH, barW);

        // Low crawl entry — opening is only 1.3 units tall (must crouch to enter)
        float crawlH = 1.3f;
        // Horizontal rail that visually marks the top of the crawl opening
        Rail("DoorCrawl_Header", root, x1, crawlH, z0 + crawlW2 / 2f, barW * 3f, barW * 3f, crawlW2 + barW);
        // Fill above the crawl header with bars — clearly shows you can't walk through standing
        float upperMidY = crawlH + (barH - crawlH) / 2f;
        float upperH    = barH - crawlH;
        for (float z = z0; z <= z0 + crawlW2 + 0.01f; z += spacing)
            Rail("BarE_upper_" + z.ToString("F1"), root, x1, upperMidY, z, barW, upperH, barW);

        // Cot inside cage
        Box("Cot_Mattress", root, -3.5f, 0.22f, 5.0f, 0.9f, 0.1f, 1.9f);
        Box("Cot_Frame",    root, -3.5f, 0.16f, 5.0f, 1.0f, 0.06f, 2.0f);
        Box("Cot_L1",       root, -3.0f, 0.08f, 4.1f, 0.07f, 0.16f, 0.07f);
        Box("Cot_L2",       root, -4.0f, 0.08f, 4.1f, 0.07f, 0.16f, 0.07f);
        Box("Cot_L3",       root, -3.0f, 0.08f, 5.9f, 0.07f, 0.16f, 0.07f);
        Box("Cot_L4",       root, -4.0f, 0.08f, 5.9f, 0.07f, 0.16f, 0.07f);
    }

    // ── HALLWAY (X: -8..8, Z: -7..1) ────────────────────────────────
    public static void BuildHallway(Transform house)
    {
        var root = Child("Hallway", house);

        // Small entry table near front door
        Box("Entry_Table",    root, -5.5f, 0.38f, -5.8f, 1.2f,  0.06f, 0.45f);
        Box("Entry_Table_L1", root, -5.0f, 0.19f, -5.6f, 0.07f, 0.38f, 0.07f);
        Box("Entry_Table_L2", root, -6.0f, 0.19f, -5.6f, 0.07f, 0.38f, 0.07f);
        Box("Entry_Table_L3", root, -5.0f, 0.19f, -6.0f, 0.07f, 0.38f, 0.07f);
        Box("Entry_Table_L4", root, -6.0f, 0.19f, -6.0f, 0.07f, 0.38f, 0.07f);

        // Tall narrow cabinet against west wall
        Box("Cabinet",      root, -7.4f, 1.1f, -2.0f, 0.6f,  2.2f, 1.0f);
        Box("Cabinet_Door", root, -7.1f, 1.1f, -2.0f, 0.05f, 2.0f, 0.9f);

        // ── Bathroom (NE corner: X 3..8, Z -4.5..1) ──────────────────
        // Wall height matches house (H = 3.5), door height matches house (DH = 2.3)
        const float wH  = 3.5f;
        const float wMY = wH / 2f;    // mid-height = 1.75
        const float dH  = 2.3f;       // door opening height

        // West partition — south panel  (Z -4.5 → -3.0, length 1.5)
        Box("Bath_Wall_W_S",    root, 3.075f, wMY,                    -3.75f, 0.15f, wH,        1.5f);
        // West partition — door lintel  (Z -3.0 → -1.8, above door)
        Box("Bath_Door_Lintel", root, 3.075f, dH + (wH - dH) / 2f,   -2.4f,  0.15f, wH - dH,  1.2f);
        // West partition — north panel  (Z -1.8 → 1.0, length 2.8)
        Box("Bath_Wall_W_N",    root, 3.075f, wMY,                    -0.4f,  0.15f, wH,        2.8f);
        // South partition  (X 3..8 at Z -4.5)
        Box("Bath_Wall_S",      root, 5.5f,   wMY,                    -4.425f, 5.0f, wH,        0.15f);

        // Bathroom door — hinge at south end of gap (Z -3.0), swings +90° into bathroom
        SwingDoor("Bath_Door", root,
                  new Vector3(3.075f, dH / 2f, -3.0f),
                  new Vector3(0f, 0f, 0.6f),
                  new Vector3(0.06f, dH, 1.2f),
                  90f);

        // ── Bathroom fixtures ──────────────────────────────────────────
        // Bathtub — south-east corner, along east wall
        Box("Tub",         root, 7.0f,  0.3f,  -3.2f,  1.8f,  0.6f,  2.0f);
        Box("Tub_Faucet",  root, 7.0f,  0.65f, -2.25f, 0.04f, 0.15f, 0.04f);

        // Toilet — north end, against east wall
        Box("Toilet_Base", root, 7.5f,  0.2f,  0.3f,   0.5f,  0.4f,  0.6f);
        Box("Toilet_Tank", root, 7.65f, 0.65f, 0.05f,  0.25f, 0.5f,  0.2f);
        Box("Toilet_Seat", root, 7.5f,  0.42f, 0.3f,   0.48f, 0.04f, 0.52f);

        // Sink / vanity — middle of east wall
        Box("Sink_Counter", root, 7.55f, 0.45f, -1.5f,  0.45f, 0.9f,  0.8f);
        Box("Sink_Basin",   root, 7.5f,  0.9f,  -1.5f,  0.3f,  0.04f, 0.4f);
        Box("Sink_Faucet",  root, 7.5f,  1.02f, -1.3f,  0.04f, 0.18f, 0.04f);
        Box("Sink_Mirror",  root, 7.87f, 1.6f,  -1.5f,  0.03f, 0.6f,  0.7f);
    }

    // ── HELPERS ──────────────────────────────────────────────────────
    static void Box(string name, Transform parent, float x, float y, float z,
                    float sx, float sy, float sz)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(x, y, z);
        go.transform.localScale    = new Vector3(sx, sy, sz);
    }

    static void Rail(string name, Transform parent, float x, float y, float z,
                     float sx, float sy, float sz)
    {
        Box(name, parent, x, y, z, sx, sy, sz);
    }

    static Transform Child(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.transform;
    }

    // Creates a pivot at hinge position with a door mesh child + Openable component
    static void SwingDoor(string name, Transform parent,
                          Vector3 pivotPos, Vector3 doorLocalPos, Vector3 doorScale,
                          float openAngle)
    {
        var pivot = new GameObject(name + "_Pivot");
        pivot.transform.SetParent(parent, false);
        pivot.transform.localPosition = pivotPos;

        var door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name = name;
        door.transform.SetParent(pivot.transform, false);
        door.transform.localPosition = doorLocalPos;
        door.transform.localScale    = doorScale;

        var openable       = pivot.AddComponent<Openable>();
        openable.openAngle = openAngle;
        openable.speed     = 4f;
    }

    // Two half-width leaves that open simultaneously: left at +90°, right at -90°
    static void DoubleCabDoor(string name, Transform parent,
                              float px, float py, float pz,
                              float totalW, float doorH, float doorThick)
    {
        float hw = totalW * 0.5f;

        // Left leaf — hinge on left edge, extends +X, opens +90° into room
        var pivL = new GameObject(name + "L_Pivot");
        pivL.transform.SetParent(parent, false);
        pivL.transform.localPosition = new Vector3(px, py, pz);
        var leafL = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leafL.name = name + "L";
        leafL.transform.SetParent(pivL.transform, false);
        leafL.transform.localPosition = new Vector3(hw * 0.5f, 0f, 0f);
        leafL.transform.localScale    = new Vector3(hw, doorH, doorThick);
        var oL = pivL.AddComponent<Openable>();
        oL.openAngle = 90f;
        oL.speed     = 4f;

        // Right leaf — hinge on right edge, extends -X, opens -90° into room
        var pivR = new GameObject(name + "R_Pivot");
        pivR.transform.SetParent(parent, false);
        pivR.transform.localPosition = new Vector3(px + totalW, py, pz);
        var leafR = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leafR.name = name + "R";
        leafR.transform.SetParent(pivR.transform, false);
        leafR.transform.localPosition = new Vector3(-hw * 0.5f, 0f, 0f);
        leafR.transform.localScale    = new Vector3(hw, doorH, doorThick);
        var oR = pivR.AddComponent<Openable>();
        oR.openAngle = -90f;
        oR.speed     = 4f;

        // Link the two leaves so pressing E on either opens both
        oL.linkedDoor = oR;
        oR.linkedDoor = oL;
    }

    static void DestroyChild(GameObject parent, string childName)
    {
        var t = parent.transform.Find(childName);
        if (t != null) Undo.DestroyObjectImmediate(t.gameObject);
    }
}
