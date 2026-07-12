using System;
using Mono.Cecil.Metadata;

namespace Mono.Cecil
{
	internal sealed class ByReferenceType : TypeSpecification
	{
		public override string Name
		{
			get
			{
				return base.Name + "&";
			}
		}

		public override string FullName
		{
			get
			{
				return base.FullName + "&";
			}
		}

		public override bool IsValueType
		{
			get
			{
				return false;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override bool IsByReference
		{
			get
			{
				return true;
			}
		}

		public ByReferenceType(TypeReference type)
			: base(type)
		{
			Mixin.CheckType(type);
			this.etype = Mono.Cecil.Metadata.ElementType.ByRef;
		}
	}
}
