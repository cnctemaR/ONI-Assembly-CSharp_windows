using System;
using System.Collections.Generic;
using UnityEngine;

public class OreSizeVisualizerComponents : KGameObjectComponentManager<OreSizeVisualizerData>
{
	public HandleVector<int>.Handle Add(GameObject go)
	{
		HandleVector<int>.Handle handle = base.Add(go, new OreSizeVisualizerData(go));
		this.OnPrefabInit(handle);
		return handle;
	}

	public static HashedString GetAnimForMass(float mass)
	{
		return OreSizeVisualizerComponents.GetAnimForMass(OreSizeVisualizerComponents.TiersSetType.Ores, mass);
	}

	public static HashedString GetAnimForMass(OreSizeVisualizerComponents.TiersSetType tierType, float mass)
	{
		for (int i = 0; i < OreSizeVisualizerComponents.TierSets[tierType].Length; i++)
		{
			if (mass <= OreSizeVisualizerComponents.TierSets[tierType][i].massRequired)
			{
				return OreSizeVisualizerComponents.TierSets[tierType][i].animName;
			}
		}
		return HashedString.Invalid;
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle handle)
	{
		OreSizeVisualizerData data = base.GetData(handle);
		data.absorbHandle = data.primaryElement.Subscribe(-2064133523, OreSizeVisualizerComponents.OnMassChangedDispatcher, handle);
		data.splitFromChunkHandle = data.primaryElement.Subscribe(1335436905, OreSizeVisualizerComponents.OnMassChangedDispatcher, handle);
		base.SetData(handle, data);
	}

	protected override void OnSpawn(HandleVector<int>.Handle handle)
	{
		OreSizeVisualizerComponents.OnMassChanged(handle, null);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle handle)
	{
		OreSizeVisualizerData data = base.GetData(handle);
		if (data.primaryElement != null)
		{
			data.primaryElement.Unsubscribe(ref data.absorbHandle);
			data.primaryElement.Unsubscribe(ref data.splitFromChunkHandle);
		}
	}

	private static void OnMassChanged(HandleVector<int>.Handle handle, object other_data)
	{
		OreSizeVisualizerData data = GameComps.OreSizeVisualizers.GetData(handle);
		PrimaryElement primaryElement = data.primaryElement;
		float mass = primaryElement.Mass;
		OreSizeVisualizerComponents.MassTier massTier = default(OreSizeVisualizerComponents.MassTier);
		OreSizeVisualizerComponents.MassTier[] array = OreSizeVisualizerComponents.TierSets[data.tierSetType];
		for (int i = 0; i < array.Length; i++)
		{
			if (mass <= array[i].massRequired)
			{
				massTier = array[i];
				break;
			}
		}
		primaryElement.GetComponent<KBatchedAnimController>().Play(massTier.animName, KAnim.PlayMode.Once, 1f, 0f);
		KCircleCollider2D component = primaryElement.GetComponent<KCircleCollider2D>();
		if (component != null)
		{
			component.radius = massTier.colliderRadius;
		}
		primaryElement.Trigger(1807976145, null);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static OreSizeVisualizerComponents()
	{
		Dictionary<OreSizeVisualizerComponents.TiersSetType, OreSizeVisualizerComponents.MassTier[]> dictionary = new Dictionary<OreSizeVisualizerComponents.TiersSetType, OreSizeVisualizerComponents.MassTier[]>();
		dictionary[OreSizeVisualizerComponents.TiersSetType.Ores] = OreSizeVisualizerComponents.MassTiers;
		dictionary[OreSizeVisualizerComponents.TiersSetType.PokeShells] = OreSizeVisualizerComponents.PokeShellMassTiers;
		dictionary[OreSizeVisualizerComponents.TiersSetType.WoodPokeShells] = OreSizeVisualizerComponents.WoodPokeShellMassTiers;
		dictionary[OreSizeVisualizerComponents.TiersSetType.PlantFiber] = OreSizeVisualizerComponents.PlantMatterMassTiers;
		OreSizeVisualizerComponents.TierSets = dictionary;
		OreSizeVisualizerComponents.OnMassChangedDispatcher = delegate(object context, object data)
		{
			OreSizeVisualizerComponents.OnMassChanged((HandleVector<int>.Handle)context, data);
		};
	}

	private static readonly OreSizeVisualizerComponents.MassTier[] MassTiers = new OreSizeVisualizerComponents.MassTier[]
	{
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle1",
			massRequired = 50f,
			colliderRadius = 0.15f
		},
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle2",
			massRequired = 600f,
			colliderRadius = 0.2f
		},
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle3",
			massRequired = float.MaxValue,
			colliderRadius = 0.25f
		}
	};

	private static readonly OreSizeVisualizerComponents.MassTier[] PokeShellMassTiers = new OreSizeVisualizerComponents.MassTier[]
	{
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle1",
			massRequired = 7.5f,
			colliderRadius = 0.15f
		},
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle2",
			massRequired = 15f,
			colliderRadius = 0.2f
		},
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle3",
			massRequired = float.MaxValue,
			colliderRadius = 0.25f
		}
	};

	private static readonly OreSizeVisualizerComponents.MassTier[] WoodPokeShellMassTiers = new OreSizeVisualizerComponents.MassTier[]
	{
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle1",
			massRequired = 75f,
			colliderRadius = 0.15f
		},
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle2",
			massRequired = 150f,
			colliderRadius = 0.2f
		},
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle3",
			massRequired = float.MaxValue,
			colliderRadius = 0.25f
		}
	};

	private static readonly OreSizeVisualizerComponents.MassTier[] PlantMatterMassTiers = new OreSizeVisualizerComponents.MassTier[]
	{
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle1",
			massRequired = 10f,
			colliderRadius = 0.15f
		},
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle2",
			massRequired = 50f,
			colliderRadius = 0.2f
		},
		new OreSizeVisualizerComponents.MassTier
		{
			animName = "idle3",
			massRequired = float.MaxValue,
			colliderRadius = 0.25f
		}
	};

	private static readonly Dictionary<OreSizeVisualizerComponents.TiersSetType, OreSizeVisualizerComponents.MassTier[]> TierSets;

	private static Action<object, object> OnMassChangedDispatcher;

	private struct MassTier
	{
		public HashedString animName;

		public float massRequired;

		public float colliderRadius;
	}

	public enum TiersSetType
	{
		Ores,
		PokeShells,
		WoodPokeShells,
		PlantFiber
	}
}
