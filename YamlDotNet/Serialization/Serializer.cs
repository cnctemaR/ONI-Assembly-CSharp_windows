using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphTraversalStrategies;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization
{
	public sealed class Serializer
	{
		internal IList<IYamlTypeConverter> Converters { get; private set; }

		public Serializer(SerializationOptions options = SerializationOptions.None, INamingConvention namingConvention = null)
		{
			this.options = options;
			this.namingConvention = namingConvention ?? new NullNamingConvention();
			this.Converters = new List<IYamlTypeConverter>();
			foreach (IYamlTypeConverter yamlTypeConverter in YamlTypeConverters.BuiltInConverters)
			{
				this.Converters.Add(yamlTypeConverter);
			}
			ITypeResolver typeResolver2;
			if (!this.IsOptionSet(SerializationOptions.DefaultToStaticType))
			{
				ITypeResolver typeResolver = new DynamicTypeResolver();
				typeResolver2 = typeResolver;
			}
			else
			{
				typeResolver2 = new StaticTypeResolver();
			}
			this.typeResolver = typeResolver2;
		}

		private bool IsOptionSet(SerializationOptions option)
		{
			return (this.options & option) != SerializationOptions.None;
		}

		public void RegisterTypeConverter(IYamlTypeConverter converter)
		{
			this.Converters.Add(converter);
		}

		public void Serialize(TextWriter writer, object graph)
		{
			this.Serialize(new Emitter(writer), graph);
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
			this.EmitDocument(emitter, new ObjectDescriptor(graph, (graph != null) ? graph.GetType() : typeof(object), typeof(object)));
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
			this.EmitDocument(emitter, new ObjectDescriptor(graph, type, type));
		}

		private void EmitDocument(IEmitter emitter, IObjectDescriptor graph)
		{
			IObjectGraphTraversalStrategy objectGraphTraversalStrategy = this.CreateTraversalStrategy();
			IEventEmitter eventEmitter = this.CreateEventEmitter(emitter);
			IObjectGraphVisitor objectGraphVisitor = this.CreateEmittingVisitor(emitter, objectGraphTraversalStrategy, eventEmitter, graph);
			emitter.Emit(new StreamStart());
			emitter.Emit(new DocumentStart());
			objectGraphTraversalStrategy.Traverse(graph, objectGraphVisitor);
			emitter.Emit(new DocumentEnd(true));
			emitter.Emit(new StreamEnd());
		}

		private IObjectGraphVisitor CreateEmittingVisitor(IEmitter emitter, IObjectGraphTraversalStrategy traversalStrategy, IEventEmitter eventEmitter, IObjectDescriptor graph)
		{
			IObjectGraphVisitor objectGraphVisitor = new EmittingObjectGraphVisitor(eventEmitter);
			objectGraphVisitor = new CustomSerializationObjectGraphVisitor(emitter, objectGraphVisitor, this.Converters);
			if (!this.IsOptionSet(SerializationOptions.DisableAliases))
			{
				AnchorAssigner anchorAssigner = new AnchorAssigner();
				traversalStrategy.Traverse(graph, anchorAssigner);
				objectGraphVisitor = new AnchorAssigningObjectGraphVisitor(objectGraphVisitor, eventEmitter, anchorAssigner);
			}
			if (!this.IsOptionSet(SerializationOptions.EmitDefaults))
			{
				objectGraphVisitor = new DefaultExclusiveObjectGraphVisitor(objectGraphVisitor);
			}
			return objectGraphVisitor;
		}

		private IEventEmitter CreateEventEmitter(IEmitter emitter)
		{
			WriterEventEmitter writerEventEmitter = new WriterEventEmitter(emitter);
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
			typeInspector = new NamingConventionTypeInspector(typeInspector, this.namingConvention);
			typeInspector = new YamlAttributesTypeInspector(typeInspector);
			if (this.IsOptionSet(SerializationOptions.Roundtrip))
			{
				return new RoundtripObjectGraphTraversalStrategy(this, typeInspector, this.typeResolver, 50);
			}
			return new FullObjectGraphTraversalStrategy(this, typeInspector, this.typeResolver, 50, this.namingConvention);
		}

		private readonly SerializationOptions options;

		private readonly INamingConvention namingConvention;

		private readonly ITypeResolver typeResolver;
	}
}
