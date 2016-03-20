using UnityEngine;
using System.Collections;

public enum ProbeSkills {Dispatch, Alarm, Survey, Scan, Map, Infiltrate, Hack, Clock, Cool, ExpandStack};

public delegate void ProbeSkillChange(PlayerIdentity player, ProbeSkills skill, SkillProgress progres);

public class ProbeSkill : MonoBehaviour {

	public static event ProbeSkillChange OnProbeSkillChange;

	public ProbeSkills skillType;

	SkillProgress _progress = SkillProgress.UnAvailable;

	public SkillProgress progress {
		get {
			return _progress;
		}
	}

	public string buttonCharacter = "A";
	public string hintText = "";

	public int learningCost;
	public int buyingCost;

	public int actionCost {
		get {
			if (_progress == SkillProgress.Learned)
				return buyingCost;
			else
				return learningCost;
		}
	}

	[SerializeField] ProbeSkill[] requirements;

	bool AllRequirementsMet {
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

	void Awake() {

		if (AllRequirementsMet)
			_progress = SkillProgress.Available;
		else
			_progress = SkillProgress.UnAvailable;

		SkillSystem.RegisterSkill (this);
		if (OnProbeSkillChange != null)
			OnProbeSkillChange (Player.LocalPlayerIdentity, skillType, _progress);
	}

	void OnEnable() {
		OnProbeSkillChange += HandleSkillChange;
	}

	void OnDisable() {
		OnProbeSkillChange -= HandleSkillChange;	
	}

	void HandleSkillChange (PlayerIdentity player, ProbeSkills skill, SkillProgress progress)
	{
		if (player != Player.LocalPlayerIdentity)
			return;

		if (_progress == SkillProgress.UnAvailable && AllRequirementsMet) {
			_progress = SkillProgress.Available;
			if (OnProbeSkillChange != null)
				OnProbeSkillChange (Player.LocalPlayerIdentity, skillType, _progress);
		}
	}

	bool Learn() {
		PlayerCredits purse = Player.GetPlayer(Player.LocalPlayerIdentity).credits;
		if (purse.ConsumeCredits (Player.LocalPlayerIdentity, learningCost)) {
			_progress = SkillProgress.Learned;
			if (OnProbeSkillChange != null)
				OnProbeSkillChange (Player.LocalPlayerIdentity, skillType, _progress);
			return true;
		}
		return false;
	}

	bool Buy() {
		if (Player.GetPlayer (Player.LocalPlayerIdentity).credits.ConsumeCredits (Player.LocalPlayerIdentity, buyingCost)) {			
			_progress = SkillProgress.Bought;
			if (OnProbeSkillChange != null)
				OnProbeSkillChange(Player.LocalPlayerIdentity, skillType, _progress);
			return true;
		}
		return false;	
	}

	public void Increase() {

		if (_progress == SkillProgress.UnAvailable)
			return;
		if (_progress == SkillProgress.Available)
			Learn ();
		else if (_progress == SkillProgress.Learned || _progress == SkillProgress.Bought)
			Buy ();

	}
}
