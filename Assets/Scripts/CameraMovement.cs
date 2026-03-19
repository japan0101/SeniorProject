using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float senX;
    public float senY;

    public Transform hOrientation;
    public Transform vOrientation;

    private bool _inputEnabled;

    private float xRotation;

    private float yRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log($"Starting rotation: {transform.rotation.eulerAngles}");
        Invoke(nameof(EnableInput), 0.1f);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_inputEnabled) return;
        //get Mouse input
        var mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senX;
        var mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senY;

        yRotation += mouseX;
        xRotation -= mouseY;

        //capping looking up and down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        hOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
        vOrientation.rotation = Quaternion.Euler(xRotation, 0, 0);
    }

    private void EnableInput()
    {
        _inputEnabled = true;
    }
}