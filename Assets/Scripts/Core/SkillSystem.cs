using UnityEngine;
using System.Collections.Generic;

public enum SkillType {Process, Probe};

public enum SkillProgress {UnAvailable, Available, Learned, Bought};


public class SkillSystem : MonoBehaviour {

	static SkillSystem _instance;

	static Dictionary<ProcSkills, ProcSkill> procSkills = new Dictionary<ProcSkills, ProcSkill>();

	static Dictionary<ProbeSkills, ProbeSkill> probeSkills = new Dictionary<ProbeSkills, ProbeSkill>();

	static SkillSystem instance {
		get {
			if (_instance == null)
				FindInstanceOrSpawn ();
			return _instance;
		}
	}


	static void FindInstanceOrSpawn() {
		_instance = FindObjectOfType<SkillSystem> ();
		if (_instance == null) {
			var GO = new GameObject ("SkillSytstem");
			_instance = GO.AddComponent<SkillSystem> ();
		}
	}
		
	public static ProcSkill getSkill(ProcSkills skill) {
		if (!procSkills.ContainsKey (skill))
			instance.Start ();
		
		if (procSkills.ContainsKey(skill))
			return procSkills[skill];
		return null;
	}

	public static ProbeSkill getSkill(ProbeSkills skill) {
		if (!probeSkills.ContainsKey (skill))
			instance.Start ();
		
		if (probeSkills.ContainsKey (skill))
			return probeSkills [skill];

		return null;
	}
		
	void Awake () {
		if (_instance == null)
			_instance = this;
		else if (_instance != this) 
			Destroy (gameObject);		
	}

	void Start() {
		var _procSkills = GetComponentsInChildren<ProcSkill> ();
		for (int i = 0; i < _procSkills.Length; i++) 
			RegisterSkill (_procSkills [i]);		

		var _probeSkills = GetComponentsInChildren<ProbeSkill> ();
		for (int i = 0; i < _probeSkills.Length; i++)
			RegisterSkill (_probeSkills [i]);
	}

	public static void RegisterSkill(ProcSkill skill) {
		if (!procSkills.ContainsKey(skill.skillType) || procSkills[skill.skillType] != skill)
			procSkills [skill.skillType] = skill;
	}

	public static void RegisterSkill(ProbeSkill skill) {
		if (!probeSkills.ContainsKey (skill.skillType) || probeSkills [skill.skillType] != skill)
			probeSkills [skill.skillType] = skill;
	}

	public static bool patternMatches(ProcSkills skill, string[] data) {
		//TODO: Add patterns
		return false;
	}

	public static string[] processedData(ProcSkills skill, string[] data) {
		//TODO: Add modifications
		return data;
	}

	public static int calculateDeploymentCost(ProcSkills skill, int priority, float duration) {
		//TODO: Add modifiers for various jobs
		float modifier = 1000f;
		var cost = Mathf.RoundToInt(modifier * (1f + Mathf.Log(1 + priority)) * Mathf.Log(4f + duration / 5f, 2f));
		Debug.Log ("Calculated deployment cost for " + skill.ToString () + " Prio: " + priority + " Duration: " + duration + " Cost: " + cost);
		return cost;
	}

}
