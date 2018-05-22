using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingMechanic : MonoBehaviour {

	public GameObject heldObject;
	public float pickupThrowDelay = 0.2f;

	GameObject highlightedObject;

	public IEnumerator PickupObject(int strength)
    {
        //ExtDebug.DrawBoxCastBox(transform.position, boxSize, transform.rotation, transform.forward, 1, Color.blue);
		if (highlightedObject)
		{
			if (highlightedObject.GetComponent<Rigidbody>().mass <= strength)
            {
				highlightedObject.transform.parent = transform;
				highlightedObject.transform.GetComponent<Rigidbody>().useGravity = false;
				highlightedObject.transform.GetComponent<Rigidbody>().isKinematic = true;
				heldObject = highlightedObject.transform.gameObject;
                yield return new WaitForSeconds(pickupThrowDelay);
            }
        }
    }

	public IEnumerator ThrowObject(float throwSpeed)
    {
        if (heldObject)
        {
            heldObject.GetComponent<Rigidbody>().useGravity = true;
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
			heldObject.GetComponent<Rigidbody>().velocity = (transform.forward * throwSpeed) + (transform.up*throwSpeed);
            heldObject.transform.parent = null;
            heldObject = null;
            yield return new WaitForSeconds(pickupThrowDelay);
        }
    }

	private void OnTriggerEnter(Collider other)
	{
		highlightedObject = other.gameObject;
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.gameObject == highlightedObject){
			highlightedObject = null;
		}
	}
}
