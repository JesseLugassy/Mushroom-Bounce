using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnter : MonoBehaviour
{
    public void OnTriggerEnter(Collider col)
    {
        Player player = transform.root.GetComponent<Player>();
        if (col.gameObject.tag == "butterfly")
        {
            player.meshRenderer.material.mainTexture = player.powerUpFace;
            Collectable c = col.gameObject.GetComponent<Collectable>();
            Spawner.Instance.powerUp = true;
            Spawner.Instance.powerUpType = c.type;
            Destroy(c.transform.parent.gameObject.gameObject);
            AudioManager.Instance.Play("wow");
        }
        else if(col.gameObject.tag == "bee")
        {
            Enemy enemy = col.gameObject.transform.root.GetComponent<Enemy>();
            enemy.move = true;
            enemy.target = enemy.transform.position + new Vector3(5, 5, -5);
            player.isStunned = true;
            player.removeStun = true;
            AudioManager.Instance.Play("bee");
            col.enabled = false;
        }
    }
}
