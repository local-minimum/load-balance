using UnityEngine;
using System.Collections.Generic;

public enum JobStatus {UnderConstruction, Deployable, Deployed, Expired};
public delegate void StatusChange(ProcJob job, JobStatus status);

public class ProcJob : MonoBehaviour {

	public event StatusChange OnStatusChange;

	public ProcSkills jobType;
	public JobStatus status;
	public float progress = 0;
	public int frame = 0;
	public PlayerIdentity playerId;

	ComputationNode node;

	float statusStart;
	float statusEnd;
	float statusDuration;

	public void Construct(float constructionTime) {
		progress = 0;
		status = JobStatus.UnderConstruction;	
		statusStart = Time.timeSinceLevelLoad;
		statusEnd = statusStart + constructionTime;
		statusDuration = constructionTime;

		StartCoroutine (_Construct ());
	}

	IEnumerator<WaitForSeconds> _Construct() {
		float t = -1;
		while (t < statusEnd && status == JobStatus.UnderConstruction) {
			t = Time.timeSinceLevelLoad;
			progress = Mathf.Clamp01((t - statusStart) / statusDuration);
			yield return new WaitForSeconds (0.05f);
		}
		if (status == JobStatus.UnderConstruction) {
			status = JobStatus.Deployable;
			progress = -1f;
			statusStart = Time.timeSinceLevelLoad;
			if (OnStatusChange != null)
				OnStatusChange (this, status);
		}			
	}

	public void SetExpired() {
		status = JobStatus.Expired;
		progress = -1;
		statusStart = Time.timeSinceLevelLoad;
		if (OnStatusChange != null)
			OnStatusChange (this, status);
		Destroy (this, 1f);
	}

	public void Deploy(ComputationNode node, int priority, float duration) {
		this.node = node;
		node.AddJob (this, priority);
		statusStart = Time.timeSinceLevelLoad;
		statusEnd = statusStart + duration;
		status = JobStatus.Deployed;
		Debug.Log (jobType + " is deployed");
		if (OnStatusChange != null)
			OnStatusChange (this, status);
	}

	public int Work() {
		progress = Mathf.Clamp01((Time.timeSinceLevelLoad - statusStart) / (statusEnd - statusStart));
		if (progress == 1) {
			SetExpired ();
		} else {
			var windowSize = SkillSystem.getSkill (this.jobType).patternWindowSize;
			var data = node.GetCachedFrame (windowSize, frame);
			if (data.Length == windowSize) {
				if (SkillSystem.patternMatches (jobType, data)) {
					node.SetCachedFrame (SkillSystem.processedData (jobType, data));
					return SkillSystem.getSkill (jobType).workCost;
				}

			}
		}
		return 0;

	}
		
}
