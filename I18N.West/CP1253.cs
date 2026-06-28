using System;
using System.Text;
using I18N.Common;

namespace I18N.West
{
	[Serializable]
	public class CP1253 : ByteEncoding
	{
		public CP1253()
			: base(1253, CP1253.ToChars, "Greek (Windows)", "iso-8859-7", "windows-1253", "windows-1253", true, true, true, true, 1253)
		{
		}

		protected unsafe override void ToBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			int num = 0;
			int num2 = 0;
			EncoderFallbackBuffer encoderFallbackBuffer = null;
			while (charCount > 0)
			{
				int num3 = (int)chars[num++];
				if (num3 >= 128)
				{
					int num4 = num3;
					switch (num4)
					{
					case 900:
						num3 = 180;
						break;
					case 901:
						num3 = 161;
						break;
					case 902:
						num3 = 162;
						break;
					default:
						switch (num4)
						{
						case 129:
						case 136:
						case 138:
						case 140:
						case 141:
						case 142:
						case 143:
						case 144:
						case 152:
						case 154:
						case 156:
						case 157:
						case 158:
						case 159:
						case 160:
						case 163:
						case 164:
						case 165:
						case 166:
						case 167:
						case 168:
						case 169:
						case 170:
						case 171:
						case 172:
						case 173:
						case 174:
						case 176:
						case 177:
						case 178:
						case 179:
						case 181:
						case 182:
						case 183:
						case 187:
						case 189:
							break;
						default:
							switch (num4)
							{
							case 8211:
								num3 = 150;
								break;
							case 8212:
								num3 = 151;
								break;
							case 8213:
								num3 = 175;
								break;
							default:
								if (num4 != 8249)
								{
									if (num4 != 8250)
									{
										if (num4 != 402)
										{
											if (num4 != 8240)
											{
												if (num4 != 8364)
												{
													if (num4 != 8482)
													{
														if (num3 >= 65281 && num3 <= 65374)
														{
															num3 -= 65248;
														}
														else
														{
															base.HandleFallback(ref encoderFallbackBuffer, chars, ref num, ref charCount, bytes, ref num2, ref byteCount);
														}
													}
													else
													{
														num3 = 153;
													}
												}
												else
												{
													num3 = 128;
												}
											}
											else
											{
												num3 = 137;
											}
										}
										else
										{
											num3 = 131;
										}
									}
									else
									{
										num3 = 155;
									}
								}
								else
								{
									num3 = 139;
								}
								break;
							case 8216:
								num3 = 145;
								break;
							case 8217:
								num3 = 146;
								break;
							case 8218:
								num3 = 130;
								break;
							case 8220:
								num3 = 147;
								break;
							case 8221:
								num3 = 148;
								break;
							case 8222:
								num3 = 132;
								break;
							case 8224:
								num3 = 134;
								break;
							case 8225:
								num3 = 135;
								break;
							case 8226:
								num3 = 149;
								break;
							case 8230:
								num3 = 133;
								break;
							}
							break;
						}
						break;
					case 904:
						num3 = 184;
						break;
					case 905:
						num3 = 185;
						break;
					case 906:
						num3 = 186;
						break;
					case 908:
						num3 = 188;
						break;
					case 910:
					case 911:
					case 912:
					case 913:
					case 914:
					case 915:
					case 916:
					case 917:
					case 918:
					case 919:
					case 920:
					case 921:
					case 922:
					case 923:
					case 924:
					case 925:
					case 926:
					case 927:
					case 928:
					case 929:
						num3 -= 720;
						break;
					case 931:
					case 932:
					case 933:
					case 934:
					case 935:
					case 936:
					case 937:
					case 938:
					case 939:
					case 940:
					case 941:
					case 942:
					case 943:
					case 944:
					case 945:
					case 946:
					case 947:
					case 948:
					case 949:
					case 950:
					case 951:
					case 952:
					case 953:
					case 954:
					case 955:
					case 956:
					case 957:
					case 958:
					case 959:
					case 960:
					case 961:
					case 962:
					case 963:
					case 964:
					case 965:
					case 966:
					case 967:
					case 968:
					case 969:
					case 970:
					case 971:
					case 972:
					case 973:
					case 974:
						num3 -= 720;
						break;
					case 981:
						num3 = 246;
						break;
					}
				}
				bytes[num2++] = (byte)num3;
				charCount--;
				byteCount--;
			}
		}

		private static readonly char[] ToChars = new char[]
		{
			'\0', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\t',
			'\n', '\v', '\f', '\r', '\u000e', '\u000f', '\u0010', '\u0011', '\u0012', '\u0013',
			'\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019', '\u001a', '\u001b', '\u001c', '\u001d',
			'\u001e', '\u001f', ' ', '!', '"', '#', '$', '%', '&', '\'',
			'(', ')', '*', '+', ',', '-', '.', '/', '0', '1',
			'2', '3', '4', '5', '6', '7', '8', '9', ':', ';',
			'<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E',
			'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
			'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y',
			'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b', 'c',
			'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
			'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w',
			'x', 'y', 'z', '{', '|', '}', '~', '\u007f', '€', '\u0081',
			'‚', 'ƒ', '„', '…', '†', '‡', '\u0088', '‰', '\u008a', '‹',
			'\u008c', '\u008d', '\u008e', '\u008f', '\u0090', '‘', '’', '“', '”', '•',
			'–', '—', '\u0098', '™', '\u009a', '›', '\u009c', '\u009d', '\u009e', '\u009f',
			'\u00a0', '\u0385', 'Ά', '£', '¤', '¥', '¦', '§', '\u00a8', '©',
			'ª', '«', '¬', '\u00ad', '®', '―', '°', '±', '²', '³',
			'\u0384', 'µ', '¶', '·', 'Έ', 'Ή', 'Ί', '»', 'Ό', '½',
			'Ύ', 'Ώ', 'ΐ', 'Α', 'Β', 'Γ', 'Δ', 'Ε', 'Ζ', 'Η',
			'Θ', 'Ι', 'Κ', 'Λ', 'Μ', 'Ν', 'Ξ', 'Ο', 'Π', 'Ρ',
			'?', 'Σ', 'Τ', 'Υ', 'Φ', 'Χ', 'Ψ', 'Ω', 'Ϊ', 'Ϋ',
			'ά', 'έ', 'ή', 'ί', 'ΰ', 'α', 'β', 'γ', 'δ', 'ε',
			'ζ', 'η', 'θ', 'ι', 'κ', 'λ', 'μ', 'ν', 'ξ', 'ο',
			'π', 'ρ', 'ς', 'σ', 'τ', 'υ', 'φ', 'χ', 'ψ', 'ω',
			'ϊ', 'ϋ', 'ό', 'ύ', 'ώ', '?'
		};
	}
}
