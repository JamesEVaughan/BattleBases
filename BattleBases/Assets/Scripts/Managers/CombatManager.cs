using UnityEngine;
using System.Collections;

using System.Collections.Generic;

/// <summary>
/// This handles combat interactions
/// </summary>
public class CombatManager : MonoBehaviour
{
	// Fields
	/// <summary>
	/// A list of the current battles taking place.
	/// </summary>
	private List<Battle> curFights;
	/// <summary>
	/// When the next attack should take place
	/// </summary>
	private float nextFightTime;
	/// <summary>
	/// The the next Battle to happen
	/// </summary>
	private Battle nextBattle;

	void Awake ()
	{
		// Initiatize our list
		curFights = new List<Battle>(10);

		nextFightTime = -1f;
	}

	// we keep time in Update
	void Update()
	{
		// Don't do anything without a battle
		if (nextFightTime < 0 || curFights.Count < 1) 
		{
			return;
		}

		// Do we have a fight pending?
		if (nextFightTime > Time.time) 
		{
			// Ok, then get it going!
			if (nextBattle.Attack ()) 
			{
				// The battle is over!
				curFights.Remove(nextBattle);
			}

			// Get the next battle to fight
			FindNextBattle();
		}
	}

	// Methods
	/// <summary>
	/// Adds a new Battle to the list, if it hasn't already been added
	/// </summary>
	/// <param name="BF1">One fighter in this Battle</param>
	/// <param name="BF2">The other fighter in this Battle</param>
	public void NewBattle(BaseFighter BF1, BaseFighter BF2)
	{
		// First, a fighter can only be in one battle
		foreach (Battle tempBatt in curFights)
		{
			if (tempBatt.Contains (BF1) || tempBatt.Contains (BF2)) 
			{
				// Do nothing
				return;
			}
		}

		// This is a new battle
		curFights.Add(new Battle(BF1, BF2));
	}


	// Helper Methods
	/// <summary>
	/// Finds the next battle to be fought
	/// </summary>
	private void FindNextBattle()
	{
		Battle tempy = curFights[0];

		foreach (Battle other in curFights) 
		{
			if (tempy.NextFight > other.NextFight) 
			{
				tempy = other;
			}
		}

		nextFightTime = tempy.NextFight;
		nextBattle = tempy;
	}

	/// <summary>
	/// A helper class, organizes the two fighters that are fighting
	/// </summary>
	private class Battle
	{
		// Properties
		/// <summary>
		/// Reference to the first fighter
		/// </summary>
		public BaseFighter Fighter1 { get; private set; }
		/// <summary>
		/// Reference to the second fighter
		/// </summary>
		public BaseFighter Fighter2 { get; private set; }
		/// <summary>
		/// When this battle fights again
		/// </summary>
		public float NextFight { get; private set; }

		// Fields
		/// <summary>
		/// Who attacks next
		/// </summary>
		private enum nextAttacker
		{
			Fighter1,
			Fighter2
		}
		private nextAttacker next;
		/// <summary>
		/// When Fighter1 attacks again
		/// </summary>
		private float nextFight1;
		/// <summary>
		/// When Fighter2 attacks again
		/// </summary>
		private float nextFight2;


		// Constructors
		public Battle (BaseFighter f1, BaseFighter f2)
		{
			if (GetInitiative(f1, f2) == 1)
			{
				// f1 is Figher1
				Fighter1 = f1;
				Fighter2 = f1;
			}
			else 
			{
				// Otherwise, flip it
				Fighter1 = f2;
				Fighter2 = f1;
			}

			// Fighter1 fights next
			next = nextAttacker.Fighter1;

			// Set nextFight to the current time
			nextFight1 = Time.time;
			nextFight2 = Time.time;
		}

		// Methods
		/// <summary>
		/// Checks if either Fighter is fight
		/// </summary>
		/// <param name="fight">Fight.</param>
		/// <returns>True if fight is one of the two fighters</returns>
		public bool Contains(BaseFighter fight)
		{
			return (Fighter1 == fight || Fighter2 == fight);
		}

		/// <summary>
		/// Sends the next attack message
		/// </summary>
		public bool Attack()
		{
			// Find out who's fighting
			BaseFighter attacker, target;
			if (next == nextAttacker.Fighter1) {
				attacker = Fighter1;
				target = Fighter2;
			} 
			else 
			{
				attacker = Fighter2;
				target = Fighter1;
			}

			// Send the attack message
			target.SendMessage("OnAttacked", attacker.gameObject, SendMessageOptions.DontRequireReceiver);

			// If this finishes the battle, let the manager know
			if (target.IsDead) 
			{
				return true;
			}

			// Otherwise, calculate when we attack again.
			GetNextfight();

			return false;
		}

		// Helper methods
		/// <summary>
		/// Calculates who attacks first based on movement
		/// </summary>
		/// <returns>Who rolled the higher initiative</returns>
		private int GetInitiative(BaseFighter f1, BaseFighter f2)
		{
			// Grab the UnitMovement behavior from the two objects
			UnitMovement um1 = f1.gameObject.GetComponent<UnitMovement>();
			UnitMovement um2 = f2.gameObject.GetComponent<UnitMovement>();

			// First, you can only have initiative if you can move
			if (um1 == null || um2 == null) 
			{
				// And, yes, if neither can move, give it to #2
				// No real reason to think to hard about this
				return (um1 == null) ? 2 : 1;
			}

			// Second, if you're not walking, you also lose
			if (!(um1.IsWalking && um2.IsWalking)) 
			{
				// Again, #2 wins if they both aren't walking, but that 
				//    shouldn't happen...
				return (um1.IsWalking) ? 1 : 2;
			}

			// Okay! Both can move, now for the fun!
			// Their "initiative" is equal to how far they've walked
			float distance1 = um1.CurWalkTime * um1.WalkSpeed;
			float distance2 = um2.CurWalkTime * um2.WalkSpeed;

			// Find a random number between the two distances and compare
			//   it against distance 1
			if (Random.Range (0f, distance1 + distance2) < distance1) {
				// 1 is the winner!
				return 1;
			} 
			else
			{
				// Otherwise, 2 won
				return 2;
			}
		}

		private void GetNextfight()
		{
			// nextAttacker just attacked, add their AttackSpd to their timer
			if (next == nextAttacker.Fighter1) {
				nextFight1 += Fighter1.AttackSpd;
			}
			else if (next == nextAttacker.Fighter2)
			{
				nextFight2 += Fighter2.AttackSpd;
			}

			// The next one to attack has the shorter time
			if (nextFight1 < nextFight2) {
				// Fighter1 fights next
				NextFight = nextFight1;
				next = nextAttacker.Fighter1;
			} 
			else 
			{
				// Fighter2 fights next
				NextFight = nextFight2;
				next = nextAttacker.Fighter2;
			}
		}
	}
}

