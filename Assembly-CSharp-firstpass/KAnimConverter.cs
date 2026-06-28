using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class KAnimConverter
{
	public const int COST_PER_CONTROLLER = 48;

	public interface IAnimConverter : IEquatable<global::UnityEngine.Object>, IEquatable<KAnimConverter.IAnimConverter>, IEquatable<object>
	{
		int GetMaxVisible();

		HashedString GetBatchGroupID(bool isEditorWindow = false);

		Tag GetPrefabTag();

		KAnimBatch GetBatch();

		void SetBatch(KAnimBatch id);

		Vector3 GetPosition();

		int GetLayer();

		string GetName();

		KAnimFile[] GetAnims();

		bool IsActive();

		bool IsVisible();

		int GetCurrentNumFrames();

		int GetFirstFrameIndex();

		int GetCurrentFrameIndex();

		Matrix4x4 GetTransformMatrix();

		Color32 GetHighlightColour();

		Color32 GetFirstTintColour();

		Color32 GetSecondTintColour();

		Color32 GetTemperatureColour();

		int GetFirstTintIndex();

		int GetSecondTintIndex();

		KAnimBatchInstanceData GetBatchInstanceData();

		KAnimBatchGroup.MaterialType GetMaterialType();
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct ByteToFloatConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public float[] floats;
	}
}
