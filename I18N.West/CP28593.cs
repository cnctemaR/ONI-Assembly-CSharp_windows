using System;
using System.Text;
using I18N.Common;

namespace I18N.West
{
	[Serializable]
	public class CP28593 : ByteEncoding
	{
		public CP28593()
			: base(28593, CP28593.ToChars, "Latin 3 (ISO)", "iso-8859-3", "iso-8859-3", "iso-8859-3", true, true, true, true, 28593)
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
				if (num3 >= 161)
				{
					int num4 = num3;
					switch (num4)
					{
					case 163:
					case 164:
					case 167:
					case 168:
					case 173:
					case 176:
					case 178:
					case 179:
					case 180:
					case 181:
					case 183:
					case 184:
					case 189:
					case 192:
					case 193:
					case 194:
					case 196:
					case 199:
					case 200:
					case 201:
					case 202:
					case 203:
					case 204:
					case 205:
					case 206:
					case 207:
					case 209:
					case 210:
					case 211:
					case 212:
					case 214:
					case 215:
					case 217:
					case 218:
					case 219:
					case 220:
					case 223:
					case 224:
					case 225:
					case 226:
					case 228:
					case 231:
					case 232:
					case 233:
					case 234:
					case 235:
					case 236:
					case 237:
					case 238:
					case 239:
					case 241:
					case 242:
					case 243:
					case 244:
					case 246:
					case 247:
					case 249:
					case 250:
					case 251:
					case 252:
						break;
					default:
						switch (num4)
						{
						case 348:
							num3 = 222;
							break;
						case 349:
							num3 = 254;
							break;
						case 350:
							num3 = 170;
							break;
						case 351:
							num3 = 186;
							break;
						default:
							if (num4 != 364)
							{
								if (num4 != 365)
								{
									if (num4 != 379)
									{
										if (num4 != 380)
										{
											if (num4 != 728)
											{
												if (num4 != 729)
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
													num3 = 255;
												}
											}
											else
											{
												num3 = 162;
											}
										}
										else
										{
											num3 = 191;
										}
									}
									else
									{
										num3 = 175;
									}
								}
								else
								{
									num3 = 253;
								}
							}
							else
							{
								num3 = 221;
							}
							break;
						}
						break;
					case 264:
						num3 = 198;
						break;
					case 265:
						num3 = 230;
						break;
					case 266:
						num3 = 197;
						break;
					case 267:
						num3 = 229;
						break;
					case 284:
						num3 = 216;
						break;
					case 285:
						num3 = 248;
						break;
					case 286:
						num3 = 171;
						break;
					case 287:
						num3 = 187;
						break;
					case 288:
						num3 = 213;
						break;
					case 289:
						num3 = 245;
						break;
					case 292:
						num3 = 166;
						break;
					case 293:
						num3 = 182;
						break;
					case 294:
						num3 = 161;
						break;
					case 295:
						num3 = 177;
						break;
					case 304:
						num3 = 169;
						break;
					case 305:
						num3 = 185;
						break;
					case 308:
						num3 = 172;
						break;
					case 309:
						num3 = 188;
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
			'x', 'y', 'z', '{', '|', '}', '~', '\u007f', '\u0080', '\u0081',
			'\u0082', '\u0083', '\u0084', '\u0085', '\u0086', '\u0087', '\u0088', '\u0089', '\u008a', '\u008b',
			'\u008c', '\u008d', '\u008e', '\u008f', '\u0090', '\u0091', '\u0092', '\u0093', '\u0094', '\u0095',
			'\u0096', '\u0097', '\u0098', '\u0099', '\u009a', '\u009b', '\u009c', '\u009d', '\u009e', '\u009f',
			'\u00a0', 'Ħ', '\u02d8', '£', '¤', '?', 'Ĥ', '§', '\u00a8', 'İ',
			'Ş', 'Ğ', 'Ĵ', '\u00ad', '?', 'Ż', '°', 'ħ', '²', '³',
			'\u00b4', 'µ', 'ĥ', '·', '\u00b8', 'ı', 'ş', 'ğ', 'ĵ', '½',
			'?', 'ż', 'À', 'Á', 'Â', '?', 'Ä', 'Ċ', 'Ĉ', 'Ç',
			'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï', '?', 'Ñ',
			'Ò', 'Ó', 'Ô', 'Ġ', 'Ö', '×', 'Ĝ', 'Ù', 'Ú', 'Û',
			'Ü', 'Ŭ', 'Ŝ', 'ß', 'à', 'á', 'â', '?', 'ä', 'ċ',
			'ĉ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í', 'î', 'ï',
			'?', 'ñ', 'ò', 'ó', 'ô', 'ġ', 'ö', '÷', 'ĝ', 'ù',
			'ú', 'û', 'ü', 'ŭ', 'ŝ', '\u02d9'
		};
	}
}
