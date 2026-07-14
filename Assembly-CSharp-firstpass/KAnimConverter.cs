using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimConverter
{
	// Note: this type is marked as 'beforefieldinit'.
	static KAnimConverter()
	{
		Dictionary<KAnimConverter.PostProcessingEffects, string> dictionary = new Dictionary<KAnimConverter.PostProcessingEffects, string>();
		dictionary[KAnimConverter.PostProcessingEffects.TemperatureOverlay] = "Klei/BatchedAnimationPstTemperature";
		KAnimConverter.ShaderNameForPostProcessingEffect = dictionary;
	}

	public static readonly Dictionary<KAnimConverter.PostProcessingEffects, string> ShaderNameForPostProcessingEffect;

	[Flags]
	public enum PostProcessingEffects : byte
	{
		TemperatureOverlay = 1
	}

	public interface IAnimConverter
	{
		int GetMaxVisible();

		KAnimConverter.PostProcessingEffects GetPostProcessingEffectsCompatibility();

		float GetPostProcessingParams();

		HashedString GetBatchGroupID(bool isEditorWindow = false);

		HashedString GetBatchGroupIDOverride();

		KAnimBatch GetBatch();

		void SetBatch(KAnimBatch id);

		Vector2I GetCellXY();

		float GetZ();

		int GetLayer();

		string GetName();

		bool IsActive();

		bool IsVisible();

		bool IsAlwaysVisible();

		Vector4 GetPositionData();

		int GetCurrentNumFrames();

		int GetFirstFrameIndex();

		int GetCurrentFrameIndex();

		Matrix2x3 GetTransformMatrix();

		KBatchedAnimInstanceData GetBatchInstanceData();

		SymbolInstanceGpuData symbolInstanceGpuData { get; }

		SymbolOverrideInfoGpuData symbolOverrideInfoGpuData { get; }

		KAnimBatchGroup.MaterialType GetMaterialType();

		bool ApplySymbolOverrides();
	}
}
