using UnityEngine;
using UnityEngine.UI;

public class ExtendStack : MonoBehaviour {

	[SerializeField]
	Button button;	

	ProbeSkill skill;

	bool hasSkill;
	bool hasCredits;

	PlayerStack playerStack;
	StackExtension[] stackExtensions;

	void Awake() {
		stackExtensions = GetComponentsInChildren<StackExtension> ();
		skill = SkillSystem.getSkill (ProbeSkills.ExpandStack);
		playerStack = Player.GetPlayer (Player.LocalPlayerIdentity).playerStack;

		for (int i = 0; i < stackExtensions.Length; i++)
			stackExtensions [i].SetDisabled ();
	}

	void OnEnable() {
		ProbeSkill.OnProbeSkillChange += HandleProbeSkillChange;
		PlayerCredits.OnPlayerCreditsChange += HandlePlayerCredits;
		playerStack.OnStackChange += HandlePlayerStackEvent;
	}
				
	void OnDisable() {
		ProbeSkill.OnProbeSkillChange -= HandleProbeSkillChange;
		PlayerCredits.OnPlayerCreditsChange -= HandlePlayerCredits;
		playerStack.OnStackChange -= HandlePlayerStackEvent;
	}

	void HandlePlayerStackEvent (int slot, StackEventType eventType)
	{
		if (eventType == StackEventType.Expanded) {
			// Debug.Log ("Slot " + slot + " active");
			stackExtensions [slot - 1].SetActiveMonitor (playerStack, slot);
		}
	}

	void HandleProbeSkillChange (PlayerIdentity player, ProbeSkills skill, SkillProgress progress)
	{
		if (player == Player.LocalPlayerIdentity && skill == this.skill.skillType) {
			hasSkill = progress == SkillProgress.Learned || progress == SkillProgress.Bought;
			button.interactable = hasSkill && hasCredits;
		}
	}

	void HandlePlayerCredits (PlayerIdentity player, int credits, Direction direction)
	{
		if (player == Player.LocalPlayerIdentity) {
			hasCredits = skill.buyingCost < credits;
			button.interactable = hasSkill && hasCredits;
		}
	}

	public void BuyExtension() {
		skill.Increase ();
	}

}
