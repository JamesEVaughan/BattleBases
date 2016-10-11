using UnityEngine;
using System.Collections;

/// <summary>
/// An interface for MonoBehaviours that can be clicked on
/// </summary>
public interface IClickable
{
	/// <summary>
	/// Handles a clicked event, returns a Unity Object as needed
	/// </summary>
	void Clicked ();
}

