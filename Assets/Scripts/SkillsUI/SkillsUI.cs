using UnityEngine;
using System.Collections.Generic;

public enum UIStatus {Showing, Contracting, Hidden, Extending};
public delegate void SkillUIStatusEvent(PlayerIdentity player, UIStatus status);

public class SkillsUI : MonoBehaviour {

	public static event SkillUIStatusEvent OnSkillUIStatusChange;

	public QuickHintText hintText;

	[SerializeField]
	float extendedX;

	[SerializeField]
	float retractedX;

	[SerializeField]
	AnimationCurve transition;

	[SerializeField, Range(0.5f, 2f)]
	float transitionTime = 1f;

	float currentX;

	UIStatus _status = UIStatus.Showing;

	public void HideUI() {
		StartCoroutine (doTransition (UIStatus.Hidden));
	}

	public void ShowUI() {
		StartCoroutine (doTransition (UIStatus.Showing));
	}

	IEnumerator<WaitForSeconds> doTransition(UIStatus target) {
		var _transition = UIStatus.Contracting;
		if (target == UIStatus.Hidden && (_status == UIStatus.Hidden || _status == UIStatus.Contracting))
			yield break;
		else if (target == UIStatus.Showing && (_status == UIStatus.Showing || _status == UIStatus.Extending))
			yield break;
		else if (target == UIStatus.Hidden) {
			_transition = UIStatus.Contracting;
		} else if (target == UIStatus.Showing) {
			_transition = UIStatus.Extending;
		} else
			yield break;


		_status = _transition;
		if (OnSkillUIStatusChange != null)
			OnSkillUIStatusChange (Player.LocalPlayerIdentity, _status);
		float startTime = Time.timeSinceLevelLoad;
		RectTransform rt = transform as RectTransform;
		Vector2 startPos = rt.anchoredPosition;
		Vector2 targetPos = rt.anchoredPosition;
		targetPos.x = target == UIStatus.Showing ? extendedX : retractedX;

		while (_status == _transition) {
			float duration = Mathf.Clamp01((Time.timeSinceLevelLoad - startTime) / transitionTime);
			rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, transition.Evaluate(duration));
			if (duration == 1f) {
				_status = target;
				if (OnSkillUIStatusChange != null)
					OnSkillUIStatusChange (Player.LocalPlayerIdentity, _status);
				break;
			}
			yield return new WaitForSeconds (0.01f);
		}
			
	}

	void Start() {
		_status = UIStatus.Contracting;
		ShowUI ();
	}
}
