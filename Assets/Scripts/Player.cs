using UnityEngine;
using System.Collections.Generic;

public enum PlayerIdentity {Player_1, Player_2, Player_3, Player_4, AI_1, AI_2, AI_3, AI_4};

public class Player : MonoBehaviour {

	public PlayerIdentity playerIdentity;
	public static PlayerIdentity LocalPlayerIdentity;
	public PlayerCredits credits;
	public PlayerStack playerStack;
	[HideInInspector] public Transform activeJobs;

	[HideInInspector] public float productionSpeedFactor = 1f;

	static Dictionary<PlayerIdentity, Player> _players = new Dictionary<PlayerIdentity, Player> ();

	void Awake() {
		//TODO: Do this properly
		credits = GetComponent<PlayerCredits>();
		playerStack = GetComponent<PlayerStack> ();
		LocalPlayerIdentity = playerIdentity;
		_players [playerIdentity] = this;
		if (activeJobs == null) {
			var GO = new GameObject ("Active Jobs");
			activeJobs = GO.transform;
			activeJobs.SetParent (transform);
		}
	}

	public static Player GetPlayer(PlayerIdentity pi) {
		if (_players.ContainsKey (pi))
			return _players [pi];
		else {
			var players = GameObject.FindObjectsOfType<Player> ();
			for (int i = 0; i < players.Length; i++) {
				players [i].Awake ();
			}
			if (_players.ContainsKey(pi))
				return _players[pi];
		}
		return null;
	}
}
