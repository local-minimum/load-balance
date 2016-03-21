using UnityEngine;
using UnityEngine.UI;

public class ExtendStack : MonoBehaviour {

	[SerializeField]
	Button button;	

	ProbeSkill skill;

	bool hasSkill;
	bool hasCredits;

	void Awake() {
		skill = SkillSystem.getSkill (ProbeSkills.ExpandStack);
	}

	void OnEnable() {
		ProbeSkill.OnProbeSkillChange += HandleProbeSkillChange;
		PlayerCredits.OnPlayerCreditsChange += HandlePlayerCredits;
	}
		
	void OnDisable() {
		ProbeSkill.OnProbeSkillChange -= HandleProbeSkillChange;
	}

	void HandleProbeSkillChange (PlayerIdentity player, ProbeSkills skill, SkillProgress progres)
	{
		if (player == Player.LocalPlayerIdentity && skill == this.skill.skillType) {
			hasSkill = progres == SkillProgress.Learned || progres == SkillProgress.Bought;
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
