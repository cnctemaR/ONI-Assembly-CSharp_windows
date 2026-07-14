using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VYaml.Emitter;
using VYaml.Internal;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(2)]
	[Nullable(0)]
	public static class YamlSerializer
	{
		[NullableContext(1)]
		private static YamlDeserializationContext GetThreadLocalDeserializationContext([Nullable(2)] YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlDeserializationContext yamlDeserializationContext;
			if ((yamlDeserializationContext = YamlSerializer.deserializationContext) == null)
			{
				yamlDeserializationContext = (YamlSerializer.deserializationContext = new YamlDeserializationContext(options));
			}
			YamlDeserializationContext yamlDeserializationContext2 = yamlDeserializationContext;
			yamlDeserializationContext2.Options = options;
			yamlDeserializationContext2.Resolver = options.Resolver;
			return yamlDeserializationContext2;
		}

		[NullableContext(1)]
		private static YamlSerializationContext GetThreadLocalSerializationContext([Nullable(2)] YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlSerializationContext yamlSerializationContext;
			if ((yamlSerializationContext = YamlSerializer.serializationContext) == null)
			{
				yamlSerializationContext = (YamlSerializer.serializationContext = new YamlSerializationContext(options));
			}
			YamlSerializationContext yamlSerializationContext2 = yamlSerializationContext;
			yamlSerializationContext2.Options = options;
			yamlSerializationContext2.Resolver = options.Resolver;
			yamlSerializationContext2.EmitOptions = options.EmitOptions;
			return yamlSerializationContext2;
		}

		[Nullable(1)]
		public static YamlSerializerOptions DefaultOptions
		{
			[NullableContext(1)]
			get
			{
				YamlSerializerOptions yamlSerializerOptions;
				if ((yamlSerializerOptions = YamlSerializer.defaultOptions) == null)
				{
					yamlSerializerOptions = (YamlSerializer.defaultOptions = YamlSerializerOptions.Standard);
				}
				return yamlSerializerOptions;
			}
			[NullableContext(1)]
			set
			{
				YamlSerializer.defaultOptions = value;
			}
		}

		[return: Nullable(0)]
		public static ReadOnlyMemory<byte> Serialize<T>([Nullable(1)] T value, YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlSerializationContext threadLocalSerializationContext = YamlSerializer.GetThreadLocalSerializationContext(options);
			ArrayBufferWriter<byte> arrayBufferWriterInternal = threadLocalSerializationContext.GetArrayBufferWriterInternal();
			Utf8YamlEmitter utf8YamlEmitter = new Utf8YamlEmitter(arrayBufferWriterInternal, null);
			threadLocalSerializationContext.Reset();
			threadLocalSerializationContext.Resolver.GetFormatterWithVerify<T>().Serialize(ref utf8YamlEmitter, value, threadLocalSerializationContext);
			return arrayBufferWriterInternal.WrittenMemory;
		}

		[NullableContext(1)]
		public static void Serialize<[Nullable(2)] T>(IBufferWriter<byte> writer, T value, [Nullable(2)] YamlSerializerOptions options = null)
		{
			Utf8YamlEmitter utf8YamlEmitter = new Utf8YamlEmitter(writer, null);
			YamlSerializer.Serialize<T>(ref utf8YamlEmitter, value, options);
		}

		public static void Serialize<T>(ref Utf8YamlEmitter emitter, [Nullable(1)] T value, YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlSerializationContext threadLocalSerializationContext = YamlSerializer.GetThreadLocalSerializationContext(options);
			threadLocalSerializationContext.Reset();
			threadLocalSerializationContext.Resolver.GetFormatterWithVerify<T>().Serialize(ref emitter, value, threadLocalSerializationContext);
		}

		[NullableContext(1)]
		public static string SerializeToString<[Nullable(2)] T>(T value, [Nullable(2)] YamlSerializerOptions options = null)
		{
			ReadOnlyMemory<byte> readOnlyMemory = YamlSerializer.Serialize<T>(value, options);
			return StringEncoding.Utf8.GetString(readOnlyMemory.Span);
		}

		[return: Nullable(1)]
		public static T Deserialize<T>([Nullable(0)] ReadOnlyMemory<byte> memory, YamlSerializerOptions options = null)
		{
			ReadOnlySequence<byte> readOnlySequence = new ReadOnlySequence<byte>(memory);
			YamlParser yamlParser = YamlParser.FromSequence(in readOnlySequence);
			return YamlSerializer.Deserialize<T>(ref yamlParser, options);
		}

		[return: Nullable(1)]
		public static T Deserialize<T>([Nullable(0)] in ReadOnlySequence<byte> sequence, YamlSerializerOptions options = null)
		{
			YamlParser yamlParser = YamlParser.FromSequence(in sequence);
			return YamlSerializer.Deserialize<T>(ref yamlParser, options);
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public static async ValueTask<T> DeserializeAsync<T>([Nullable(1)] Stream stream, YamlSerializerOptions options = null)
		{
			ReusableByteSequenceBuilder reusableByteSequenceBuilder = await StreamHelper.ReadAsSequenceAsync(stream, default(CancellationToken));
			T t;
			try
			{
				ReadOnlySequence<byte> readOnlySequence = reusableByteSequenceBuilder.Build();
				t = YamlSerializer.Deserialize<T>(in readOnlySequence, options);
			}
			finally
			{
				ReusableByteSequenceBuilderPool.Return(reusableByteSequenceBuilder);
			}
			return t;
		}

		[return: Nullable(1)]
		public static T Deserialize<T>(ref YamlParser parser, YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlDeserializationContext threadLocalDeserializationContext = YamlSerializer.GetThreadLocalDeserializationContext(options);
			threadLocalDeserializationContext.Reset();
			parser.SkipHeader();
			if (parser.End)
			{
				return default(T);
			}
			IYamlFormatter<T> formatterWithVerify = options.Resolver.GetFormatterWithVerify<T>();
			return threadLocalDeserializationContext.DeserializeWithAlias<T>(formatterWithVerify, ref parser);
		}

		[return: Nullable(new byte[] { 0, 1, 1 })]
		public static async ValueTask<IEnumerable<T>> DeserializeMultipleDocumentsAsync<T>([Nullable(1)] Stream stream, YamlSerializerOptions options = null)
		{
			ReusableByteSequenceBuilder reusableByteSequenceBuilder = await StreamHelper.ReadAsSequenceAsync(stream, default(CancellationToken));
			IEnumerable<T> enumerable;
			try
			{
				ReadOnlySequence<byte> readOnlySequence = reusableByteSequenceBuilder.Build();
				enumerable = YamlSerializer.DeserializeMultipleDocuments<T>(in readOnlySequence, options);
			}
			finally
			{
				ReusableByteSequenceBuilderPool.Return(reusableByteSequenceBuilder);
			}
			return enumerable;
		}

		[return: Nullable(1)]
		public static IEnumerable<T> DeserializeMultipleDocuments<T>([Nullable(0)] ReadOnlyMemory<byte> memory, YamlSerializerOptions options = null)
		{
			ReadOnlySequence<byte> readOnlySequence = new ReadOnlySequence<byte>(memory);
			YamlParser yamlParser = YamlParser.FromSequence(in readOnlySequence);
			return YamlSerializer.DeserializeMultipleDocuments<T>(ref yamlParser, options);
		}

		[return: Nullable(1)]
		public static IEnumerable<T> DeserializeMultipleDocuments<T>([Nullable(0)] in ReadOnlySequence<byte> sequence, YamlSerializerOptions options = null)
		{
			YamlParser yamlParser = YamlParser.FromSequence(in sequence);
			return YamlSerializer.DeserializeMultipleDocuments<T>(ref yamlParser, options);
		}

		[return: Nullable(1)]
		public static IEnumerable<T> DeserializeMultipleDocuments<T>(ref YamlParser parser, YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlDeserializationContext threadLocalDeserializationContext = YamlSerializer.GetThreadLocalDeserializationContext(options);
			IYamlFormatter<T> formatterWithVerify = options.Resolver.GetFormatterWithVerify<T>();
			List<T> list = new List<T>();
			for (;;)
			{
				parser.SkipAfter(ParseEventType.DocumentStart);
				if (parser.End)
				{
					break;
				}
				threadLocalDeserializationContext.Reset();
				T t = threadLocalDeserializationContext.DeserializeWithAlias<T>(formatterWithVerify, ref parser);
				list.Add(t);
			}
			return list;
		}

		[ThreadStatic]
		private static YamlDeserializationContext deserializationContext;

		[ThreadStatic]
		private static YamlSerializationContext serializationContext;

		private static YamlSerializerOptions defaultOptions;
	}
}
