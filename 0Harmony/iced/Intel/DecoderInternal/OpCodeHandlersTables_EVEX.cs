using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class OpCodeHandlersTables_EVEX
	{
		static OpCodeHandlersTables_EVEX()
		{
			EvexOpCodeHandlerReader evexOpCodeHandlerReader = new EvexOpCodeHandlerReader();
			TableDeserializer tableDeserializer = new TableDeserializer(evexOpCodeHandlerReader, 10, OpCodeHandlersTables_EVEX.GetSerializedTables());
			tableDeserializer.Deserialize();
			OpCodeHandlersTables_EVEX.Handlers_0F = tableDeserializer.GetTable(9U);
			OpCodeHandlersTables_EVEX.Handlers_0F38 = tableDeserializer.GetTable(5U);
			OpCodeHandlersTables_EVEX.Handlers_0F3A = tableDeserializer.GetTable(6U);
			OpCodeHandlersTables_EVEX.Handlers_MAP5 = tableDeserializer.GetTable(7U);
			OpCodeHandlersTables_EVEX.Handlers_MAP6 = tableDeserializer.GetTable(8U);
		}

		private unsafe static global::System.ReadOnlySpan<byte> GetSerializedTables()
		{
			return new global::System.ReadOnlySpan<byte>((void*)(&<b37590d4-39fb-478a-88de-d293f3364852><PrivateImplementationDetails>.7979334842423742EE34478883EFCF2BE7DC055D484045F7D6F6F6FDAF3D9C6B), 12430);
		}

		internal static readonly OpCodeHandler[] Handlers_0F;

		internal static readonly OpCodeHandler[] Handlers_0F38;

		internal static readonly OpCodeHandler[] Handlers_0F3A;

		internal static readonly OpCodeHandler[] Handlers_MAP5;

		internal static readonly OpCodeHandler[] Handlers_MAP6;

		private const int MaxIdNames = 10;

		private const uint Handlers_0FIndex = 9U;

		private const uint Handlers_0F38Index = 5U;

		private const uint Handlers_0F3AIndex = 6U;

		private const uint Handlers_MAP5Index = 7U;

		private const uint Handlers_MAP6Index = 8U;
	}
}
