using UnityEngine;
using System.Collections;

public enum ProbeSkills {Dispatch, Alarm, Survey, Scan, Map, Infiltrate, Hack, Clock, Cool, ExpandStack};

public delegate void ProbeSkillChange(PlayerIdentity player, ProbeSkills skill, SkillProgress progres);

public class ProbeSkill : AbstractSkill {

	public static event ProbeSkillChange OnProbeSkillChange;

	public ProbeSkills skillType;

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
