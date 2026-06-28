using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class AttackableBase : Workable, IApproachable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.attributeConverter = Db.Get().AttributeConverters.AttackDamage;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		this.SetupScenePartitioner(null);
		base.Subscribe(1088554450, delegate(object o)
		{
			if (this.scenePartitionerEntry != null)
			{
				this.scenePartitionerEntry.UpdatePosition(Grid.PosToCell(base.gameObject));
			}
		});
		base.Subscribe(-1506500077, new Action<object>(this.OnDefeated));
		base.Subscribe(-1256572400, new Action<object>(this.SetupScenePartitioner));
		base.Subscribe(1623392196, new Action<object>(this.OnDefeated));
		Prioritizable component = base.GetComponent<Prioritizable>();
		if (component != null)
		{
			component.showIcon = false;
		}
	}

	public float GetDamageMultiplier()
	{
		if (this.attributeConverter != null && base.worker != null)
		{
			AttributeConverterInstance converter = base.worker.GetComponent<AttributeConverters>().GetConverter(this.attributeConverter.Id);
			return Mathf.Max(1f + converter.Evaluate(), 0.1f);
		}
		return 1f;
	}

	private void SetupScenePartitioner(object data = null)
	{
		Extents extents = new Extents(Grid.PosToXY(base.transform.GetPosition()).x, Grid.PosToXY(base.transform.GetPosition()).y, 1, 1);
		this.scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.gameObject.name, base.GetComponent<FactionAlignment>(), extents, GameScenePartitioner.Instance.attackableEntitiesLayer, null);
	}

	private void OnDefeated(object data = null)
	{
		if (this.scenePartitionerEntry != null)
		{
			this.scenePartitionerEntry.Release();
			this.scenePartitionerEntry = null;
		}
	}

	public override float GetEfficiencyMultiplier(Worker worker)
	{
		return 1f;
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(-1506500077, new Action<object>(this.OnDefeated));
		base.Unsubscribe(1623392196, new Action<object>(this.OnDefeated));
		base.Unsubscribe(-1256572400, new Action<object>(this.SetupScenePartitioner));
		if (this.scenePartitionerEntry != null)
		{
			this.scenePartitionerEntry.Release();
			this.scenePartitionerEntry = null;
		}
		base.OnCleanUp();
	}

	private GameScenePartitionerEntry scenePartitionerEntry;
}
