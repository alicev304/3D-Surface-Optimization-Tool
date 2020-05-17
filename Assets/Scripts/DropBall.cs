using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropBall : MonoBehaviour
{
    private GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        ball = gameObject;
    }

    public void dropBall()
    {
        ball.transform.position = new Vector3(0, 20, 0);
        Rigidbody rigidbody = ball.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
    }
}
