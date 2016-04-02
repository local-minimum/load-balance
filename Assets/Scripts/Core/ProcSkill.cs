using UnityEngine;
using System.Collections;

public enum ProcSkills {Frag, Flip, Push, Mash, Rend, Zoom, Unknown};

public delegate void ProcSkillChange(PlayerIdentity player, ProcSkills skill, SkillProgress progress);

public class ProcSkill : AbstractSkill {

	public ProcSkills skillType;

	public float productionTime = -1;
	public int patternWindowSize = 4;

	public static event ProcSkillChange OnSkillChange;

	bool Learn() {
		PlayerCredits purse = Player.GetPlayer(Player.LocalPlayerIdentity).credits;
		if (purse.ConsumeCredits (Player.LocalPlayerIdentity, learningCost)) {
			_progress = SkillProgress.Learned;
			if (OnSkillChange != null)
				OnSkillChange (Player.LocalPlayerIdentity, skillType, _progress);
			return true;
		}
		return false;
	}

	bool Buy() {
		if (Player.GetPlayer (Player.LocalPlayerIdentity).credits.ConsumeCredits (Player.LocalPlayerIdentity, buyingCost)) {			
			_progress = SkillProgress.Bought;
			if (OnSkillChange != null)
					OnSkillChange(Player.LocalPlayerIdentity, skillType, _progress);
			return true;
		}
		return false;	
	}

	public override void Increase() {
		
		if (_progress == SkillProgress.UnAvailable)
			return;
		if (_progress == SkillProgress.Available)
			Learn ();
		else if (_progress == SkillProgress.Learned || _progress == SkillProgress.Bought)
			Buy ();
		
	}
				
	void Awake() {
		
		if (AllRequirementsMet)
			_progress = SkillProgress.Available;
		else
			_progress = SkillProgress.UnAvailable;
	
		SkillSystem.RegisterSkill (this);
		if (OnSkillChange != null)
			OnSkillChange (Player.LocalPlayerIdentity, skillType, _progress);
	}

	void OnEnable() {
		ProcSkill.OnSkillChange += HandleSkillChange;
	}

	void OnDisable() {
		ProcSkill.OnSkillChange -= HandleSkillChange;
	}

		
	void HandleSkillChange (PlayerIdentity player, ProcSkills skill, SkillProgress progress)
	{
		if (player != Player.LocalPlayerIdentity)
			return;

		if (_progress == SkillProgress.UnAvailable && AllRequirementsMet) {
			_progress = SkillProgress.Available;
			if (OnSkillChange != null)
				OnSkillChange (Player.LocalPlayerIdentity, skillType, _progress);
		}
	}


}
