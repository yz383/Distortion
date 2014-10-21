﻿using UnityEngine;
using System.Collections;

public class SingularityEffector : DeformationEffector {

	private float gravitationalForce = 98f;
	private float gravitationalRadius = 10.0f;

	public override bool ObjectInRange ( Transform obj ) {
		return (this.transform.position - obj.position).sqrMagnitude < 2500.0f;
	}

	/* This function has been combined with "transform vertex" for efficiency's sake. 
	We're already performing the vector subtraction */
	/*public override bool VertexInRange ( Vector3 ms_position, Vector3 vertex ) {
		return (ms_position - vertex).sqrMagnitude < 1000.0f;
	}*/

	public override Vector3 TransformVertex ( Vector3 ms_position, Vector3 vertex ) {
		float offsetMag = (ms_position - vertex).sqrMagnitude;

		if ( offsetMag < 1000.0f ) {
			float strength = GameStateManager.instance.maxSingularityStrength;
			float maxOffset = GameStateManager.instance.maxSingularityDisplacement;

			float mag = strength / offsetMag;
			return Vector3.MoveTowards( vertex, ms_position, Mathf.Min( mag, maxOffset ) );
		}
		
		return vertex;
	}

	public void FixedUpdate () {
		// fetch objects within a given radius.
		Collider[] others = Physics.OverlapSphere( this.transform.position, this.gravitationalRadius );
		foreach ( Collider other in others ) {
			if ( other.rigidbody ) {
				Vector3 offsetVec = (this.transform.position - other.transform.position);
				other.rigidbody.AddForce( GameStateManager.instance.singularityGravityMultiplier * offsetVec.normalized * this.gravitationalForce / offsetVec.sqrMagnitude, ForceMode.Acceleration );
			}
		}
	}

}