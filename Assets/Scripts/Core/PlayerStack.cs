using UnityEngine;
using System.Collections.Generic;

public enum StackEventType {Expanded, Expired};

public delegate void StackEvent(int slot, StackEventType eventType);
public delegate void JobEvent(int slot, ProcJob job);

public class PlayerStack : MonoBehaviour {

	public event StackEvent OnStackChange;
	public event JobEvent OnJobEvent;

	Player player;

	List<ProcJob> queue = new List<ProcJob>();

	int[] expansionSlots = new int[3] {5, 2, 1};

	float[] expansionTimes = new float[3] {-1, 0, 0};

	float stackExpansionDuration = -1;

	float[] expansionTimeProgress = new float[3] {0, 0, 0};

	bool monitoringExpansions = false;

	[SerializeField]
	float expansionMonitoringFrequency = 0.1f;
		
	public int currentCapacity {
		get {
			int capacity = 0;
			for (int i = 0; i < expansionTimeProgress.Length; i++) {
				if (expansionTimes[i] < 0 || expansionTimeProgress[i] < 1)
					capacity += expansionSlots [i];
			}
			return capacity;
		}
	}

	public int currentOccupation {
		get {
			return queue.Count;
		}
	}

	public bool hasEmptySlots {
		get {
			return currentOccupation < currentCapacity;
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
		
	void HandleProcSkill (PlayerIdentity playerIdentity, ProcSkills skill, SkillProgress progress)
	{
		if (playerIdentity == player.playerIdentity && progress == SkillProgress.Bought) {
			if (hasEmptySlots) {
				var job = player.activeJobs.gameObject.AddComponent<ProcJob> ();
				job.jobType = skill;
				job.Construct (SkillSystem.getSkill (skill).productionTime * player.productionSpeedFactor);
				queue.Add (job);
				if (OnJobEvent != null)
					OnJobEvent (queue.IndexOf (job), job);
			} else {
				if (OnJobEvent != null)
					OnJobEvent (-1, null);
			}
		}
	}

	void ForceZeroProgress(int slot) {
		if (expansionTimeProgress != null && expansionTimeProgress.Length > slot)
			expansionTimeProgress [slot] = 0;
	}

	IEnumerator<WaitForSeconds> expansionMonitor() {
		
		monitoringExpansions = true;

		while (true) {
			
			float curTime = Time.timeSinceLevelLoad;
			bool noExpansionActive = true;

			for (int i = 0; i < expansionTimeProgress.Length; i++) {
				
				if (expansionTimes [i] < 0)
					continue;
				
				bool wasActive = expansionTimeProgress [i] < 1f;
				expansionTimeProgress [i] = Mathf.Clamp01 ((curTime - expansionTimes [i]) / stackExpansionDuration);

				if (wasActive && expansionTimeProgress [i] == 1f) {
					TrimQueue ();
					if (OnStackChange != null) {
						OnStackChange (i, StackEventType.Expired);
					}
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

	public void TrimQueue() {
		var capacity = currentCapacity;
		Debug.Log ("Trim queue: " + queue.Count + "/" + capacity);
		while (queue.Count > capacity) {
			var job = queue [queue.Count - 1];
			queue.Remove (job);
			job.SetExpired ();
		}
	}

}
