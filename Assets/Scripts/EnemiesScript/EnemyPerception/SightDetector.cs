using System;
using UnityEngine;

public class SightDetector : MonoBehaviour
{
    public bool IsTargetInRange { get; private set; }
    public bool IsTargetVisible { get; private set; }
    public Vector3 targetPosition { get; private set; }
    public GameObject targetGameObject { get; private set; }
    public bool isShuttingDown { get; private set; }
    public string targetTag = "Player";
    public Transform origin;
    private void Start()
    {
        try
        {
            GetComponent<Collider>();
        }
        catch (Exception)
        {
            Debug.LogError("Error Cannot detect Collider");
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (origin == null) return;
        if (other.CompareTag(targetTag))
        {
            IsTargetInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        if (origin == null) return;
        if (other.CompareTag(targetTag))
        {
            IsTargetInRange = false;
            IsTargetVisible = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other == null) return;
        if (origin == null) return;
        if (other.CompareTag(targetTag))
        {
            targetPosition = other.transform.position;
            targetGameObject = other.transform.gameObject;
            Ray ray = new Ray(origin.position, targetPosition - origin.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawRay(origin.position, hit.point - origin.position, Color.green);
                Debug.Log("Is Hit" + hit.transform.gameObject.CompareTag(targetTag));
                if (hit.transform.gameObject.CompareTag(targetTag))
                {
                    IsTargetVisible = true;
                    Debug.Log(hit.transform.gameObject.tag + " Position:" + hit.transform.position);
                }
                else
                {
                    IsTargetVisible = false;
                    //Debug.Log("Obstacle:" + hit.transform.position);
                }
            }
        }
    }
    void OnDestroy()
    {
        isShuttingDown = true;
    }
    void OnApplicationQuit()
    {
        isShuttingDown = true;
    }
    public GameObject getTarget()
    {
        return targetGameObject;
    }
}
