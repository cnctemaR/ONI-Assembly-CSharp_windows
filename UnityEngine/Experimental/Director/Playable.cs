using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[RequiredByNativeCode]
	public class Playable
	{
		public static implicit operator PlayableHandle(Playable b)
		{
			return b.handle;
		}

		public bool IsValid()
		{
			return this.handle.IsValid();
		}

		public PlayableHandle handle;
	}
}
