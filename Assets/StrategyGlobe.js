#pragma strict

public var reference : GameObject;

function Start () {
	renderer.material.SetColor("_TintColor", Color(0.0, 1.0, 0.2, 0.1));
}

function Update () {
	this.transform.localRotation = Quaternion.Inverse(reference.transform.rotation);
}