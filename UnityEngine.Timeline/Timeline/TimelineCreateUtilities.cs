using System;
using System.Collections.Generic;

namespace UnityEngine.Timeline
{
	internal static class TimelineCreateUtilities
	{
		public static string GenerateUniqueActorName(List<ScriptableObject> tracks, string name)
		{
			string text;
			if (!tracks.Exists((ScriptableObject x) => x != null && x.name == name))
			{
				text = name;
			}
			else
			{
				int num = 0;
				string text2 = name;
				if (!string.IsNullOrEmpty(name) && name[name.Length - 1] == ')')
				{
					int num2 = name.LastIndexOf('(');
					if (num2 > 0)
					{
						string text3 = name.Substring(num2 + 1, name.Length - num2 - 2);
						if (int.TryParse(text3, out num))
						{
							num++;
							text2 = name.Substring(0, num2);
						}
					}
				}
				text2 = text2.TrimEnd(new char[0]);
				for (int i = num; i < num + 5000; i++)
				{
					if (i > 0)
					{
						string result = string.Format("{0} ({1})", text2, i);
						if (!tracks.Exists((ScriptableObject x) => x != null && x.name == result))
						{
							return result;
						}
					}
				}
				text = name;
			}
			return text;
		}

		public static void SaveAssetIntoObject(Object childAsset, Object masterAsset)
		{
			if ((masterAsset.hideFlags & HideFlags.DontSave) != HideFlags.None)
			{
				childAsset.hideFlags |= HideFlags.DontSave;
			}
			else
			{
				childAsset.hideFlags |= HideFlags.HideInHierarchy;
			}
		}

		internal static bool ValidateParentTrack(TrackAsset parent, Type childType)
		{
			bool flag;
			if (childType == null || !typeof(TrackAsset).IsAssignableFrom(childType))
			{
				flag = false;
			}
			else if (parent == null)
			{
				flag = true;
			}
			else
			{
				SupportsChildTracksAttribute supportsChildTracksAttribute = Attribute.GetCustomAttribute(parent.GetType(), typeof(SupportsChildTracksAttribute)) as SupportsChildTracksAttribute;
				if (supportsChildTracksAttribute == null)
				{
					flag = false;
				}
				else if (supportsChildTracksAttribute.childType == null)
				{
					flag = true;
				}
				else if (childType == supportsChildTracksAttribute.childType)
				{
					int num = 0;
					TrackAsset trackAsset = parent;
					while (trackAsset != null && trackAsset.isSubTrack)
					{
						num++;
						trackAsset = trackAsset.parent as TrackAsset;
					}
					flag = num < supportsChildTracksAttribute.levels;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}
	}
}
