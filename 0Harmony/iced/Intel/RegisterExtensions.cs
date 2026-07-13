using System;

namespace Iced.Intel
{
	internal static class RegisterExtensions
	{
		public static bool IsSegmentRegister(this Register register)
		{
			return Register.ES <= register && register <= Register.GS;
		}

		public static bool IsGPR(this Register register)
		{
			return Register.AL <= register && register <= Register.R15;
		}

		public static bool IsGPR8(this Register register)
		{
			return Register.AL <= register && register <= Register.R15L;
		}

		public static bool IsGPR16(this Register register)
		{
			return Register.AX <= register && register <= Register.R15W;
		}

		public static bool IsGPR32(this Register register)
		{
			return Register.EAX <= register && register <= Register.R15D;
		}

		public static bool IsGPR64(this Register register)
		{
			return Register.RAX <= register && register <= Register.R15;
		}

		public static bool IsXMM(this Register register)
		{
			return Register.XMM0 <= register && register <= Register.XMM31;
		}

		public static bool IsYMM(this Register register)
		{
			return Register.YMM0 <= register && register <= Register.YMM31;
		}

		public static bool IsZMM(this Register register)
		{
			return Register.ZMM0 <= register && register <= Register.ZMM31;
		}

		public static bool IsIP(this Register register)
		{
			return register == Register.EIP || register == Register.RIP;
		}

		public static bool IsK(this Register register)
		{
			return Register.K0 <= register && register <= Register.K7;
		}

		public static bool IsCR(this Register register)
		{
			return Register.CR0 <= register && register <= Register.CR15;
		}

		public static bool IsDR(this Register register)
		{
			return Register.DR0 <= register && register <= Register.DR15;
		}

		public static bool IsTR(this Register register)
		{
			return Register.TR0 <= register && register <= Register.TR7;
		}

		public static bool IsST(this Register register)
		{
			return Register.ST0 <= register && register <= Register.ST7;
		}

		public static bool IsBND(this Register register)
		{
			return Register.BND0 <= register && register <= Register.BND3;
		}

		public static bool IsMM(this Register register)
		{
			return Register.MM0 <= register && register <= Register.MM7;
		}

		public static bool IsTMM(this Register register)
		{
			return Register.TMM0 <= register && register <= Register.TMM7;
		}

		public static bool IsVectorRegister(this Register register)
		{
			return Register.XMM0 <= register && register <= Register.ZMM31;
		}
	}
}
