using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    public Renderer wing1;
    public Renderer wing2;

    public int type;

    void Update()
    {
        if (Spawner.Instance.mainCamera.transform.position.y - 10 > transform.position.y)
        {
            Destroy(transform.parent.gameObject);
            Spawner.Instance.SpawnCollectable();
        }
    }
}
