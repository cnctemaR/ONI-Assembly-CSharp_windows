using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.NodeTypeResolvers;
using YamlDotNet.Serialization.ObjectFactories;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;
using YamlDotNet.Serialization.Utilities;
using YamlDotNet.Serialization.ValueDeserializers;

namespace YamlDotNet.Serialization
{
	public sealed class Deserializer
	{
		private void ThrowUnlessInBackwardsCompatibleMode()
		{
			if (this.backwardsCompatibleConfiguration == null)
			{
				throw new InvalidOperationException("This method / property exists for backwards compatibility reasons, but the Deserializer was created using the new configuration mechanism. To configure the Deserializer, use the DeserializerBuilder.");
			}
		}

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This property will be removed in future releases.")]
		public IList<INodeDeserializer> NodeDeserializers
		{
			get
			{
				this.ThrowUnlessInBackwardsCompatibleMode();
				return this.backwardsCompatibleConfiguration.NodeDeserializers;
			}
		}

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This property will be removed in future releases.")]
		public IList<INodeTypeResolver> TypeResolvers
		{
			get
			{
				this.ThrowUnlessInBackwardsCompatibleMode();
				return this.backwardsCompatibleConfiguration.TypeResolvers;
			}
		}

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This constructor will be removed in future releases.")]
		public Deserializer(IObjectFactory objectFactory = null, INamingConvention namingConvention = null, bool ignoreUnmatched = false, YamlAttributeOverrides overrides = null)
		{
			this.backwardsCompatibleConfiguration = new Deserializer.BackwardsCompatibleConfiguration(objectFactory, namingConvention, ignoreUnmatched, overrides);
			this.valueDeserializer = this.backwardsCompatibleConfiguration.valueDeserializer;
		}

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This method will be removed in future releases.")]
		public void RegisterTagMapping(string tag, Type type)
		{
			this.ThrowUnlessInBackwardsCompatibleMode();
			this.backwardsCompatibleConfiguration.RegisterTagMapping(tag, type);
		}

		[Obsolete("Please use DeserializerBuilder to customize the Deserializer. This method will be removed in future releases.")]
		public void RegisterTypeConverter(IYamlTypeConverter typeConverter)
		{
			this.ThrowUnlessInBackwardsCompatibleMode();
			this.backwardsCompatibleConfiguration.RegisterTypeConverter(typeConverter);
		}

		public Deserializer()
		{
			this.backwardsCompatibleConfiguration = new Deserializer.BackwardsCompatibleConfiguration(null, null, false, null);
			this.valueDeserializer = this.backwardsCompatibleConfiguration.valueDeserializer;
		}

		private Deserializer(IValueDeserializer valueDeserializer)
		{
			if (valueDeserializer == null)
			{
				throw new ArgumentNullException("valueDeserializer");
			}
			this.valueDeserializer = valueDeserializer;
		}

		public static Deserializer FromValueDeserializer(IValueDeserializer valueDeserializer)
		{
			return new Deserializer(valueDeserializer);
		}

		public T Deserialize<T>(string input)
		{
			T t;
			using (StringReader stringReader = new StringReader(input))
			{
				t = (T)((object)this.Deserialize(stringReader, typeof(T)));
			}
			return t;
		}

		public T Deserialize<T>(TextReader input)
		{
			return (T)((object)this.Deserialize(input, typeof(T)));
		}

		public object Deserialize(TextReader input)
		{
			return this.Deserialize(input, typeof(object));
		}

		public object Deserialize(string input, Type type)
		{
			object obj;
			using (StringReader stringReader = new StringReader(input))
			{
				obj = this.Deserialize(stringReader, type);
			}
			return obj;
		}

		public object Deserialize(TextReader input, Type type)
		{
			return this.Deserialize(new Parser(input), type);
		}

		public T Deserialize<T>(IParser parser)
		{
			return (T)((object)this.Deserialize(parser, typeof(T)));
		}

		public object Deserialize(IParser parser)
		{
			return this.Deserialize(parser, typeof(object));
		}

		public object Deserialize(IParser parser, Type type)
		{
			if (parser == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			bool flag = parser.Allow<StreamStart>() != null;
			bool flag2 = parser.Allow<DocumentStart>() != null;
			object obj = null;
			if (!parser.Accept<DocumentEnd>() && !parser.Accept<StreamEnd>())
			{
				using (SerializerState serializerState = new SerializerState())
				{
					obj = this.valueDeserializer.DeserializeValue(parser, type, serializerState, this.valueDeserializer);
					serializerState.OnDeserialization();
				}
			}
			if (flag2)
			{
				parser.Expect<DocumentEnd>();
			}
			if (flag)
			{
				parser.Expect<StreamEnd>();
			}
			return obj;
		}

