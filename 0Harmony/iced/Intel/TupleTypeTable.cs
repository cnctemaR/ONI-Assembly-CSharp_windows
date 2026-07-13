using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	internal static class TupleTypeTable
	{
		private unsafe static global::System.ReadOnlySpan<byte> tupleTypeData
		{
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<b37590d4-39fb-478a-88de-d293f3364852><PrivateImplementationDetails>.D79DD1C8B320F03F943FB02BA6A6D20562AB453315E27BF5C551D37DD74F1F13), 38);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint GetDisp8N(TupleType tupleType, bool bcst)
		{
			int num = (int)(((int)tupleType << 1) | ((bcst > false) ? TupleType.N2 : TupleType.N1));
			return (uint)(*TupleTypeTable.tupleTypeData[num]);
		}
	}
}
