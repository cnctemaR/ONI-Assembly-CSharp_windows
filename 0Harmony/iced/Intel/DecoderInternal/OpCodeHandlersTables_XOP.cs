using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class OpCodeHandlersTables_XOP
	{
		static OpCodeHandlersTables_XOP()
		{
			VexOpCodeHandlerReader vexOpCodeHandlerReader = new VexOpCodeHandlerReader();
			TableDeserializer tableDeserializer = new TableDeserializer(vexOpCodeHandlerReader, 7, OpCodeHandlersTables_XOP.GetSerializedTables());
			tableDeserializer.Deserialize();
			OpCodeHandlersTables_XOP.Handlers_MAP8 = tableDeserializer.GetTable(4U);
			OpCodeHandlersTables_XOP.Handlers_MAP9 = tableDeserializer.GetTable(5U);
			OpCodeHandlersTables_XOP.Handlers_MAP10 = tableDeserializer.GetTable(6U);
		}

		private unsafe static global::System.ReadOnlySpan<byte> GetSerializedTables()
		{
			return new global::System.ReadOnlySpan<byte>((void*)(&<b37590d4-39fb-478a-88de-d293f3364852><PrivateImplementationDetails>.2DC269275D4CFA9BCF8160F362F35004DDCE0867005D0BB1888BBD51488F59DE), 768);
		}

		internal static readonly OpCodeHandler[] Handlers_MAP8;

		internal static readonly OpCodeHandler[] Handlers_MAP9;

		internal static readonly OpCodeHandler[] Handlers_MAP10;

		private const int MaxIdNames = 7;

		private const uint Handlers_MAP8Index = 4U;

		private const uint Handlers_MAP9Index = 5U;

		private const uint Handlers_MAP10Index = 6U;
	}
}
