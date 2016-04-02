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

	public void Work() {

	}
		
}
