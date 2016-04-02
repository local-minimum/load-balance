using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ComputationNode : MonoBehaviour {

	[SerializeField, Range(3, 50)] int cacheSize;

	[SerializeField, Range(9, 99)] int maxPriority = 42;

	DataStream stream;

	int index = -1;

	[SerializeField] string[] cache;

	[SerializeField, Range(0, 30)] int delay;

	bool hasWarped = false;

	static System.Random _rnd;

	[SerializeField] Dictionary<int, List<ProcJob>> jobs = new Dictionary<int, List<ProcJob>>();

	void Awake() {
		stream = GetComponentInChildren<DataStream> ();
		cache = new string[cacheSize];
		if (_rnd == null)
			_rnd = new System.Random ();
	}

	void OnEnable() {
		stream.OnToken += HandleToken;
	}

	void OnDisable() {
		stream.OnToken -= HandleToken;
	}

	void HandleToken (string token, float frequency)
	{
		index++;
		if (!hasWarped && index > cache.Length)
			hasWarped = true;
		index %= cache.Length;
		cache [index] = token;

		DispatchToJobs ();
	}

	void DispatchToJobs() {
		for (int i = 0; i < maxPriority; i++) {
			if (jobs.ContainsKey (i) && jobs [i].Count > 0) {
				var selectFrom = Shuffle(jobs [i]);
				for (int j = 0; j < selectFrom.Length; j++) {
					var job = selectFrom [j];
					if (job == null || job.status == JobStatus.Expired) {
						jobs [i].Remove (job);
					} else {
						job.Work ();
					}
				}
			}
		}

	}

	static T[] Shuffle<T>(IEnumerable<T> data) {
		List<KeyValuePair<int, T>> list = new List<KeyValuePair<int, T>> ();

		foreach (T item in data) {
			list.Add (new KeyValuePair<int, T> (_rnd.Next (), item));
		}

		var sorted = from item in list
		             orderby item.Key
		             select item.Value;

		return sorted.ToArray ();
	}

	void Update() {
		if (cacheSize > cache.Length) {
			var newCache = new string[cacheSize];
			System.Array.Copy (cache, newCache, cache.Length);
			cache = newCache;
		}
	}

	public string[] GetCachedFrame(int size, int frame) {

		var visibleIndex = (index - delay);

		if (frame >= size) {
			
			return new string[0];

		} else if (hasWarped || size < visibleIndex - frame) {
			
			var ret = new string[size];
			var endIndex = (visibleIndex - frame) % cache.Length;
			var startIndex = (endIndex - size) % cache.Length;
			if (startIndex < endIndex)
				System.Array.Copy (cache, startIndex, ret, 0, size);
			else {
				System.Array.Copy (cache, 0, ret, size - endIndex - 1, endIndex + 1);
				System.Array.Copy (cache, startIndex, ret, 0, cache.Length - startIndex);
			}
			return ret;

		} else {
			
			return new string[0];

		}
	}

	public void AddJob(ProcJob job, int priority) {
		if (!jobs.ContainsKey (priority)) {
			jobs.Add (priority, new List<ProcJob> ());
		}
		jobs [priority].Add (job);
		job.frame = GetFrame (SkillSystem.getSkill (job.jobType).patternWindowSize);
	}

	public int GetFrame(int windowSize) {
		return index % windowSize;
	}
		
}
