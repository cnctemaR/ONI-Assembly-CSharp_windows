using System;
using Unity;

namespace System.Linq.Expressions
{
	public sealed class LabelTarget
	{
		internal LabelTarget(Type type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public string Name { get; }

		public Type Type { get; }

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return "UnamedLabel";
		}

		internal LabelTarget()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
