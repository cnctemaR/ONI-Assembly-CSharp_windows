using System;
using System.Text;
using I18N.Common;

namespace I18N.West
{
	[Serializable]
	public class CP10000 : ByteEncoding
	{
		public CP10000()
			: base(10000, CP10000.ToChars, "Western European (Mac)", "macintosh", "macintosh", "macintosh", false, false, false, false, 1252)
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
					case 160:
						num3 = 202;
						break;
					case 161:
						num3 = 193;
						break;
					case 162:
					case 163:
					case 169:
					case 177:
					case 181:
						break;
					case 164:
						num3 = 219;
						break;
					case 165:
						num3 = 180;
						break;
					default:
						switch (num4)
						{
						case 8211:
							num3 = 208;
							break;
						case 8212:
							num3 = 209;
							break;
						default:
							switch (num4)
							{
							case 728:
								num3 = 249;
								break;
							case 729:
								num3 = 250;
								break;
							case 730:
								num3 = 251;
								break;
							case 731:
								num3 = 254;
								break;
							case 732:
								num3 = 247;
								break;
							case 733:
								num3 = 253;
								break;
							default:
								switch (num4)
								{
								case 8800:
									num3 = 173;
									break;
								default:
									switch (num4)
									{
									case 8719:
										num3 = 184;
										break;
									default:
										if (num4 != 338)
										{
											if (num4 != 339)
											{
												if (num4 != 710)
												{
													if (num4 != 711)
													{
														if (num4 != 8249)
														{
															if (num4 != 8250)
															{
																if (num4 != 64257)
																{
																	if (num4 != 64258)
																	{
																		if (num4 != 376)
																		{
																			if (num4 != 402)
																			{
																				if (num4 != 960)
																				{
																					if (num4 != 8240)
																					{
																						if (num4 != 8260)
																						{
																							if (num4 != 8482)
																							{
																								if (num4 != 8486)
																								{
																									if (num4 != 8706)
																									{
																										if (num4 != 8710)
																										{
																											if (num4 != 8730)
																											{
																												if (num4 != 8734)
																												{
																													if (num4 != 8747)
																													{
																														if (num4 != 8776)
																														{
																															if (num4 != 8984)
																															{
																																if (num4 != 9674)
																																{
																																	if (num4 != 9830)
																																	{
																																		if (num4 != 10003)
																																		{
																																			if (num4 != 63743)
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
																																				num3 = 240;
																																			}
																																		}
																																		else
																																		{
																																			num3 = 18;
																																		}
																																	}
																																	else
																																	{
																																		num3 = 19;
																																	}
																																}
																																else
																																{
																																	num3 = 215;
																																}
																															}
																															else
																															{
																																num3 = 17;
																															}
																														}
																														else
																														{
																															num3 = 197;
																														}
																													}
																													else
																													{
																														num3 = 186;
																													}
																												}
																												else
																												{
																													num3 = 176;
																												}
																											}
																											else
																											{
																												num3 = 195;
																											}
																										}
																										else
																										{
																											num3 = 198;
																										}
																									}
																									else
																									{
																										num3 = 182;
																									}
																								}
																								else
																								{
																									num3 = 189;
																								}
																							}
																							else
																							{
																								num3 = 170;
																							}
																						}
																						else
																						{
																							num3 = 218;
																						}
																					}
																					else
																					{
																						num3 = 228;
																					}
																				}
																				else
																				{
																					num3 = 185;
																				}
																			}
																			else
																			{
																				num3 = 196;
																			}
																		}
																		else
																		{
																			num3 = 217;
																		}
																	}
																	else
																	{
																		num3 = 223;
																	}
																}
																else
																{
																	num3 = 222;
																}
															}
															else
															{
																num3 = 221;
															}
														}
														else
														{
															num3 = 220;
														}
													}
													else
													{
														num3 = 255;
													}
												}
												else
												{
													num3 = 246;
												}
											}
											else
											{
												num3 = 207;
											}
										}
										else
										{
											num3 = 206;
										}
										break;
									case 8721:
										num3 = 183;
										break;
									}
									break;
								case 8804:
									num3 = 178;
									break;
								case 8805:
									num3 = 179;
									break;
								}
								break;
							}
							break;
						case 8216:
							num3 = 212;
							break;
						case 8217:
							num3 = 213;
							break;
						case 8218:
							num3 = 226;
							break;
						case 8220:
							num3 = 210;
							break;
						case 8221:
							num3 = 211;
							break;
						case 8222:
							num3 = 227;
							break;
						case 8224:
							num3 = 160;
							break;
						case 8225:
							num3 = 224;
							break;
						case 8226:
							num3 = 165;
							break;
						case 8230:
							num3 = 201;
							break;
						}
						break;
					case 167:
						num3 = 164;
						break;
					case 168:
						num3 = 172;
						break;
					case 170:
						num3 = 187;
						break;
					case 171:
						num3 = 199;
						break;
					case 172:
						num3 = 194;
						break;
					case 174:
						num3 = 168;
						break;
					case 175:
						num3 = 248;
						break;
					case 176:
						num3 = 161;
						break;
					case 180:
						num3 = 171;
						break;
					case 182:
						num3 = 166;
						break;
					case 183:
						num3 = 225;
						break;
					case 184:
						num3 = 252;
						break;
					case 186:
						num3 = 188;
						break;
					case 187:
						num3 = 200;
						break;
					case 191:
						num3 = 192;
						break;
					case 192:
						num3 = 203;
						break;
					case 193:
						num3 = 231;
						break;
					case 194:
						num3 = 229;
						break;
					case 195:
						num3 = 204;
						break;
					case 196:
						num3 = 128;
						break;
					case 197:
						num3 = 129;
						break;
					case 198:
						num3 = 174;
						break;
					case 199:
						num3 = 130;
						break;
					case 200:
						num3 = 233;
						break;
					case 201:
						num3 = 131;
						break;
					case 202:
						num3 = 230;
						break;
					case 203:
						num3 = 232;
						break;
					case 204:
						num3 = 237;
						break;
					case 205:
						num3 = 234;
						break;
					case 206:
						num3 = 235;
						break;
					case 207:
						num3 = 236;
						break;
					case 209:
						num3 = 132;
						break;
					case 210:
						num3 = 241;
						break;
					case 211:
						num3 = 238;
						break;
					case 212:
						num3 = 239;
						break;
					case 213:
						num3 = 205;
						break;
					case 214:
						num3 = 133;
						break;
					case 216:
						num3 = 175;
						break;
					case 217:
						num3 = 244;
						break;
					case 218:
						num3 = 242;
						break;
					case 219:
						num3 = 243;
						break;
					case 220:
						num3 = 134;
						break;
					case 223:
						num3 = 167;
						break;
					case 224:
						num3 = 136;
						break;
					case 225:
						num3 = 135;
						break;
					case 226:
						num3 = 137;
						break;
					case 227:
						num3 = 139;
						break;
					case 228:
						num3 = 138;
						break;
					case 229:
						num3 = 140;
						break;
					case 230:
						num3 = 190;
						break;
					case 231:
						num3 = 141;
						break;
					case 232:
						num3 = 143;
						break;
					case 233:
						num3 = 142;
						break;
					case 234:
						num3 = 144;
						break;
					case 235:
						num3 = 145;
						break;
					case 236:
						num3 = 147;
						break;
					case 237:
						num3 = 146;
						break;
					case 238:
						num3 = 148;
						break;
					case 239:
						num3 = 149;
						break;
					case 241:
						num3 = 150;
						break;
					case 242:
						num3 = 152;
						break;
					case 243:
						num3 = 151;
						break;
					case 244:
						num3 = 153;
						break;
					case 245:
						num3 = 155;
						break;
					case 246:
						num3 = 154;
						break;
					case 247:
						num3 = 214;
						break;
					case 248:
						num3 = 191;
						break;
					case 249:
						num3 = 157;
						break;
					case 250:
						num3 = 156;
						break;
					case 251:
						num3 = 158;
						break;
					case 252:
						num3 = 159;
						break;
					case 255:
						num3 = 216;
						break;
					case 305:
						num3 = 245;
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
			'x', 'y', 'z', '{', '|', '}', '~', '\u007f', 'Ä', 'Å',
			'Ç', 'É', 'Ñ', 'Ö', 'Ü', 'á', 'à', 'â', 'ä', 'ã',
			'å', 'ç', 'é', 'è', 'ê', 'ë', 'í', 'ì', 'î', 'ï',
			'ñ', 'ó', 'ò', 'ô', 'ö', 'õ', 'ú', 'ù', 'û', 'ü',
			'†', '°', '¢', '£', '§', '•', '¶', 'ß', '®', '©',
			'™', '\u00b4', '\u00a8', '≠', 'Æ', 'Ø', '∞', '±', '≤', '≥',
			'¥', 'µ', '∂', '∑', '∏', 'π', '∫', 'ª', 'º', 'Ω',
			'æ', 'ø', '¿', '¡', '¬', '√', 'ƒ', '≈', '∆', '«',
			'»', '…', '\u00a0', 'À', 'Ã', 'Õ', 'Œ', 'œ', '–', '—',
			'“', '”', '‘', '’', '÷', '◊', 'ÿ', 'Ÿ', '⁄', '¤',
			'‹', '›', 'ﬁ', 'ﬂ', '‡', '·', '‚', '„', '‰', 'Â',
			'Ê', 'Á', 'Ë', 'È', 'Í', 'Î', 'Ï', 'Ì', 'Ó', 'Ô',
			'\uf8ff', 'Ò', 'Ú', 'Û', 'Ù', 'ı', 'ˆ', '\u02dc', '\u00af', '\u02d8',
			'\u02d9', '\u02da', '\u00b8', '\u02dd', '\u02db', 'ˇ'
		};
	}
}
