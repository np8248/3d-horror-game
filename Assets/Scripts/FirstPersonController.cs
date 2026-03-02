using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed   = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float gravity     = -20f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle     = 80f;

    [Header("Crouch Settings (toggle with C)")]
    public float standHeight           = 1.75f;
    public float crouchHeight          = 0.6f;
    public float crouchTransitionSpeed = 8f;

    private CharacterController controller;
    private Camera playerCamera;
    private float  rotationX = 0f;
    private float  velocityY = 0f;
    private bool   isCrouching = false;
    private float  standCamY;
    private float  crouchCamY;

    void Start()
    {
        controller   = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Compute eye heights from standHeight so they don't depend on where
        // the camera object was placed in the hierarchy in the Inspector.
        // Pivot sits at standHeight/2 above the floor (CharacterController math),
        // so subtracting 0.15 puts the camera ~1.6 m from the floor.
        standCamY  = standHeight / 2f - 0.15f;
        crouchCamY = standCamY * (crouchHeight / standHeight);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse    = Mouse.current;
        if (keyboard == null) return;

        // ── Crouch toggle ──────────────────────────────────────────
        if (keyboard.cKey.wasPressedThisFrame)
            isCrouching = !isCrouching;

        float targetH = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetH, Time.deltaTime * crouchTransitionSpeed);
        controller.center = new Vector3(0f, controller.height / 2f - standHeight / 2f, 0f);

        float targetCamY = isCrouching ? crouchCamY : standCamY;
        var camPos = playerCamera.transform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * crouchTransitionSpeed);
        playerCamera.transform.localPosition = camPos;

        // ── WASD movement ──────────────────────────────────────────
        float x = 0f, z = 0f;
        if (keyboard.dKey.isPressed) x += 1f;
        if (keyboard.aKey.isPressed) x -= 1f;
        if (keyboard.wKey.isPressed) z += 1f;
        if (keyboard.sKey.isPressed) z -= 1f;

        float speed = isCrouching ? crouchSpeed :
                      keyboard.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;

        controller.Move((transform.right * x + transform.forward * z) * speed * Time.deltaTime);

        // ── Jump — works whenever you're not already going up ──────
        if (keyboard.spaceKey.wasPressedThisFrame && velocityY <= 0f)
            velocityY = jumpForce;

        // ── Gravity ────────────────────────────────────────────────
        velocityY += gravity * Time.deltaTime;
        if (controller.isGrounded && velocityY < -2f)
            velocityY = -2f;

        controller.Move(Vector3.up * velocityY * Time.deltaTime);

        // ── Mouse look ─────────────────────────────────────────────
        if (mouse != null)
        {
            float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity * 0.1f;
            float mouseY = mouse.delta.y.ReadValue() * mouseSensitivity * 0.1f;

            transform.Rotate(Vector3.up * mouseX);
            rotationX = Mathf.Clamp(rotationX - mouseY, -maxLookAngle, maxLookAngle);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }

        // ── Unlock cursor ──────────────────────────────────────────
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
    }
}
