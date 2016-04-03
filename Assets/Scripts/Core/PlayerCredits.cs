using UnityEngine;
using System.Collections;

public enum Direction {Increase, Decrease};

public delegate void PlayerCreditsChange(PlayerIdentity player, int credits, Direction direction);

public class PlayerCredits : MonoBehaviour {

	public static event PlayerCreditsChange OnPlayerCreditsChange;

	Player player;

	[SerializeField]
	int credits = 0;

	public void AddCredits(PlayerIdentity player, int ammount) {
		if (player != this.player.playerIdentity)
			return;

		credits += ammount;

		if (OnPlayerCreditsChange != null)
			OnPlayerCreditsChange (player, credits, Direction.Increase);
	}

	public bool ConsumeCredits(PlayerIdentity player, int amount) {
		if (player != this.player.playerIdentity)
			return false;
		else if (amount > credits)
			return false;

		credits -= amount;
		if (OnPlayerCreditsChange != null)
			OnPlayerCreditsChange (player, credits, Direction.Decrease);

		return true;
	}

	void Awake() {
		player = GetComponent<Player> ();
	}

	void OnEnable() {
		ComputationNode.OnAwardWorkDone += HandleWorkAward;
	}

	void OnDisable() {
		ComputationNode.OnAwardWorkDone -= HandleWorkAward;
	}

	void HandleWorkAward (ProcJob job, int award)
	{
		AddCredits (job.playerId, award);
	}

	#if UNITY_EDITOR
	[SerializeField, Range(0, 1000)] int autoUpdateAmmount;

	void Update() {		
		if (autoUpdateAmmount > 0)
			AddCredits (player.playerIdentity, autoUpdateAmmount);
	}
	#endif
}
