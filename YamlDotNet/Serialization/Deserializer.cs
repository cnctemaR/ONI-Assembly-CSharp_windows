using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
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
		public IList<INodeDeserializer> NodeDeserializers { get; private set; }

		public IList<INodeTypeResolver> TypeResolvers { get; private set; }

		public Deserializer(IObjectFactory objectFactory = null, INamingConvention namingConvention = null, bool ignoreUnmatched = false)
		{
			objectFactory = objectFactory ?? new DefaultObjectFactory();
			namingConvention = namingConvention ?? new NullNamingConvention();
			this.typeDescriptor.TypeDescriptor = new YamlAttributesTypeInspector(new NamingConventionTypeInspector(new ReadableAndWritablePropertiesTypeInspector(new ReadablePropertiesTypeInspector(new StaticTypeResolver())), namingConvention));
			this.converters = new List<IYamlTypeConverter>();
			foreach (IYamlTypeConverter yamlTypeConverter in YamlTypeConverters.BuiltInConverters)
			{
				this.converters.Add(yamlTypeConverter);
			}
			this.NodeDeserializers = new List<INodeDeserializer>();
			this.NodeDeserializers.Add(new TypeConverterNodeDeserializer(this.converters));
			this.NodeDeserializers.Add(new NullNodeDeserializer());
			this.NodeDeserializers.Add(new ScalarNodeDeserializer());
			this.NodeDeserializers.Add(new ArrayNodeDeserializer());
			this.NodeDeserializers.Add(new GenericDictionaryNodeDeserializer(objectFactory));
			this.NodeDeserializers.Add(new NonGenericDictionaryNodeDeserializer(objectFactory));
			this.NodeDeserializers.Add(new GenericCollectionNodeDeserializer(objectFactory));
			this.NodeDeserializers.Add(new NonGenericListNodeDeserializer(objectFactory));
			this.NodeDeserializers.Add(new EnumerableNodeDeserializer());
			this.NodeDeserializers.Add(new ObjectNodeDeserializer(objectFactory, this.typeDescriptor, ignoreUnmatched));
			this.tagMappings = new Dictionary<string, Type>(Deserializer.predefinedTagMappings);
			this.TypeResolvers = new List<INodeTypeResolver>();
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
			this.converters.Add(typeConverter);
		}

		public T Deserialize<T>(TextReader input)
		{
			return (T)((object)this.Deserialize(input, typeof(T)));
		}

		public object Deserialize(TextReader input)
		{
			return this.Deserialize(input, typeof(object));
		}

		public object Deserialize(TextReader input, Type type)
		{
			return this.Deserialize(new EventReader(new Parser(input)), type);
		}

		public T Deserialize<T>(EventReader reader)
		{
			return (T)((object)this.Deserialize(reader, typeof(T)));
		}

		public object Deserialize(EventReader reader)
		{
			return this.Deserialize(reader, typeof(object));
		}

		public object Deserialize(EventReader reader, Type type)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			bool flag = reader.Allow<StreamStart>() != null;
			bool flag2 = reader.Allow<DocumentStart>() != null;
			object obj = null;
			if (!reader.Accept<DocumentEnd>() && !reader.Accept<StreamEnd>())
			{
				using (SerializerState serializerState = new SerializerState())
				{
					obj = this.valueDeserializer.DeserializeValue(reader, type, serializerState, this.valueDeserializer);
					serializerState.OnDeserialization();
				}
			}
			if (flag2)
			{
				reader.Expect<DocumentEnd>();
			}
			if (flag)
			{
				reader.Expect<StreamEnd>();
			}
			return obj;
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

		private Deserializer.TypeDescriptorProxy typeDescriptor = new Deserializer.TypeDescriptorProxy();

		private IValueDeserializer valueDeserializer;

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
