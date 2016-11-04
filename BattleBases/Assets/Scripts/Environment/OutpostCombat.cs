using UnityEngine;
using System.Collections;

/// <summary>
/// This behaviour handles combat involving an Outpost, a structure that spawns new
/// units and can be destroyed by opposing units. It is unique in that it doesn't
/// attack itself.
/// </summary>
public class OutpostCombat : BaseFighter
{
	// Unity methods
	public void Awake()
	{
		// Call BaseFighter's initializer
		Init ();
	}

	public void OnTriggerEnter(Collider other)
	{
		// Sanity check, we only care about RigidBodies entering our trigger
		if (other.attachedRigidbody == null)
		{
			return;
		}

		// Find the attached BaseFighter component
		BaseFighter othBF = other.GetComponent<BaseFighter>();

		if (othBF != null)
		{
			// Finally, is it an enemy?
			if (other.gameObject.CompareTag (enemyTag))
			{
				// Raise a found enemy event
				OnEnemyDetected(othBF);
			}
		}
	}

	// Implementation of BaseFighter
	public override void OnAttacked (object other)
	{
		base.OnAttacked (other);

		// Add Outpost specific code here
	}

	protected override void OnDeath ()
	{
		base.OnDeath ();

		// Add OutpostCombat specific code here
	}

	protected override void OnEnemyDetected (BaseFighter other)
	{
		base.OnEnemyDetected (other);

		// Add OutpostCombat specific code here
	}

	protected override void Attack ()
	{
		// We can't attack
		//base.Attack ();
	}

	public override BaseFighter Target {
		get 
		{
			// We can't attack, just pass a null
			return null;
		}
	}
}

