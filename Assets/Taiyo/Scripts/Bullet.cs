using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float lifeTime;

    float timer = 0;


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
