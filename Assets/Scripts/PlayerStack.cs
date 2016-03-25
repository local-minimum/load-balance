using UnityEngine;
using System.Collections.Generic;

public enum StackEventType {Expanded, Expired};

public delegate void StackEvent(int slot, StackEventType eventType);

public class PlayerStack : MonoBehaviour {

	public event StackEvent OnStackChange;

	Player player;

	[SerializeField] int[] slots;

	[SerializeField] float[] expansionTimes;

	float stackExpansionDuration = -1;

	float[] expansionTimeProgress;

	bool monitoringExpansions = false;

	[SerializeField]
	float expansionMonitoringFrequency = 0.1f;
		
	public int currentCapacity {
		get {
			int capacity = 0;
			float t = Time.timeSinceLevelLoad - stackExpansionDuration;
			for (int i = 0; i < expansionTimes.Length; i++) {
				if (expansionTimes[i] < 0 || t < expansionTimes[i])
					capacity += slots [i];
			}
			return capacity;
		}
	}

	public float GetDurationProgress(int slot) {
		if (expansionTimeProgress == null)
			return 0;
		return expansionTimeProgress [slot];
	}

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

			stackExpansionDuration = SkillSystem.getSkill (skill).duration;

			//Pushes all expiration times on to the right
			float t = -1f;
			for (int i = 0; i < expansionTimes.Length; i++) {
				if (expansionTimes [i] < 0)
					continue;

				if (t < 0) {
					t = expansionTimes [i];
					expansionTimes [i] = Time.timeSinceLevelLoad;
					if (OnStackChange != null) {
						ForceZeroProgress (i);
						OnStackChange (i, StackEventType.Expanded);
					}
				} else {
					float t2 = expansionTimes [i];
					expansionTimes [i] = t;
					if (t != 0 && Time.timeSinceLevelLoad - stackExpansionDuration < t && OnStackChange != null) {
						ForceZeroProgress (i);
						OnStackChange (i, StackEventType.Expanded);
					}
					t = t2;
				}

			}

			if (!monitoringExpansions)
				StartCoroutine (expansionMonitor ());
		}
	}
		
	void HandleProcSkill (PlayerIdentity player, ProcSkills skill, SkillProgress progress)
	{
		if (player == this.player.playerIdentity && progress == SkillProgress.Bought) {
			//TODO: Start unit production
		}
	}

	void ForceZeroProgress(int slot) {
		if (expansionTimeProgress != null && expansionTimeProgress.Length > slot)
			expansionTimeProgress [slot] = 0;
	}

	IEnumerator<WaitForSeconds> expansionMonitor() {
		
		monitoringExpansions = true;
		expansionTimeProgress = new float[slots.Length];
		Debug.Log ("Expansion monitor");
		while (true) {
			
			float curTime = Time.timeSinceLevelLoad;
			bool noExpansionActive = true;

			for (int i = 0; i < expansionTimeProgress.Length; i++) {
				
				if (expansionTimes [i] < 0)
					continue;
				
				bool wasActive = expansionTimeProgress [i] < 1f;
				expansionTimeProgress [i] = Mathf.Clamp01 ((curTime - expansionTimes [i]) / stackExpansionDuration);

				if (wasActive && expansionTimeProgress [i] == 1f && OnStackChange != null) {
					OnStackChange (i, StackEventType.Expired);
				} else if (expansionTimeProgress [i] < 1f) {
					noExpansionActive = false;
				}					
			}

			if (noExpansionActive)
				break;
			
			yield return new WaitForSeconds (expansionMonitoringFrequency);
		}

		monitoringExpansions = false;
	}

}
