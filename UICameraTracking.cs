using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks the positioning of a UI window to the camera with configurable anchor, offset, scale, and follow speed.
/// Can be used for general UI elements (Vitals, map, etc.).
/// <para>Attach this script to a parent GameObject that represents the UI window you want to track.</para>
/// </summary>
public class UIWindowFollower : MonoBehaviour
{
    [Header("Window Settings")]
    [SerializeField] private TextAnchor windowAnchor = TextAnchor.LowerCenter;
    [SerializeField] private Vector2 windowOffset = new(0.1f, 0.1f);
    [SerializeField, Range(0.5f, 5.0f)] private float windowScale = 1.0f;
    [SerializeField, Range(0.0f, 100.0f)] private float windowFollowSpeed = 5.0f;
    [SerializeField] private Transform window; // Assign your UI root transform here

    private static readonly Vector3 defaultWindowScale = new(0.2f, 0.04f, 1.0f);
    private Quaternion windowHorizontalRotation;
    private Quaternion windowHorizontalRotationInverse;
    private Quaternion windowVerticalRotation;
    private Quaternion windowVerticalRotationInverse;
    private static readonly Vector2 defaultWindowRotation = new(10.0f, 20.0f);

    void Awake()
    {
        windowHorizontalRotation = Quaternion.AngleAxis(defaultWindowRotation.y, Vector3.right);
        windowHorizontalRotationInverse = Quaternion.Inverse(windowHorizontalRotation);
        windowVerticalRotation = Quaternion.AngleAxis(defaultWindowRotation.x, Vector3.up);
        windowVerticalRotationInverse = Quaternion.Inverse(windowVerticalRotation);
    }

    void LateUpdate()
    {
        if (window == null) return;

        Transform cameraTransform = Camera.main ? Camera.main.transform : null;
        if (cameraTransform != null)
        {
            float t = Time.deltaTime * windowFollowSpeed;
            window.SetPositionAndRotation(Vector3.Lerp(window.position, CalculateWindowPosition(cameraTransform), t), Quaternion.Slerp(window.rotation, CalculateWindowRotation(cameraTransform), t));
            window.localScale = defaultWindowScale * windowScale;
        }
    }

    private Vector3 CalculateWindowPosition(Transform cameraTransform)
    {
        float windowDistance = Mathf.Max(16.0f / Camera.main.fieldOfView, Mathf.Max(Camera.main.nearClipPlane, 0.5f));
        Vector3 position = cameraTransform.position + (cameraTransform.forward * windowDistance);
        Vector3 horizontalOffset = cameraTransform.right * windowOffset.x;
        Vector3 verticalOffset = cameraTransform.up * windowOffset.y;

        switch (windowAnchor)
        {
            case TextAnchor.UpperLeft: position += verticalOffset - horizontalOffset; break;
            case TextAnchor.UpperCenter: position += verticalOffset; break;
            case TextAnchor.UpperRight: position += verticalOffset + horizontalOffset; break;
            case TextAnchor.MiddleLeft: position -= horizontalOffset; break;
            case TextAnchor.MiddleRight: position += horizontalOffset; break;
            case TextAnchor.LowerLeft: position -= verticalOffset + horizontalOffset; break;
            case TextAnchor.LowerCenter: position -= verticalOffset; break;
            case TextAnchor.LowerRight: position -= verticalOffset - horizontalOffset; break;
        }

        return position;
    }

    private Quaternion CalculateWindowRotation(Transform cameraTransform)
    {
        Quaternion rotation = cameraTransform.rotation;

        switch (windowAnchor)
        {
            case TextAnchor.UpperLeft: rotation *= windowHorizontalRotationInverse * windowVerticalRotationInverse; break;
            case TextAnchor.UpperCenter: rotation *= windowHorizontalRotationInverse; break;
            case TextAnchor.UpperRight: rotation *= windowHorizontalRotationInverse * windowVerticalRotation; break;
            case TextAnchor.MiddleLeft: rotation *= windowVerticalRotationInverse; break;
            case TextAnchor.MiddleRight: rotation *= windowVerticalRotation; break;
            case TextAnchor.LowerLeft: rotation *= windowHorizontalRotation * windowVerticalRotationInverse; break;
            case TextAnchor.LowerCenter: rotation *= windowHorizontalRotation; break;
            case TextAnchor.LowerRight: rotation *= windowHorizontalRotation * windowVerticalRotation; break;
        }

        return rotation;
    }

    // Optional: Expose properties for runtime adjustment
    public TextAnchor WindowAnchor { get => windowAnchor; set => windowAnchor = value; }
    public Vector2 WindowOffset { get => windowOffset; set => windowOffset = value; }
    public float WindowScale { get => windowScale; set => windowScale = Mathf.Clamp(value, 0.5f, 5.0f); }
    public float WindowFollowSpeed { get => windowFollowSpeed; set => windowFollowSpeed = Mathf.Abs(value); }
    public Transform Window { get => window; set => window = value; }
}