using System;
using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

public class MonumentPart : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.MonumentParts.Add(this);
		if (!string.IsNullOrEmpty(this.chosenState))
		{
			this.SetState(this.chosenState);
		}
		this.UpdateMonumentDecor();
	}

	protected override void OnCleanUp()
	{
		Components.MonumentParts.Remove(this);
		this.RemoveMonumentPiece();
		base.OnCleanUp();
	}

	public void SetState(string state)
	{
		base.GetComponent<KBatchedAnimController>().Play(state, KAnim.PlayMode.Once, 1f, 0f);
		this.chosenState = state;
	}

	public bool IsMonumentCompleted()
	{
		bool flag = this.GetMonumentPart(MonumentPart.Part.Top) != null;
		bool flag2 = this.GetMonumentPart(MonumentPart.Part.Middle) != null;
		bool flag3 = this.GetMonumentPart(MonumentPart.Part.Bottom) != null;
		return flag && flag3 && flag2;
	}

	public void UpdateMonumentDecor()
	{
		GameObject monumentPart = this.GetMonumentPart(MonumentPart.Part.Middle);
		if (this.IsMonumentCompleted())
		{
			DecorProvider component = monumentPart.GetComponent<DecorProvider>();
			component.SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.COMPLETE);
			List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>());
			foreach (GameObject gameObject in attachedNetwork)
			{
				if (gameObject != monumentPart)
				{
					DecorProvider component2 = gameObject.GetComponent<DecorProvider>();
					component2.SetValues(BUILDINGS.DECOR.NONE);
				}
			}
		}
	}

	public void RemoveMonumentPiece()
	{
		if (this.IsMonumentCompleted())
		{
			List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>());
			foreach (GameObject gameObject in attachedNetwork)
			{
				if (gameObject.GetComponent<MonumentPart>() != this)
				{
					DecorProvider component = gameObject.GetComponent<DecorProvider>();
					component.SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE);
				}
			}
		}
	}

	private GameObject GetMonumentPart(MonumentPart.Part requestPart)
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>());
		foreach (GameObject gameObject in attachedNetwork)
		{
			MonumentPart component = gameObject.GetComponent<MonumentPart>();
			if (!(component == null))
			{
				if (component.part == requestPart)
				{
					return gameObject;
				}
			}
		}
		return null;
	}

	public MonumentPart.Part part;

	public List<Tuple<string, string>> selectableStatesAndSymbols = new List<Tuple<string, string>>();

	public string stateUISymbol;

	[Serialize]
	private string chosenState;

	public enum Part
	{
		Bottom,
		Middle,
		Top
	}
}
