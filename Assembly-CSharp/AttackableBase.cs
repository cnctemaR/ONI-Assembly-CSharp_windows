using System;

public class AttackableBase : Workable, IApproachable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.SetupScenePartitioner(null);
		this.Subscribe(1088554450, delegate(object o)
		{
			if (this.scenePartitionerEntry != null)
			{
				this.scenePartitionerEntry.UpdatePosition(Grid.PosToCell(base.gameObject));
			}
		});
		this.Subscribe(-1506500077, new Action<object>(this.OnDefeated));
		this.Subscribe(-1256572400, new Action<object>(this.SetupScenePartitioner));
		this.Subscribe(1623392196, new Action<object>(this.OnDefeated));
	}

	private void SetupScenePartitioner(object data = null)
	{
		Extents extents = new Extents(Grid.PosToXY(this.transform.position).x, Grid.PosToXY(this.transform.position).y, 1, 1);
		int mask = GameScenePartitioner.Instance.attackableEntities.mask;
		this.scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.gameObject.name, base.GetComponent<FactionAlignment>(), extents, mask, null);
	}

	private void OnDefeated(object data = null)
	{
		if (this.scenePartitionerEntry != null)
		{
			this.scenePartitionerEntry.Release();
			this.scenePartitionerEntry = null;
		}
	}

	protected override void OnCleanUp()
	{
		this.Unsubscribe(-1506500077, new Action<object>(this.OnDefeated));
		this.Unsubscribe(1623392196, new Action<object>(this.OnDefeated));
		this.Unsubscribe(-1256572400, new Action<object>(this.SetupScenePartitioner));
		if (this.scenePartitionerEntry != null)
		{
			this.scenePartitionerEntry.Release();
			this.scenePartitionerEntry = null;
		}
		base.OnCleanUp();
	}

	private GameScenePartitionerEntry scenePartitionerEntry;
}
