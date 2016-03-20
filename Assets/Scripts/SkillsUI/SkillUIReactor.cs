using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillUIReactor : MonoBehaviour {

	[SerializeField]
	UIStatus[] enabledWhen;

	Button button;
	EventTrigger trigger;

	void Awake() {
		button = GetComponentInChildren<Button> ();
		trigger = GetComponentInChildren<EventTrigger> ();
	}

	void OnEnable() {
		SkillsUI.OnSkillUIStatusChange += HandleSkillsUIStatus;
	}

	void OnDisable() {
		SkillsUI.OnSkillUIStatusChange -= HandleSkillsUIStatus;
	}

	void HandleSkillsUIStatus (PlayerIdentity player, UIStatus status)
	{
		if (player != Player.LocalPlayerIdentity)
			return;

		for (int i = 0; i < enabledWhen.Length; i++) {
			if (status == enabledWhen [i]) {
				if (button)
					button.interactable = true;
				if (trigger)
					trigger.enabled = true;
				return;
			}
		}
		if (trigger)
			trigger.enabled = false;

		if (button)
			button.interactable = false;
	}
}
