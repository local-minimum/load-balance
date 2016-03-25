using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StackSlot : MonoBehaviour {

	[SerializeField] Image maskImage;
	[SerializeField] Text textField;

	[SerializeField] float updateFrequency = 0.1f;

	ProcJob job;

	bool _monitoring = false;

	void Reset() {
		maskImage = GetComponent<Image> ();
		textField = GetComponentInChildren<Text> ();
	}

	public void NoJob() {
		maskImage.fillAmount = 0;
	}

	public void ShowJob(ProcJob job) {
		if (job.status == JobStatus.Deployable || job.status == JobStatus.UnderConstruction) {
			job.OnStatusChange += HandleJobStatusChange;
			if (this.job != null)
				this.job.OnStatusChange -= HandleJobStatusChange;
			this.job = job;
		} else
			return;

		if (job.status == JobStatus.UnderConstruction && !_monitoring) {
			StartCoroutine(MonitorJobConstruction ());
		} else if (job.status == JobStatus.Deployable) {
			textField.text = SkillSystem.getSkill(job.jobType).buttonCharacter;
			maskImage.fillAmount = 1;
			//TODO: Some nice ready-effect?
		}

	}

	void HandleJobStatusChange (ProcJob job, JobStatus status)
	{
		if (job == this.job) {
			if (status == JobStatus.Expired) {
				//TODO: Destroy Effect
				job.OnStatusChange -= HandleJobStatusChange;
				this.job = null;
				maskImage.fillAmount = 0f;
			} else if (status == JobStatus.Deployed) {
				job.OnStatusChange -= HandleJobStatusChange;
				this.job = null;
				maskImage.fillAmount = 0f;
			}
		} else {
			job.OnStatusChange -= HandleJobStatusChange;
		}
	}

	IEnumerator<WaitForSeconds> MonitorJobConstruction() {
		_monitoring = true;
		textField.text = SkillSystem.getSkill(job.jobType).buttonCharacter;
		while (job != null && job.status == JobStatus.UnderConstruction) {
			maskImage.fillAmount = job.progress;
			yield return new WaitForSeconds (updateFrequency);
		}

		if (job.status == JobStatus.Deployable) {
			maskImage.fillAmount = 1f;
			//TODO: Some nice ready-effect?
		} else {
			maskImage.fillAmount = 0;
			job = null;
		}
		_monitoring = false;
	}

}
