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
	private enum Team
	{
		Red,
		Blue
	};

	// Methods
	void Awake()
	{
		// Nothing for now
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
		if (t == Team.Red) 
		{
			Instantiate (RedUnit, RedSpawn.transform.position, RedSpawn.transform.rotation);
		} 
		else if (t == Team.Blue) 
		{
			Instantiate (BlueUnit, BlueSpawn.transform.position, BlueSpawn.transform.rotation);
		}

		SpawnTimer = Time.time + 1f;
	}
}
