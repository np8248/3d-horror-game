using UnityEngine;
using UnityEditor;

public static class HouseGenerator
{
    // House dimensions
    const float W  = 16f;   // width  (X axis)
    const float D  = 14f;   // depth  (Z axis)
    const float H  = 3.5f;  // wall height
    const float T  = 0.3f;  // wall thickness
    const float DW = 1.5f;  // door width
    const float DH = 2.3f;  // door height

    [MenuItem("Tools/Generate House")]
    public static void GenerateHouse()
    {
        // Remove old house if regenerating
        var existing = GameObject.Find("House");
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing);
        }

        var root = new GameObject("House");
        Undo.RegisterCreatedObjectUndo(root, "Generate House");

        float hW = W / 2f;
        float hD = D / 2f;
        float hH = H / 2f;

        // ── FLOOR & CEILING ──────────────────────────────────────────
        Box("Floor",   root.transform, 0, -0.1f,    0,  W,     0.2f, D);
        Box("Ceiling", root.transform, 0,  H + 0.1f, 0, W + T, 0.2f, D + T);

        // ── EXTERIOR WALLS ───────────────────────────────────────────
        Box("Wall_Back",  root.transform,  0,  hH,  hD, W + T, H, T);
        Box("Wall_Left",  root.transform, -hW, hH,   0, T,     H, D);
        Box("Wall_Right", root.transform,  hW, hH,   0, T,     H, D);

        // Front wall — split around the front door (centered at X=0)
        float fdX = 0f;
        float flw = hW + fdX - DW / 2f;   // left section width
        float frw = hW - fdX - DW / 2f;   // right section width
        Box("Wall_Front_L",      root.transform, -hW + flw / 2f,          hH,                 -hD, flw, H,        T);
        Box("Wall_Front_R",      root.transform,  fdX + DW / 2f + frw / 2f, hH,              -hD, frw, H,        T);
        Box("Wall_Front_Lintel", root.transform,  fdX,                      DH + (H - DH) / 2f, -hD, DW, H - DH, T);

        // ── INTERIOR WALLS ───────────────────────────────────────────

        // Horizontal divider at Z=1 (splits entrance hall from back rooms)
        float divZ = 1f;
        float idX  = -2f;   // door center X
        float ilw  = hW + idX - DW / 2f;
        float irw  = hW - idX - DW / 2f;
        Box("Wall_Div_L",      root.transform, -hW + ilw / 2f,             hH,                 divZ, ilw, H,        T);
        Box("Wall_Div_R",      root.transform,  idX + DW / 2f + irw / 2f,  hH,                 divZ, irw, H,        T);
        Box("Wall_Div_Lintel", root.transform,  idX,                        DH + (H - DH) / 2f, divZ, DW,  H - DH,  T);

        // Vertical divider at X=1 (splits back-left bedroom from back-right kitchen — wider kitchen)
        // Runs from divZ to hD with a doorway in the middle
        float divX = 1f;
        float vd   = hD - divZ;          // total length of this wall
        float vdZ  = (divZ + hD) / 2f;   // door centre Z
        float vtd  = vd / 2f - DW / 2f;  // length of each half
        Box("Wall_Vert_Back",   root.transform, divX, hH,                  divZ + vtd / 2f,         T, H,        vtd);
        Box("Wall_Vert_Front",  root.transform, divX, hH,                  vdZ + DW / 2f + vtd / 2f, T, H,       vtd);
        Box("Wall_Vert_Lintel", root.transform, divX, DH + (H - DH) / 2f, vdZ,                       T, H - DH,  DW);

        // ── YARD ─────────────────────────────────────────────────────
        var yard = new GameObject("Yard");
        yard.transform.SetParent(root.transform);

        const float YW = 50f;  // yard width
        const float YD = 45f;  // yard depth
        const float FH = 1.8f; // fence height
        const float FT = 0.2f; // fence thickness
        const float GW = 3f;   // gate gap width

        float yhW        = YW / 2f;
        float yhD        = YD / 2f;
        float yardCenter = 3f;   // slightly behind Z=0 so yard wraps house

        Box("Yard_Ground", yard.transform, 0, -0.15f, yardCenter, YW, 0.1f, YD);

        // Perimeter fence — back, left, right
        Box("Fence_Back",  yard.transform,  0,    FH / 2f, yardCenter + yhD, YW, FH, FT);
        Box("Fence_Left",  yard.transform, -yhW,  FH / 2f, yardCenter,       FT, FH, YD);
        Box("Fence_Right", yard.transform,  yhW,  FH / 2f, yardCenter,       FT, FH, YD);

        // Front fence — split around the gate (centered at X=0)
        float ffl        = yhW - GW / 2f;  // half-length of each side
        float frontFenceZ = yardCenter - yhD;
        Box("Fence_Front_L", yard.transform, -(GW / 2f + ffl / 2f), FH / 2f, frontFenceZ, ffl, FH, FT);
        Box("Fence_Front_R", yard.transform,  (GW / 2f + ffl / 2f), FH / 2f, frontFenceZ, ffl, FH, FT);

        Debug.Log("House generated! Select Scene view and press F to frame it.");
        Selection.activeGameObject = root;
        SceneView.FrameLastActiveSceneView();
    }

    static void Box(string name, Transform parent, float x, float y, float z,
                    float sx, float sy, float sz)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(x, y, z);
        go.transform.localScale    = new Vector3(sx, sy, sz);
    }
}
