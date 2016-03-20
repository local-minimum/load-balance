using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProbeSkillUIButton : MonoBehaviour {

	[SerializeField] Color32 learnedColor;

	[SerializeField] ProbeSkills skill;

	ProbeSkill _skill;

	Button button;
	Text buttonText;

	bool hasCredit = false;

	SkillsUI _ui;

	void Awake () {
		_ui = GetComponentInParent<SkillsUI> ();
		button = GetComponent<Button> ();
		buttonText = GetComponentInChildren<Text> ();

		_skill = SkillSystem.getSkill (skill);
		if (_skill == null) {
			button.interactable = false;
			buttonText.text = "?";
		} else {
			button.interactable = _skill.progress == SkillProgress.Available && hasCredit;
			if (_skill.progress == SkillProgress.Learned || _skill.progress == SkillProgress.Bought) {
				var colors = button.colors;
				colors.disabledColor = learnedColor;
			}
			buttonText.text = _skill.buttonCharacter;
		}
	}

	void OnEnable() {
		ProbeSkill.OnProbeSkillChange += HandleSkillProgress;
		PlayerCredits.OnPlayerCreditsChange += HandleCreditsChange;
	}

	void OnDisable() {
		ProbeSkill.OnProbeSkillChange -= HandleSkillProgress;
		PlayerCredits.OnPlayerCreditsChange -= HandleCreditsChange;
	}

	void HandleCreditsChange (PlayerIdentity player, int credits, Direction direction)
	{
		if (player != Player.LocalPlayerIdentity)
			return;

		//If has credit and inceases credit or doesn't have and decreases, do nothing
		if (hasCredit == (direction == Direction.Increase))
			return;

		if (_skill.progress == SkillProgress.Learned || _skill.progress == SkillProgress.Bought)
			return;
		
		hasCredit = _skill != null && _skill.actionCost <= credits;
		button.interactable = hasCredit && _skill.progress == SkillProgress.Available;
	}

	void HandleSkillProgress (PlayerIdentity player, ProbeSkills skill, SkillProgress progress)
	{
		if (player == Player.LocalPlayerIdentity && skill == this.skill) {
			if (_skill == null) {
				_skill = SkillSystem.getSkill (skill);
				buttonText.text = _skill.buttonCharacter;
			}					

			if (progress == SkillProgress.UnAvailable) {
				button.interactable = false;
			} else if (progress == SkillProgress.Learned || progress == SkillProgress.Bought) {
				button.interactable = false;
				var colors = button.colors;
				colors.disabledColor = learnedColor;
				button.colors = colors;
			} else {
				button.interactable = hasCredit;
			}
		}
	}

	public void EnterPointer() {
		if (_skill)
			_ui.hintText.Hint (_skill.hintText);
	}

	public void ExitPointer() {
		if (_skill)
			_ui.hintText.UnHint (_skill.hintText);
	}

	public void AttemptAction() {
		if (_skill)
			_skill.Increase ();
	}

}
