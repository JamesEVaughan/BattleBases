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

	// Fields to be accessed by descendants
	/// <summary>
	/// The tag of our enemies!
	/// </summary>
	protected string enemyTag;

	// Events
	/// <summary>
	/// Fired on CambatEvents, as defined in subclasses
	/// </summary>
	public event CombatEventHandler CombatEvent;

	/// <summary>
	/// Raises a combat event for when an enemy is detected
	/// </summary>
	/// <param name="enemy">Enemy.</param>
	protected virtual void OnEnemyDetected(BaseFighter enemy)
	{
		CombatEventHandler hand = CombatEvent;
		if (hand != null) 
		{
			hand(this, new CombatEventArgs(enemy));
		}
	}

	/// <summary>
	/// Raises a combat event for when this unit is defeated
	/// </summary>
	protected virtual void OnDeath()
	{
		CombatEventHandler hand = CombatEvent;
		if (hand != null)
		{
			// Fire the event using the special Death version
			hand(this, new CombatEventArgs(null, CombatEventArgs.CombatMsg.IsDefeated));
		}
	}

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
			// We've been hit!
			Health -= attacker.AttackStr;

			// If Health is 0, we are dead
			IsDead = (Health == 0);

			// Lastly, send the OnDeath message to this gameObject
			if (IsDead)
			{
				SendMessage ("OnDeath");
			}
		}
	}
}