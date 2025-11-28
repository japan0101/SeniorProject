using EnemiesScript;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class SightDetector : MonoBehaviour
{
    public bool IsTargetInRange = false;
    public bool IsTargetVisible = false;
    public Vector3 targetPosition;
    public string targetTag = "Player";
    [SerializeField] Transform origin;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            IsTargetInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            IsTargetInRange = false;
            IsTargetVisible = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            targetPosition = other.transform.position;
            Ray ray = new Ray(origin.position, targetPosition - origin.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawRay(origin.position, hit.point - origin.position, Color.green);
                Debug.Log("Is Hit Player:" + hit.transform.gameObject.CompareTag(targetTag));
                if (hit.transform.gameObject.CompareTag(targetTag))
                {
                    IsTargetVisible = true;
                    Debug.Log("Player Position:" + hit.transform.position);
                }
                else
                {
                    IsTargetVisible = false;
                    Debug.Log("Obstacle:" + hit.transform.position);
                }
            }
        }
    }
}
