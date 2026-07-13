using System;
using System.Collections.Generic;

namespace Database
{
	public class MooSongModifiers : ResourceSet<MooSongModifier>
	{
		public List<MooSongModifier> GetForTag(Tag searchTag)
		{
			List<MooSongModifier> list = new List<MooSongModifier>();
			foreach (MooSongModifier mooSongModifier in this.resources)
			{
				if (mooSongModifier.TargetTag == searchTag)
				{
					list.Add(mooSongModifier);
				}
			}
			return list;
		}
	}
}
