using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameStateManager : MonoBehaviour {

	// A static instance reference for the singleton pattern. 
	private static GameStateManager _instance;

	// Debug Parameters
	public float maxSingularityStrength = 200.0f;
	public float maxSingularityDisplacement = 5.0f;
	public float singularityGravityMultiplier = 1.0f;

	// This defines a getter for the instance variable. When this static variable is called, it will
	// return the currently active manager instance, allowing for singleton style access.
	public static GameStateManager instance {
		get {
			if ( _instance == null )
				_instance = Object.FindObjectOfType<GameStateManager>();
			return _instance;
		}
	}

	private bool DEBUG_devToolsVisible = false;
	public void OnGUI () {
		if ( Input.GetKeyDown("`") )
			this.DEBUG_devToolsVisible = true;

		if ( this.DEBUG_devToolsVisible ) {
			GUILayout.BeginArea( new Rect( 10, 10, 300, 500 ), "DEBUG", "window" );
			GUILayout.Label( "Max Singularity Strength: " + this.maxSingularityStrength );
			this.maxSingularityStrength = GUILayout.HorizontalSlider( this.maxSingularityStrength, 50, 1000 );
			GUILayout.Label( "Max Singularity Displacement: " + this.maxSingularityDisplacement );
			this.maxSingularityDisplacement = GUILayout.HorizontalSlider( this.maxSingularityDisplacement, 1, 10 );
			GUILayout.Label( "Singularity Gravity Multiplier: " + this.singularityGravityMultiplier );
			this.singularityGravityMultiplier = GUILayout.HorizontalSlider( this.singularityGravityMultiplier, 0, 2 );
			
			GUILayout.EndArea();
		}

	}

}
