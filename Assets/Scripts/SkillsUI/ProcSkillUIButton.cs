using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProcSkillUIButton : MonoBehaviour {

	[SerializeField]
	ProcSkills skillType;

	ProcSkill _skill;

	Button button;
	Text buttonText;

	bool hasCredit = false;

	SkillsUI _ui;

	void Awake () {
		_ui = GetComponentInParent<SkillsUI> ();
		button = GetComponent<Button> ();
		buttonText = GetComponentInChildren<Text> ();

		_skill = SkillSystem.getSkill (skillType);
		if (_skill == null) {
			button.interactable = false;
			buttonText.text = "?";
		} else {
			button.interactable = _skill.progress != SkillProgress.UnAvailable && hasCredit;
			buttonText.text = _skill.buttonCharacter;
		}
	}
	
	void OnEnable() {
		ProcSkill.OnSkillChange += HandleSkillProgress;
		PlayerCredits.OnPlayerCreditsChange += HandleCreditsChange;
	}
		
	void OnDisable() {
		ProcSkill.OnSkillChange -= HandleSkillProgress;
		PlayerCredits.OnPlayerCreditsChange -= HandleCreditsChange;
	}

	void HandleCreditsChange (PlayerIdentity player, int credits, Direction direction)
	{
		if (player != Player.LocalPlayerIdentity)
			return;

		//If has credit and inceases credit or doesn't have and decreases, do nothing
		if (hasCredit == (direction == Direction.Increase))
			return;

		hasCredit = _skill != null && _skill.actionCost <= credits;
		button.interactable = hasCredit;
	}

	void HandleSkillProgress (PlayerIdentity player, ProcSkills skill, SkillProgress progress)
	{
		if (player == Player.LocalPlayerIdentity && skill == skillType) {
			if (_skill == null) {
				_skill = SkillSystem.getSkill (skill);
				buttonText.text = _skill.buttonCharacter;
			}					

			if (progress == SkillProgress.UnAvailable) {
				button.interactable = false;
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
