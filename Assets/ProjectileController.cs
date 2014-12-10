using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {


	private GameObject target;
	private GameObject owner;

}

//
//private var target:GameObject;
//private var owner:GameObject;
//
//function setTarget(newOwner:GameObject, newSpeed:float, newDamage:float, newTarget:GameObject)
//{
//	speed = newSpeed;
//	damage = newDamage;
//	target = newTarget;
//	owner = newOwner;
//}
//
//function Start () {
//	
//}
//
//function Update()
//{
//	
//}
//
//function FixedUpdate () {
//	if(target == null)
//	{
//		if(networkView.isMine)
//		{
//			NetworkManager.Destroy(gameObject);
//		}
//		return;
//	}
//	
//	var targetPosition:Vector3 = Vector3(target.transform.position.x, target.transform.position.y + 0.5, target.transform.position.z);
//	
//	var directionVector:Vector3 = targetPosition - transform.position;
//	if(directionVector.x > 0)
//	{
//		transform.localScale.x = Mathf.Abs(transform.localScale.x);
//	}
//	else
//	{
//		transform.localScale.x = -Mathf.Abs(transform.localScale.x);
//	}
//	transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed*Time.fixedDeltaTime);
//	
//	if(transform.position == targetPosition)
//	{
//		NetworkManager.Destroy(gameObject);
//		
//		if (networkView.isMine) {
//			if(target.GetComponent.<EntityController>() != null)
//			{
//				target.GetComponent.<EntityController>().attackEntity(damage, owner.networkView.viewID,false);
//			}
//			else if(target.GetComponent.<PlayerController>() != null)
//			{
//				target.GetComponent.<PlayerController>().attackPlayer(damage, owner.networkView.viewID,false);
//			}
//			
//			return;
//		}
//	}
//}
