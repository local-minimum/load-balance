using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class DataStreamBitUI : MonoBehaviour {

	Text textField;

	[SerializeField, Range(0, 100)] float sineAmplitude = 30f;
	[SerializeField, Range(0, 100)] float sineFrequency = 10f;

	float updateFreq = 0.03f;

	void Awake() {
		textField = GetComponent<Text> ();
		textField.enabled = false;
	}

	public void Animate(string text, RectTransform source, RectTransform target, float duration) {
		transform.position = source.position;
		textField.text = text;
		StartCoroutine(_animate(source, target, duration));
	}

	IEnumerator<WaitForSeconds> _animate(RectTransform source, RectTransform target, float duration) {
		float startTime = Time.timeSinceLevelLoad;
		float progress = 0;
		do {
			if (!textField.enabled)
				textField.enabled = true;
			progress = (Time.timeSinceLevelLoad - startTime) / duration;
			var basePos = Vector3.Lerp(source.position, target.position, progress);
			transform.position = basePos + Vector3.up * Mathf.Sin(progress * sineFrequency * 2f * Mathf.PI) * sineAmplitude * transform.lossyScale.y;
			yield return new WaitForSeconds(updateFreq);

		} while (progress < 1);
		textField.enabled = false;
	}
}
