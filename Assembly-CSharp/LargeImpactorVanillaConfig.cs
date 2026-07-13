using System;
using UnityEngine;

public class LargeImpactorVanillaConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return new string[] { "", "DLC4_ID" };
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	GameObject IEntityConfig.CreatePrefab()
	{
		return LargeImpactorVanillaConfig.ConfigCommon(LargeImpactorVanillaConfig.ID, LargeImpactorVanillaConfig.NAME);
	}

	public static GameObject ConfigCommon(string id, string name)
	{
		GameObject gameObject = EntityTemplates.CreateEntity(id, name, true);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<StateMachineController>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGet<LoopingSounds>();
		LargeImpactorStatus.Def def = gameObject.AddOrGetDef<LargeImpactorStatus.Def>();
		def.MAX_HEALTH = 1000;
		def.EventID = "LargeImpactor";
		gameObject.AddOrGet<LargeImpactorVisualizer>();
		gameObject.AddOrGet<LargeImpactorCrashStamp>().largeStampTemplate = "dlc4::poi/asteroid_impacts/potato_large";
		gameObject.AddOrGetDef<LargeImpactorNotificationMonitor.Def>();
		gameObject.AddOrGet<ParallaxBackgroundObject>().Initialize("Demolior_final_whole");
		return gameObject;
	}

	void IEntityConfig.OnPrefabInit(GameObject inst)
	{
	}

	private static LargeImpactorStatus.Instance GetStatusMonitor()
	{
		return ((LargeImpactorEvent.StatesInstance)GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.Id, -1).smi).impactorInstance.GetSMI<LargeImpactorStatus.Instance>();
	}

	public static void SpawnCommon(GameObject inst)
	{
		ParallaxBackgroundObject component = inst.GetComponent<ParallaxBackgroundObject>();
		component.motion = new LargeImpactorVanillaConfig.BackgroundMotion();
		LargeImpactorStatus.Instance statusMonitor = LargeImpactorVanillaConfig.GetStatusMonitor();
		if (statusMonitor != null)
		{
			LargeImpactorStatus.Instance instance = statusMonitor;
			instance.OnDamaged = (Action<int>)Delegate.Combine(instance.OnDamaged, new Action<int>(component.TriggerShaderDamagedEffect));
		}
	}

	void IEntityConfig.OnSpawn(GameObject inst)
	{
		LargeImpactorVanillaConfig.SpawnCommon(inst);
	}

	public static string ID = "LargeImpactorVanilla";

	public static string NAME = "LargestPotaytoeVanilla";

	public class BackgroundMotion : ParallaxBackgroundObject.IMotion
	{
		private LargeImpactorStatus.Instance StatusMonitor
		{
			get
			{
				if (this.statusMonitor == null)
				{
					this.statusMonitor = LargeImpactorVanillaConfig.GetStatusMonitor();
				}
				return this.statusMonitor;
			}
		}

		public float GetETA()
		{
			if (!this.StatusMonitor.IsRunning())
			{
				return this.GetDuration();
			}
			return this.StatusMonitor.TimeRemainingBeforeCollision;
		}

		public float GetDuration()
		{
			return LargeImpactorEvent.GetImpactTime();
		}

		public void OnNormalizedDistanceChanged(float normalizedDistance)
		{
			AmbienceManager.Quadrant[] quadrants = Game.Instance.GetComponent<AmbienceManager>().quadrants;
			for (int i = 0; i < quadrants.Length; i++)
			{
				quadrants[i].spaceLayer.SetCustomParameter("distanceToMeteor", normalizedDistance);
			}
		}

		private LargeImpactorStatus.Instance statusMonitor;
	}
}
