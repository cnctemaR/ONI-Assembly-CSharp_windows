using System;
using KSerialization;
using Satsuma;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Arc
	{
		public Arc arc { get; private set; }

		public void SetArc(Arc arc)
		{
			Debug.Assert(!this.arcSet, "Tried setting up an Arc twice, no go.");
			this.arc = arc;
			this.arcSet = true;
		}

		public void SetType(string type)
		{
			this.type = type;
		}

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

		private bool arcSet;

		[Serialize]
		public string type = "";

		[Serialize]
		public TagSet tags;
	}
}
