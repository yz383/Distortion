using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
Here, a DeformableMesh object is defined. this allows us to make a nice little copy of an existing
mesh, and modify it without fear of damaging the assets themselves. This also lets us maintain a link
between the gameObject, meshCollider, and meshFilter components for future use.
*/
[System.Serializable]
public class DeformableMesh {
	// the transform of the MeshFilter to which this mesh is associated.
	public Transform transform;
	// The mesh filter that is being used as the deformable mesh object.
	public MeshFilter meshFilter;
	// the collider which is linked to this meshFilter
	public MeshCollider meshCollider;
	// a copy of the original mesh at the start of the game. This will allow us to "reset" the deformation,
	// without too much worry.
	private Mesh baseMesh;

	// The constructor just takes a mesh filter, and collider component (optional). These will be used to
	// build the DeformableMesh object.
	public DeformableMesh ( MeshFilter meshFilter, MeshCollider meshCollider = null ) {
		this.transform = meshFilter.transform;
		this.meshFilter = meshFilter;
		this.meshCollider = meshCollider;
		this.baseMesh = CopyMesh( meshFilter.mesh );
		//this.displacementMap = new Texture2D( GameStateManager.instance.deformationMapResolution, GameStateManager.instance.deformationMapResolution );
	}

	//public Texture2D displacementMap;

	// this function will return the nearest point on a triangle mesh in 3D, note that this is computationally
	// expensive, and shouldn't really be called very often.
	// information concerning barycentric coordinates found here. http://answers.unity3d.com/questions/424974/nearest-point-on-mesh.html
	/*private Vector3 NearestPointOnMesh ( Vector3 point ) {
        Vector3 ms_point = this.transform.InverseTransformPoint( point );
        float minDist = Mathf.Infinity;

        float cutoffDist = GameStateManager.instance.collisionCorrectionTriangleThreshold;
        cutoffDist *= cutoffDist;

        Vector3 returnPoint = point;

		for ( int tri = 0; tri < this.meshFilter.mesh.triangles.Length; tri += 3 ) {
	        //Get the vertices of the triangle
			Vector3 a = this.meshFilter.mesh.vertices[ this.meshFilter.mesh.triangles[tri+0] ];
			Vector3 b = this.meshFilter.mesh.vertices[ this.meshFilter.mesh.triangles[tri+1] ];
			Vector3 c = this.meshFilter.mesh.vertices[ this.meshFilter.mesh.triangles[tri+2] ];
			// get the center of the triangle. we'll use this to early-out on the computation, if the triangle is below a given tolerance.
			Vector3 center = 0.333f * (a + b + c);
			if ( (ms_point - center).sqrMagnitude < cutoffDist ) {

				// calculate the normal.
				Vector3 n = Vector3.Cross((b-a).normalized, (c-a).normalized);
		        // and project our point onto the triangle plane.
		        Vector3 projected = ms_point + Vector3.Dot((a - ms_point), n) * n;

		        // Calculate barycentric coordinates for this triangle...
		        float u = ((projected.x * b.y) - (projected.x * c.y) - (b.x * projected.y) + (b.x * c.y) + (c.x * projected.y) - (c.x  * b.y)) / 
		        			((a.x * b.y)  - (a.x * c.y)  - (b.x * a.y) + (b.x * c.y) + (c.x * a.y)  - (c.x * b.y));
	        	float v = ((a.x * projected.y) - (a.x * c.y) - (projected.x * a.y) + (projected.x * c.y) + (c.x * a.y) - (c.x * projected.y))/
	                		((a.x * b.y)  - (a.x * c.y)  - (b.x * a.y) + (b.x * c.y) + (c.x * a.y)  - (c.x * b.y));
	        	float w = ((a.x * b.y) - (a.x * projected.y) - (b.x * a.y) + (b.x * projected.y) + (projected.x * a.y) - (projected.x * b.y))/
	                		((a.x * b.y)  - (a.x * c.y)  - (b.x * a.y) + (b.x * c.y) + (c.x * a.y)  - (c.x * b.y));

	            // now, find the nearest point on this triangle.
				Vector3 vector = (new Vector3(u,v,w)).normalized;

				// and convert it into actual model-space coordinates.
				Vector3 nearest = a * vector.x + b * vector.y + c * vector.z;
				
				// if the nearest point on this triangle is closer than the previous best, update our "best value".
				float dist = (nearest-point).sqrMagnitude;
				if ( dist < minDist ) {
					minDist = dist;
					returnPoint = nearest;
				}
			}
		}

		// now, just return the nearest point on the mesh!
		return returnPoint;
	} */

