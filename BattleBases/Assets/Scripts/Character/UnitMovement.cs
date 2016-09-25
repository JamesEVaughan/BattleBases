using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour 
{
	// Put things to change in Unity editor here
	/// <summary>
	/// How many units can we walk each turn
	/// </summary>
	public float WalkSpeed = .5f;
	/// <summary>
	/// The time each turn takes
	/// </summary>
	public static float TurnTime = 0.5f;

	// Fields
	/// <summary>
	/// Represents the token's rigidbody
	/// </summary>
	private Rigidbody tokenRG;
	/// <summary>
	/// This is the direction we are walking in
	/// </summary>
	private Vector3 direction;
	/// <summary>
	/// How far we move in a given walking turn
	/// </summary>
	private Vector3 walkDistance;
	/// <summary>
	/// When we start walking again.
	/// </summary>
	private float nextWalk;
	/// <summary>
	/// True if this unit is walking
	/// </summary>
	private bool isWalking;
	/// <summary>
	/// Ture if this unit is in combat
	/// </summary>
	private bool isFighting;
	/// <summary>
	/// When the walk will end
	/// </summary>
	private float endWalk;

	// Initialization
	void Awake()
	{
		// Grab a reference to our RigidBody
		tokenRG = GetComponent<Rigidbody>();

		// Get a quick reference to our direction
		direction = transform.TransformDirection(Vector3.left);

		// We start walking, but not fighting on awake
		StartWalking(Time.time);
		isFighting = false;
	}

	// Physics update
	void FixedUpdate()
	{
		// First, are we currently fighting?
		if (isFighting) 
		{
			// Then we're not moving
			return;
		}

		// Should we start walking?
		if (!isWalking && nextWalk > Time.fixedTime) 
		{
			StartWalking (Time.fixedTime);
		}

		if (isWalking) 
		{
			Walk (Time.fixedTime);
		}

	}

	// Helper functions
	/// <summary>
	/// Moves unit based on
	/// </summary>
	private void Walk(float t)
	{
		isWalking = endWalk < t;
	}
	/// <summary>
	/// Set the unit to walking
	/// </summary>
	/// <param name="t">The time this update started</param>
	private void StartWalking(float t)
	{
		isWalking = true;
		endWalk = t + TurnTime;
	}
}
