using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FMODUnityResonance
{
	[AddComponentMenu("ResonanceAudio/FmodResonanceAudioRoom")]
	public class FmodResonanceAudioRoom : MonoBehaviour
	{
		private void OnEnable()
		{
			FmodResonanceAudio.UpdateAudioRoom(this, FmodResonanceAudio.IsListenerInsideRoom(this));
		}

		private void OnDisable()
		{
			FmodResonanceAudio.UpdateAudioRoom(this, false);
		}

		private void Update()
		{
			FmodResonanceAudio.UpdateAudioRoom(this, FmodResonanceAudio.IsListenerInsideRoom(this));
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, this.Size);
		}

		[FormerlySerializedAs("leftWall")]
		public FmodResonanceAudioRoom.SurfaceMaterial LeftWall = FmodResonanceAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;

		[FormerlySerializedAs("rightWall")]
		public FmodResonanceAudioRoom.SurfaceMaterial RightWall = FmodResonanceAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;

		[FormerlySerializedAs("floor")]
		public FmodResonanceAudioRoom.SurfaceMaterial Floor = FmodResonanceAudioRoom.SurfaceMaterial.ParquetOnConcrete;

		[FormerlySerializedAs("ceiling")]
		public FmodResonanceAudioRoom.SurfaceMaterial Ceiling = FmodResonanceAudioRoom.SurfaceMaterial.PlasterRough;

		[FormerlySerializedAs("backWall")]
		public FmodResonanceAudioRoom.SurfaceMaterial BackWall = FmodResonanceAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;

		[FormerlySerializedAs("frontWall")]
		public FmodResonanceAudioRoom.SurfaceMaterial FrontWall = FmodResonanceAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;

		[FormerlySerializedAs("reflectivity")]
		public float Reflectivity = 1f;

		[FormerlySerializedAs("reverbGainDb")]
		public float ReverbGainDb;

		[FormerlySerializedAs("reverbBrightness")]
		public float ReverbBrightness;

		[FormerlySerializedAs("reverbTime")]
		public float ReverbTime = 1f;

		[FormerlySerializedAs("size")]
		public Vector3 Size = Vector3.one;

		public enum SurfaceMaterial
		{
			Transparent,
			AcousticCeilingTiles,
			BrickBare,
			BrickPainted,
			ConcreteBlockCoarse,
			ConcreteBlockPainted,
			CurtainHeavy,
			FiberglassInsulation,
			GlassThin,
			GlassThick,
			Grass,
			LinoleumOnConcrete,
			Marble,
			Metal,
			ParquetOnConcrete,
			PlasterRough,
			PlasterSmooth,
			PlywoodPanel,
			PolishedConcreteOrTile,
			Sheetrock,
			WaterOrIceSurface,
			WoodCeiling,
			WoodPanel
		}
	}
}
