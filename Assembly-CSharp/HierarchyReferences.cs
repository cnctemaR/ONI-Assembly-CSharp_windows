using System;
using UnityEngine;

public class HierarchyReferences : KMonoBehaviour
{
	public bool HasReference(string name)
	{
		foreach (ElementReference elementReference in this.references)
		{
			if (elementReference.Name == name)
			{
				return true;
			}
		}
		return false;
	}

	public MonoBehaviour GetReference(string name)
	{
		foreach (ElementReference elementReference in this.references)
		{
			if (elementReference.Name == name)
			{
				return elementReference.behaviour;
			}
		}
		global::Debug.LogWarning("Couldn't find reference to object named {0} Make sure the name matches the field in the inspector.", null);
		return null;
	}

	public ElementReference[] references;
}
