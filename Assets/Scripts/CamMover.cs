using UnityEngine;

public class CamMover : MonoBehaviour
{
    public Transform camPosition;

    // Update is called once per frame
    private void Update()
    {
        transform.position = camPosition.position;
    }
}