	// the CopyMesh function just creates a new mesh object free of external references, with the same data
	// as a given mesh. This is used on startup to ensure that all original mesh data is preserved.
	private Mesh CopyMesh ( Mesh mesh ) {
		Mesh returnMesh = new Mesh();
		returnMesh.vertices = mesh.vertices;
		returnMesh.normals = mesh.normals;
		returnMesh.uv = mesh.uv;
		returnMesh.triangles = mesh.triangles;
		returnMesh.tangents = mesh.tangents;

		return returnMesh;
	}

	// The resetMesh function just sets the modified components of a mesh back to those we cached at the start
	public void ResetMesh () {
		if ( this.meshFilter != null )
			this.meshFilter.mesh.vertices = this.baseMesh.vertices;
		if ( this.meshCollider != null ) {
			this.meshCollider.sharedMesh = null;
			this.meshCollider.sharedMesh = this.meshFilter.mesh;
		}
	}

	// work in progress funciton to reduce geometric complexity for automatic collision geometry calculation.
	// Look into Vertex Clustering decimation algorithms.
	/*public Mesh SimplifyMesh ( Mesh mesh, float cell_size ) {
		// make a mutable array out of mesh properties
		List<Vector3> vertices = new List<Vector3>(mesh.vertices);
		List<int> triangles = new List<int>(mesh.triangles);

		// now, apply our changes, and return.
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		return mesh;
	}*/

	public void DrawShittySphere ( Vector3 point, float radius, Color color, float duration ) {
		Debug.DrawLine( point + Vector3.right * radius, point + Vector3.forward * radius, color, duration );
		Debug.DrawLine( point + Vector3.forward * radius, point + Vector3.right * radius, color, duration );
		Debug.DrawLine( point + -Vector3.right * radius, point + -Vector3.forward * radius, color, duration );
		Debug.DrawLine( point + -Vector3.forward * radius, point + Vector3.right * radius, color, duration );

		Debug.DrawLine( point + Vector3.right * radius, point + Vector3.up * radius, color, duration );
		Debug.DrawLine( point + Vector3.forward * radius, point + Vector3.up * radius, color, duration );
		Debug.DrawLine( point + -Vector3.right * radius, point + Vector3.up * radius, color, duration );
		Debug.DrawLine( point + -Vector3.forward * radius, point + Vector3.up * radius, color, duration );

		Debug.DrawLine( point + Vector3.right * radius, point + -Vector3.up * radius, color, duration );
		Debug.DrawLine( point + Vector3.forward * radius, point + -Vector3.up * radius, color, duration );
		Debug.DrawLine( point + -Vector3.right * radius, point + -Vector3.up * radius, color, duration );
		Debug.DrawLine( point + -Vector3.forward * radius, point + -Vector3.up * radius, color, duration );
	}

	// The DeformMesh function is called from the deformation manager, and takes a list of active effectors.
	// For each of these effectors, the model-space position is calculated, and vertices are transformed in 
	// model-space, preventing the calculation required to convert individual vertices to world-space. This
	// function will update the meshFilter, and the mesh collider provided that one exists.
	public void DeformMesh ( DeformationEffector effector ) {
		// get the model-space position of the effector.
		Vector3 ms_position = this.transform.InverseTransformPoint( effector.transform.position );

		// pull in the vertices for this mesh,
		if ( effector.ObjectInRange( this.transform ) ) {
			Vector3[] vertices = this.meshFilter.mesh.vertices;
			//Vector3[] normals = this.meshFilter.mesh.normals;

			for ( int index = 0; index < vertices.Length; index ++ ) {
				// and transform each vertex.
				if ( effector.VertexInRange( ms_position, vertices[index] ) ) {
					//Vector3 start = vertices[ index ];
					vertices[index] = effector.TransformVertex( ms_position, vertices[ index ] );
					//Vector3 end = vertices[ index ];

					// slow and bad, and bad and slow, but it'll solve most of the collision issues :\
					/*Vector3 midpoint = this.transform.TransformPoint( (start + end) / 2 );
					float radius = Vector3.Project( end-start, normals[index] ).magnitude/2;

					DrawShittySphere( midpoint, radius, Color.yellow, 10.0f );
					//Debug.DrawLine( this.transform.TransformPoint( start ), this.transform.TransformPoint(start + normals[index] * radius*2), Color.red, 10.0f );

					Collider[] others = Physics.OverlapSphere( midpoint, radius, Physics.AllLayers^(1<<LayerMask.NameToLayer("Deformable")) );
					foreach ( Collider other in others ) {
						other.transform.position += this.transform.TransformDirection( normals[index] ) * radius;

					}*/
					
				}

				//Color displacementColor = new Color( 0.5f + displacement.x / 10.0f, 0.5f + displacement.y / 10.0f, 0.5f + displacement.z / 10.0f );
				//this.displacementMap.SetPixel( (int)(this.displacementMap.width * uv[ index ].x), (int)(this.displacementMap.height * uv[ index ].y), displacementColor );
			}
			//this.displacementMap.Apply();
			
			// lastly, update the mesh data.
			this.meshFilter.mesh.vertices = vertices;
			this.meshFilter.mesh.RecalculateBounds();
			//this.meshFilter.mesh.RecalculateNormals();

			// if the mesh collider exists, set it to use the same mesh as the visual model.
			if ( this.meshCollider != null ) {
				Mesh collisionMesh = this.CopyMesh( this.meshFilter.mesh );
				//this.SimplifyMesh( collisionMesh, 0.25f );

				this.meshCollider.sharedMesh = null;
				this.meshCollider.sharedMesh = collisionMesh;
			}
		}
	}

}


