using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class ArtifactSelector : KMonoBehaviour
{
	public int AnalyzedArtifactCount
	{
		get
		{
			return this.analyzedArtifactCount;
		}
	}

	public int AnalyzedSpaceArtifactCount
	{
		get
		{
			return this.analyzedSpaceArtifactCount;
		}
	}

	public List<string> GetAnalyzedArtifactIDs()
	{
		return this.analyzedArtifatIDs;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ArtifactSelector.Instance = this;
	}

	public bool RecordArtifactAnalyzed(string id)
	{
		if (this.analyzedArtifatIDs.Contains(id))
		{
			return false;
		}
		this.analyzedArtifatIDs.Add(id);
		return true;
	}

	public void IncrementAnalyzedTerrestrialArtifacts()
	{
		this.analyzedArtifactCount++;
	}

	public void IncrementAnalyzedSpaceArtifacts()
	{
		this.analyzedSpaceArtifactCount++;
	}

	public string GetUniqueArtifactID()
	{
		List<string> list = new List<string>();
		foreach (string text in ArtifactConfig.artifactItems)
		{
			if (!this.placedArtifacts.Contains(text))
			{
				list.Add(text);
			}
		}
		string text2 = "artifact_officemug";
		if (list.Count != 0)
		{
			text2 = list[global::UnityEngine.Random.Range(0, list.Count)];
		}
		this.placedArtifacts.Add(text2);
		return text2;
	}

	public void ReserveArtifactID(string artifactID)
	{
		if (this.placedArtifacts.Contains(artifactID))
		{
			DebugUtil.Assert(true, string.Format("Tried to add {0} to placedArtifacts but it already exists in the list!", artifactID));
		}
		this.placedArtifacts.Add(artifactID);
	}

	public static ArtifactSelector Instance;

	[Serialize]
	private List<string> placedArtifacts = new List<string>();

	[Serialize]
	private int analyzedArtifactCount;

	[Serialize]
	private int analyzedSpaceArtifactCount;

	[Serialize]
	private List<string> analyzedArtifatIDs = new List<string>();

	private const string DEFAULT_ARTIFACT_ID = "artifact_officemug";
}
