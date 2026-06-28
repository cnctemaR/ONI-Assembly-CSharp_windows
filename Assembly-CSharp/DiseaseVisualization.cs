using System;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseVisualization : ScriptableObject
{
	public DiseaseVisualization.Info GetInfo(HashedString id)
	{
		foreach (DiseaseVisualization.Info info in this.info)
		{
			if (id == info.name)
			{
				return info;
			}
		}
		return default(DiseaseVisualization.Info);
	}

	public Sprite overlaySprite = null;

	public List<DiseaseVisualization.Info> info = new List<DiseaseVisualization.Info>();

	[Serializable]
	public struct Info
	{
		public Info(string name)
		{
			this.name = name;
			this.overlayColour = Color.red;
		}

		public string name;

		public Color32 overlayColour;
	}
}
