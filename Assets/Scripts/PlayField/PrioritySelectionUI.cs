using UnityEngine;
using System.Collections;

public class PrioritySelectionUI : MonoBehaviour {

	ProcJob job;
	ComputationNode node;
	Player player;
	int priority = 0;
	float duration = 0;

	[SerializeField] float minDuration = 10f;
	[SerializeField] float maxDuration = 180f;

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
			player = Player.GetPlayer (job.playerId);
			UpdateUI ();
		}
	}

	void UpdateUI() {
		//TODO: Set slider min max;
		Debug.Log("Deploy considering " + job.jobType);
	}

	public void Deploy() {
		if (player.credits.ConsumeCredits (job.playerId, SkillSystem.calculateDeploymentCost(job.jobType, priority, duration))) {
			job.Deploy (node, priority, duration);
			Dismiss ();
		}
	}

	public void Dismiss() {
		job = null;
		node = null;
		//TODO: Hide UI

	}

	void OnGUI() {
		// Placeholder

		if (job != null && node != null) {
			GUILayout.BeginArea (new Rect (200, 10, 150, 100));
			GUILayout.BeginVertical ();

			GUILayout.Label (job.jobType.ToString());

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Priority");
			priority = Mathf.RoundToInt(GUILayout.HorizontalSlider (priority, 0, node.maxPriority))	;
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Duration");
			duration = GUILayout.HorizontalSlider (duration, minDuration, maxDuration);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			var deployable = player.credits.CanAfford (SkillSystem.calculateDeploymentCost (job.jobType, priority, duration));
			if (GUILayout.Button("Dismiss")) {
				Dismiss();
			}
				
			if (deployable && GUILayout.Button ("Deploy")) {
				Deploy();
			}
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
		}
	}
}
