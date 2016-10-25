using UnityEngine;
using System.Collections;

public class Battle
{
	// Properties
	/// <summary>
	/// Reference to the first fighter
	/// </summary>
	public BaseFighter Fighter1 
	{ 
		get
		{ 
			return fighter1;
		}
	}
	/// <summary>
	/// Reference to the second fighter
	/// </summary>
	public BaseFighter Fighter2 
	{
		get
		{
			return fighter2;
		}
	}
	/// <summary>
	/// Flags this Battle as being on hold
	/// </summary>
	/// <value><c>true</c> if this Battle is paused; otherwise, <c>false</c>.</value>
	public bool IsPaused { get; set; }

	// Fields
	/// <summary>
	/// Field behind the Fighter1 property
	/// </summary>
	private readonly BaseFighter fighter1;
	/// <summary>
	/// Field behind the Fighter2 property
	/// </summary>
	private readonly BaseFighter fighter2;
	/// <summary>
	/// How long Fighter1 has been resting since their last attack
	/// </summary>
	private float fighter1Timer;
	/// <summary>
	/// How long Fighter2 has been resting since their last attack
	/// </summary>
	private float fighter2Timer;
	/// <summary>
	/// True only if Fighter2 can fighter
	/// </summary>
	private bool canFighter2Fight;

	// Events
	public event CombatEventHandler BattleEvent;

	// Constructors
	/// <summary>
	/// Private. Initializes a new instance of the Battle class. Must use
	/// approiate Make method to create an instance.
	/// </summary>
	/// <param name="f1">The BaseFighter that goes first</param>
	/// <param name="f2">The BaseFighter that goes second</param>
	private Battle (BaseFighter f1, BaseFighter f2)
	{
		// First, set up the fighters
		fighter1 = f1;
		fighter2 = f2;
	}

	// Factory methods
	/// <summary>
	/// Makes a new Battle between two fighter units
	/// </summary>
	/// <returns>The battle with fighters.</returns>
	/// <param name="fight1">One fighter</param>
	/// <param name="fight2">The other fighter</param>
	public static Battle MakeBattleWithFighters(BaseFighter combatant1, BaseFighter combatant2)
	{
		// Sanity check, we need valid inputs
		if (combatant1 == null || combatant2 == null)
		{
			// We can't do anything
			return null;
		}

		// Since their both fighters, roll for initiative
		int whoGoesFirst = GetInitiative(combatant1, combatant2);

		// Assign first place to who goes first
		Battle tempy = (whoGoesFirst == 1) ?
			new Battle(combatant1, combatant2) :
			new Battle(combatant2, combatant1);

		// Both of these fighters can fight, set their timers
		// Fighter1 fights first, set their timer to their AttackSpd
		tempy.fighter1Timer = tempy.Fighter1.AttackSpd;
		// Fighter2 has to wait for half of their AttackSpd before being able to fight
		tempy.fighter2Timer = tempy.Fighter2.AttackSpd / 0.5f;

		// And, yes, Fighter2 can fight
		tempy.canFighter2Fight = true;

		// Let MakeFinisher handle the rest
		return MakeFinalizer (tempy);
	}

