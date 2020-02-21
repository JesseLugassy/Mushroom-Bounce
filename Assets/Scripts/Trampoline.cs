using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float bouncinessY = 200;
    public float bouncinessX = 400;
    public bool dieOnTouch;

    public int pointValue;

    public Collider bottom;

    public Animator animator;

    public Renderer renderer;

    public ParticleSystem particleSystem;

    public Shader transparentShader;
    public Shader MultiplierTransparentShader;
    public MeshRenderer bottomMesh;
    //public MeshRenderer powerUpCylinder;

    public GameObject cloud;
    public Mesh plane;
    public Mesh defaultMesh;

    bool hit = false;
    bool frame = false;
    public virtual void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (hit)
                return;
            hit = true;


            if (Spawner.Instance.powerUpType != 2)
            {
                animator.Play("bounce");
                if (dieOnTouch)
                    AudioManager.Instance.Play("boing");
                else
                    AudioManager.Instance.Play("blop");
            }
            else if (!dieOnTouch)
            {
                animator.Play("bounce");
                AudioManager.Instance.Play("blop");
            }
            else
            {
                AudioManager.Instance.Play("swoosh");
            }
            //Debug.Log("test");
            int temp = pointValue;
            if(Spawner.Instance.powerUpType == 1)
            {
                temp *= 4;
            }

            Spawner.Instance.points += temp;
            Spawner.Instance.pointsText.text = Spawner.Instance.points.ToString();


            Player player = col.transform.root.GetComponent<Player>();

            player.hasBounced = true;

            foreach (Rigidbody a in player.gameObject.GetComponentsInChildren<Rigidbody>())
            {
                if (bouncinessY == 0)
                {
                    Vector3 ValueToSubtract = new Vector3(a.velocity.x, 0, 0);
                    a.velocity -= ValueToSubtract;
                }
                else
                    a.velocity = new Vector3(0, 0, 0);
            }


            //Debug.Log(player.velocity.y);
            float bounceAngle = -(transform.root.transform.rotation.z * bouncinessX);
            player.rb.AddForce(bounceAngle, bouncinessY, 0, ForceMode.Impulse);

            if (dieOnTouch)
            {
                GetComponent<Collider>().enabled = false;
                if (Spawner.Instance.powerUpType != 2)
                {
                    if (Spawner.Instance.powerUpType == 1)
                        GetComponent<SkinnedMeshRenderer>().material.shader = MultiplierTransparentShader;
                    else
                        GetComponent<SkinnedMeshRenderer>().material.shader = transparentShader;
                    bottomMesh.material.shader = transparentShader;
                    particleSystem.Play();
                }
                Spawner.Instance.trampolines.Remove(transform.root.gameObject);
                GetComponent<MeshCollider>().enabled = false;
                bottom.enabled = false;
                animator.speed = 1.5f; 
                animator.SetBool("DieOnTouch",true);
                //Destroy(transform.root.gameObject);
                Invoke("DestroyMe", .8f);
            }
        }
    }
    void DestroyMe()
    {
        Destroy(transform.root.gameObject);
    }
    void Update()
    {
        if (dieOnTouch)
        {
            if (Spawner.Instance.mainCamera.transform.position.y - 10 > transform.position.y)
            {
                Spawner.Instance.trampolines.Remove(transform.root.gameObject);
                Destroy(transform.root.gameObject);
            }
        }
        else if (hit)
        {
            if (frame)
            {
                hit = false; //wait a frame to stop a double collision adding excess force
                frame = false;
            }
            else
                frame = true;
        }
    }
}
