using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphTraversalStrategies;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;

namespace YamlDotNet.Serialization
{
	public sealed class Serializer
	{
		private void ThrowUnlessInBackwardsCompatibleMode()
		{
			if (this.backwardsCompatibleConfiguration == null)
			{
				throw new InvalidOperationException("This method / property exists for backwards compatibility reasons, but the Serializer was created using the new configuration mechanism. To configure the Serializer, use the SerializerBuilder.");
			}
		}

		[Obsolete("Please use SerializerBuilder to customize the Serializer. This constructor will be removed in future releases.")]
		public Serializer(SerializationOptions options = SerializationOptions.None, INamingConvention namingConvention = null, YamlAttributeOverrides overrides = null)
		{
			this.backwardsCompatibleConfiguration = new Serializer.BackwardsCompatibleConfiguration(options, namingConvention, overrides);
		}

		[Obsolete("Please use SerializerBuilder to customize the Serializer. This method will be removed in future releases.")]
		public void RegisterTypeConverter(IYamlTypeConverter converter)
		{
			this.ThrowUnlessInBackwardsCompatibleMode();
			this.backwardsCompatibleConfiguration.Converters.Insert(0, converter);
		}

		public Serializer()
		{
			this.backwardsCompatibleConfiguration = new Serializer.BackwardsCompatibleConfiguration(SerializationOptions.None, null, null);
		}

		private Serializer(IValueSerializer valueSerializer)
		{
			if (valueSerializer == null)
			{
				throw new ArgumentNullException("valueSerializer");
			}
			this.valueSerializer = valueSerializer;
		}

		public static Serializer FromValueSerializer(IValueSerializer valueSerializer)
		{
			return new Serializer(valueSerializer);
		}

		public void Serialize(TextWriter writer, object graph)
		{
			this.Serialize(new Emitter(writer), graph);
		}

		public string Serialize(object graph)
		{
			string text;
			using (StringWriter stringWriter = new StringWriter())
			{
				this.Serialize(stringWriter, graph);
				text = stringWriter.ToString();
			}
			return text;
		}

		public void Serialize(TextWriter writer, object graph, Type type)
		{
			this.Serialize(new Emitter(writer), graph, type);
		}

		public void Serialize(IEmitter emitter, object graph)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			this.EmitDocument(emitter, graph, null);
		}

