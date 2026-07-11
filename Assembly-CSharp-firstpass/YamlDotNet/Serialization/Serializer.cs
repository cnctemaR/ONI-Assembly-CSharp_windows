using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization
{
	public sealed class Serializer
	{
		public Serializer()
			: this(new SerializerBuilder().BuildValueSerializer())
		{
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
			this.valueSerializer.SerializeValue(emitter, graph, type);
			emitter.Emit(new DocumentEnd(true));
			emitter.Emit(new StreamEnd());
		}

		private readonly IValueSerializer valueSerializer;
	}
}
