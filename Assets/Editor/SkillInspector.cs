using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AbstractSkill), true)]
public class SkillInspector : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		var myTarget = target as AbstractSkill;

		if (myTarget.progress == SkillProgress.UnAvailable)
			return;
		
		if (GUILayout.Button (myTarget.progress == SkillProgress.Available ? "Learn" : "Buy")) {
			myTarget.Increase ();
		}

		EditorGUILayout.HelpBox (myTarget.progress.ToString (), MessageType.Info);
	}
}
