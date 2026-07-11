using System;

namespace System.Xml.Serialization
{
	internal abstract class TypeModel
	{
		protected TypeModel(Type type, TypeDesc typeDesc, ModelScope scope)
		{
			this.scope = scope;
			this.type = type;
			this.typeDesc = typeDesc;
		}

		internal Type Type
		{
			get
			{
				return this.type;
			}
		}

		internal ModelScope ModelScope
		{
			get
			{
				return this.scope;
			}
		}

		internal TypeDesc TypeDesc
		{
			get
			{
				return this.typeDesc;
			}
		}

		private TypeDesc typeDesc;

		private Type type;

		private ModelScope scope;
	}
}
