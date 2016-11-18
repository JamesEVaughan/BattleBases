using UnityEngine;
using System.Collections;

/// <summary>
/// This is the combat behavior for fighter units
/// </summary>
public class FighterUnit : BaseFighter
{
	// Fields
	/// <summary>
	/// Our currently detected target. Null if we don't have one
	/// </summary>
	private BaseFighter target;

	// Methods
	public void Awake()
	{
		// Call BaseFighter's initializer
		Init ();

		// Flag to show that there isn't an active target
		target = null;
		attackTimer = -1.0f;
	}

	public void Update()
	{
		// First, have we found anything?
		if (attackTimer < 0.0f)
		{
			// Ignore
			return;
		}

		// Increment our timer
		attackTimer += Time.deltaTime;

		// Can we attack again?
		if (attackTimer >= AttackSpd && target != null)
		{
			// Do we have a target?
			if (target != null)
			{
				// Attack!
				Attack();
			}
		}
	}

	/// <summary>
	/// Fired when we're involved in a TriggerEnter event
	/// </summary>
	void OnTriggerEnter(Collider other)
	{
		BaseFighter othBF = other.GetComponent<BaseFighter> ();

		if ( othBF == null) 
		{
			// If we can't find a BaseFighter component, we can't fight this thing
			// Ignore
			return;
		}

		// We're here if something entered our trigger collider and we can fight it!
		if (other.tag == enemyTag) 
		{
			// We found an enemy!
			OnEnemyDetected(othBF);
		}
	}

	// Events
	/// <summary>
	/// Listens for combat events, specifically when the target dies
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public void TargetDefeated(object sender, CombatEventArgs args)
	{
		// Seriously, am I in this method without a valid FighterUnit?
		if (this == null)
		{
			// WHYYYYYY?
			// UPDATE: We're here not because this is null, but because the gameObject has
			//   been destroyed. This is what "this == null" means
			return;
		}
		// Sanity check: sender should be our target and the target must exist
		if (target == null || (sender as BaseFighter) != target)
		{
			// Ignore
			return;
		}
		// We only care about death events
		if (args.Message != CombatEventArgs.CombatMsg.IsDefeated)
		{
			// Ignore it
			return;
		}

		// First, unsubscribe from their events
		target.CombatEvent -= TargetDefeated;

		// Remove the reference
		target = null;

		// Tell your gameObject that the battle is over
		if (gameObject != null)
		{
			// But only if the gameObject hasn't been deleted?
			// For some reason?
			gameObject.SendMessage ("OnVictory", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Implementation of BaseFighter
	public override void OnAttacked (object other)
	{
		base.OnAttacked (other);

		// Add FighterUnit specific code here
	}

	// BaseFighter events
	protected override void OnEnemyDetected (BaseFighter enemy)
	{
		base.OnEnemyDetected (enemy);

		// Unit FighterUnit specific code here

		// Set this as our current target
		target = enemy;

		// Flag the timer so that we attack ASAP
		attackTimer = AttackSpd+1.0f;

		// Finally, subscribe to their CombatEvents
		enemy.CombatEvent += TargetDefeated;
	}

	protected override void OnDeath ()
	{
		base.OnDeath ();

		// Add fighter specific code here
	}

	public override BaseFighter Target {
		get 
		{
			return target;
		}
	}

	protected override void Attack ()
	{
		base.Attack ();
	}
}

