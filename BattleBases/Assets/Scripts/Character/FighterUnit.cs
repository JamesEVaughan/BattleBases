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

		// We always start alive
		IsDead = true;
	}

	// Implementation of BaseFighter
	public override void OnAttacked (object other)
	{
		base.OnAttacked (other);

		// If this killed us, Send a OnDeath message
		// we don't care if anyone hears this
		if (IsDead) 
		{
			gameObject.SendMessage ("OnDeath", null, SendMessageOptions.DontRequireReceiver);
		}
	}
}

