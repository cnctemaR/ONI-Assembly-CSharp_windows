using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	internal class ClassDataNode : DataNode<object>
	{
		internal ClassDataNode()
		{
			this.dataType = Globals.TypeOfClassDataNode;
		}

		internal IList<ExtensionDataMember> Members
		{
			get
			{
				return this.members;
			}
			set
			{
				this.members = value;
			}
		}

		public override void Clear()
		{
			base.Clear();
			this.members = null;
		}

		private IList<ExtensionDataMember> members;
	}
}
