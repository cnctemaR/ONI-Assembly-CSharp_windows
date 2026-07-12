using System;
using Klei;
using UnityEngine;

public class AutoStorageDropper : GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.EventTransition(GameHashes.OnStorageChange, this.pre_drop, null);
		this.pre_drop.ScheduleGoTo(0f, this.dropping);
		this.dropping.Enter(delegate(AutoStorageDropper.Instance smi)
		{
			smi.Drop();
		}).GoTo(this.idle);
	}

	private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State idle;

	private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State pre_drop;

	private GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.State dropping;

	public class Def : StateMachine.BaseDef
	{
		public Tag dropTag;

		public CellOffset dropOffset;

		public bool asOre;

		public float maxRate = float.MaxValue;

		public bool onlyWhenOperational;
	}

	public new class Instance : GameStateMachine<AutoStorageDropper, AutoStorageDropper.Instance, IStateMachineTarget, AutoStorageDropper.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, AutoStorageDropper.Def def)
			: base(master, def)
		{
			this.storage = master.GetComponent<Storage>();
			this.rotatable = master.GetComponent<Rotatable>();
		}

		public void Drop()
		{
			for (int i = this.storage.Count - 1; i >= 0; i--)
			{
				GameObject gameObject = this.storage.items[i];
				if (gameObject.HasTag(base.def.dropTag))
				{
					if (base.def.asOre)
					{
						Vector3 vector = ((this.rotatable != null) ? (base.transform.GetPosition() + this.rotatable.GetRotatedCellOffset(base.def.dropOffset).ToVector3()) : (base.transform.GetPosition() + base.def.dropOffset.ToVector3()));
						this.storage.Drop(gameObject, true);
						gameObject.transform.SetPosition(vector);
					}
					else
					{
						int num = ((this.rotatable != null) ? Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), this.rotatable.GetRotatedCellOffset(base.def.dropOffset)) : Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), base.def.dropOffset));
						float num2;
						SimUtil.DiseaseInfo diseaseInfo;
						float num3;
						this.storage.ConsumeAndGetDisease(base.def.dropTag, float.MaxValue, out num2, out diseaseInfo, out num3);
						Element element = ElementLoader.GetElement(base.def.dropTag);
						byte idx = element.idx;
						if (element.IsLiquid)
						{
							FallingWater.instance.AddParticle(num, idx, num2, num3, diseaseInfo.idx, diseaseInfo.count, true, false, false, false);
						}
						else
						{
							SimMessages.ModifyCell(num, (int)idx, num3, num2, diseaseInfo.idx, diseaseInfo.count, SimMessages.ReplaceType.None, false, -1);
						}
					}
				}
			}
		}

		private Storage storage;

		private Rotatable rotatable;
	}
}
