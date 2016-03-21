using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public delegate void PanelSelect(CanvasGroup panel);

public class PanelButton : MonoBehaviour {

	[SerializeField] bool startSelected = false;
	[SerializeField] CanvasGroup panel;

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

	void HandlePanelSelect (CanvasGroup panel)
	{
		bool selected = panel == this.panel;
		this.panel.alpha = selected ? 1 : 0;
		this.panel.interactable = selected;
		this.panel.blocksRaycasts = selected;
		button.interactable = !selected;
	}

	public void Select() {
		if (OnPanelSelect != null)
			OnPanelSelect (panel);
	}

}
