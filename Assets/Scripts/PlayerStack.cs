using UnityEngine;
using System.Collections;

public class PlayerStack : MonoBehaviour {

	Player player;

	// Use this for initialization
	void Awake () {
		player = GetComponent<Player> ();
	}

	void OnEnable() {
		ProcSkill.OnSkillChange += HandleProcSkill;
		ProbeSkill.OnProbeSkillChange += HandleProbeSkill;
	}

	void OnDisable() {
		ProcSkill.OnSkillChange -= HandleProcSkill;
		ProbeSkill.OnProbeSkillChange -= HandleProbeSkill;
	}

	void HandleProbeSkill (PlayerIdentity player, ProbeSkills skill, SkillProgress progres)
	{
		if (skill == ProbeSkills.ExpandStack && progres == SkillProgress.Bought && player == this.player.playerIdentity) {
			//TODO: Cause Extension
		}
	}
		
	void HandleProcSkill (PlayerIdentity player, ProcSkills skill, SkillProgress progress)
	{
		if (player == this.player.playerIdentity && progress == SkillProgress.Bought) {
			//TODO: Start unit production
		}
	}
}
