using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	// Things to fiddle with in Unity Editor
	/// <summary>
	/// The Red Team Spawnpoint
	/// </summary>
	public GameObject RedSpawn;
	/// <summary>
	/// The Blue Team Spawnpoint
	/// </summary>
	public GameObject BlueSpawn;
	/// <summary>
	/// A unit of the Red Team to be spawned
	/// </summary>
	public GameObject RedUnit;
	/// <summary>
	/// A unit of the Blue Team to be spawned
	/// </summary>
	public GameObject BlueUnit;
	/// <summary>
	/// How fast can we spawn?
	/// </summary>
	public float SpawnTimer = 0f;

	// Fields
	/// <summary>
	/// A reference to our active combat manager
	/// </summary>
	private CombatManager combat;

	// Enums
	/// <summary>
	/// Handy reference to the available teams
	/// </summary>
	private enum Team
	{
		Red,
		Blue
	};

	// Methods
	void Awake()
	{
		// Grab our CombatManger
		combat = GetComponent<CombatManager>();
	}

	void Update()
	{
		// Hacky tester, spawn based on key press
		if (SpawnTimer > Time.time) 
		{
			// Don't do anything
			return;
		}
		if (Input.GetKeyDown (KeyCode.R)) 
		{
			Spawn (Team.Red);
		}
		if (Input.GetKey (KeyCode.B)) 
		{
			Spawn (Team.Blue);
		}
	}

	// Helper functions
	private void Spawn(Team t)
	{
		// Reference to the new object we're spawning
		Object tempObj = null;

		if (t == Team.Red) 
		{
			tempObj = Instantiate (RedUnit, RedSpawn.transform.position, RedSpawn.transform.rotation);
		} 
		else if (t == Team.Blue) 
		{
			tempObj = Instantiate (BlueUnit, BlueSpawn.transform.position, BlueSpawn.transform.rotation);
		}

		// Only do changes if we spawned something
		if (tempObj is GameObject) 
		{
			// Tell the combat manager we spawned a new unit
			combat.OnSpawn (tempObj as GameObject);

			SpawnTimer = Time.time + 1f;
		}
	}
}
