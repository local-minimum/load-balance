using UnityEngine;
using System.Collections.Generic;

public enum SingletonType {MainCamera, EventSystem};

public class SingletonByType : MonoBehaviour {

	public SingletonType singletonType;

	static Dictionary<SingletonType, MonoBehaviour> lookup = new Dictionary<SingletonType, MonoBehaviour>();

	void Awake () {
		if (lookup.ContainsKey (singletonType)) {
			if (lookup [singletonType] != this)
				Destroy (gameObject);
		} else {
			lookup [singletonType] = this;
		}
	}

}
