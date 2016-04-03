using UnityEngine;
using System.Collections;

public class ComputationNodeUI : MonoBehaviour {

	ComputationNode node;

	void Start () {
		node = GetComponentInChildren<ComputationNode> ();
	}

	void OnMouseEnter() {
		ComputationNode.SetFocus (node);
	}

	void OnMouseExit() {
		ComputationNode.UnsetFocus (node);
	}
}
