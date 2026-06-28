using System;

public class KAnimConverter
{
	public interface IAnimConverter
	{
		int GetMaxVisible();

		HashedString GetBatchGroupID(bool isEditorWindow = false);

		KAnimBatch GetBatch();

		void SetBatch(KAnimBatch id);

		Vector2I GetCellXY();

		float GetZ();

		int GetLayer();

		string GetName();

		KAnimFile[] GetAnims();

		bool IsActive();

		bool IsVisible();

		int GetCurrentNumFrames();

		int GetFirstFrameIndex();

		int GetCurrentFrameIndex();

		Matrix2x3 GetTransformMatrix();

		KBatchedAnimInstanceData GetBatchInstanceData();

		SymbolInstanceGpuData symbolInstanceGpuData { get; }

		BatchGroupInstance batchGroupInstance { get; }

		KAnimBatchGroup.MaterialType GetMaterialType();
	}
}
