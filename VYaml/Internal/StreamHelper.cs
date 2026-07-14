using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace VYaml.Internal
{
	internal static class StreamHelper
	{
		[NullableContext(1)]
		[return: Nullable(new byte[] { 0, 1 })]
		public static async ValueTask<ReusableByteSequenceBuilder> ReadAsSequenceAsync(Stream stream, CancellationToken cancellation = default(CancellationToken))
		{
			ReusableByteSequenceBuilder builder = ReusableByteSequenceBuilderPool.Rent();
			try
			{
				MemoryStream memoryStream = stream as MemoryStream;
				ArraySegment<byte> arraySegment;
				if (memoryStream != null && memoryStream.TryGetBuffer(out arraySegment))
				{
					cancellation.ThrowIfCancellationRequested();
					memoryStream.Seek((long)arraySegment.Count, SeekOrigin.Current);
					builder.Add(arraySegment.AsMemory<byte>(), false);
					return builder;
				}
				byte[] buffer = ArrayPool<byte>.Shared.Rent(65536);
				int offset = 0;
				int num;
				do
				{
					if (offset == buffer.Length)
					{
						builder.Add(buffer, true);
						buffer = ArrayPool<byte>.Shared.Rent(StreamHelper.NewArrayCapacity(buffer.Length));
						offset = 0;
					}
					try
					{
						num = await stream.ReadAsync(buffer.AsMemory<byte>(offset, buffer.Length - offset), cancellation).ConfigureAwait(false);
					}
					catch
					{
						ArrayPool<byte>.Shared.Return(buffer, false);
						throw;
					}
					offset += num;
				}
				while (num != 0);
				builder.Add(buffer.AsMemory<byte>(0, offset), true);
				buffer = null;
			}
			catch (Exception)
			{
				ReusableByteSequenceBuilderPool.Return(builder);
				throw;
			}
			return builder;
		}

		private static int NewArrayCapacity(int size)
		{
			int num = size * 2;
			if (num > 2147483591)
			{
				num = 2147483591;
			}
			return num;
		}

		private const int ArrayMexLength = 2147483591;
	}
}
