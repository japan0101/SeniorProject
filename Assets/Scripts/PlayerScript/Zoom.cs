using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour
{
    public int zoom = 20;
    int normal = 60;
    public float smooth = 5;

    private bool isZoomed=false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = !isZoomed;
        }
    }

    public void speedZoom(float speed)
    {
        if (isZoomed)
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, zoom + speed - 7, Time.deltaTime * smooth);
        }
        else
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, normal + speed - 7, Time.deltaTime * smooth);
        }
    }
}
