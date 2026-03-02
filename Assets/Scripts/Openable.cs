using UnityEngine;

public class Openable : MonoBehaviour
{
    public float    openAngle  = -90f;
    public float    speed      = 4f;
    public Openable linkedDoor = null;   // set on double-door pairs

    private bool       isOpen;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = transform.localRotation;
        openRot   = closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    // Called by the player — also opens the paired leaf if present
    public void Toggle()
    {
        isOpen = !isOpen;
        linkedDoor?.ToggleSelf();
    }

    // Called by the linked door to avoid infinite recursion
    public void ToggleSelf()
    {
        isOpen = !isOpen;
    }

    void Update()
    {
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            isOpen ? openRot : closedRot,
            Time.deltaTime * speed);
    }
}
