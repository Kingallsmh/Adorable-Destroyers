using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonControl : MonoBehaviour {

	Rigidbody rb;
	public float speed;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		Movement();
	}

	Vector3 GetDirectionInput()
    {
		//input = control.MoveInput;
		//input *= speed * Time.deltaTime;
		return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * speed * 10 * Time.deltaTime;
    }

	void Movement(){
		Transform trans = Camera.main.transform;
		trans.eulerAngles = new Vector3(0, trans.rotation.eulerAngles.y, 0);
		//input.y = rb.velocity.y;
		//Jump();
		//if (input.y > -maxVelChange)
		//{
		//    input.y += currentGravity;
		//}
		Vector3 input = GetDirectionInput();

        rb.velocity = trans.transform.TransformDirection(input);
        //input.y = 0;
        if (input != Vector3.zero)
        {
            Vector3 tempVel = rb.velocity;
            tempVel.y = 0;
            //Use FaceTowardsMovement
            FaceTowardsDirection(tempVel);
        }

        //anim.SetBool("Moving", input.magnitude > 0.1);
        //isGrounded = false;
	}

	void FaceTowardsDirection(Vector3 direction)
    {
        //If rotation in minute in difference, don't bother rotating
        if (direction != Vector3.zero)
        {
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rb.velocity.normalized)) > 0.01)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized), 0.2f);
            }
        }
    }
}
