using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class OpCodeHandlersTables_VEX
	{
		static OpCodeHandlersTables_VEX()
		{
			VexOpCodeHandlerReader vexOpCodeHandlerReader = new VexOpCodeHandlerReader();
			TableDeserializer tableDeserializer = new TableDeserializer(vexOpCodeHandlerReader, 16, OpCodeHandlersTables_VEX.GetSerializedTables());
			tableDeserializer.Deserialize();
			OpCodeHandlersTables_VEX.Handlers_0F = tableDeserializer.GetTable(15U);
			OpCodeHandlersTables_VEX.Handlers_0F38 = tableDeserializer.GetTable(12U);
			OpCodeHandlersTables_VEX.Handlers_0F3A = tableDeserializer.GetTable(13U);
		}

		private unsafe static global::System.ReadOnlySpan<byte> GetSerializedTables()
		{
			return new global::System.ReadOnlySpan<byte>((void*)(&<b37590d4-39fb-478a-88de-d293f3364852><PrivateImplementationDetails>.B081B50E102A74FCF59603CD7779F68C5513793D1E1BF7D19655D078BBDC30A6), 6767);
		}

		internal static readonly OpCodeHandler[] Handlers_0F;

		internal static readonly OpCodeHandler[] Handlers_0F38;

		internal static readonly OpCodeHandler[] Handlers_0F3A;

		private const int MaxIdNames = 16;

		private const uint Handlers_MAP0Index = 14U;

		private const uint Handlers_0FIndex = 15U;

		private const uint Handlers_0F38Index = 12U;

		private const uint Handlers_0F3AIndex = 13U;
	}
}
