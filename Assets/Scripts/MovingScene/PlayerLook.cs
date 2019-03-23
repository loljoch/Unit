using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour {


    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float cameraTransitionTime;
    [SerializeField] private float LerpWaitTime;
    private WaitForSeconds lerpRotationTime;

    private Transform playerBody;
    private GameObject[] otherBodies;
    [SerializeField] private string enemyTag;
    [HideInInspector] public bool isLerpingRotation = false;

    private float xAxisClamp;

    private void Awake() {
        lerpRotationTime = new WaitForSeconds(LerpWaitTime);
        AssignPlayer();
        LockCursor();
        xAxisClamp = 0.0f;
    }


    private void LockCursor() {
        //Locks cursor in middle of screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        CameraRotation();
    }

    private void LateUpdate()
    {
        LerpToPlayer();
    }

    private void CameraRotation() {
        //Gets mouse inputAxis
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;

        //Makes sure that you can't do a 360 with your camera when you keep looking upwards/downwards
        if(xAxisClamp > 90.0f) {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        } else if (xAxisClamp < -90.0f) {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }

        //Rotates camera
        //Findobjectswithtag("enemy") en dan
        //FOREACH playerbody rotate, zodat alle spelers dezelfde richting opkijken en lopen
        //Zet de array met enemies opnieuw elke keer dat je switched
        transform.Rotate(Vector3.left * mouseY);
        playerBody.Rotate(Vector3.up * mouseX);

        otherBodies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (var body in otherBodies)
        {
            body.transform.Rotate(Vector3.up * mouseX);
        }

        

    }



    private void ClampXAxisRotationToValue(float value){

        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;

    }

    public void AssignPlayer()
    {
        //Assigning the player object
        playerBody = transform.parent.parent;


    }

    private void LerpToPlayer()
    {
        //Lerps to the new camera stance
        transform.position = Vector3.Lerp(transform.position, transform.parent.position, cameraTransitionTime * Time.deltaTime);

        if (isLerpingRotation)
        {
            //Rotates to the camera stance rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.parent.rotation, cameraTransitionTime * Time.deltaTime);
        }
    }

    IEnumerator StopLerpRotation()
    {
        //Timer to end lerping so you don't get stuck with you angle in one direction
        yield return lerpRotationTime;
        isLerpingRotation = false;
    }

}
