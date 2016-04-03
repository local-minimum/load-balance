using UnityEngine;
using System.Collections;

public class PrioritySelectionUI : MonoBehaviour {

	ProcJob job;
	ComputationNode node;
	int priority = 0;
	float duration = 0;

	void OnEnable() {
		ComputationNode.OnConsiderDeployment += HandleConsiderDeployment;
	}

	void OnDisable() {
		ComputationNode.OnConsiderDeployment -= HandleConsiderDeployment;
	}

	void HandleConsiderDeployment (ProcJob job, ComputationNode node)
	{
		if (job.playerId == Player.LocalPlayerIdentity) {
			this.job = job;
			this.node = node;

			UpdateUI ();
		}
	}

	void UpdateUI() {
		//TODO: Set slider min max;

	}

	public void Deploy() {
		if (Player.GetPlayer (job.playerId).credits.ConsumeCredits (job.playerId, SkillSystem.calculateDeploymentCost(job.jobType, priority, duration))) {
			job.Deploy (node, priority, duration);
			Dismiss ();
		}
	}

	public void Dismiss() {
		job = null;
		node = null;
		//TODO: Hide UI

	}
}
