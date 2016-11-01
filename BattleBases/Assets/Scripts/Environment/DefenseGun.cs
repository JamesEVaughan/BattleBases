using UnityEngine;
using System.Collections;

using System.Collections.Generic;

/// <summary>
/// The defensive gun used by the base to attack nearby enemy units.
/// </summary>
public class DefenseGun : BaseFighter
{
	// Accessible in Unity Editor
	public int Strength = 5;
	public float Speed = 0.25f;

	// Fields
	/// <summary>
	/// A list of the targets for the gun that have been discovered
	/// </summary>
	private List<BaseFighter> targets;
	/// <summary>
	/// A list of targets that are to be culled from the list at the end of this Update loop
	/// </summary>
	private List<BaseFighter> cullTargets;
	/// <summary>
	/// Tells us when we can attack again.
	/// </summary>
	private float attackTimer;

	// Unity Methods
	// Use this for initialization
	void Start ()
	{
		// Setup based on the values given to us by in the Editor
		AttackStr = Strength;
		AttackSpd = Speed;

		// Set Health to 10 to make everything else work
		Health = 10;

		// Snag the tag of our enemies
		enemyTag = (tag == "Blue") ? "Red" : "Blue";

		// We start alive
		IsDead = false;

		// A size of 10 should be a good place to start for the targets List
		targets = new List<BaseFighter>(10);

		// Flag the timer to show that we haven't found a unit yet.
		attackTimer = -1.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Only run if we've found something
		if (attackTimer < 0.0f)
		{
			// Ignore
			return;
		}

		// Increment our timer
		attackTimer += Time.deltaTime;

		// Can we attack and is there a target?
		if (attackTimer >= AttackSpd && targets.Count > 0)
		{
			// This will be our eventual target
			BaseFighter targetBF = null;

			// Our list may need to be culled, see if cullTargets has anything
			if (cullTargets.Count > 0)
			{
				foreach (BaseFighter tempBF in targets)
				{
					// Grab the first one that isn't in cullTargets
					if (!cullTargets.Contains (tempBF))
					{
						targetBF = tempBF;

						// Found it, exit
						break;
					}
				}
			}
			else
			{
				// If it's empty, grab the first BaseFighter in targets
				targetBF = targets[0];
			}

			// Send the attack message only if we found something
			if (targetBF != null)
			{
				targetBF.OnAttacked (this);

				// Only reset the timer if we successfully attacked
				attackTimer = 0.0f;
			}
		}
	}

	void LateUpdate()
	{
		// Call CullTargetList only if we need to
		if (cullTargets.Count > 0)
		{
			CullTargetsList ();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		// Is this us entering someone else's trigger?
		if (other.isTrigger)
		{
			// Ignore it
			return;
		}

		BaseFighter otherBF = other.GetComponent<BaseFighter> ();

		// We can only attack GameObjects with a BaseFighter script
		if (otherBF != null)
		{
			// We can only fight our enemies
			if (otherBF.tag == enemyTag)
			{
				// We found an enemy!
				OnEnemyDetected(otherBF);
			}
		}
	}

	// Events
	/// <summary>
	/// Listens for death events from members of the targets List
	/// </summary>
	/// <param name="sender">The BaseFighter that was defeated</param>
	/// <param name="args">Arguments.</param>
	public void TargetDefeated (object sender, CombatEventArgs args)
	{
		// First, we only care about Defeated events
		if (args.Message != CombatEventArgs.CombatMsg.IsDefeated)
		{
			// Ignore
			return;
		}

		BaseFighter senderBF = sender as BaseFighter;

		// Unsubscribe so we don't hear from them again
		senderBF.CombatEvent -= TargetDefeated;

		// Sanity check: is this a in targets?
		if (!targets.Contains (senderBF))
		{
			// Odd, but whatever, we're done
			return;
		}

		// Add them to the cullTargets to be removed later
		cullTargets.Add(senderBF);
	}

	// Helper methods
	/// <summary>
	/// Culls targets based on cullTargets
	/// </summary>
	protected void CullTargetsList()
	{
		// Loop through cullTargets...
		foreach (BaseFighter tempBF in cullTargets)
		{
			// Remove them from targets
			targets.Remove(tempBF);
		}

		// Reset cullTargets
		cullTargets.Clear();
	}

	// Implementation of BaseFighter
	public override void OnAttacked (object other)
	{
		// We're not an availabe target
		//base.OnAttacked(other)
	}

	protected override void OnDeath ()
	{
		// We don't need to broadcast death events
		//base.OnDeath ();
	}

	protected override void OnEnemyDetected (BaseFighter enemy)
	{
		// We're doing a complete rewrite of the enemy detection logic
		//base.OnEnemyDetected (enemy);

		// First things first, add this enemy to the list
		targets.Add(enemy);

		// Subscribe to their combat events
		enemy.CombatEvent += TargetDefeated;

		// Finally, if this is the first enemy we've found
		if (attackTimer < 0.0f)
		{
			// Set attackTimer to go now
			attackTimer = AttackSpd + 1.0f;
		}
	}
}

