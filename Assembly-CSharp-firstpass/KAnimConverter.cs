using System;
using UnityEngine;

public class KAnimConverter
{
	public interface IAnimConverter
	{
		int GetMaxVisible();

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
