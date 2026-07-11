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
			monumentPart.GetComponent<DecorProvider>().SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.COMPLETE);
			foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
			{
				if (gameObject != monumentPart)
				{
					gameObject.GetComponent<DecorProvider>().SetValues(BUILDINGS.DECOR.NONE);
				}
			}
		}
	}

	public void RemoveMonumentPiece()
	{
		if (this.IsMonumentCompleted())
		{
			foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
			{
				if (gameObject.GetComponent<MonumentPart>() != this)
				{
					gameObject.GetComponent<DecorProvider>().SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE);
				}
			}
		}
	}

	private GameObject GetMonumentPart(MonumentPart.Part requestPart)
	{
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
		{
			MonumentPart component = gameObject.GetComponent<MonumentPart>();
			if (!(component == null) && component.part == requestPart)
			{
				return gameObject;
			}
		}
		return null;
	}

	public MonumentPart.Part part;

	public List<global::Tuple<string, string>> selectableStatesAndSymbols = new List<global::Tuple<string, string>>();

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
