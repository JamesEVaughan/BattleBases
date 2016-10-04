using UnityEngine;
using System.Collections;

/// <summary>
/// This is the combat behavior for fighter units
/// </summary>
public class FighterUnit : BaseFighter
{
	// Fields to edit in Unity
	public int HP = 100;
	public int Strength = 10;
	public float Speed = 0.5f;

	// Methods
	public void Awake()
	{
		// Set the properties according to the Unity setup
		Health = HP;
		AttackStr = Strength;

		// Note: If we do animations, we need to factor that in here somehow...
		AttackSpd = Speed;

		// Find out which tag is our enemies
		enemyTag = (tag == "Blue") ? "Red" : "Blue";

		// We always start alive
		IsDead = false;
	}

	/// <summary>
	/// Fired when we're involved in a TriggerEnter event
	/// </summary>
	void OnTriggerEnter(Collider other)
	{
		// If this is us entering a Trigger
		if (other.isTrigger) 
		{
			// Do this stuff
			// for now, just ignore the event
			return;
		}

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

	// Helper functions
	/// <summary>
	/// Raises the death event.
	/// </summary>
	protected void OnDeath()
	{
		// We've been defeated!

		// For now, just destroy the GameObject
		Destroy(gameObject);
	}

	// Implementation of BaseFighter
	public override void OnAttacked (object other)
	{
		base.OnAttacked (other);

		// Add FighterUnit specific code here
	}

	protected override void TookDamage (int dam)
	{
		base.TookDamage (dam);

		// Add UnitFighter specific code here
	}
}

