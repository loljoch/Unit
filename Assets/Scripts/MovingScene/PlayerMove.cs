using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;

    [SerializeField] private float walkSpeed, runSpeed;
    [SerializeField] private float runBuildUpSpeed;
    [SerializeField] private KeyCode runKey;

    private float movementSpeed;

    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;

    private CharacterController charController;

    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private KeyCode jumpKey;

    private bool isJumping;

    [SerializeField] private string playerTag;
    [SerializeField] private string enemyTag;

    private void Awake()
    {
        if (transform.tag != enemyTag && transform.tag != playerTag)
        {
            if (GameObject.FindGameObjectWithTag(playerTag))
            {
                gameObject.transform.tag = enemyTag;

            } else {
                gameObject.transform.tag = playerTag;
            }
        }
        charController = GetComponent<CharacterController>();

    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement() {
        //Gets button input
        float horizInput = Input.GetAxis(horizontalInputName);
        float vertInput = Input.GetAxis(verticalInputName);

        //transforms button input into a transform
        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        //Moves the character by the transformed button input clamped by 1 so diagonal speed isn't greater than 1 button press speed
        charController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * movementSpeed);

        //When on a slope get dragged down so slopes doesn't feel like going down stairs
        if ((vertInput != 0 || horizInput != 0) && onSlope())
        {
            charController.Move(Vector3.down * charController.height / 2 * slopeForce * Time.deltaTime);

        }

        SetMovementSpeed();
        JumpInput();
    }

    private void JumpInput()
    {

        if(Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }

    }

    private bool onSlope()
    {
        if (isJumping)
        {
            return false;
        }

        RaycastHit hit;
        //Raycasts downward
        if(Physics.Raycast(transform.position, Vector3.down, out hit, charController.height / 2 * slopeForceRayLength))
        {
            //If the raycast doesn't register a flat service, then it's a slope
            if(hit.normal != Vector3.up)
            {
                return true;
            }

        }
        return false;

    }

    private void SetMovementSpeed()
    {
        //When pressing the runkey, lerp movementspeed to runspeed
        if (Input.GetKey(runKey))
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
        } else {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
        }

    }



    private IEnumerator JumpEvent()
    {
        charController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

        charController.slopeLimit = 45.0f;
        isJumping = false;

    }


}
