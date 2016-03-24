using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StackExtension : MonoBehaviour {

	[SerializeField] AnimationCurve fieldAlpha;

	[SerializeField] AnimationCurve timerAlpha;

	[SerializeField] AnimationCurve timerFill;

	[SerializeField] CanvasGroup canvasGroup;

	[SerializeField, Range(0, 0.2f)] float updateFrequency = 0.07f;

	[SerializeField] Image timer;

	bool monitoring;

	void Reset() {
		canvasGroup = GetComponent<CanvasGroup> ();
	}

	public void SetActiveMonitor(PlayerStack stack, int slot) {
		if (!monitoring)
			StartCoroutine (animateDuration (stack, slot));
	}

	public void SetDisabled() {
		canvasGroup.alpha = 0;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
	}

	IEnumerator<WaitForSeconds> animateDuration (PlayerStack stack, int slot) {

		canvasGroup.alpha = 1;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = true;

		monitoring = true;
		var timerColor = timer.color;
		float a = timerColor.a;

		while (monitoring) {
			
			float progress = stack.GetDurationProgress (slot);

			timerColor.a = timerAlpha.Evaluate (progress) * a;
			timer.color = timerColor;

			timer.fillAmount = timerFill.Evaluate (progress);

			canvasGroup.alpha = fieldAlpha.Evaluate (progress);

			if (progress == 1)
				break;
			
			yield return new WaitForSeconds (updateFrequency);
		}

		SetDisabled ();
		timerColor.a = a;
		timer.color = timerColor;
		monitoring = false;
	}
}
