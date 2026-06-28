using System;
using KSerialization;
using UnityEngine;

public class SeedProducer : KMonoBehaviour
{
	public void Configure(string SeedID)
	{
		this.SeedID = SeedID;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(-216549700, new EventSystem.EventHandler(this.ProduceSeed));
		this.Subscribe(1623392196, new EventSystem.EventHandler(this.ProduceSeed));
	}

	public void ProduceSeed(object data)
	{
		if (this.produced)
		{
			return;
		}
		if (this.SeedID != null)
		{
			Vector3 vector = base.gameObject.transform.position + new Vector3(0f, 0.5f, 0f);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag(this.SeedID)), vector, Grid.SceneLayer.Use, SceneOrganizer.Instance.GetFolder(Folder.Entities), null, 0);
			PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			component2.Temperature = component.Temperature;
			gameObject.SetActive(true);
			this.produced = true;
		}
	}

	public string SeedID;

	[Serialize]
	private bool produced;
}
