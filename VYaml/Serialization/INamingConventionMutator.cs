using System;

namespace VYaml.Serialization
{
	public interface INamingConventionMutator
	{
		bool TryMutate(ReadOnlySpan<char> source, Span<char> destination, out int written);

		bool TryMutate(ReadOnlySpan<byte> sourceUtf8, Span<byte> destinationUtf8, out int written);
	}
}
