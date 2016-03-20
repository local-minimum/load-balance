using UnityEngine;
using System.Collections;

public class CreditsBar : MonoBehaviour {

	[SerializeField]
	Vector2 minPosition;

	[SerializeField]
	Vector2 maxPosition;

	[SerializeField]
	int minPositionCredits = 0;

	[SerializeField]
	int maxPositionCredits = 1000000;

	[SerializeField, Range(0.5f, 4f)]
	float logBase = 2f;

	void Start() {
		(transform as RectTransform).anchoredPosition = minPosition;
	}

	void OnEnable() {
		PlayerCredits.OnPlayerCreditsChange += HandleCreditsChange;
	}

	void OnDisable() {
		PlayerCredits.OnPlayerCreditsChange -= HandleCreditsChange;
	}

	void HandleCreditsChange (PlayerIdentity player, int credits, Direction direction)
	{
		if (player != Player.LocalPlayerIdentity)
			return;
		
		float f = Mathf.Log(credits - minPositionCredits, logBase) / Mathf.Log(maxPositionCredits - minPositionCredits, logBase);
		(transform as RectTransform).anchoredPosition = Vector2.Lerp(minPosition, maxPosition, f * f);
	}

}