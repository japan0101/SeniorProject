using UnityEngine;

public class CamMover : MonoBehaviour
{
    public Transform camPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = camPosition.position;
    }
}
