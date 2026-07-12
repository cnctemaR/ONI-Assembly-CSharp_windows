using System;
using KSerialization;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Cell : Node
	{
		public long NodeId
		{
			get
			{
				return base.node.Id;
			}
		}
	}
}
