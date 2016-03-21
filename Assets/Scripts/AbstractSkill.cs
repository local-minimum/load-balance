using UnityEngine;
using System.Collections;

public abstract class AbstractSkill : MonoBehaviour {

	public string buttonCharacter = "A";
	public string hintText = "";
	public int learningCost;
	public int buyingCost;

	protected SkillProgress _progress = SkillProgress.UnAvailable;

	public SkillProgress progress {
		get {
			return _progress;
		}
	}

	public int actionCost {
		get {
			if (_progress == SkillProgress.Learned)
				return buyingCost;
			else
				return learningCost;
		}
	}

	[SerializeField] protected AbstractSkill[] requirements;

	public bool AllRequirementsMet {
		get {
			if (requirements == null || requirements.Length == 0) {
				return true;
			}
			for (int i = 0; i < requirements.Length; i++) {
				if (requirements [i]._progress != SkillProgress.Learned && requirements[i]._progress != SkillProgress.Bought)
					return false;
			}
			return true;
		}

	}

	public abstract void Increase ();
}
