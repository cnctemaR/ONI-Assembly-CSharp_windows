using System;
using System.Text;
using I18N.Common;

namespace I18N.West
{
	[Serializable]
	public class CP1252 : ByteEncoding
	{
		public CP1252()
			: base(1252, CP1252.ToChars, "Western European (Windows)", "iso-8859-1", "Windows-1252", "Windows-1252", true, true, true, true, 1252)
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
					case 157:
					case 160:
					case 161:
					case 162:
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
					case 175:
					case 176:
					case 177:
					case 178:
					case 179:
					case 180:
					case 181:
					case 182:
					case 183:
					case 184:
					case 185:
					case 186:
					case 187:
					case 188:
					case 189:
					case 190:
					case 191:
					case 192:
					case 193:
					case 194:
					case 195:
					case 196:
					case 197:
					case 198:
					case 199:
					case 200:
					case 201:
					case 202:
					case 203:
					case 204:
					case 205:
					case 206:
					case 207:
					case 208:
					case 209:
					case 210:
					case 211:
					case 212:
					case 213:
					case 214:
					case 215:
					case 216:
					case 217:
					case 218:
					case 219:
					case 220:
					case 221:
					case 222:
					case 223:
					case 224:
					case 225:
					case 226:
					case 227:
					case 228:
					case 229:
					case 230:
					case 231:
					case 232:
					case 233:
					case 234:
					case 235:
					case 236:
					case 237:
					case 238:
					case 239:
					case 240:
					case 241:
					case 242:
					case 243:
					case 244:
					case 245:
					case 246:
					case 247:
					case 248:
					case 249:
					case 250:
					case 251:
					case 252:
					case 253:
					case 254:
					case 255:
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
							case 141:
							case 143:
							case 144:
								break;
							default:
								if (num4 != 381)
								{
									if (num4 != 382)
									{
										if (num4 != 8249)
										{
											if (num4 != 8250)
											{
												if (num4 != 129)
												{
													if (num4 != 376)
													{
														if (num4 != 402)
														{
															if (num4 != 710)
															{
																if (num4 != 732)
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
																	num3 = 152;
																}
															}
															else
															{
																num3 = 136;
															}
														}
														else
														{
															num3 = 131;
														}
													}
													else
													{
														num3 = 159;
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
									}
									else
									{
										num3 = 158;
									}
								}
								else
								{
									num3 = 142;
								}
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
					case 338:
						num3 = 140;
						break;
					case 339:
						num3 = 156;
						break;
					case 352:
						num3 = 138;
						break;
					case 353:
						num3 = 154;
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
			'‚', 'ƒ', '„', '…', '†', '‡', 'ˆ', '‰', 'Š', '‹',
			'Œ', '\u008d', 'Ž', '\u008f', '\u0090', '‘', '’', '“', '”', '•',
			'–', '—', '\u02dc', '™', 'š', '›', 'œ', '\u009d', 'ž', 'Ÿ',
			'\u00a0', '¡', '¢', '£', '¤', '¥', '¦', '§', '\u00a8', '©',
			'ª', '«', '¬', '\u00ad', '®', '\u00af', '°', '±', '²', '³',
			'\u00b4', 'µ', '¶', '·', '\u00b8', '¹', 'º', '»', '¼', '½',
			'¾', '¿', 'À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'Æ', 'Ç',
			'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï', 'Ð', 'Ñ',
			'Ò', 'Ó', 'Ô', 'Õ', 'Ö', '×', 'Ø', 'Ù', 'Ú', 'Û',
			'Ü', 'Ý', 'Þ', 'ß', 'à', 'á', 'â', 'ã', 'ä', 'å',
			'æ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í', 'î', 'ï',
			'ð', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö', '÷', 'ø', 'ù',
			'ú', 'û', 'ü', 'ý', 'þ', 'ÿ'
		};
	}
}
