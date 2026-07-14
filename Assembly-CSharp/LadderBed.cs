using System;
using System.Collections.Generic;
using FMOD.Studio;

public class LadderBed : GameStateMachine<LadderBed, LadderBed.Instance, IStateMachineTarget, LadderBed.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
	}

	public static string lightBedShakeSoundPath = GlobalAssets.GetSound("LadderBed_LightShake", false);

	public static string noDupeBedShakeSoundPath = GlobalAssets.GetSound("LadderBed_Shake", false);

	public static string LADDER_BED_COUNT_BELOW_PARAMETER = "bed_count";

	public class Def : StateMachine.BaseDef
	{
		public CellOffset[] offsets;
	}

	public new class Instance : GameStateMachine<LadderBed, LadderBed.Instance, IStateMachineTarget, LadderBed.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, LadderBed.Def def)
			: base(master, def)
		{
			ScenePartitionerLayer scenePartitionerLayer = GameScenePartitioner.Instance.objectLayers[40];
			this.m_cell = Grid.PosToCell(master.gameObject);
			foreach (CellOffset cellOffset in def.offsets)
			{
				int num = Grid.OffsetCell(this.m_cell, cellOffset);
				if (Grid.IsValidCell(this.m_cell) && Grid.IsValidCell(num))
				{
					this.m_partitionEntires.Add(GameScenePartitioner.Instance.Add("LadderBed.Constructor", base.gameObject, num, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(this.OnMoverChanged)));
					this.OnMoverChanged(null);
				}
			}
			AttachableBuilding attachable = this.m_attachable;
			attachable.onAttachmentNetworkChanged = (Action<object>)Delegate.Combine(attachable.onAttachmentNetworkChanged, new Action<object>(this.OnAttachmentChanged));
			this.OnAttachmentChanged(null);
			base.Subscribe(-717201811, new Action<object>(this.OnSleepDisturbedByMovement));
			master.GetComponent<KAnimControllerBase>().GetLayering().SetSyncLayeringTint(false);
		}

		private void OnSleepDisturbedByMovement(object obj)
		{
			base.GetComponent<KAnimControllerBase>().Play("interrupt_light", KAnim.PlayMode.Once, 1f, 0f);
			EventInstance eventInstance = SoundEvent.BeginOneShot(LadderBed.lightBedShakeSoundPath, base.smi.transform.GetPosition(), 1f, false);
			eventInstance.setParameterByName(LadderBed.LADDER_BED_COUNT_BELOW_PARAMETER, (float)this.numBelow, false);
			SoundEvent.EndOneShot(eventInstance);
		}

		private void OnAttachmentChanged(object data)
		{
			this.numBelow = AttachableBuilding.CountAttachedBelow(this.m_attachable);
		}

		private void OnMoverChanged(object obj)
		{
			Pickupable pickupable = obj as Pickupable;
			if (pickupable != null && pickupable.gameObject != null && pickupable.KPrefabID.HasTag(GameTags.BaseMinion) && pickupable.GetComponent<Navigator>().CurrentNavType == NavType.Ladder)
			{
				if (this.m_sleepable.worker == null)
				{
					base.GetComponent<KAnimControllerBase>().Play("interrupt_light_nodupe", KAnim.PlayMode.Once, 1f, 0f);
					EventInstance eventInstance = SoundEvent.BeginOneShot(LadderBed.noDupeBedShakeSoundPath, base.smi.transform.GetPosition(), 1f, false);
					eventInstance.setParameterByName(LadderBed.LADDER_BED_COUNT_BELOW_PARAMETER, (float)this.numBelow, false);
					SoundEvent.EndOneShot(eventInstance);
					return;
				}
				if (pickupable.gameObject != this.m_sleepable.worker.gameObject)
				{
					this.m_sleepable.worker.Trigger(-717201811, null);
				}
			}
		}

		protected override void OnCleanUp()
		{
			foreach (HandleVector<int>.Handle handle in this.m_partitionEntires)
			{
				GameScenePartitioner.Instance.Free(ref handle);
			}
			AttachableBuilding attachable = this.m_attachable;
			attachable.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(attachable.onAttachmentNetworkChanged, new Action<object>(this.OnAttachmentChanged));
			base.OnCleanUp();
		}

		private List<HandleVector<int>.Handle> m_partitionEntires = new List<HandleVector<int>.Handle>();

		private int m_cell;

		[MyCmpGet]
		private Ownable m_ownable;

		[MyCmpGet]
		private Sleepable m_sleepable;

		[MyCmpGet]
		private AttachableBuilding m_attachable;

		private int numBelow;
	}
}
