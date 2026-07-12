using System;
using Mono.Cecil.Metadata;

namespace Mono.Cecil
{
	internal sealed class RequiredModifierType : TypeSpecification, IModifierType
	{
		public TypeReference ModifierType
		{
			get
			{
				return this.modifier_type;
			}
			set
			{
				this.modifier_type = value;
			}
		}

		public override string Name
		{
			get
			{
				return base.Name + this.Suffix;
			}
		}

		public override string FullName
		{
			get
			{
				return base.FullName + this.Suffix;
			}
		}

		private string Suffix
		{
			get
			{
				string text = " modreq(";
				TypeReference typeReference = this.modifier_type;
				return text + ((typeReference != null) ? typeReference.ToString() : null) + ")";
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

		public override bool IsRequiredModifier
		{
			get
			{
				return true;
			}
		}

		public override bool ContainsGenericParameter
		{
			get
			{
				return this.modifier_type.ContainsGenericParameter || base.ContainsGenericParameter;
			}
		}

		public RequiredModifierType(TypeReference modifierType, TypeReference type)
			: base(type)
		{
			if (modifierType == null)
			{
				throw new ArgumentNullException(Mixin.Argument.modifierType.ToString());
			}
			Mixin.CheckType(type);
			this.modifier_type = modifierType;
			this.etype = Mono.Cecil.Metadata.ElementType.CModReqD;
		}

		private TypeReference modifier_type;
	}
}
