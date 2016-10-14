using UnityEngine;
using System.Collections;

using System;

/// <summary>
/// This is the Event Argument class for combat related events.
/// </summary>
public class CombatEventArgs : EventArgs
{
	// Enums
	public enum CombatMsg
	{
		/// <summary>
		/// We found an enemy
		/// </summary>
		FoundEnemy,
		/// <summary>
		/// We were defeated in combat
		/// </summary>
		IsDefeated,
		/// <summary>
		/// This Battle has completed, only used by Battle
		/// </summary>
		BattleComplete
	};
	// Properties
	/// <summary>
	/// This is the object that initiated combat with us.
	/// Must be a subclass of BaseFighter
	/// </summary>
	public BaseFighter Opponent { get; private set; }

	public CombatMsg Message { get; private set; }

	// Constructors
	/// <summary>
	/// Creates a new instance of CombatEventArgs
	/// </summary>
	public CombatEventArgs(BaseFighter oppo, CombatMsg mess = CombatMsg.FoundEnemy)
	{
		Opponent = oppo;
		Message = mess;
	}
}

// Event delegate
/// <summary>
/// The CombatEventHandler signature
/// </summary>
public delegate void CombatEventHandler(object sender, CombatEventArgs combatArgs);