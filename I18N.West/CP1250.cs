using System;
using System.Text;
using I18N.Common;

namespace I18N.West
{
	[Serializable]
	public class CP1250 : ByteEncoding
	{
		public CP1250()
			: base(1250, CP1250.ToChars, "Central European (Windows)", "iso-8859-2", "windows-1250", "windows-1250", true, true, true, true, 1250)
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
				charCount--;
				if (num3 >= 128)
				{
					int num4 = num3;
					switch (num4)
					{
					case 152:
					case 160:
					case 164:
					case 166:
					case 167:
					case 168:
					case 169:
					case 171:
					case 172:
					case 173:
					case 174:
					case 176:
					case 177:
					case 180:
					case 181:
					case 182:
					case 183:
					case 184:
					case 187:
					case 193:
					case 194:
					case 196:
					case 199:
					case 201:
					case 203:
					case 205:
					case 206:
					case 211:
					case 212:
					case 214:
					case 215:
					case 218:
					case 220:
					case 221:
					case 223:
					case 225:
					case 226:
					case 228:
					case 231:
					case 233:
					case 235:
					case 237:
					case 238:
					case 243:
					case 244:
					case 246:
					case 247:
					case 250:
					case 252:
					case 253:
						break;
					default:
						switch (num4)
						{
						case 313:
							num3 = 197;
							break;
						case 314:
							num3 = 229;
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
							default:
								switch (num4)
								{
								case 728:
									num3 = 162;
									break;
								case 729:
									num3 = 255;
									break;
								default:
									switch (num4)
									{
									case 129:
									case 131:
										break;
									default:
										if (num4 != 8249)
										{
											if (num4 != 8250)
											{
												if (num4 != 136 && num4 != 144)
												{
													if (num4 != 711)
													{
														if (num4 != 8240)
														{
															if (num4 != 8364)
															{
																if (num4 != 8482)
																{
																	if (num3 < 65281 || num3 > 65374)
																	{
																		base.HandleFallback(ref encoderFallbackBuffer, chars, ref num, ref charCount, bytes, ref num2, ref byteCount);
																		continue;
																	}
																	num3 -= 65248;
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
														num3 = 161;
													}
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
									}
									break;
								case 731:
									num3 = 178;
									break;
								case 733:
									num3 = 189;
									break;
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
						case 317:
							num3 = 188;
							break;
						case 318:
							num3 = 190;
							break;
						case 321:
							num3 = 163;
							break;
						case 322:
							num3 = 179;
							break;
						case 323:
							num3 = 209;
							break;
						case 324:
							num3 = 241;
							break;
						case 327:
							num3 = 210;
							break;
						case 328:
							num3 = 242;
							break;
						case 336:
							num3 = 213;
							break;
						case 337:
							num3 = 245;
							break;
						case 340:
							num3 = 192;
							break;
						case 341:
							num3 = 224;
							break;
						case 344:
							num3 = 216;
							break;
						case 345:
							num3 = 248;
							break;
						case 346:
							num3 = 140;
							break;
						case 347:
							num3 = 156;
							break;
						case 350:
							num3 = 170;
							break;
						case 351:
							num3 = 186;
							break;
						case 352:
							num3 = 138;
							break;
						case 353:
							num3 = 154;
							break;
						case 354:
							num3 = 222;
							break;
						case 355:
							num3 = 254;
							break;
						case 356:
							num3 = 141;
							break;
						case 357:
							num3 = 157;
							break;
						case 366:
							num3 = 217;
							break;
						case 367:
							num3 = 249;
							break;
						case 368:
							num3 = 219;
							break;
						case 369:
							num3 = 251;
							break;
						case 377:
							num3 = 143;
							break;
						case 378:
							num3 = 159;
							break;
						case 379:
							num3 = 175;
							break;
						case 380:
							num3 = 191;
							break;
						case 381:
							num3 = 142;
							break;
						case 382:
							num3 = 158;
							break;
						}
						break;
					case 258:
						num3 = 195;
						break;
					case 259:
						num3 = 227;
						break;
					case 260:
						num3 = 165;
						break;
					case 261:
						num3 = 185;
						break;
					case 262:
						num3 = 198;
						break;
					case 263:
						num3 = 230;
						break;
					case 268:
						num3 = 200;
						break;
					case 269:
						num3 = 232;
						break;
					case 270:
						num3 = 207;
						break;
					case 271:
						num3 = 239;
						break;
					case 272:
						num3 = 208;
						break;
					case 273:
						num3 = 240;
						break;
					case 280:
						num3 = 202;
						break;
					case 281:
						num3 = 234;
						break;
					case 282:
						num3 = 204;
						break;
					case 283:
						num3 = 236;
						break;
					}
				}
				bytes[num2++] = (byte)num3;
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
			'‚', '\u0083', '„', '…', '†', '‡', '\u0088', '‰', 'Š', '‹',
			'Ś', 'Ť', 'Ž', 'Ź', '\u0090', '‘', '’', '“', '”', '•',
			'–', '—', '\u0098', '™', 'š', '›', 'ś', 'ť', 'ž', 'ź',
			'\u00a0', 'ˇ', '\u02d8', 'Ł', '¤', 'Ą', '¦', '§', '\u00a8', '©',
			'Ş', '«', '¬', '\u00ad', '®', 'Ż', '°', '±', '\u02db', 'ł',
			'\u00b4', 'µ', '¶', '·', '\u00b8', 'ą', 'ş', '»', 'Ľ', '\u02dd',
			'ľ', 'ż', 'Ŕ', 'Á', 'Â', 'Ă', 'Ä', 'Ĺ', 'Ć', 'Ç',
			'Č', 'É', 'Ę', 'Ë', 'Ě', 'Í', 'Î', 'Ď', 'Đ', 'Ń',
			'Ň', 'Ó', 'Ô', 'Ő', 'Ö', '×', 'Ř', 'Ů', 'Ú', 'Ű',
			'Ü', 'Ý', 'Ţ', 'ß', 'ŕ', 'á', 'â', 'ă', 'ä', 'ĺ',
			'ć', 'ç', 'č', 'é', 'ę', 'ë', 'ě', 'í', 'î', 'ď',
			'đ', 'ń', 'ň', 'ó', 'ô', 'ő', 'ö', '÷', 'ř', 'ů',
			'ú', 'ű', 'ü', 'ý', 'ţ', '\u02d9'
		};
	}
}
