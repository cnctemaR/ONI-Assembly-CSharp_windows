using System;
using Klei;
using Klei.AI;
using UnityEngine;

public class OilChangerWorkableUse : Workable, IGameObjectEffectDescriptor
{
	private OilChangerWorkableUse()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
		base.SetWorkTime(8.5f);
	}

	protected override void OnStartWork(WorkerBase worker)
	{
		base.OnStartWork(worker);
		if (worker != null)
		{
			Vector3 position = worker.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse);
			worker.transform.SetPosition(position);
		}
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			roomOfGameObject.roomType.TriggerRoomEffects(base.GetComponent<KPrefabID>(), worker.GetComponent<Effects>());
		}
	}

	protected override void OnStopWork(WorkerBase worker)
	{
		if (worker != null)
		{
			Vector3 position = worker.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
			worker.transform.SetPosition(position);
		}
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		Storage component = base.GetComponent<Storage>();
		BionicOilMonitor.Instance smi = worker.GetSMI<BionicOilMonitor.Instance>();
		if (smi != null)
		{
			float num = 200f - smi.CurrentOilMass;
			float num2 = Mathf.Min(component.GetMassAvailable(GameTags.LubricatingOil), num);
			float num3 = num2;
			float num4 = 0f;
			Storage component2 = base.GetComponent<Storage>();
			SimHashes simHashes = SimHashes.CrudeOil;
			foreach (SimHashes simHashes2 in BionicOilMonitor.LUBRICANT_TYPE_EFFECT.Keys)
			{
				float num5;
				SimUtil.DiseaseInfo diseaseInfo;
				float num6;
				component2.ConsumeAndGetDisease(simHashes2.CreateTag(), num3, out num5, out diseaseInfo, out num6);
				if (num5 > num4)
				{
					simHashes = simHashes2;
					num4 = num5;
				}
				num3 -= num5;
			}
			base.GetComponent<Storage>().ConsumeIgnoringDisease(GameTags.LubricatingOil, num3);
			smi.RefillOil(num2);
			Effects component3 = worker.GetComponent<Effects>();
			foreach (SimHashes simHashes3 in BionicOilMonitor.LUBRICANT_TYPE_EFFECT.Keys)
			{
				Effect effect = BionicOilMonitor.LUBRICANT_TYPE_EFFECT[simHashes3];
				if (simHashes == simHashes3)
				{
					component3.Add(effect, true);
				}
				else
				{
					component3.Remove(effect);
				}
			}
		}
		base.OnCompleteWork(worker);
	}
}
