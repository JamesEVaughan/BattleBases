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
	/// How long to wait for the next walking turn
	/// </summary>
	public float TurnTime = 0.5f;

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
	/// True if this unit is walking
	/// </summary>
	private bool isWalking;
	/// <summary>
	/// Ture if this unit is in combat
	/// </summary>
	private bool isFighting;
	/// <summary>
	/// Long we've been walking
	/// </summary>
	private float curWalkTime;
	/// <summary>
	/// Where we started walking
	/// </summary>
	private Vector3 WalkStart;
	/// <summary>
	/// Where we'll end up at the end of our walk
	/// </summary>
	private Vector3 WalkDest;
	/// <summary>
	/// When to start walking again
	/// </summary>
	private float nextWalk;
	/// <summary>
	/// The tag of our enemies
	/// </summary>
	private string enemyTag;

	// Initialization
	void Awake()
	{
		// Grab a reference to our RigidBody
		tokenRG = GetComponent<Rigidbody>();

		// Get a quick reference to our direction
		direction = transform.TransformDirection(Vector3.left);

		// We start walking, but not fighting on awake
		StartWalking();
		isFighting = false;

		enemyTag = (this.tag == "Blue") ? "Red" : "Blue";
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
		if (!isWalking && nextWalk < Time.fixedTime) 
		{
			float tempy = Time.fixedTime;
			StartWalking ();
		}

		if (isWalking) 
		{
			// Tell Walk() how long has passed
			Walk (Time.deltaTime);
		}

	}

	void OnTriggerEnter(Collider other)
	{
		// Let's find out what entered our trigger
		string tempTag = other.tag;

		if (tempTag == enemyTag) {
			// We're fighting now!
			StartFighting (other.gameObject);
		} 
		else 
		{
			// Otherwise, just stop moving
			isWalking = false;
		}
	}

	/// <summary>
	/// Called when we start fighting, will fire a fighting event
	/// </summary>
	/// <param name="enemy">The enemy we're fighting against</param>
	void StartFighting(GameObject enemy)
	{
		isFighting = true;
	}

	// Helper functions
	/// <summary>
	/// Moves unit forward, using interpolation
	/// </summary>
	private void Walk(float t)
	{
		curWalkTime += t;

		// If this is longer than we're supposed to walk
		if (curWalkTime > TurnTime) 
		{
			// Make it the last turn
			curWalkTime = TurnTime;

			// Stop walking after this time
			isWalking = false;

			// Start walking after TurnTime
			nextWalk = Time.fixedTime + TurnTime;
		}

		transform.position = Vector3.Lerp (WalkStart, WalkDest, curWalkTime / TurnTime);
	}
	/// <summary>
	/// We're about to start walking, setup our variables
	/// </summary>
	/// <param name="t">The time this update started</param>
	private void StartWalking()
	{
		// Let the world know we are walking
		isWalking = true;

		curWalkTime = 0f;

		// Caculate our destination
		WalkStart = transform.position;
			
		WalkDest = transform.position + (direction * WalkSpeed);
	}
}
