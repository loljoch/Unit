using UnityEngine;

public class gunScript : MonoBehaviour
{
    [SerializeField] private string fireButton;
    [SerializeField] private float fireRange = 100f;

    [SerializeField] private Transform player;

    [SerializeField] private string playerTag;
    [SerializeField] private string enemyTag;
    [SerializeField] private string camStanceTag;

    [SerializeField] private Camera fpsCam;
    private PlayerLook camScript;
    public ObjectStopper objectStopperScript;

    //Sound
    private AudioSource audioSource;
    [SerializeField] private AudioClip swapSound, hitSound;

    private void Awake()
    {
        AssignPlayer();
        audioSource = GetComponent<AudioSource>();
        camScript = fpsCam.GetComponent<PlayerLook>();
    }

    private void Update() {

        if (Input.GetButtonDown(fireButton)) {
            ShootRay();
        }
    }

    private void ShootRay() {

        RaycastHit hit;

        //Shoots a raycast from the camera's position and gives back name of object
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, fireRange)) {
            Debug.Log(hit.transform.tag);
            if (hit.transform.tag != null)
            {
                if (hit.transform.CompareTag(enemyTag) && camScript.isLerpingRotation == false)
                {
                    ShootCamera(hit);
                }
            }
            if (hit.transform.gameObject.GetComponent<leverScript>() != null)
            {
                Debug.Log("FOUND A LEVER");
                audioSource.PlayOneShot(hitSound);
                hit.transform.gameObject.GetComponent<leverScript>().door.SetBool("PlayAnim", true);
            }


        }
    }

    //Makes sure everyone has the right parent and shoots the camera
    private void ShootCamera(RaycastHit target)
    {
        audioSource.PlayOneShot(swapSound);
        target.transform.tag = playerTag;
        target.transform.gameObject.layer = 2;
        player.tag = enemyTag;
        player.gameObject.layer = 0;

        Transform newCam = target.transform.Find(camStanceTag);

        fpsCam.transform.parent = newCam;
        AssignPlayer();
        objectStopperScript.AssignPlayer();
        camScript.AssignPlayer();
        camScript.StartCoroutine("StopLerpRotation");
        camScript.isLerpingRotation = true;

    }

    private void AssignPlayer()
    {
        player = transform.parent.parent.parent;

    }
}