/*
This script will be applied to deformation targets automatically by the deformation manager, and will generate 
DeformableMesh objects for each meshFilter in the object hierarchy.
*/
public class DeformationTarget : MonoBehaviour {
	// a list of all deformable meshes in this object and its children
	public DeformableMesh mesh; 
	// when a mesh is deformed, we need to correct collision by moving objects out of the volume. To do this,
	// keep track of all the objects currently interacting with the volume.
	private Dictionary<int, Collision> trackedCollisions = new Dictionary<int, Collision>();

	// When this script is first instantiated, fetch all meshFilters in our hierarchy, and build deformable
	// mesh objects for them.
	public void Awake () {
		//MeshFilter[] filters = gameObject.GetComponentsInChildren<MeshFilter>();
		//this.meshes = new DeformableMesh[ filters.Length ];

		//for ( int index = 0; index < filters.Length; index ++ )
		//	this.meshes[ index ] = new DeformableMesh( filters[ index ], filters[ index ].GetComponent<MeshCollider>() );
		this.mesh = new DeformableMesh( this.gameObject.GetComponent<MeshFilter>(), this.gameObject.GetComponent<MeshCollider>() );
	}

	public void OnCollisionEnter ( Collision collision ) {
		Debug.Log( "DEBUG: collision information updated" );
		this.trackedCollisions[ collision.gameObject.GetInstanceID() ] = collision;
	}

	public void OnCollisionExit ( Collision collision ) {
		Debug.Log( "DEBUG: collision information removed" );
		this.trackedCollisions.Remove( collision.gameObject.GetInstanceID() );
	}

	public void OnCollisionStay ( Collision collision ) {
		Debug.Log( "DEBUG: collision information updated" );
		this.trackedCollisions[ collision.gameObject.GetInstanceID() ] = collision;
	}

	// The deformation manager will call this function, and it will iterate through all meshes and sub-meshes, and will call
	// their respective deformation functions.
	public void DeformMesh ( List<DeformationEffector> effectors ) {
		//foreach ( DeformableMesh mesh in this.meshes ) {
			// reset the mesh to it's base
			this.mesh.ResetMesh();

			// and for every effector,
			foreach ( DeformationEffector effector in effectors ) {
				// apply that effector's changes.
				this.mesh.DeformMesh( effector );
			}
		//}

		/*
		// now, use our list of intersecting objects to push other objects out of the geometry.
		foreach ( KeyValuePair<int,Collision> collision in this.trackedCollisions ) {
			// we want to move the object out along its incoming velocity vector until we hit the new surface. To do this,
			// just raycast from the object's position a ways out, and find the point of impact.
			Vector3 contact_offset = collision.Value.transform.position - collision.Value.contacts[0].point;
			Vector3 ray_dir = -collision.Value.contacts[0].normal;
			Vector3 ray_pos = collision.Value.contacts[0].point - ray_dir * 5.0f;

			RaycastHit hit;
			if ( Physics.Raycast( ray_pos, ray_dir, out hit, 5.0f ) ) {
				// then, set the position of the colliding object to the new contact point, applying the offset from the initial contact.
				collision.Value.transform.position = hit.point + contact_offset;
			}
		}*/
	}

}
