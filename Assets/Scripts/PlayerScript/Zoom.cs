using UnityEngine;

public class Zoom : MonoBehaviour
{
    public int zoom = 20;
    public float smooth = 5;
    private readonly int normal = 60;

    private bool isZoomed;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) isZoomed = !isZoomed;
    }

    public void speedZoom(float speed)
    {
        if (isZoomed)
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, zoom + speed - 7,
                Time.deltaTime * smooth);
        else
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, normal + speed - 7,
                Time.deltaTime * smooth);
    }
}