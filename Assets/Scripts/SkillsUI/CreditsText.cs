using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsText : MonoBehaviour {

	Text text;

	void Awake () {
		text = GetComponent<Text> ();	
	}

	void OnEnable() {
		PlayerCredits.OnPlayerCreditsChange += HandleNewCredits;
	}

	void OnDisable() {
		PlayerCredits.OnPlayerCreditsChange -= HandleNewCredits;
	}

	void HandleNewCredits (PlayerIdentity player, int credits, Direction direction)
	{
		if (player != Player.LocalPlayerIdentity)
			return;

		text.text = credits.ToString ();

		//TODO: Add some event for increases?
			
	}


}
