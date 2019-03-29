using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnPath : MonoBehaviour
{
    public EditorPathScript pathToFollow;

    public int currentWayPointID = 0;
    [SerializeField] private float speed;
    private float reachDistance = 0f;
    private bool canRotate = false;

    Vector3 lastPosition;
    Vector3 currentPosition;

    Quaternion rotation;
    Vector3 mustRotateTo;

    //Does the camera move on its own or only when player is on in
    [SerializeField] private bool moveOnCameraJump;
    //Can camera look around
    public bool canLookAround;
    //Go back and forth automatically
    [SerializeField] private bool isPingPonging;

    private bool isNotFirstTimeGoing = false;
    [HideInInspector] public bool cameraIsOnMe;
    private bool isReversing;

    private void Start()
    {
        lastPosition = transform.position;
        rotation = transform.rotation;
    }

    private void Update()
    {
        if (moveOnCameraJump && cameraIsOnMe || !moveOnCameraJump)
        {
            FollowPath();
        }

        RotateObject();


    }

    private void FollowPath()
    {
        if (GetComponent<AudioSource>().isPlaying == false)
        {
            GetComponent<AudioSource>().Play();
        }
        currentPosition = transform.position;
        transform.position = Vector3.MoveTowards(currentPosition, pathToFollow.pathObjects[currentWayPointID].position, speed * Time.deltaTime);
        CheckPath();
    }

    private void CheckPath()
    {
        float distance = Vector3.Distance(pathToFollow.pathObjects[currentWayPointID].position, currentPosition);

        if (distance <= reachDistance)
        {
            if (!isReversing)
            {
                Debug.Log("IM NOT REVERSE");

                //Checks if it's at the beginning of the path
                if (currentWayPointID == 0 && isNotFirstTimeGoing && !isPingPonging)
                {
                    cameraIsOnMe = false;
                }

                currentWayPointID++;

            } else
            {
                Debug.Log("IM REVERSE");

                //Checks if it's at the end of the path
                if (currentWayPointID == pathToFollow.pathObjects.Count - 1 && !isPingPonging)
                {
                    cameraIsOnMe = false;
                }

                currentWayPointID--;
            }

            //Checks when to reverse direction
            if (currentWayPointID == pathToFollow.pathObjects.Count - 1)
            {
                isNotFirstTimeGoing = true;
                isReversing = true;
            } else if (currentWayPointID == 0)
            {
                isReversing = false;
            }

            
        }

    }


    //Gets the information of the rotation the camera has to make
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<RotateCameraScript>() != null)
        {
            mustRotateTo = other.GetComponent<RotateCameraScript>().rotateBy;
            rotation = transform.rotation;

            if (isReversing) {
                rotation *= Quaternion.Euler(mustRotateTo*-1);
            } else
            {
                rotation *= Quaternion.Euler(mustRotateTo);
            }

            canRotate = true;
            StartCoroutine(RotateTimer());
        }   
    }


    //Rotates camera when meeting a corner
    private void RotateObject()
    {
        //Lerps towards rotation
        if (canRotate == true)
        {
            Debug.Log("is lerping");
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 6f * Time.deltaTime);

        } else if(transform.rotation != rotation)
        {
            //fail safe
            transform.rotation = new Quaternion (Mathf.RoundToInt(rotation.x), Mathf.RoundToInt(rotation.y), Mathf.RoundToInt(rotation.z), rotation.w);
        }
    }

    //fail safe
    IEnumerator RotateTimer()
    {
        yield return new WaitForSeconds(0.8f);
        canRotate = false;
    }


}
