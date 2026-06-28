using System;
using UnityEngine;

public interface IAnimBehaviour
{
	string name { get; }

	int previousFrame { get; }

	int currentFrame { get; }

	string currentAnimFile { get; }

	string currentAnim { get; }

	float normalizedTime { get; }

	Vector3 position { get; }

	ComponentType GetComponent<ComponentType>();

	void AddUpdatingEvent(AnimEvent ev);
}
