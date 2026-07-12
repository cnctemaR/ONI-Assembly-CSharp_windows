using System;
using System.Configuration;

namespace System.Runtime.Serialization.Configuration
{
	public sealed class TypeElement : ConfigurationElement
	{
		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ConfigurationPropertyCollection
					{
						new ConfigurationProperty("", typeof(ParameterElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection),
						new ConfigurationProperty("type", typeof(string), string.Empty, null, new StringValidator(0, int.MaxValue, null), ConfigurationPropertyOptions.None),
						new ConfigurationProperty("index", typeof(int), 0, null, new IntegerValidator(0, int.MaxValue, false), ConfigurationPropertyOptions.None)
					};
				}
				return this.properties;
			}
		}

		public TypeElement()
		{
		}

		public TypeElement(string typeName)
			: this()
		{
			if (string.IsNullOrEmpty(typeName))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
			}
			this.Type = typeName;
		}

		internal string Key
		{
			get
			{
				return this.key;
			}
		}

		[ConfigurationProperty("", DefaultValue = null, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ParameterElementCollection Parameters
		{
			get
			{
				return (ParameterElementCollection)base[""];
			}
		}

		protected override void Reset(ConfigurationElement parentElement)
		{
			TypeElement typeElement = (TypeElement)parentElement;
			this.key = typeElement.key;
			base.Reset(parentElement);
		}

		[StringValidator(MinLength = 0)]
		[ConfigurationProperty("type", DefaultValue = "")]
		public string Type
		{
			get
			{
				return (string)base["type"];
			}
			set
			{
				base["type"] = value;
			}
		}

		[IntegerValidator(MinValue = 0)]
		[ConfigurationProperty("index", DefaultValue = 0)]
		public int Index
		{
			get
			{
				return (int)base["index"];
			}
			set
			{
				base["index"] = value;
			}
		}

		internal Type GetType(string rootType, Type[] typeArgs)
		{
			return TypeElement.GetType(rootType, typeArgs, this.Type, this.Index, this.Parameters);
		}

		internal static Type GetType(string rootType, Type[] typeArgs, string type, int index, ParameterElementCollection parameters)
		{
			if (!string.IsNullOrEmpty(type))
			{
				Type type2 = global::System.Type.GetType(type, true);
				if (type2.IsGenericTypeDefinition)
				{
					if (parameters.Count != type2.GetGenericArguments().Length)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(global::System.Runtime.Serialization.SR.GetString("Generic parameter count do not match between known type and configuration. Type is '{0}', known type has {1} parameters, configuration has {2} parameters.", new object[]
						{
							type,
							type2.GetGenericArguments().Length,
							parameters.Count
						}));
					}
					Type[] array = new Type[parameters.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = parameters[i].GetType(rootType, typeArgs);
					}
					type2 = type2.MakeGenericType(array);
				}
				return type2;
			}
			if (typeArgs != null && index < typeArgs.Length)
			{
				return typeArgs[index];
			}
			int num = ((typeArgs == null) ? 0 : typeArgs.Length);
			if (num == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(global::System.Runtime.Serialization.SR.GetString("For known type configuration, index is out of bound. Root type: '{0}' has {1} type arguments, and index was {2}.", new object[] { rootType, num, index }));
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(global::System.Runtime.Serialization.SR.GetString("For known type configuration, index is out of bound. Root type: '{0}' has {1} type arguments, and index was {2}.", new object[] { rootType, num, index }));
		}

		private ConfigurationPropertyCollection properties;

		private string key = Guid.NewGuid().ToString();
	}
}
