using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonControl : MonoBehaviour {

	Animator anim;
	Rigidbody rb;
	public float speed;
	public float jumpSpeed;
	Transform trans;

	//For grabbing and throwing
	public GrabbingMechanic grabbing;
	public float throwingSpeed = 10;

	//Stats for Character
	[Header("Stats")]
	public float hp = 10;
	public int strength = 1;


	// Use this for initialization
	void Start () {
		GameObject obj = new GameObject();
		trans = obj.transform;
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		StartCoroutine(CharacterLoop());
	}

	// Update is called once per frame
	//void Update () {
	//	Movement();
	//	PickupObject(myCol);
	//	ThrowObject();
	//}

	IEnumerator CharacterLoop(){
		while(true){
			Movement();
			if (Input.GetKeyDown(KeyCode.Z))
			{
				if(!grabbing.heldObject){
					yield return StartCoroutine(grabbing.PickupObject(strength));
				}
				else{
					yield return StartCoroutine(grabbing.ThrowObject(throwingSpeed));
				}
			}
			yield return null;
		}
	}

	Vector3 GetDirectionInput()
    {
		//input = control.MoveInput;
		//input *= speed * Time.deltaTime;
		return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * speed * 10 * Time.deltaTime;
    }

	void Movement(){
		trans.position = Camera.main.transform.position;
		trans.rotation = Camera.main.transform.rotation;
		trans.eulerAngles = new Vector3(0, trans.rotation.eulerAngles.y, 0);
		Jump();
		//if (input.y > -maxVelChange)
		//{
		//    input.y += currentGravity;
		//}
		Vector3 input = GetDirectionInput();
		input.y = rb.velocity.y;
        rb.velocity = trans.transform.TransformDirection(input);
        //input.y = 0;
        if (input != Vector3.zero)
        {
            Vector3 tempVel = rb.velocity;
            tempVel.y = 0;
            //Use FaceTowardsMovement
            FaceTowardsDirection(tempVel);
        }

        anim.SetBool("Moving", input.magnitude > 0.2f);
        //isGrounded = false;
	}

	void Jump(){
		if(Input.GetKeyDown(KeyCode.Space)){
			rb.AddForce(0, jumpSpeed * 10, 0);
		}
	}

	void FaceTowardsDirection(Vector3 direction)
    {
        //If rotation is minute in difference, don't bother rotating
        if (direction != Vector3.zero)
        {
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rb.velocity.normalized)) > 0.01)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized), 0.1f);
            }
        }
    }
}