		private readonly Deserializer.BackwardsCompatibleConfiguration backwardsCompatibleConfiguration;

		private readonly IValueDeserializer valueDeserializer;

		private class BackwardsCompatibleConfiguration
		{
			public IList<INodeDeserializer> NodeDeserializers { get; private set; }

			public IList<INodeTypeResolver> TypeResolvers { get; private set; }

			public BackwardsCompatibleConfiguration(IObjectFactory objectFactory, INamingConvention namingConvention, bool ignoreUnmatched, YamlAttributeOverrides overrides)
			{
				objectFactory = objectFactory ?? new DefaultObjectFactory();
				namingConvention = namingConvention ?? new NullNamingConvention();
				this.typeDescriptor.TypeDescriptor = new CachedTypeInspector(new NamingConventionTypeInspector(new YamlAttributesTypeInspector(new YamlAttributeOverridesInspector(new ReadableAndWritablePropertiesTypeInspector(new ReadablePropertiesTypeInspector(new StaticTypeResolver())), overrides)), namingConvention));
				this.converters = new List<IYamlTypeConverter>();
				this.converters.Add(new GuidConverter(false));
				this.NodeDeserializers = new List<INodeDeserializer>();
				this.NodeDeserializers.Add(new YamlConvertibleNodeDeserializer(objectFactory));
				this.NodeDeserializers.Add(new YamlSerializableNodeDeserializer(objectFactory));
				this.NodeDeserializers.Add(new TypeConverterNodeDeserializer(this.converters));
				this.NodeDeserializers.Add(new NullNodeDeserializer());
				this.NodeDeserializers.Add(new ScalarNodeDeserializer());
				this.NodeDeserializers.Add(new ArrayNodeDeserializer());
				this.NodeDeserializers.Add(new DictionaryNodeDeserializer(objectFactory));
				this.NodeDeserializers.Add(new CollectionNodeDeserializer(objectFactory));
				this.NodeDeserializers.Add(new EnumerableNodeDeserializer());
				this.NodeDeserializers.Add(new ObjectNodeDeserializer(objectFactory, this.typeDescriptor, ignoreUnmatched));
				this.tagMappings = new Dictionary<string, Type>(Deserializer.BackwardsCompatibleConfiguration.predefinedTagMappings);
				this.TypeResolvers = new List<INodeTypeResolver>();
				this.TypeResolvers.Add(new YamlConvertibleTypeResolver());
				this.TypeResolvers.Add(new YamlSerializableTypeResolver());
				this.TypeResolvers.Add(new TagNodeTypeResolver(this.tagMappings));
				this.TypeResolvers.Add(new TypeNameInTagNodeTypeResolver());
				this.TypeResolvers.Add(new DefaultContainersNodeTypeResolver());
				this.valueDeserializer = new AliasValueDeserializer(new NodeValueDeserializer(this.NodeDeserializers, this.TypeResolvers));
			}

			public void RegisterTagMapping(string tag, Type type)
			{
				this.tagMappings.Add(tag, type);
			}

			public void RegisterTypeConverter(IYamlTypeConverter typeConverter)
			{
				this.converters.Insert(0, typeConverter);
			}

			private static readonly Dictionary<string, Type> predefinedTagMappings = new Dictionary<string, Type>
			{
				{
					"tag:yaml.org,2002:map",
					typeof(Dictionary<object, object>)
				},
				{
					"tag:yaml.org,2002:bool",
					typeof(bool)
				},
				{
					"tag:yaml.org,2002:float",
					typeof(double)
				},
				{
					"tag:yaml.org,2002:int",
					typeof(int)
				},
				{
					"tag:yaml.org,2002:str",
					typeof(string)
				},
				{
					"tag:yaml.org,2002:timestamp",
					typeof(DateTime)
				}
			};

			private readonly Dictionary<string, Type> tagMappings;

			private readonly List<IYamlTypeConverter> converters;

			private Deserializer.BackwardsCompatibleConfiguration.TypeDescriptorProxy typeDescriptor = new Deserializer.BackwardsCompatibleConfiguration.TypeDescriptorProxy();

			public IValueDeserializer valueDeserializer;

			private class TypeDescriptorProxy : ITypeInspector
			{
				public IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
				{
					return this.TypeDescriptor.GetProperties(type, container);
				}

				public IPropertyDescriptor GetProperty(Type type, object container, string name, bool ignoreUnmatched)
				{
					return this.TypeDescriptor.GetProperty(type, container, name, ignoreUnmatched);
				}

				public ITypeInspector TypeDescriptor;
			}
		}
	}
}
