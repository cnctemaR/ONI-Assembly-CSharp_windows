using System;

public class BuildingPointStraw : KMonoBehaviour
{
	public int currentDepth
	{
		get
		{
			return this.depthAvailable;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.RefreshDepthAvailable();
		this.RegisterListenersToCellChanges();
	}

	protected override void OnCleanUp()
	{
		this.UnregisterListenersToCellChanges();
		base.OnCleanUp();
	}

	public int GetDepthOffset()
	{
		if (this.depthAvailable <= 0)
		{
			return -1;
		}
		return -this.depthAvailable;
	}

	public string GetSymbolSuffix()
	{
		if (this.depthAvailable <= 0)
		{
			return "1";
		}
		return this.depthAvailable.ToString();
	}

	public string GetAnimSuffix()
	{
		if (this.depthAvailable <= 0)
		{
			return "_1";
		}
		return "_" + this.depthAvailable.ToString();
	}

	public CellOffset GetBottomCellOffset()
	{
		int depthOffset = this.GetDepthOffset();
		return new CellOffset(0, depthOffset);
	}

	public int GetStrawCell()
	{
		return Grid.OffsetCell(Grid.PosToCell(base.gameObject), this.GetBottomCellOffset());
	}

	private int GetDepthAvailable()
	{
		int num = Grid.PosToCell(this);
		int num2 = 0;
		bool flag = false;
		for (int i = 1; i <= this.maxDepth; i++)
		{
			int num3 = Grid.OffsetCell(num, 0, -i);
			if (!Grid.IsValidCell(num3) || Grid.Solid[num3] || (Grid.ObjectLayers[1].ContainsKey(num3) && Grid.ObjectLayers[1][num3] != null && Grid.ObjectLayers[1][num3] != base.gameObject))
			{
				break;
			}
			num2 = i;
			if (Grid.IsLiquid(num3))
			{
				if (flag)
				{
					break;
				}
				flag = true;
			}
		}
		return num2;
	}

	private void RefreshDepthAvailable()
	{
		int num = this.GetDepthAvailable();
		bool flag = num != this.depthAvailable;
		this.depthAvailable = num;
		bool flag2 = Grid.IsLiquid(this.GetStrawCell());
		bool flag3 = this.isInLiquid != flag2;
		this.isInLiquid = flag2;
		if (!flag3 && !flag)
		{
			return;
		}
		this.RefreshAnims();
		base.Trigger(360192579, this);
	}

	private void RefreshAnims()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (this.usesSymbols)
		{
			for (int i = 1; i <= this.maxDepth; i++)
			{
				string text = "straw" + i.ToString();
				bool flag = i <= this.depthAvailable;
				component.SetSymbolVisiblity(text, flag);
			}
			return;
		}
		if (this.canControlAnimStates)
		{
			string text2 = ((this.depthAvailable > 0) ? ("_" + this.depthAvailable.ToString()) : "_1");
			string text3 = "on" + text2;
			component.Play(text3, KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	private void OnCellChanged(object data)
	{
		this.RefreshDepthAvailable();
	}

	private void RegisterListenersToCellChanges()
	{
		CellOffset[] array = new CellOffset[this.maxDepth];
		for (int i = 0; i < this.maxDepth; i++)
		{
			array[i] = new CellOffset(0, -(i + 1));
		}
		Extents extents = new Extents(Grid.PosToCell(base.transform.GetPosition()), array);
		this.partitionerEntry_solids = GameScenePartitioner.Instance.Add("FishDeliveryPointStraw", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnCellChanged));
		this.partitionerEntry_buildings = GameScenePartitioner.Instance.Add("FishDeliveryPointStraw", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnCellChanged));
		this.partitionerEntry_liquid = GameScenePartitioner.Instance.Add("FishDeliveryPointStraw", base.gameObject, extents, GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnCellChanged));
	}

	private void UnregisterListenersToCellChanges()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry_solids);
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry_buildings);
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry_liquid);
	}

	public const string SYMBOL_PREFIX = "straw";

	public const string ANIM_PREFIX = "on";

	public bool canControlAnimStates;

	public bool usesSymbols;

	public bool isInLiquid;

	public int maxDepth = 4;

	private int depthAvailable = -1;

	private HandleVector<int>.Handle partitionerEntry_solids;

	private HandleVector<int>.Handle partitionerEntry_buildings;

	private HandleVector<int>.Handle partitionerEntry_liquid;
}
