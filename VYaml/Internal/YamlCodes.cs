using System;
using System.Runtime.CompilerServices;

namespace VYaml.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	public static class YamlCodes
	{
		unsafe static YamlCodes()
		{
			YamlCodes.EmptyTable[32] = true;
			YamlCodes.EmptyTable[9] = true;
			YamlCodes.EmptyTable[10] = true;
			YamlCodes.EmptyTable[13] = true;
			YamlCodes.BlankTable[32] = true;
			YamlCodes.BlankTable[9] = true;
			YamlCodes.FlowSymbolTable[44] = true;
			YamlCodes.FlowSymbolTable[91] = true;
			YamlCodes.FlowSymbolTable[93] = true;
			YamlCodes.FlowSymbolTable[123] = true;
			YamlCodes.FlowSymbolTable[125] = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlphaNumericDashOrUnderscore(byte code)
		{
			bool flag = YamlCodes.IsNumber(code) || YamlCodes.IsAlphabet(code);
			if (!flag)
			{
				bool flag2 = code == 45 || code == 95;
				flag = flag2;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWordChar(byte code)
		{
			return YamlCodes.IsNumber(code) || YamlCodes.IsAlphabet(code) || code == 45;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsUriChar(byte code)
		{
			if (code >= 65)
			{
				if (code >= 97)
				{
					if (code > 122 && code != 126)
					{
						goto IL_00C2;
					}
				}
				else if (code > 90)
				{
					switch (code)
					{
					case 91:
					case 93:
					case 95:
						break;
					case 92:
					case 94:
						goto IL_00C2;
					default:
						goto IL_00C2;
					}
				}
			}
			else if (code >= 48)
			{
				if (code > 57)
				{
					switch (code)
					{
					case 58:
					case 59:
					case 61:
					case 63:
					case 64:
						break;
					case 60:
					case 62:
						goto IL_00C2;
					default:
						goto IL_00C2;
					}
				}
			}
			else
			{
				switch (code)
				{
				case 33:
				case 35:
				case 36:
				case 38:
				case 39:
				case 40:
				case 41:
				case 42:
				case 43:
				case 44:
				case 45:
				case 46:
				case 47:
					break;
				case 34:
				case 37:
					goto IL_00C2;
				default:
					goto IL_00C2;
				}
			}
			return true;
			IL_00C2:
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsTagChar(byte code)
		{
			if (code >= 65)
			{
				if (code >= 97)
				{
					if (code > 122 && code != 126)
					{
						goto IL_009F;
					}
				}
				else if (code > 90 && code != 95)
				{
					goto IL_009F;
				}
			}
			else if (code >= 48)
			{
				if (code > 57)
				{
					switch (code)
					{
					case 58:
					case 59:
					case 61:
					case 63:
					case 64:
						break;
					case 60:
					case 62:
						goto IL_009F;
					default:
						goto IL_009F;
					}
				}
			}
			else
			{
				switch (code)
				{
				case 35:
				case 36:
				case 38:
				case 39:
				case 42:
				case 43:
				case 45:
				case 46:
				case 47:
					break;
				case 37:
				case 40:
				case 41:
				case 44:
					goto IL_009F;
				default:
					goto IL_009F;
				}
			}
			return true;
			IL_009F:
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAscii(byte code)
		{
			return code <= 127;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumber(byte c)
		{
			return (c | 32) - 48 < 10;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmpty(byte code)
		{
			return YamlCodes.EmptyTable[(int)code];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBlank(byte code)
		{
			return YamlCodes.BlankTable[(int)code];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLineBreak(byte code)
		{
			return code == 10 || code == 13;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlphabet(byte c)
		{
			return (c | 32) - 97 < 26;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsHexAlphabet(byte c)
		{
			return (c | 32) - 97 < 6;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsHex(byte code)
		{
			return YamlCodes.IsNumber(code) || YamlCodes.IsHexAlphabet(code);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAnyFlowSymbol(byte code)
		{
			return YamlCodes.FlowSymbolTable[(int)code];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte AsHex(byte code)
		{
			int num = (int)(code - 48);
			if (num <= 9)
			{
				return (byte)num;
			}
			num = (int)((code | 32) - 97);
			if (num <= 5)
			{
				return (byte)(num + 10);
			}
			throw new InvalidOperationException();
		}

		public static readonly byte[] YamlDirectiveName = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.192A6594F7D6F237B39D25500D9CC5C43B831FB7BFBD9A05959B3904F191CF7D), 4).ToArray();

		public static readonly byte[] TagDirectiveName = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.E6FF0252F1F3DBFDBF7A0094AB626875736B62A74427CB436D4F745C690619FB), 3).ToArray();

		public static readonly byte[] Utf8Bom = new byte[] { 239, 187, 191 };

		public static readonly byte[] StreamStart = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.34C7DACA4944C07680F6D0C19C5D6BA053AA33CA4391D4437E7FBCFF7C49A4BE), 3).ToArray();

		public static readonly byte[] DocStart = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.2C2B93AB063D64D75B80C9901170CE7AE0D79A64BC7F077CE1A010AB0956A391), 3).ToArray();

		public static readonly byte[] CrLf = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.426520180EE94EF36224225E32706BC9F2BE242ACF51BB23D69FEA7E6D92A20A), 2).ToArray();

		public static readonly byte[] Null0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.97CCCB1B1197F11C6EDBB0D93975220592EF8FAF618C8770A131E4F7DFE567CC), 4).ToArray();

		public static readonly byte[] Null1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.E2A44980115D37CA32F2FE010F0A8822F4DFE1324C318BFD6738BCECA3CA5303), 4).ToArray();

		public static readonly byte[] Null2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.37DF53FD8A4AA5F7A8189EE54DB2EC6A9CB3AF0058A85135A031D9970C4C857A), 4).ToArray();

		public const byte NullAlias = 126;

		public static readonly byte[] True0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.DEBC2F07DB78D52D2DEF07B7BC620D7042367501D9439A62BA09B559A98E0957), 4).ToArray();

		public static readonly byte[] True1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.1F419139F10195A90BB89635E7CCFD5D1614E64DA16F8F068BF684FEC5073111), 4).ToArray();

		public static readonly byte[] True2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.5DE7A893E93DF4BCD4B5640BF2B35AC9518615DED6E37BEAAE19C338CD781D67), 4).ToArray();

		public static readonly byte[] False0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.98151954F217A510702D236DE168CC35D0AB2F99C4479CC9B07EEEDE7EF73A66), 5).ToArray();

		public static readonly byte[] False1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.819738269F2DACAAE5A199D3E902828E334B698E7AC1C904F4E5E39931943931), 5).ToArray();

		public static readonly byte[] False2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.073E1639B291E9B236CF6C65D243386C48AF8D073EC2BBA92E691D9F6CC6BCA9), 5).ToArray();

		public static readonly byte[] Inf0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.B0E3BCFB004E75722B099E02DA1194F9206F3557664CF8564AA2366D7C8887C7), 4).ToArray();

		public static readonly byte[] Inf1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.80DD5A1BDB212C727AFCD48AFCB758A055E2F6C69BF3123ACFE8D6125D1EE785), 4).ToArray();

		public static readonly byte[] Inf2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.C4731A696EBE2CA31BCB86C040BC65002F5B5E712C9DDF616EB7EF396EC302BC), 4).ToArray();

		public static readonly byte[] Inf3 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.50D7FC22E41D65D370B93EB30D3E0389CFA6A6E352FD8CE20DBC50D3675847D5), 5).ToArray();

		public static readonly byte[] Inf4 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.CC5DB039B93143EF3B9BA348C54AD32D0C2E14BA237C08FBBC62E25EEA255489), 5).ToArray();

		public static readonly byte[] Inf5 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.160551D8E04FF4D11FB3C9A8CD63AF9BF3DB4C22B7917A5F8CC28279B404DE1C), 5).ToArray();

		public static readonly byte[] Yes0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.357C2320B43A6C0482A5DA2F108E4DE667FBF36FCB00D4A8A8C1E7B47F914DD7), 3).ToArray();

		public static readonly byte[] Yes1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.03CB5FB3DD8F56AA60BA08FE532D55484FFACA42439A4CE03FD1DB3BFE54A8AA), 3).ToArray();

		public static readonly byte[] Yes2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.6FEE33318CB19FB9796CF1D8E24C8B381B566DE1330C1CFB714D722D2B23A423), 3).ToArray();

		public static readonly byte[] No0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.DB907DB790B1CDBB7C068BA83647F6599A0A182D649BB878905964A650E4F5CD), 2).ToArray();

		public static readonly byte[] No1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.511A5D61E579E32D7A0329F681E54C26440084A69A7CE1C3C3F837A96E06AA29), 2).ToArray();

		public static readonly byte[] No2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.40338EC8978115E259F8A7ACC9E4B14D6A62772B653ED431C7B480BECC0E3924), 2).ToArray();

		public static readonly byte[] On0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.7396B2DFCC8E6F5C6B317AF0CE5E1279B04F424D002ADABC4C0AEB25A7B416F7), 2).ToArray();

		public static readonly byte[] On1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.1DB5C4B0C73BA8F545BC5D8DA2E13286B0E7072B9F318D642F3319A312D7781F), 2).ToArray();

		public static readonly byte[] On2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.311231C9E51D649DB52EB8F6ADADB3B7FCCF55262985DF7AE1657DACDC42A331), 2).ToArray();

		public static readonly byte[] Off0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.941700040B1123DCC02E4FE94D60E57C764BB5CDB36459216CEC158F35906493), 3).ToArray();

		public static readonly byte[] Off1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.5159727DC0C967B872D7E6169552AC6F4B0A0812D296B129695EE4A27343568B), 3).ToArray();

		public static readonly byte[] Off2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.DDF50895B4B4E1F2D8874FD73A7146DEAC78CD64AE337981901A26B9D38C1CF6), 3).ToArray();

		public static readonly byte[] NegInf0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.7ECB5047FBAFE98545CCFF6CE8580ACCE31E38B9C34FBA4056B8741AF5D472DD), 5).ToArray();

		public static readonly byte[] NegInf1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.BDD15E8C6CF4CB8BF357A709CAE2AC20B85EFB5F65E842DE1DAC9F0279D3EF16), 5).ToArray();

		public static readonly byte[] NegInf2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.80FD8B78F3212D83047D9FF49C6965DB8C67235333E2768C82D7B5EB01A59361), 5).ToArray();

		public static readonly byte[] Nan0 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.9E1177F5CDEC36E092332F98906C41EA7084F70584627A2D07D3A5EC43ED3263), 4).ToArray();

		public static readonly byte[] Nan1 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.BB9617754F988DB2317DCDE2C763F70E628FC22480CAB9E3CAE2AB61250C92DB), 4).ToArray();

		public static readonly byte[] Nan2 = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.4DE903E5767AA18027F864614FF3198F2647A322C9E57219265CC558FE478D44), 4).ToArray();

		public static readonly byte[] HexPrefix = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.61575F52DC2418D9A0894EAF4FD21C7DD65E868231EBD4F33CC2B7C46209388C), 2).ToArray();

		public static readonly byte[] HexPrefixNegative = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.995F4D7DC1983EB2CFAB5F18EB1D8767A37850784F410DAA47ECE9F51311900A), 3).ToArray();

		public static readonly byte[] OctalPrefix = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.97565685E6EE2333233D7D45D6518E12CB327A62663624C4375FE4D6AC2D2318), 2).ToArray();

		public static readonly byte[] UnityStrippedSymbol = new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.93058D82F95B49F5ACA346FAB60250513CE858A1AAD471326797323ABB8514EF), 8).ToArray();

		public const byte Space = 32;

		public const byte Tab = 9;

		public const byte Lf = 10;

		public const byte Cr = 13;

		public const byte Comment = 35;

		public const byte DirectiveLine = 37;

		public const byte Alias = 42;

		public const byte Anchor = 38;

		public const byte Tag = 33;

		public const byte SingleQuote = 39;

		public const byte DoubleQuote = 34;

		public const byte LiteralScalerHeader = 124;

		public const byte FoldedScalerHeader = 62;

		public const byte Comma = 44;

		public const byte BlockEntryIndent = 45;

		public const byte ExplicitKeyIndent = 63;

		public const byte MapValueIndent = 58;

		public const byte FlowMapStart = 123;

		public const byte FlowMapEnd = 125;

		public const byte FlowSequenceStart = 91;

		public const byte FlowSequenceEnd = 93;

		private static readonly bool[] EmptyTable = new bool[256];

		private static readonly bool[] BlankTable = new bool[256];

		private static readonly bool[] FlowSymbolTable = new bool[256];
	}
}
