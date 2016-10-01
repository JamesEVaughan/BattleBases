using UnityEngine;
using System.Collections;

/// <summary>
/// The base for any fighter behaviours
/// </summary>
public abstract class BaseFighter : MonoBehaviour
{
	// Base Properties
	/// <summary>
	/// This is the health of the fighter
	/// </summary>
	/// <value>The health.</value>
	public int Health 
	{ 
		get 
		{
			return health;
		}
		protected set
		{
			// Health can only be set to positive values
			health = (value > 0) ? value : 0;
		}
	}

	/// <summary>
	/// This is how strong the fighter's attack is
	/// </summary>
	public int AttackStr 
	{ 
		get
		{
			return attackStr;
		}
		protected set
		{ 
			// AttackStr can only be set a positive value
			attackStr = (value > 0) ? value : 0;
		}
	}

	/// <summary>
	/// How often the fighter attacks, in seconds
	/// </summary>
	public float AttackSpd
	{
		get
		{ 
			return attackSpd;
		}
		protected set
		{
			// Use -1.0 as a flag for special cases
			attackSpd = (value > 0f) ? value : -1f;
		}
	}

	/// <summary>
	/// The state of this fighter being alive or dead
	/// </summary>
	/// <value><c>true</c> If this instance is dead; otherwise, <c>false</c>.</value>
	public bool IsDead { get; protected set; }

	// Base Fields
	/// <summary>
	/// Field behind for Health property
	/// </summary>
	private int health;
	/// <summary>
	/// Field behind for AttackStr property
	/// </summary>
	private int attackStr;
	/// <summary>
	/// Field behind for AttackSpd propery
	/// </summary>
	private float attackSpd;

	// Base Messages
	// These are the messages that this type handles
	/// <summary>
	/// OnAttacked method, called when this GameObject was attacked
	/// </summary>
	/// <param name="other">The GameObject that attacked us</param>
	public virtual void OnAttacked(object other)
	{
		// First, find the BaseFighter component
		BaseFighter attacker = (other as GameObject).GetComponent<BaseFighter> ();

		// Only subclasses of BaseFighter can attack us
		if (attacker != null) 
		{
			TookDamage (attacker.AttackStr);
		}
	}

	// Base Helper Methods
	/// <summary>
	/// Resolves OnAttacked messages
	/// </summary>
	/// <param name="dam">How strong the attack was</param>
	protected virtual void TookDamage(int dam)
	{
		Health -= dam;

		if (Health == 0) 
		{
			// We died
			IsDead = true;
		}
	}
}