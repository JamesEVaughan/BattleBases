using UnityEngine;
using System.Collections;

using System.ComponentModel;
/// <summary>
/// Treasury keeps tabs on the money available to one team. It generates more money
/// at a rate defined by the fundsPerTurn and secondsPerTurn, which are serialized to
/// be edited in the editor. The property Funds will fire PropertyChanged events to allow
/// for interested classes to hook into updates.
/// </summary>
public class Treasury : MonoBehaviour, INotifyPropertyChanged
{
	// Properties
	/// <summary>
	/// The currently available funds
	/// </summary>
	/// <value>The funds.</value>
	public int Funds
	{
		get { return funds; }
		set 
		{
			// Only allow new, positive values
			if (value >= 0 && value != funds)
			{
				funds = value;
				OnPropertyChanged ("Treasury");
			}
		}
	}

	// Fields
	/// <summary>
	/// This is the current amount of funds available to this team.
	/// </summary>
	[SerializeField]
	private int funds;

	/// <summary>
	/// How much money is generated each turn. Turn length defined by
	/// secondsPerTurn.
	/// </summary>
	[SerializeField]
	private int fundsPerTurn = 5;

	/// <summary>
	/// How long each turn lasts. Amount of money generated is defined by
	/// fundsPerTurn.
	/// </summary>
	[SerializeField]
	private float secondsPerTurn = 1.0f;

	/// <summary>
	/// A handy timer for when to release the next round of funds
	/// </summary>
	private float fundsTimer;

	// Unity Methods
	public void Start()
	{
		// Make sure our fields valid
		if (funds < 0)
		{
			// Default to zero
			Funds = 0;
		}
		// NOTE: The ones defining the fund rate MUST be greater than 0, should be
		//     fairly obvious why...
		if (fundsPerTurn <= 0)
		{
			// Default to 5
			fundsPerTurn = 5;
		}
		if (secondsPerTurn <= 0)
		{
			// Default to 1.0f
			secondsPerTurn = 1.0f;
		}

		// Start out at 0
		fundsTimer = 0.0f;
	}

	public void Update()
	{
		// Sanity check: Only do stuff if we're alive
		if (this == null)
		{
			// Do nothing
			return;
		}

		// Increment timer
		fundsTimer += Time.deltaTime;

		// Can we release more funds?
		if (fundsTimer >= secondsPerTurn)
		{
			ReleaseFunding ();
		}
	}

	// Methods

	// Helper Methods
	private void ReleaseFunding()
	{
		// Send more funds
		Funds += fundsPerTurn;

		// Reset our timer
		fundsTimer = 0.0f;
	}

	// Implementation of INotifyPropertyChanged
	public event PropertyChangedEventHandler PropertyChanged;

	/// <summary>
	/// Raises the property changed event.
	/// </summary>
	/// <param name="propName">Name of the property that changed.</param>
	private void OnPropertyChanged(string propName)
	{
		PropertyChangedEventHandler hand = PropertyChanged;

		if (hand != null)
		{
			hand(this, new PropertyChangedEventArgs(propName));
		}
	}
}

