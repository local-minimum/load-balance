using UnityEngine;
using System.Collections.Generic;

public enum PlayerIdentity {Player_1, Player_2, Player_3, Player_4, AI_1, AI_2, AI_3, AI_4};

public class Player : MonoBehaviour {

	public PlayerIdentity playerIdentity;
	public static PlayerIdentity LocalPlayerIdentity;
	public PlayerCredits credits;
	public PlayerStack playerStack;

	static Dictionary<PlayerIdentity, Player> _players = new Dictionary<PlayerIdentity, Player> ();

	void Awake() {
		//TODO: Do this properly
		credits = GetComponent<PlayerCredits>();
		playerStack = GetComponent<PlayerStack> ();
		LocalPlayerIdentity = playerIdentity;
		_players [playerIdentity] = this;
	}

	public static Player GetPlayer(PlayerIdentity pi) {
		if (_players.ContainsKey(pi))
			return _players[pi];
		return null;
	}
}
