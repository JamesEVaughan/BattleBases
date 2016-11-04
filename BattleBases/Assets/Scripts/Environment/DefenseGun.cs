using UnityEngine;
using System.Collections;

using System.Collections.Generic;

/// <summary>
/// The defensive gun used by the base to attack nearby enemy units.
/// </summary>
public class DefenseGun : BaseFighter
{
	// Fields
	/// <summary>
	/// A list of the targets for the gun that have been discovered
	/// </summary>
	private List<BaseFighter> targets;

	/// <summary>
	/// This is a reference to our current target, null if there isn't one
	/// </summary>
	private BaseFighter curTarget;

	// Unity Methods
	// Use this for initialization
	void Start ()
	{
		// Call BaseFighter's initializer
		Init ();

		// A size of 10 should be a good place to start for our lists
		targets = new List<BaseFighter>(10);

		// Flag the timer to show that we haven't found a unit yet.
		attackTimer = -1.0f;

		// Flag curTarget to null
		curTarget = null;
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
		if (attackTimer >= AttackSpd && curTarget != null)
		{

			Attack ();
		}
		// Can we attack but there aren't any targets?
		else if (attackTimer >= AttackSpd && curTarget == null)
		{
			// Flag timer to -1 to avoid unnecessary checks
			attackTimer = -1.0f;
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

		// Remove them from our target list
		targets.Remove(senderBF);

		// If this was our current target
		if (curTarget == senderBF)
		{
			// We've got to find a new one
			FindCurrentTarget ();
		}
	}

	// Helper methods
	/// <summary>
	/// Sets curTargetIndex to the correct target
	/// </summary>
	private void FindCurrentTarget()
	{
		// Sanity check, is our current target alive?
		if (curTarget != null && !curTarget.IsDead)
		{
			// Whoops, we're good
			return;
		}

		// Safety catch: set curTarget to null
		curTarget = null;

		// Trivial cases:
		switch (targets.Count)
		{
		// targets is empty
		case 0:
			// No change
			return;

		// There is only one object in targets
		case 1:
			// Make sure it's alive
			curTarget = (targets[0].IsDead) ? null : targets[0];
			return;
		}

		// If we have more than that, loop through targets
		foreach (BaseFighter temp in targets)
		{
			if (!temp.IsDead)
			{
				// If it's alive, we've found it
				curTarget = temp;
				return;
			}
		}
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

		// See if we need to update our current target
		FindCurrentTarget();

		// Subscribe to their combat events
		enemy.CombatEvent += TargetDefeated;

		// Finally, if this is the first enemy we've found
		if (attackTimer < 0.0f)
		{
			// Set attackTimer to go now
			attackTimer = AttackSpd + 1.0f;
		}
	}

	protected override void Attack ()
	{
		base.Attack ();
	}

	public override BaseFighter Target {
		get 
		{
			// Pass the value of curTarget
			return curTarget;
		}
	}
}

