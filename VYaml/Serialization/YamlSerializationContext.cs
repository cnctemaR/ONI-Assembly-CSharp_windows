using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using VYaml.Emitter;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class YamlSerializationContext : IDisposable
	{
		public YamlSerializerOptions Options { get; set; }

		public IYamlFormatterResolver Resolver { get; set; }

		public YamlEmitOptions EmitOptions { get; set; }

		public YamlSerializationContext(YamlSerializerOptions options)
		{
			this.primitiveValueBuffer = ArrayPool<byte>.Shared.Rent(64);
			this.Options = options;
			this.Resolver = options.Resolver;
			this.EmitOptions = options.EmitOptions;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Serialize<[Nullable(2)] T>(ref Utf8YamlEmitter emitter, T value)
		{
			this.Resolver.GetFormatterWithVerify<T>().Serialize(ref emitter, value, this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ArrayBufferWriter<byte> GetArrayBufferWriterInternal()
		{
			ArrayBufferWriter<byte> arrayBufferWriter;
			if ((arrayBufferWriter = this.arrayBufferWriter) == null)
			{
				arrayBufferWriter = (this.arrayBufferWriter = new ArrayBufferWriter<byte>(65536));
			}
			return arrayBufferWriter;
		}

		public ArrayBufferWriter<byte> GetArrayBufferWriter()
		{
			return this.GetArrayBufferWriterInternal();
		}

		public void Reset()
		{
			ArrayBufferWriter<byte> arrayBufferWriter = this.arrayBufferWriter;
			if (arrayBufferWriter == null)
			{
				return;
			}
			arrayBufferWriter.Clear();
		}

		public void Dispose()
		{
			ArrayPool<byte>.Shared.Return(this.primitiveValueBuffer, false);
		}

		public byte[] GetBuffer64()
		{
			return this.primitiveValueBuffer;
		}

		private readonly byte[] primitiveValueBuffer;

		[Nullable(2)]
		private ArrayBufferWriter<byte> arrayBufferWriter;
	}
}
