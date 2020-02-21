using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector3 target;
    public bool move = false;
    void Update()
    {
        if (move)
        {
            transform.position = Vector3.Lerp(transform.position, target, 1 * Time.deltaTime);
            if (transform.position == target)
                move = false;
        }
        if (Spawner.Instance.mainCamera.transform.position.y - 10 > transform.position.y)
        {
            Spawner.Instance.enemies.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
