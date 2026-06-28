using System;
using KSerialization;
using Satsuma;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Arc
	{
		public Arc()
		{
		}

		public Arc(string type)
		{
			this.type = type;
		}

		public Arc(Arc arc, string type)
		{
			this.arc = arc;
			this.type = type;
		}

		public Arc arc { get; private set; }

		[Serialize]
		public string type = "";

		[Serialize]
		public TagSet tags;
	}
}
