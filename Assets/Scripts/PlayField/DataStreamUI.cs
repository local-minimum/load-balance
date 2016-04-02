using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class DataStreamUI : MonoBehaviour {

	[SerializeField] RectTransform source;
	[SerializeField] RectTransform target;

	[SerializeField] Text template;

	[SerializeField, Range(1, 50)] int length;

	[SerializeField] DataStream bitStream;

	List<DataStreamBitUI> bits  = new List<DataStreamBitUI>();

	int next = 0;

	void Awake() {
		bits.Add (template.gameObject.GetComponent<DataStreamBitUI>());
		while (bits.Count < length) {
			var GO = Instantiate (template.gameObject);
			GO.transform.SetParent (transform);
			GO.transform.localScale = template.transform.localScale;
			bits.Add (GO.GetComponent<DataStreamBitUI> ());
		}
			
	}

	void OnEnable() {
		bitStream.OnToken += HandleNewBit;
	}

	void OnDisable() {
		bitStream.OnToken -= HandleNewBit;
	}

	void HandleNewBit (string token, float frequency)
	{
		bits [next].Animate (token, source, target, length * frequency * 0.95f);
		next++;
		next %= length;
	}
}
