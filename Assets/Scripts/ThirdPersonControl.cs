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
	public Collider myCol;
	public float throwSpeed;
	public float pickupThrowDelay = 0.2f;
	GameObject heldObject;


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
				if(!heldObject){
					yield return StartCoroutine(PickupObject(myCol));
				}
				else{
					yield return StartCoroutine(ThrowObject());
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
        //If rotation in minute in difference, don't bother rotating
        if (direction != Vector3.zero)
        {
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rb.velocity.normalized)) > 0.01)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized), 0.2f);
            }
        }
    }

	IEnumerator PickupObject(Collider col){
		RaycastHit hit;
		if(Physics.BoxCast(col.transform.position, col.bounds.extents, transform.forward, out hit, transform.rotation, 1)){
			hit.transform.parent = transform;
			hit.transform.GetComponent<Rigidbody>().useGravity = false;
			hit.transform.GetComponent<Rigidbody>().isKinematic = true;
			heldObject = hit.transform.gameObject;
			yield return new WaitForSeconds(pickupThrowDelay);
		}
	}

	IEnumerator ThrowObject(){
		if(heldObject){
			heldObject.GetComponent<Rigidbody>().useGravity = true;
			heldObject.GetComponent<Rigidbody>().isKinematic = false;
			heldObject.GetComponent<Rigidbody>().AddForce(transform.forward * throwSpeed);
			heldObject.transform.parent = null;
			heldObject = null;
			yield return new WaitForSeconds(pickupThrowDelay);
		}
	}
}
