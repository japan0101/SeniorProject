using UnityEngine;

public class testRay : MonoBehaviour
{
    [Header("Ray Settings")]
    public Transform rayOrigin;
    public float rayLength = 10f;
    public LayerMask detectionLayer;
    public Color rayColor = Color.red;

    [Header("Target Settings")]
    public Transform objectToRotate; // The object that will face the point
    public Transform shot;
    public Vector3 rotationOffset; // Optional rotation adjustment
    public Transform visualizer; //to display hit position

    void Update()
    {
        // Cast the ray forward from this object's position
        Ray ray = new Ray(rayOrigin.transform.position, rayOrigin.transform.forward);
        RaycastHit hit;
        Vector3 targetPoint;

        // Check if ray hits something
        if (Physics.Raycast(ray, out hit, rayLength, detectionLayer))
        {
            // Use the hit point if we hit something
            targetPoint = hit.point;
            Debug.Log("Hit: " + hit.collider.name);
        }
        else
        {
            // Use the ray's end point if nothing was hit
            targetPoint = ray.origin + ray.direction * rayLength;
        }

        // Make the object face the target point
        if (objectToRotate != null)
        {
            RotateToFacePoint(targetPoint);
        }
        if (visualizer != null) {
            visualizer.position = targetPoint;
        }

        // Debug visualization
        Debug.DrawRay(shot.position, ray.direction * rayLength, rayColor);
    }

    void RotateToFacePoint(Vector3 targetPoint)
    {
        // Calculate direction to the target point
        Vector3 direction = targetPoint - objectToRotate.position;

        // Only rotate on Y axis (horizontal) - remove if you want full 3D rotation
        //direction.y = 0;

        // Create the rotation to look at the point
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Apply optional offset
        targetRotation *= Quaternion.Euler(rotationOffset);

        // Apply the rotation
        objectToRotate.rotation = targetRotation;
    }
}
