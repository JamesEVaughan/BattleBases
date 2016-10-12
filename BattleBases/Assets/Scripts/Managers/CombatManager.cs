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

	void Start ()
	{
		// Initiatize our list
		curFights = new List<Battle>(10);

		nextFightTime = -1f;

		// Subscribe to Outpost events
		OutpostSpawner[] spawners = Object.FindObjectsOfType<OutpostSpawner>();
		OutpostCombat tempCombat;

		// And listen to their spawn events
		foreach (OutpostSpawner temp in spawners)
		{
			temp.SpawnEvent += UnitSpawned;

			tempCombat = temp.GetComponent<OutpostCombat> ();

			// Each one should have one, so something bad happened if they don't
			if (tempCombat != null)
			{
				// Subscribe to their combat events
				tempCombat.CombatEvent += OnCombat;
			}
		}


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
		if (nextFightTime < Time.time) 
		{
			// Ok, then get it going!
			if (nextBattle.Attack ()) 
			{
				// The battle is over, tell the combatants
				if (nextBattle.Fighter1.IsDead) 
				{
					// Fighter1 is dead! Long live Fighter2!
					nextBattle.Fighter1.gameObject.SendMessage ("OnDeath", null, SendMessageOptions.DontRequireReceiver);
					nextBattle.Fighter2.gameObject.SendMessage ("OnVictory", null, SendMessageOptions.DontRequireReceiver);
				} 
				else 
				{
					// Fighter2 is dead! Long live Fighter1!
					nextBattle.Fighter2.gameObject.SendMessage ("OnDeath", null, SendMessageOptions.DontRequireReceiver);
					nextBattle.Fighter1.gameObject.SendMessage ("OnVictory", null, SendMessageOptions.DontRequireReceiver);
				}
				curFights.Remove(nextBattle);
			}

			// Get the next battle to fight
			FindNextBattle();
		}
	}

	// Methods

	// Events
	/// <summary>
	/// Listens for spawn events
	/// </summary>
	public void UnitSpawned(object sender, SpawnEventArgs spawnArgs)
	{
		// We only care about GameObjects with BaseFighter components
		BaseFighter newBF = spawnArgs.UnitSpawned.GetComponent<BaseFighter>();
		if (newBF == null) 
		{
			// It doesn't have a BaseFighter, ignore
			return;
		}

		// Subscribe to it's CombatEvents
		newBF.CombatEvent += OnCombat;
	}

	/// <summary>
	/// Listens for CombatEvents
	/// </summary>
	/// <param name="sender">The object sending this argument</param>
	/// <param name="combatArgs">The event arguments</param>
	public void OnCombat(object sender, CombatEventArgs combatArgs)
	{
		BaseFighter tempBF = sender as BaseFighter;
		if (tempBF == null) 
		{
			// Ignore
			return;
		}

		// It is! Add this to our battle list
		NewBattle(tempBF, combatArgs.Opponent);
	}


	// Helper Methods
	/// <summary>
	/// Finds the next battle to be fought
	/// </summary>
	private void FindNextBattle()
	{
		// Sanity check, there only a next battle if there are battles
		if (curFights.Count == 0) 
		{
			return;
		}
			
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
	/// Adds a new Battle to the list, if it hasn't already been added
	/// </summary>
	/// <param name="BF1">One fighter in this Battle</param>
	/// <param name="BF2">The other fighter in this Battle</param>
	private void NewBattle(BaseFighter BF1, BaseFighter BF2)
	{
		// First, a fighter can only be in one battle
		foreach (Battle tempBatt in curFights)
		{
			if (tempBatt.Contains (BF1) || tempBatt.Contains (BF2)) 
			{
				// Ignore it
				return;
			}
		}

		// This is a new battle
		curFights.Add(new Battle(BF1, BF2));
		// Update our fight timer
		FindNextBattle();
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
		/// <summary>
		/// Can this battle be interrupted by another fighter?
		/// </summary>
		public bool isInterruptable { get; private set; }
		/// <summary>
		/// Is this battle on hold?
		/// </summary>
		public bool isPaused { get; set; }

		// Fields
		/// <summary>
		/// True if Fighter1 is next, false if Fighter2 is next
		/// </summary>
		private bool isFighter1Next;
		/// <summary>
		/// How long between attack turns
		/// </summary>
		private float turnTime;


		// Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CombatManager+Battle"/> class.
		/// </summary>
		/// <param name="f1">One BaseFighter</param>
		/// <param name="f2">The other BaseFighter</param>
		/// <param name="turnTime">How long between attacks, default 0.5 seconds</param>
		public Battle (BaseFighter f1, BaseFighter f2, float turnLength = 0.5f)
		{
			if (GetInitiative(f1, f2) == 1)
			{
				// f1 is Figher1
				Fighter1 = f1;
				Fighter2 = f2;
			}
			else 
			{
				// Otherwise, flip it
				Fighter1 = f2;
				Fighter2 = f1;
			}

			// Fighter1 fights next
			isFighter1Next = true;

			// Set nextFight to the current time
			turnTime = turnLength;
			NextFight = Time.time + turnTime;

			// Finally, a battle involving an Outpost can be paused
			if (f1 is OutpostCombat || f2 is OutpostCombat)
			{
				isInterruptable = true;
			}

			else
			{
				isInterruptable = false;
			}
		}

		// Methods
		/// <summary>
		/// Checks if either BaseFighter fight is in this battle
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
			if (isFighter1Next) 
			{
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

			// Negate isFighter1Next to switch to the other fighter's turn
			isFighter1Next = !isFighter1Next;

			NextFight = Time.time + turnTime;
		}
	}
}

