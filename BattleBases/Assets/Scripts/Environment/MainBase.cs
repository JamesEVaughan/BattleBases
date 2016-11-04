using UnityEngine;
using System.Collections;

public class MainBase : BaseFighter
{
	// Unity methods
	void Awake ()
	{
		// Call BaseFighter's initializer
		Init ();
	}

	// Implementation of BaseFighter
	public override void OnAttacked (object other)
	{
		base.OnAttacked (other);
	}

	protected override void OnDeath ()
	{
		base.OnDeath ();
	}

	protected override void OnEnemyDetected (BaseFighter enemy)
	{
		base.OnEnemyDetected (enemy);
	}

	protected override void Attack ()
	{
		// We can't attack
		//base.Attack ();
	}

	public override BaseFighter Target {
		get 
		{
			// We can't attack, pass a null
			return null;
		}
	}
}