	/// <summary>
	/// Makes a Battle where only one fighter can attack
	/// </summary>
	/// <returns>A Battle</returns>
	/// <param name="theCombatant">The one that can attack</param>
	/// <param name="theNotCombatant">One that cannot attack</param>
	public static Battle MakeBattleOneAttacker(BaseFighter theCombatant, BaseFighter theNotCombatant)
	{
		// Sanity check, we need valid inputs
		if (theCombatant == null || theNotCombatant == null)
		{
			// We can't make anything
			return null;
		}

		// Assigning who goes first is easy, it's theCombatant
		Battle tempy = new Battle(theCombatant, theNotCombatant);

		// Fighter1 is the only one that can fighter, start them off attacking
		tempy.fighter1Timer = tempy.Fighter1.AttackSpd;
		// Fighter2 cannot fight, flag it with a negative
		tempy.fighter2Timer = -1.0f;

		// And, no, Fighter2 cannot fight
		tempy.canFighter2Fight = false;

		// Let MakeFinisher handle the rest
		return MakeFinalizer (tempy);
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
	/// Checks to see if fight is in this battle and if fight can fight.
	/// </summary>
	/// <returns><c>true</c>, if fight exists and can fight, <c>false</c> otherwise.</returns>
	/// <param name="fight">Fight.</param>
	public bool ContainsCanfight(BaseFighter fight)
	{
		// First, this is trivially false if fight can't fight
		if (fight.AttackStr <= 0)
		{
			return false;
		}

		// It only matters if fight is Fighter2
		else if (fight == Fighter2)
		{
			return canFighter2Fight;
		}
		else
		{
			// If we didn't catch it before, it's true
			return true;
		}
	}

	// Event handling
	/// <summary>
	/// Listens for a unit to fire the defeated event
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="args">Arguments</param>
	public void Defeated(object sender, CombatEventArgs args)
	{
		// First, we only care about Death events
		if (args.Message != CombatEventArgs.CombatMsg.IsDefeated)
		{
			// Ignore the event
			return;
		}

		// Raise the event specifing who was defeated
		OnBattleCompletion(sender as BaseFighter);
	}

	/// <summary>
	/// Adds the time to the current timers. Sends a Message for the fighters to attack if their
	/// time is up. Should be called during every FixedUpdate cycle
	/// </summary>
	/// <param name="timeDelta">How much time has passed since the last run.</param>
	public void AddTime (float timeDelta)
	{
		// If this battle is paused, skip
		if (IsPaused)
		{
			return;
		}

		fighter1Timer += timeDelta;
		fighter2Timer += timeDelta;

		// Is Fighter1 ready to attack?
		if (fighter1Timer > Fighter1.AttackSpd)
		{
			// Tell Fighter2 they've been attacked
			Fighter2.gameObject.SendMessage("OnAttacked", Fighter1.gameObject);

			// If defeated Fighter2, exit this method
			if (Fighter2.IsDead)
			{
				// Don't worry, clean up will be handled by our Defeated listener
				return;
			}

			// Still here, reset the clock
			fighter1Timer = 0.0f;
		}
		// Can Fighter2 attack and are they ready?
		if (canFighter2Fight && fighter2Timer > Fighter2.AttackSpd)
		{
			// Tell Fighter1 they've been attacked
			Fighter1.gameObject.SendMessage("OnAttacked", Fighter2.gameObject);

			// If this defeats Fighter1, exit this method
			if (Fighter1.IsDead)
			{
				// Don't worry, clean up will be handled by our Defeated listener
				return;
			}

			// Still here, reset the clock
			fighter2Timer = 0.0f;
		}
	}

	// Helper methods
	/// <summary>
	/// Calculates who attacks first based on movement
	/// </summary>
	/// <returns>Who rolled the higher initiative</returns>
	private static int GetInitiative(BaseFighter f1, BaseFighter f2)
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

	/// <summary>
	/// Finalizes the battle for the various Make methods. The difference between the Make
	/// methods are setting up turn order. After that the actions are the same.
	/// </summary>
	/// <returns>The finished Battle</returns>
	/// <param name="halfBaked">The partially built Battle</param>
	private static Battle MakeFinalizer(Battle halfBaked)
	{
		// Ok, now for the basic making

		// Subscribe to their CombatEvents for the Death event
		halfBaked.Fighter1.CombatEvent += halfBaked.Defeated;
		halfBaked.Fighter2.CombatEvent += halfBaked.Defeated;

		// We start out not paused
		halfBaked.IsPaused = false;

		// Everything's set!
		return halfBaked;
	}

	/// <summary>
	/// Raises the battle completion event.
	/// </summary>
	/// <param name="defeatedFight">The fighter that was defeated</param>
	private void OnBattleCompletion(BaseFighter defeatedFight)
	{
		// Is anybody listening?
		CombatEventHandler hand = BattleEvent;

		if (hand != null)
		{
			hand(this, new CombatEventArgs(defeatedFight, CombatEventArgs.CombatMsg.BattleComplete));
		}
	}
}

