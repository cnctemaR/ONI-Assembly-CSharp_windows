using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ValveBase : KMonoBehaviour, ISaveLoadable
{
	public float CurrentFlow
	{
		get
		{
			return this.currentFlow;
		}
		set
		{
			this.currentFlow = value;
		}
	}

	public Accumulator Accumulator
	{
		get
		{
			return this.flowAccumulator;
		}
	}

	public float MaxFlow
	{
		get
		{
			return this.maxFlow;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.flowAccumulator = new Accumulator("Flow", this, 3f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.inputCell = component.GetUtilityInputCell();
		this.outputCell = component.GetUtilityOutputCell();
		Conduit.GetFlowManager(this.conduitType).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlow.Priority.Default);
		this.UpdateAnim();
		this.OnCmpEnable();
	}

	protected override void OnCleanUp()
	{
		Conduit.GetFlowManager(this.conduitType).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
		ConduitFlow.Conduit conduit = flowManager.GetConduit(this.inputCell);
		ConduitFlow.Conduit conduit2 = flowManager.GetConduit(this.outputCell);
		if (conduit == null || conduit2 == null)
		{
			this.UpdateAnim();
			return;
		}
		ConduitFlow.ConduitContents contents = conduit.GetContents();
		float num = Mathf.Min(contents.mass, this.currentFlow * dt);
		if (num > 0f)
		{
			float num2 = num / contents.mass;
			int num3 = (int)(num2 * (float)contents.diseaseCount);
			float num4 = flowManager.AddElement(this.outputCell, contents.element, num, contents.temperature, contents.diseaseIdx, num3);
			this.flowAccumulator.Accumulate(num4);
			if (num4 > 0f)
			{
				flowManager.RemoveElement(this.inputCell, num4);
			}
		}
		this.UpdateAnim();
	}

	public virtual void UpdateAnim()
	{
		float avgRate = this.flowAccumulator.AvgRate;
		if (avgRate > 0f)
		{
			for (int i = 0; i < this.animFlowRanges.Length; i++)
			{
				if (avgRate <= this.animFlowRanges[i].minFlow)
				{
					if (this.curFlowIdx != i)
					{
						this.curFlowIdx = i;
						this.controller.Play(this.animFlowRanges[i].animName, (avgRate > 0f) ? KAnim.PlayMode.Loop : KAnim.PlayMode.Once, 1f, 0f);
					}
					break;
				}
			}
		}
		else
		{
			this.controller.Play("off", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public float maxFlow = 0.5f;

	[Serialize]
	private float currentFlow;

	[MyCmpGet]
	protected KBatchedAnimController controller;

	protected Accumulator flowAccumulator;

	private int curFlowIdx = -1;

	private int inputCell;

	private int outputCell;

	[SerializeField]
	public ValveBase.AnimRangeInfo[] animFlowRanges;

	[Serializable]
	public struct AnimRangeInfo
	{
		public AnimRangeInfo(float min_flow, string anim_name)
		{
			this.minFlow = min_flow;
			this.animName = anim_name;
		}

		public float minFlow;

		public string animName;
	}
}
