using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	public sealed class PropertyDescriptor : IPropertyDescriptor
	{
		public PropertyDescriptor(IPropertyDescriptor baseDescriptor)
		{
			this.baseDescriptor = baseDescriptor;
			this.Name = baseDescriptor.Name;
		}

		public string Name { get; set; }

		public Type Type
		{
			get
			{
				return this.baseDescriptor.Type;
			}
		}

		public Type TypeOverride
		{
			get
			{
				return this.baseDescriptor.TypeOverride;
			}
			set
			{
				this.baseDescriptor.TypeOverride = value;
			}
		}

		public int Order { get; set; }

		public ScalarStyle ScalarStyle
		{
			get
			{
				return this.baseDescriptor.ScalarStyle;
			}
			set
			{
				this.baseDescriptor.ScalarStyle = value;
			}
		}

		public bool CanWrite
		{
			get
			{
				return this.baseDescriptor.CanWrite;
			}
		}

		public void Write(object target, object value)
		{
			this.baseDescriptor.Write(target, value);
		}

		public T GetCustomAttribute<T>() where T : Attribute
		{
			return this.baseDescriptor.GetCustomAttribute<T>();
		}

		public IObjectDescriptor Read(object target)
		{
			return this.baseDescriptor.Read(target);
		}

		private readonly IPropertyDescriptor baseDescriptor;
	}
}
