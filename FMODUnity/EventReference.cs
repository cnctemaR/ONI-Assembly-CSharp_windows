using System;
using FMOD;

namespace FMODUnity
{
	[Serializable]
	public struct EventReference
	{
		public override string ToString()
		{
			return this.Guid.ToString();
		}

		public bool IsNull
		{
			get
			{
				return this.Guid.IsNull;
			}
		}

		public GUID Guid;
	}
}
