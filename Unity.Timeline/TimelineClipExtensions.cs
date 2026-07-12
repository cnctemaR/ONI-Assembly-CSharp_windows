using System;

namespace UnityEngine.Timeline
{
	public static class TimelineClipExtensions
	{
		public static void MoveToTrack(this TimelineClip clip, TrackAsset destinationTrack)
		{
			if (clip == null)
			{
				throw new ArgumentNullException("'this' argument for MoveToTrack cannot be null.");
			}
			if (destinationTrack == null)
			{
				throw new ArgumentNullException("Cannot move TimelineClip to a null track.");
			}
			TrackAsset parentTrack = clip.GetParentTrack();
			Object asset = clip.asset;
			if (asset == null)
			{
				throw new InvalidOperationException("Cannot move a TimelineClip to a different track if the TimelineClip's PlayableAsset is null.");
			}
			if (parentTrack == destinationTrack)
			{
				throw new InvalidOperationException("TimelineClip is already on " + destinationTrack.name + ".");
			}
			if (!destinationTrack.ValidateClipType(asset.GetType()))
			{
				throw new InvalidOperationException(string.Concat(new string[]
				{
					"Track ",
					destinationTrack.name,
					" cannot contain clips of type ",
					clip.GetType().Name,
					"."
				}));
			}
			TimelineClipExtensions.MoveToTrack_Impl(clip, destinationTrack, asset, parentTrack);
		}

		public static bool TryMoveToTrack(this TimelineClip clip, TrackAsset destinationTrack)
		{
			if (clip == null)
			{
				throw new ArgumentNullException("'this' argument for TryMoveToTrack cannot be null.");
			}
			if (destinationTrack == null)
			{
				throw new ArgumentNullException("Cannot move TimelineClip to a null parent.");
			}
			TrackAsset parentTrack = clip.GetParentTrack();
			Object asset = clip.asset;
			if (asset == null)
			{
				return false;
			}
			if (!(parentTrack != destinationTrack))
			{
				return false;
			}
			if (!destinationTrack.ValidateClipType(asset.GetType()))
			{
				return false;
			}
			TimelineClipExtensions.MoveToTrack_Impl(clip, destinationTrack, asset, parentTrack);
			return true;
		}

		private static void MoveToTrack_Impl(TimelineClip clip, TrackAsset destinationTrack, Object asset, TrackAsset parentTrack)
		{
			parentTrack != null;
			clip.SetParentTrack_Internal(destinationTrack);
			if (parentTrack == null)
			{
				TimelineCreateUtilities.SaveAssetIntoObject(asset, destinationTrack);
				return;
			}
			if (parentTrack.timelineAsset != destinationTrack.timelineAsset)
			{
				TimelineCreateUtilities.RemoveAssetFromObject(asset, parentTrack);
				TimelineCreateUtilities.SaveAssetIntoObject(asset, destinationTrack);
			}
		}

		private static readonly string k_UndoSetParentTrackText = "Move Clip";
	}
}
