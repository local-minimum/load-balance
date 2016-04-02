using UnityEngine;
using System.Collections.Generic;


public delegate void Token(string token, float frequency);

public class DataStream : MonoBehaviour {

	public event Token OnToken;

	[SerializeField] string[] tokens;

	[SerializeField, Range(0, 1)] float frequency = 0.25f;

	[SerializeField] float[] probabilities;

	[SerializeField, Range(0, 1)] float pMutate = 0.01f;

	[SerializeField, Range(0, 1)] float pMutationSize = 0.2f;

	void Start() {
		Seed ();
		StartCoroutine (_streamer ());
	}

	IEnumerator<WaitForSeconds> _streamer() {
		while (true) {

			if (Random.value < pMutate)
				Mutate ();
			var rnd = Random.value;
			int item = -1;
			for (item = 0; item < probabilities.Length; item++) {
				if (rnd < probabilities [item])
					break;
				else
					rnd -= probabilities [item];
			}

			if (item >= 0) {
				if (item == tokens.Length)
					item--;
				
				var token = tokens [item];
				if (OnToken != null)
					OnToken (token, frequency);

			}
			yield return new WaitForSeconds (frequency);
		}
	}

	void Seed() {
		probabilities = new float[tokens.Length];
		for (int i = 0; i < probabilities.Length; i++)
			probabilities [i] = 0.5f + Random.Range(-pMutationSize, pMutationSize);
		Norm ();
	}

	void Mutate() {
		for (int i = 0; i < probabilities.Length; i++) {
			probabilities [i] += Random.Range (-pMutationSize, pMutationSize);
		}
		Norm ();
	}

	void Norm() {
		float sum = 0;
		for (int i=0; i<probabilities.Length; i++) {
			probabilities [i] = Mathf.Clamp01 (probabilities [i]);
			sum += probabilities [i];
		}
		if (sum == 0f)
			Seed ();
		else {
			for (int i = 0; i < probabilities.Length; i++) {
				probabilities [i] /= sum;
			}
		}
			
	}
}
