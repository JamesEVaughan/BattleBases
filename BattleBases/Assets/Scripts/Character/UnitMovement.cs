using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour 
{
	// Put things to change in Unity editor here
	/// <summary>
	/// How many units can we walk each turn
	/// </summary>
	public float WalkSpeed = 1f;
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
	/// Ture if this unit can walk, as in is not blocked
	/// </summary>
	private bool canWalk;
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

	//Properties
	/// <summary>
	/// Long we've been walking
	/// </summary>
	public float CurWalkTime { get; private set; }
	/// <summary>
	/// True if this unit is walking
	/// </summary>
	public bool IsWalking { get; private set; }

	// Events
	public event CombatEventHandler DeathEvent;


	// Initialization
	void Awake()
	{
		// Grab a reference to our RigidBody
		tokenRG = GetComponent<Rigidbody>();

		// Get a quick reference to our direction
		direction = transform.TransformDirection(Vector3.left);

		// We start walking
		StartWalking();
		canWalk = true;

		enemyTag = (this.tag == "Blue") ? "Red" : "Blue";
	}

	// Physics update
	void FixedUpdate()
	{
		// First, are we allowed to walk
		if (!canWalk) 
		{
			// Then we're not moving
			return;
		}

		// Should we start walking?
		if (!IsWalking && nextWalk < Time.fixedTime) 
		{
			float tempy = Time.fixedTime;
			StartWalking ();
		}

		if (IsWalking) 
		{
			// Tell Walk() how long has passed
			Walk (Time.deltaTime);
		}

	}

	/// <summary>
	/// Fired when something enters our front collider
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter(Collider other)
	{
		// First, did we enter someone else's Trigger box?
		if (!other.isTrigger)
		{
			// Something is in our collider, don't walk
			canWalk = false;

			// If this is a friendly unit, listen fo death events
			UnitMovement otherUM = other.GetComponent<UnitMovement> ();
			if (otherUM != null && otherUM.enemyTag == enemyTag)
			{
				otherUM.DeathEvent += FriendDeath;
			}
		} 

		// If we entered someone else trigger, don't do anything

		// Special case, did we enter the trigger of an enemy outpost?
		else if (other.CompareTag (enemyTag))
		{
			// Use the spawner to check if this is an outpost
			OutpostSpawner tempOutpost = other.GetComponent<OutpostSpawner>();

			if (tempOutpost != null)
			{
				// This is an enemy outpost, stop!
				canWalk = false;
			}
		}
	}

	/// <summary>
	/// Fired when something leaves our front collider
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerExit(Collider other)
	{
		// First, did someone leave our trigger box?
		if (!other.isTrigger) 
		{
			// We can walk again!
			StartWalking();
		}
	}

	/// <summary>
	/// Raised when we have won a battle
	/// </summary>
	public void OnVictory()
	{
		// If we defeated our foe, we can walk again
		StartWalking();
	}

	/// <summary>
	/// Fired when this unit dies
	/// </summary>
	/// <param name="obj">Object.</param>
	public void OnDestroy()
	{
		// Broadcast this message to anyone listening
		CombatEventHandler hand = DeathEvent;
		if (hand != null)
		{
			hand (this, new CombatEventArgs(null, CombatEventArgs.CombatMsg.IsDefeated));
		}
	}

	/// <summary>
	/// Listens for an allied unit death
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="combatArgs">Combat arguments.</param>
	public void FriendDeath(object sender, CombatEventArgs combatArgs)
	{
		// We can walk again
		StartWalking();
	}

	// Helper functions
	/// <summary>
	/// Moves unit forward, using interpolation
	/// </summary>
	private void Walk(float t)
	{
		CurWalkTime += t;

		// If this is longer than we're supposed to walk
		if (CurWalkTime > TurnTime) 
		{
			// Make it the last turn
			CurWalkTime = TurnTime;

			// Stop walking after this time
			IsWalking = false;

			// Start walking after TurnTime
			nextWalk = Time.fixedTime + TurnTime;
		}

		transform.position = Vector3.Lerp (WalkStart, WalkDest, CurWalkTime / TurnTime);
	}
	/// <summary>
	/// We're about to start walking, setup our variables
	/// </summary>
	/// <param name="t">The time this update started</param>
	private void StartWalking()
	{
		// Let the world know we are walking
		IsWalking = true;
		canWalk = true;

		CurWalkTime = 0f;

		// Caculate our destination
		WalkStart = transform.position;
			
		WalkDest = transform.position + (direction * WalkSpeed);
	}
}
