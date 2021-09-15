using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("I am alive!");
        Debug.Log(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= 20f) {
            transform.Translate(1f * Time.deltaTime, 1f * Time.deltaTime, 0);
        }
    }
}