		public void Serialize(IEmitter emitter, object graph, Type type)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.EmitDocument(emitter, graph, type);
		}

		private void EmitDocument(IEmitter emitter, object graph, Type type)
		{
			emitter.Emit(new StreamStart());
			emitter.Emit(new DocumentStart());
			IValueSerializer valueSerializer = this.backwardsCompatibleConfiguration;
			(valueSerializer ?? this.valueSerializer).SerializeValue(emitter, graph, type);
			emitter.Emit(new DocumentEnd(true));
			emitter.Emit(new StreamEnd());
		}

		private readonly IValueSerializer valueSerializer;

		private readonly Serializer.BackwardsCompatibleConfiguration backwardsCompatibleConfiguration;

		private class BackwardsCompatibleConfiguration : IValueSerializer
		{
			public IList<IYamlTypeConverter> Converters { get; private set; }

			public BackwardsCompatibleConfiguration(SerializationOptions options, INamingConvention namingConvention, YamlAttributeOverrides overrides)
			{
				this.options = options;
				this.namingConvention = namingConvention ?? new NullNamingConvention();
				this.overrides = overrides;
				this.Converters = new List<IYamlTypeConverter>();
				this.Converters.Add(new GuidConverter(this.IsOptionSet(SerializationOptions.JsonCompatible)));
				ITypeResolver typeResolver2;
				if (!this.IsOptionSet(SerializationOptions.DefaultToStaticType))
				{
					ITypeResolver typeResolver = new DynamicTypeResolver();
					typeResolver2 = typeResolver;
				}
				else
				{
					ITypeResolver typeResolver = new StaticTypeResolver();
					typeResolver2 = typeResolver;
				}
				this.typeResolver = typeResolver2;
			}

			public bool IsOptionSet(SerializationOptions option)
			{
				return (this.options & option) > SerializationOptions.None;
			}

			private IObjectGraphVisitor<IEmitter> CreateEmittingVisitor(IEmitter emitter, IObjectGraphTraversalStrategy traversalStrategy, IEventEmitter eventEmitter, IObjectDescriptor graph)
			{
				IObjectGraphVisitor<IEmitter> objectGraphVisitor = new EmittingObjectGraphVisitor(eventEmitter);
				ObjectSerializer objectSerializer = delegate(object v, Type t)
				{
					this.SerializeValue(emitter, v, t);
				};
				objectGraphVisitor = new CustomSerializationObjectGraphVisitor(objectGraphVisitor, this.Converters, objectSerializer);
				if (!this.IsOptionSet(SerializationOptions.DisableAliases))
				{
					AnchorAssigner anchorAssigner = new AnchorAssigner(this.Converters);
					traversalStrategy.Traverse<Nothing>(graph, anchorAssigner, null);
					objectGraphVisitor = new AnchorAssigningObjectGraphVisitor(objectGraphVisitor, eventEmitter, anchorAssigner);
				}
				if (!this.IsOptionSet(SerializationOptions.EmitDefaults))
				{
					objectGraphVisitor = new DefaultExclusiveObjectGraphVisitor(objectGraphVisitor);
				}
				return objectGraphVisitor;
			}

			private IEventEmitter CreateEventEmitter()
			{
				WriterEventEmitter writerEventEmitter = new WriterEventEmitter();
				if (this.IsOptionSet(SerializationOptions.JsonCompatible))
				{
					return new JsonEventEmitter(writerEventEmitter);
				}
				return new TypeAssigningEventEmitter(writerEventEmitter, this.IsOptionSet(SerializationOptions.Roundtrip));
			}

			private IObjectGraphTraversalStrategy CreateTraversalStrategy()
			{
				ITypeInspector typeInspector = new ReadablePropertiesTypeInspector(this.typeResolver);
				if (this.IsOptionSet(SerializationOptions.Roundtrip))
				{
					typeInspector = new ReadableAndWritablePropertiesTypeInspector(typeInspector);
				}
				typeInspector = new YamlAttributeOverridesInspector(typeInspector, this.overrides);
				typeInspector = new YamlAttributesTypeInspector(typeInspector);
				typeInspector = new NamingConventionTypeInspector(typeInspector, this.namingConvention);
				if (this.IsOptionSet(SerializationOptions.DefaultToStaticType))
				{
					typeInspector = new CachedTypeInspector(typeInspector);
				}
				if (this.IsOptionSet(SerializationOptions.Roundtrip))
				{
					return new RoundtripObjectGraphTraversalStrategy(this.Converters, typeInspector, this.typeResolver, 50);
				}
				return new FullObjectGraphTraversalStrategy(typeInspector, this.typeResolver, 50, this.namingConvention);
			}

			public void SerializeValue(IEmitter emitter, object value, Type type)
			{
				ObjectDescriptor objectDescriptor = ((type != null) ? new ObjectDescriptor(value, type, type) : new ObjectDescriptor(value, (value != null) ? value.GetType() : typeof(object), typeof(object)));
				IObjectGraphTraversalStrategy objectGraphTraversalStrategy = this.CreateTraversalStrategy();
				IObjectGraphVisitor<IEmitter> objectGraphVisitor = this.CreateEmittingVisitor(emitter, objectGraphTraversalStrategy, this.CreateEventEmitter(), objectDescriptor);
				objectGraphTraversalStrategy.Traverse<IEmitter>(objectDescriptor, objectGraphVisitor, emitter);
			}

			private readonly SerializationOptions options;

			private readonly INamingConvention namingConvention;

			private readonly ITypeResolver typeResolver;

			private readonly YamlAttributeOverrides overrides;
		}
	}
}
