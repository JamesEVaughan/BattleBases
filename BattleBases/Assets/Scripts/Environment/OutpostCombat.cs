using UnityEngine;
using System.Collections;

/// <summary>
/// This behaviour handles combat involving an Outpost, a structure that spawns new
/// units and can be destroyed by opposing units. It is unique in that it doesn't
/// attack itself.
/// </summary>
public class OutpostCombat : BaseFighter
{
	// Accessible in Unity
	public int HP = 300;

	// Unity methods
	public void Awake()
	{
		// Set the base class properties
		Health = HP;
		IsDead = false;	// We start alive

		// We can't attack, set those values to zero
		AttackSpd = 0f;
		AttackStr = 0;

		// Find out our enemies tag
		enemyTag = (tag == "Blue") ? "Red" : "Blue";
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
}

