using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	internal static class IcedFeatures
	{
		public static bool HasGasFormatter
		{
			get
			{
				return false;
			}
		}

		public static bool HasIntelFormatter
		{
			get
			{
				return false;
			}
		}

		public static bool HasMasmFormatter
		{
			get
			{
				return false;
			}
		}

		public static bool HasNasmFormatter
		{
			get
			{
				return false;
			}
		}

		public static bool HasFastFormatter
		{
			get
			{
				return false;
			}
		}

		public static bool HasDecoder
		{
			get
			{
				return true;
			}
		}

		public static bool HasEncoder
		{
			get
			{
				return true;
			}
		}

		public static bool HasBlockEncoder
		{
			get
			{
				return true;
			}
		}

		public static bool HasOpCodeInfo
		{
			get
			{
				return false;
			}
		}

		public static bool HasInstructionInfo
		{
			get
			{
				return false;
			}
		}

		public static void Initialize()
		{
			RuntimeHelpers.RunClassConstructor(typeof(Decoder).TypeHandle);
		}
	}
}
