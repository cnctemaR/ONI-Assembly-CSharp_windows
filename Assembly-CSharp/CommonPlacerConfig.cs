using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CommonPlacerConfig
{
	public GameObject CreatePrefab(string id, string name, Material default_material)
	{
		GameObject gameObject = EntityTemplates.CreateEntity(id, name);
		gameObject.layer = LayerMask.NameToLayer("PlaceWithDepth");
		SaveLoadRoot saveLoadRoot = gameObject.AddOrGet<SaveLoadRoot>();
		saveLoadRoot.folder = Folder.Placers;
		gameObject.AddOrGet<SavedObject>();
		gameObject.AddOrGet<StateMachineController>();
		Prioritizable prioritizable = gameObject.AddOrGet<Prioritizable>();
		prioritizable.iconOffset = new Vector2(0.3f, 0.32f);
		KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kboxCollider2D.offset = new Vector2(0f, 0.5f);
		kboxCollider2D.size = new Vector2(1f, 1f);
		GameObject gameObject2 = new GameObject("Mask");
		gameObject2.layer = LayerMask.NameToLayer("PlaceWithDepth");
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.transform.SetLocalPosition(new Vector3(0f, 0.5f, -3.537f));
		gameObject2.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = Assets.instance.commonPlacerAssets.mesh;
		MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
		meshRenderer.lightProbeUsage = LightProbeUsage.Off;
		meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		meshRenderer.sharedMaterial = default_material;
		EasingAnimations easingAnimations = gameObject2.AddComponent<EasingAnimations>();
		EasingAnimations.AnimationScales animationScales = default(EasingAnimations.AnimationScales);
		animationScales.name = "ScaleUp";
		animationScales.startScale = 0f;
		animationScales.endScale = 1f;
		animationScales.type = EasingAnimations.AnimationScales.AnimationType.EaseInOutBack;
		animationScales.easingMultiplier = 5f;
		EasingAnimations.AnimationScales animationScales2 = new EasingAnimations.AnimationScales
		{
			name = "ScaleDown",
			startScale = 1f,
			endScale = 0f,
			type = EasingAnimations.AnimationScales.AnimationType.EaseOutBack,
			easingMultiplier = 1f
		};
		easingAnimations.scales = new EasingAnimations.AnimationScales[] { animationScales, animationScales2 };
		return gameObject;
	}

	[Serializable]
	public class CommonPlacerAssets
	{
		public Mesh mesh;
	}
}
