using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float senX;
    public float senY;

    public Transform hOrientation;
    public Transform vOrientation;

    float xRotation;
    float yRotation;

    private bool _inputEnabled = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log($"Starting rotation: {transform.rotation.eulerAngles}");
        Invoke(nameof(EnableInput), 0.1f);
    }
    void EnableInput() => _inputEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if (!_inputEnabled) return;
        //get Mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senY;

        yRotation += mouseX;
        xRotation -= mouseY;

        //capping looking up and down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        hOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
        vOrientation.rotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
