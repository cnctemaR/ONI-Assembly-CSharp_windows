using System;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class DataMemberAttribute : Attribute
	{
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.isNameSetExplicitly = true;
			}
		}

		public bool IsNameSetExplicitly
		{
			get
			{
				return this.isNameSetExplicitly;
			}
		}

		public int Order
		{
			get
			{
				return this.order;
			}
			set
			{
				if (value < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Property 'Order' in DataMemberAttribute attribute cannot be a negative number.")));
				}
				this.order = value;
			}
		}

		public bool IsRequired
		{
			get
			{
				return this.isRequired;
			}
			set
			{
				this.isRequired = value;
			}
		}

		public bool EmitDefaultValue
		{
			get
			{
				return this.emitDefaultValue;
			}
			set
			{
				this.emitDefaultValue = value;
			}
		}

		private string name;

		private bool isNameSetExplicitly;

		private int order = -1;

		private bool isRequired;

		private bool emitDefaultValue = true;
	}
}
