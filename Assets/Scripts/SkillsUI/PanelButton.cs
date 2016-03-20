using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public delegate void PanelSelect(RectTransform panel);

public class PanelButton : MonoBehaviour {

	[SerializeField] bool startSelected = false;
	[SerializeField] RectTransform panel;

	Button button;

	static event PanelSelect OnPanelSelect;

	void Awake() {
		button = GetComponentInChildren<Button> ();
	}

	void Start() {
		if (startSelected)
			Select ();
	}

	void OnEnable() {
		OnPanelSelect += HandlePanelSelect;
	}

	void OnDisable() {
		OnPanelSelect -= HandlePanelSelect;
	}

	void HandlePanelSelect (RectTransform panel)
	{
		bool selected = panel == this.panel;
		this.panel.gameObject.SetActive (selected);
		button.interactable = !selected;
	}

	public void Select() {
		if (OnPanelSelect != null)
			OnPanelSelect (panel);
	}

}
