using System;
using System.Collections.Generic;

namespace UnityEngine.Timeline
{
	internal static class TimelineCreateUtilities
	{
		public static string GenerateUniqueActorName(List<ScriptableObject> tracks, string name)
		{
			if (!tracks.Exists((ScriptableObject x) => x != null && x.name == name))
			{
				return name;
			}
			int num = 0;
			string text = name;
			if (!string.IsNullOrEmpty(name) && name[name.Length - 1] == ')')
			{
				int num2 = name.LastIndexOf('(');
				if (num2 > 0 && int.TryParse(name.Substring(num2 + 1, name.Length - num2 - 2), out num))
				{
					num++;
					text = name.Substring(0, num2);
				}
			}
			text = text.TrimEnd(Array.Empty<char>());
			for (int i = num; i < num + 5000; i++)
			{
				if (i > 0)
				{
					string result = string.Format("{0} ({1})", text, i);
					if (!tracks.Exists((ScriptableObject x) => x != null && x.name == result))
					{
						return result;
					}
				}
			}
			return name;
		}

		public static void SaveAssetIntoObject(Object childAsset, Object masterAsset)
		{
			if (childAsset == null || masterAsset == null)
			{
				return;
			}
			if ((masterAsset.hideFlags & HideFlags.DontSave) != HideFlags.None)
			{
				childAsset.hideFlags |= HideFlags.DontSave;
				return;
			}
			childAsset.hideFlags |= HideFlags.HideInHierarchy;
		}

		public static AnimationClip CreateAnimationClipForTrack(string name, TrackAsset track, bool isLegacy)
		{
			TimelineAsset timelineAsset = ((track != null) ? track.timelineAsset : null);
			HideFlags hideFlags = ((track != null) ? track.hideFlags : HideFlags.None);
			AnimationClip animationClip = new AnimationClip();
			animationClip.legacy = isLegacy;
			animationClip.name = name;
			animationClip.frameRate = ((timelineAsset == null) ? TimelineAsset.EditorSettings.kDefaultFps : timelineAsset.editorSettings.fps);
			TimelineCreateUtilities.SaveAssetIntoObject(animationClip, timelineAsset);
			animationClip.hideFlags = hideFlags & ~HideFlags.HideInHierarchy;
			return animationClip;
		}

		public static bool ValidateParentTrack(TrackAsset parent, Type childType)
		{
			if (childType == null || !typeof(TrackAsset).IsAssignableFrom(childType))
			{
				return false;
			}
			if (parent == null)
			{
				return true;
			}
			if (parent is ILayerable && !parent.isSubTrack && parent.GetType() == childType)
			{
				return true;
			}
			SupportsChildTracksAttribute supportsChildTracksAttribute = Attribute.GetCustomAttribute(parent.GetType(), typeof(SupportsChildTracksAttribute)) as SupportsChildTracksAttribute;
			if (supportsChildTracksAttribute == null)
			{
				return false;
			}
			if (supportsChildTracksAttribute.childType == null)
			{
				return true;
			}
			if (childType == supportsChildTracksAttribute.childType)
			{
				int num = 0;
				TrackAsset trackAsset = parent;
				while (trackAsset != null && trackAsset.isSubTrack)
				{
					num++;
					trackAsset = trackAsset.parent as TrackAsset;
				}
				return num < supportsChildTracksAttribute.levels;
			}
			return false;
		}
	}
}
