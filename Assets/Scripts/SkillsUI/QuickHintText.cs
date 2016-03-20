using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class QuickHintText : MonoBehaviour {

	[SerializeField] Text textUI;

	[SerializeField] float unhintSpeed = 0.4f;

	bool unHinting = false;

	void Reset() {
		textUI = GetComponent<Text> ();
	}

	public void Hint(string message) {
		unHinting = false;
		textUI.text = message;
	}

	public void UnHint(string message) {
		if (textUI.text == message && !unHinting) {
			StartCoroutine (animateUnhint ());
		}
	}

	IEnumerator<WaitForSeconds> animateUnhint() {
		
		unHinting = true;
		while (textUI.text.Length > 0 && unHinting) {
			textUI.text = textUI.text.Substring (1);
			yield return new WaitForSeconds (unhintSpeed);
		}
		unHinting = false;
	}
}
