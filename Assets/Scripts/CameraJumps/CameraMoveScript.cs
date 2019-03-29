using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMoveScript : MonoBehaviour
{
    //gunscrippt
    [SerializeField] private string fireButton;
    [SerializeField] private float fireRange = 100f;
    [SerializeField] private string camStanceTag;

    //lookscript
    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float cameraTransitionTime;
    [SerializeField] private float LerpWaitTime;
    private WaitForSeconds lerpRotationTime;

    private Transform playerBody;
    private GameObject[] otherBodies;
    [SerializeField] private string enemyTag;
    [HideInInspector] public bool isLerpingRotation = false;
    [HideInInspector] public bool canMove;
    private float xAxisClamp;

    private void Awake()
    {
        lerpRotationTime = new WaitForSeconds(LerpWaitTime);
        AssignPlayer();
        LockCursor();
        xAxisClamp = 0.0f;
        canMove = true;
    }


    private void LockCursor()
    {
        //Locks cursor in middle of screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (canMove)
        {
            CameraRotation();
        }
        if (Input.GetButtonDown(fireButton))
        {
            ShootRay();
        }
    }

    //SHOOT
    private void ShootRay()
    {

        RaycastHit hit;

        //Shoots a raycast from the camera's position and gives back name of object
        if (Physics.Raycast(transform.position, transform.forward, out hit, fireRange))
        {
            Debug.Log(hit.transform.tag);
            if (hit.transform.tag != null)
            {
                if (hit.transform.CompareTag(enemyTag) && isLerpingRotation == false)
                {
                    ShootCamera(hit);
                } else if (hit.transform.CompareTag("Finish"))
                {
                    SceneManager.LoadScene(1);
                }
            }

        }
    }

    //Makes sure everyone has the right parent and shoots the camera
    private void ShootCamera(RaycastHit target)
    {
        canMove = target.transform.gameObject.GetComponent<MoveOnPath>().canLookAround;
        target.transform.gameObject.GetComponent<MoveOnPath>().cameraIsOnMe = true;
        target.transform.gameObject.layer = 2;

        Transform newCam = target.transform.Find(camStanceTag);
        newCam.GetComponentInChildren<Light>().gameObject.SetActive(false);

        transform.parent = newCam;
        AssignPlayer();
        StartCoroutine("StopLerpRotation");
        isLerpingRotation = true;

    }
    //END SHOOT

    private void LateUpdate()
    {
        LerpToPlayer();
    }

    private void CameraRotation()
    {
        //Gets mouse inputAxis
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        //xAxisClamp += mouseY;

        ////Makes sure that you can't do a 360 with your camera when you keep looking upwards/downwards
        //if (xAxisClamp > 90.0f)
        //{
        //    xAxisClamp = 90.0f;
        //    mouseY = 0.0f;
        //    ClampXAxisRotationToValue(270.0f);
        //} else if (xAxisClamp < -90.0f)
        //{
        //    xAxisClamp = -90.0f;
        //    mouseY = 0.0f;
        //    ClampXAxisRotationToValue(90.0f);
        //}

        //Rotates camera
        //Findobjectswithtag("enemy") en dan
        //FOREACH playerbody rotate, zodat alle spelers dezelfde richting opkijken en lopen
        //Zet de array met enemies opnieuw elke keer dat je switched
        transform.Rotate(Vector3.left * mouseY);
        playerBody.Rotate(Vector3.up * mouseX);



    }



    private void ClampXAxisRotationToValue(float value)
    {

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
        transform.rotation = transform.parent.rotation;
        isLerpingRotation = false;
    }

}
