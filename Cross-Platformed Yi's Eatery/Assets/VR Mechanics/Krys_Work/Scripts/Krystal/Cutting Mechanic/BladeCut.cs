using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
public class BladeCut : MonoBehaviour {

	#region Commented out code
	/* 	
	void Update(){

		if(Input.GetMouseButtonDown(0)){
			RaycastHit hit;

			if(Physics.Raycast(transform.position, transform.forward, out hit)){

				GameObject victim = hit.collider.gameObject;

				GameObject[] pieces = BLINDED_AM_ME.MeshCut.Cut(victim, transform.position, transform.right, capMaterial);

				if(!pieces[1].GetComponent<Rigidbody>())
					pieces[1].AddComponent<Rigidbody>();

				Destroy(pieces[1], 1);
			}

		}
	}
	void OnDrawGizmosSelected() {

		Gizmos.color = Color.green;

		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5.0f);
		Gizmos.DrawLine(transform.position + transform.up * 0.5f, transform.position + transform.up * 0.5f + transform.forward * 5.0f);
		Gizmos.DrawLine(transform.position + -transform.up * 0.5f, transform.position + -transform.up * 0.5f + transform.forward * 5.0f);

		Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.5f);
		Gizmos.DrawLine(transform.position,  transform.position + -transform.up * 0.5f);

	}
	*/
	#endregion

	public Material capMaterial;

	//cut the mesh of the object when the blade hits the object
	private void OnCollisionEnter(Collision collision)
	{

		if (collision.gameObject.GetComponent<TempIngredient>())
		{
			//get collision ingredient properties
			TempIngredient collisionProperties = collision.gameObject.GetComponent<TempIngredient>();

			if (collision.gameObject.GetComponent<TempIngredient>().canBeCut)
			{
				//ensure that the object is only cut once
				if (collisionProperties.isColliding)
				{
					return;
				}
				else
				{
					Debug.Log("Knife entered " + collision.gameObject.name);
					collisionProperties.isColliding = true;
				}

				GameObject ogGameObj = collision.gameObject;
				string ogGameObjName = collision.gameObject.name;

				GameObject victim = collision.collider.gameObject;

				GameObject[] pieces = BLINDED_AM_ME.MeshCut.Cut(victim, transform.position, transform.right, capMaterial);



				//hold the index number of the piece
				int pieceIndexNum = 0;

				foreach (GameObject piece in pieces)
				{
					piece.transform.parent = ogGameObj.transform.parent;

					//rename each piece after the original parent + their index num
					piece.gameObject.name = ogGameObjName + "_piece" + pieceIndexNum;
					Debug.Log("cut piece " + Array.IndexOf(pieces, piece) + ": " + piece.gameObject.name);
					pieceIndexNum++;

					AddCollidersToNewPiece(piece);

					if (!piece.gameObject.GetComponent<TempIngredient>())
						piece.gameObject.AddComponent<TempIngredient>(ogGameObj.GetComponent<TempIngredient>()); //Note: Get obj data from CSV reader in future


				}//end of foreach


			}//end of if ingredient can be cut statement


		}//end of if collider has ingredient script statement


	}//end of OnCollisionEnter



	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.GetComponent<TempIngredient>())
		{
			collision.gameObject.GetComponent<TempIngredient>().isColliding = false;
		}

	}


	//add colliders to new pieces
	void AddCollidersToNewPiece(GameObject newObj)
	{
		//remove collider from gameobj
		if (newObj.GetComponent<Rigidbody>())
			newObj.GetComponent<Rigidbody>().useGravity = false;

		if (newObj.GetComponent<BoxCollider>())
			Destroy(newObj.gameObject.GetComponent<Collider>());

		//replace collider
		if (!newObj.GetComponent<BoxCollider>())
			newObj.AddComponent<BoxCollider>();

		//add a rigidbody and collider to each piece
		if (!newObj.GetComponent<Rigidbody>())
			newObj.AddComponent<Rigidbody>();

		newObj.GetComponent<Rigidbody>().useGravity = true;
	}




}
