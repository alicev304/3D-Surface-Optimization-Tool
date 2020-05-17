using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Solution : MonoBehaviour
{
    public Text solution;
    private Rigidbody ball;

    // Start is called before the first frame update
    void Start() 
    {
        ball = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ball.IsSleeping())
        {
            solution.text = "Solution\nX = " + ball.position.x + "\nY = " + ball.position.y + "\nZ = " + ball.position.z;
        }
    }
}
