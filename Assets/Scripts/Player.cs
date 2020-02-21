using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject cloudGuy;

    public GameObject wall1;
    public GameObject wall2;
    float speed = 200;
    public Vector3 direction;
    public Rigidbody rb;
    public bool hasBounced;
    public bool isStunned;
    public bool removeStun;
    bool windSoundPlaying;
    bool isDead;
    Vector3 targetCamPos;
    Vector3 targetCloudPos;
    Vector3 targetCloudRot;
    int cloudSpeed;
    int cloudRotationSpeed;

    public SkinnedMeshRenderer meshRenderer;

    public Texture defaultFace;
    public Texture powerUpFace;
    public Texture bounceFace;
    public Texture stunnedFace;

    public Texture defaultCloudFace;
    public Texture blowLeft;
    public Texture blowRight;
    public Texture cloudStunnedFace;
    public Texture cloudCryingFace;


    public Transform location;

    public ParticleSystem particleSystem;
    ParticleSystem.VelocityOverLifetimeModule velocityModule;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Spawner.Instance.mainCamera;
        cloudGuy = Spawner.Instance.cloudGuy;
        wall1 = Spawner.Instance.wall1;
        wall2 = Spawner.Instance.wall2;
        direction = new Vector3(0, 0);
        targetCamPos = new Vector3(0, location.position.y + 2, -10);
        targetCloudPos = new Vector3(0.41f, location.position.y + 4, 3.82f);
        targetCloudRot = new Vector3(-90, 180, 0);

        particleSystem.emissionRate = 0;
        cloudSpeed = 4;
        cloudRotationSpeed = 2;
        velocityModule = cloudGuy.GetComponent<ParticleSystem>().velocityOverLifetime;
    }

    void RemoveStun()
    {
        isStunned = false;
        if(Spawner.Instance.powerUp)
            meshRenderer.material.mainTexture = powerUpFace;
    }
    void DieCondition()
    {
        if (location.position.y < mainCamera.transform.position.y - 7)
        {
            if (!isDead)
            {
                cloudGuy.GetComponent<MeshRenderer>().material.mainTexture = cloudCryingFace;
                isDead = true;
                rb.isKinematic = true;
                targetCloudPos = targetCamPos + new Vector3(3,4, 13.82f);
                targetCloudRot = new Vector3(-90, 210, 0);
                cloudGuy.GetComponent<ParticleSystem>().emissionRate = 0;
                AudioManager.Instance.Pause("wind");
                AudioManager.Instance.Stop("timer");
                windSoundPlaying = false;
                Spawner.Instance.GameOver();
            }
        }
    }
    void CharacterMovement()
    {
        if (hasBounced)
        {
            cloudSpeed = 1;
            if (location.position.y > mainCamera.transform.position.y - 2)
            {
                if (!Spawner.Instance.powerUp)
                    meshRenderer.material.mainTexture = bounceFace;
                targetCamPos = new Vector3(0, location.position.y + 2, -10);
                wall1.transform.position = new Vector3(4, location.position.y);
                wall2.transform.position = new Vector3(-4, location.position.y);
                particleSystem.emissionRate = rb.velocity.y * 4;
                if (Spawner.Instance.powerUpType == 3)
                {
                    if (Spawner.Instance.safetyNetMushroom != null)
                    {
                        Spawner.Instance.safetyNetMushroom.transform.position = new Vector3(0, mainCamera.transform.position.y - 6);
                    }
                }
            }
            else if (!Spawner.Instance.powerUp)
            {
                meshRenderer.material.mainTexture = defaultFace;
            }
        }
    }
    void CloudMovement()
    {
        if (!isDead)
        {
            if (direction.x == 0)
            {
                targetCloudPos = new Vector3(0.41f, location.position.y + 8, 3.82f);
                targetCloudRot = new Vector3(-90, 180, 0);
                cloudGuy.GetComponent<MeshRenderer>().material.mainTexture = defaultCloudFace;
                cloudGuy.GetComponent<ParticleSystem>().emissionRate = 0;
            }
            else if (direction.x < 1)
            {
                targetCloudPos = new Vector3(3, location.position.y + 8, 3.82f);
                targetCloudRot = new Vector3(-90, 210, 0);
                cloudGuy.GetComponent<MeshRenderer>().material.mainTexture = blowLeft;
                cloudGuy.GetComponent<ParticleSystem>().emissionRate = 30;
                velocityModule.xMultiplier = -10;
            }
            else
            {
                targetCloudPos = new Vector3(-3, location.position.y + 8, 3.82f);
                targetCloudRot = new Vector3(-90, 150, 0);
                cloudGuy.GetComponent<MeshRenderer>().material.mainTexture = blowRight;
                cloudGuy.GetComponent<ParticleSystem>().emissionRate = 30;
                velocityModule.xMultiplier = 10;
            }
        }
    }
    bool Stunned()
    {
        if (isStunned)
        {
            if (removeStun)
            {
                Invoke("RemoveStun", 2);
                removeStun = false;
            }
            if (windSoundPlaying)
            {
                AudioManager.Instance.Pause("wind");
                windSoundPlaying = false;
            }
            meshRenderer.material.mainTexture = stunnedFace;
            cloudGuy.GetComponent<MeshRenderer>().material.mainTexture = cloudStunnedFace;
            direction = new Vector3(0, 0);
            return true;
        }
        return false;
    }
    void InputHandler()
    {
//#if UNITY_EDITOR || UNITY_STANDALONE
//        if (Input.GetKey(KeyCode.LeftArrow))
//        {
//            direction = Vector3.left;
//            if (!windSoundPlaying)
//            {
//                AudioManager.Instance.Play("wind");
//                windSoundPlaying = true;
//            }
//        }
//        else if (Input.GetKey(KeyCode.RightArrow))
//        {
//            direction = Vector3.right;
//            if (!windSoundPlaying)
//            {
//                AudioManager.Instance.Play("wind");
//                windSoundPlaying = true;
//            }
//        }
//        else
//        {
//            direction = new Vector3(0, 0);
//            if (windSoundPlaying)
//            {
//                AudioManager.Instance.Stop("wind");
//                windSoundPlaying = false;
//            }
//        }
//#endif
//#if UNITY_ANDROID || UNITY_IPHONE
        //Debug.Log("test");
        if (Spawner.Instance.left)
        {
            direction = Vector3.left;
            if (!windSoundPlaying)
            {
                AudioManager.Instance.Play("wind");
                windSoundPlaying = true;
            }
        }
        else if (Spawner.Instance.right)
        {
            direction = Vector3.right;
            if (!windSoundPlaying)
            {
                AudioManager.Instance.Play("wind");
                windSoundPlaying = true;
            }
        }
        else
        {
            direction = new Vector3(0, 0);
            if (windSoundPlaying)
            {
                AudioManager.Instance.Pause("wind");
                windSoundPlaying = false;
            }
        }
//#endif
    }
    void Update()
    {
        DieCondition();
        CharacterMovement();
        CloudMovement();
        if (Stunned() || isDead)
            return;
        InputHandler();
    }
    void FixedUpdate()//lerps in fixed update were faster.
    {
        cloudGuy.transform.rotation = Quaternion.Lerp(cloudGuy.transform.rotation, Quaternion.Euler(targetCloudRot), cloudRotationSpeed * Time.fixedDeltaTime);
        cloudGuy.transform.position = Vector3.Lerp(cloudGuy.transform.position, targetCloudPos, cloudSpeed * Time.fixedDeltaTime);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, 5 * Time.fixedDeltaTime);
        rb.AddForce(direction * speed);
    }
}
