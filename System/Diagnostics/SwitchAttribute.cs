using System;
using System.Reflection;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
	[global::System.MonoLimitation("This attribute is not considered in trace support.")]
	public sealed class SwitchAttribute : Attribute
	{
		public SwitchAttribute(string switchName, Type switchType)
		{
			if (switchName == null)
			{
				throw new ArgumentNullException("switchName");
			}
			if (switchType == null)
			{
				throw new ArgumentNullException("switchType");
			}
			this.name = switchName;
			this.type = switchType;
		}

		public static SwitchAttribute[] GetAll(Assembly assembly)
		{
			object[] customAttributes = assembly.GetCustomAttributes(typeof(SwitchAttribute), false);
			SwitchAttribute[] array = new SwitchAttribute[customAttributes.Length];
			for (int i = 0; i < customAttributes.Length; i++)
			{
				array[i] = (SwitchAttribute)customAttributes[i];
			}
			return array;
		}

		public string SwitchName
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.name == null)
				{
					throw new ArgumentNullException("value");
				}
				this.name = value;
			}
		}

		public string SwitchDescription
		{
			get
			{
				return this.desc;
			}
			set
			{
				if (this.desc == null)
				{
					throw new ArgumentNullException("value");
				}
				this.desc = value;
			}
		}

		public Type SwitchType
		{
			get
			{
				return this.type;
			}
			set
			{
				if (this.type == null)
				{
					throw new ArgumentNullException("value");
				}
				this.type = value;
			}
		}

		private string name;

		private string desc = string.Empty;

		private Type type;
	}
}
