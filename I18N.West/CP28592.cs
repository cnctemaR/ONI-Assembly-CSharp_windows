using System;
using System.Text;
using I18N.Common;

namespace I18N.West
{
	[Serializable]
	public class CP28592 : ByteEncoding
	{
		public CP28592()
			: base(28592, CP28592.ToChars, "Central European (ISO)", "iso-8859-2", "iso-8859-2", "iso-8859-2", true, true, true, true, 1250)
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
					case 193:
					case 194:
					case 196:
					case 199:
					case 201:
					case 203:
					case 205:
					case 206:
					case 208:
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
							case 162:
								num3 = 141;
								break;
							default:
								switch (num4)
								{
								case 9786:
									num3 = 1;
									break;
								case 9787:
									num3 = 2;
									break;
								case 9788:
									num3 = 15;
									break;
								default:
									switch (num4)
									{
									case 9552:
										num3 = 157;
										break;
									case 9553:
										num3 = 138;
										break;
									default:
										switch (num4)
										{
										case 9824:
											num3 = 6;
											break;
										default:
											switch (num4)
											{
											case 65512:
												num3 = 131;
												break;
											case 65513:
												num3 = 27;
												break;
											case 65514:
												num3 = 24;
												break;
											case 65515:
												num3 = 26;
												break;
											case 65516:
												num3 = 25;
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
													case 8592:
														num3 = 27;
														break;
													case 8593:
														num3 = 24;
														break;
													case 8594:
														num3 = 26;
														break;
													case 8595:
														num3 = 25;
														break;
													case 8596:
														num3 = 29;
														break;
													case 8597:
														num3 = 18;
														break;
													default:
														switch (num4)
														{
														case 9562:
															num3 = 152;
															break;
														default:
															switch (num4)
															{
															case 9568:
																num3 = 156;
																break;
															default:
																switch (num4)
																{
																case 9574:
																	num3 = 155;
																	break;
																default:
																	switch (num4)
																	{
																	case 9472:
																		num3 = 148;
																		break;
																	default:
																		switch (num4)
																		{
																		case 9617:
																			num3 = 128;
																			break;
																		case 9618:
																			num3 = 129;
																			break;
																		case 9619:
																			num3 = 130;
																			break;
																		default:
																			switch (num4)
																			{
																			case 9658:
																				num3 = 16;
																				break;
																			default:
																				switch (num4)
																				{
																				case 9834:
																					num3 = 13;
																					break;
																				default:
																					if (num4 != 9688)
																					{
																						if (num4 != 9689)
																						{
																							if (num4 != 711)
																							{
																								if (num4 != 8226)
																								{
																									if (num4 != 8252)
																									{
																										if (num4 != 8616)
																										{
																											if (num4 != 8735)
																											{
																												if (num4 != 9484)
																												{
																													if (num4 != 9488)
																													{
																														if (num4 != 9492)
																														{
																															if (num4 != 9496)
																															{
																																if (num4 != 9500)
																																{
																																	if (num4 != 9508)
																																	{
																																		if (num4 != 9516)
																																		{
																																			if (num4 != 9524)
																																			{
																																				if (num4 != 9532)
																																				{
																																					if (num4 != 9580)
																																					{
																																						if (num4 != 9600)
																																						{
																																							if (num4 != 9604)
																																							{
																																								if (num4 != 9608)
																																								{
																																									if (num4 != 9644)
																																									{
																																										if (num4 != 9650)
																																										{
																																											if (num4 != 9668)
																																											{
																																												if (num4 != 9675)
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
																																													num3 = 9;
																																												}
																																											}
																																											else
																																											{
																																												num3 = 17;
																																											}
																																										}
																																										else
																																										{
																																											num3 = 30;
																																										}
																																									}
																																									else
																																									{
																																										num3 = 22;
																																									}
																																								}
																																								else
																																								{
																																									num3 = 135;
																																								}
																																							}
																																							else
																																							{
																																								num3 = 150;
																																							}
																																						}
																																						else
																																						{
																																							num3 = 151;
																																						}
																																					}
																																					else
																																					{
																																						num3 = 158;
																																					}
																																				}
																																				else
																																				{
																																					num3 = 149;
																																				}
																																			}
																																			else
																																			{
																																				num3 = 145;
																																			}
																																		}
																																		else
																																		{
																																			num3 = 146;
																																		}
																																	}
																																	else
																																	{
																																		num3 = 132;
																																	}
																																}
																																else
																																{
																																	num3 = 147;
																																}
																															}
																															else
																															{
																																num3 = 133;
																															}
																														}
																														else
																														{
																															num3 = 144;
																														}
																													}
																													else
																													{
																														num3 = 143;
																													}
																												}
																												else
																												{
																													num3 = 134;
																												}
																											}
																											else
																											{
																												num3 = 28;
																											}
																										}
																										else
																										{
																											num3 = 23;
																										}
																									}
																									else
																									{
																										num3 = 19;
																									}
																								}
																								else
																								{
																									num3 = 7;
																								}
																							}
																							else
																							{
																								num3 = 183;
																							}
																						}
																						else
																						{
																							num3 = 10;
																						}
																					}
																					else
																					{
																						num3 = 8;
																					}
																					break;
																				case 9836:
																					num3 = 14;
																					break;
																				}
																				break;
																			case 9660:
																				num3 = 31;
																				break;
																			}
																			break;
																		}
																		break;
																	case 9474:
																		num3 = 131;
																		break;
																	}
																	break;
																case 9577:
																	num3 = 154;
																	break;
																}
																break;
															case 9571:
																num3 = 137;
																break;
															}
															break;
														case 9565:
															num3 = 140;
															break;
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
											case 65518:
												num3 = 9;
												break;
											}
											break;
										case 9827:
											num3 = 5;
											break;
										case 9829:
											num3 = 3;
											break;
										case 9830:
											num3 = 4;
											break;
										}
										break;
									case 9556:
										num3 = 153;
										break;
									case 9559:
										num3 = 139;
										break;
									}
									break;
								case 9792:
									num3 = 12;
									break;
								case 9794:
									num3 = 11;
									break;
								}
								break;
							case 164:
							case 167:
							case 168:
							case 173:
							case 176:
							case 180:
							case 184:
								break;
							case 165:
								num3 = 142;
								break;
							case 169:
								num3 = 136;
								break;
							case 174:
								num3 = 159;
								break;
							case 182:
								num3 = 20;
								break;
							}
							break;
						case 317:
							num3 = 165;
							break;
						case 318:
							num3 = 181;
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
							num3 = 166;
							break;
						case 347:
							num3 = 182;
							break;
						case 350:
							num3 = 170;
							break;
						case 351:
							num3 = 186;
							break;
						case 352:
							num3 = 169;
							break;
						case 353:
							num3 = 185;
							break;
						case 354:
							num3 = 222;
							break;
						case 355:
							num3 = 254;
							break;
						case 356:
							num3 = 171;
							break;
						case 357:
							num3 = 187;
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
							num3 = 172;
							break;
						case 378:
							num3 = 188;
							break;
						case 379:
							num3 = 175;
							break;
						case 380:
							num3 = 191;
							break;
						case 381:
							num3 = 174;
							break;
						case 382:
							num3 = 190;
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
						num3 = 161;
						break;
					case 261:
						num3 = 177;
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
			'\u00a0', 'Ą', '\u02d8', 'Ł', '¤', 'Ľ', 'Ś', '§', '\u00a8', 'Š',
			'Ş', 'Ť', 'Ź', '\u00ad', 'Ž', 'Ż', '°', 'ą', '\u02db', 'ł',
			'\u00b4', 'ľ', 'ś', 'ˇ', '\u00b8', 'š', 'ş', 'ť', 'ź', '\u02dd',
			'ž', 'ż', 'Ŕ', 'Á', 'Â', 'Ă', 'Ä', 'Ĺ', 'Ć', 'Ç',
			'Č', 'É', 'Ę', 'Ë', 'Ě', 'Í', 'Î', 'Ď', 'Đ', 'Ń',
			'Ň', 'Ó', 'Ô', 'Ő', 'Ö', '×', 'Ř', 'Ů', 'Ú', 'Ű',
			'Ü', 'Ý', 'Ţ', 'ß', 'ŕ', 'á', 'â', 'ă', 'ä', 'ĺ',
			'ć', 'ç', 'č', 'é', 'ę', 'ë', 'ě', 'í', 'î', 'ď',
			'đ', 'ń', 'ň', 'ó', 'ô', 'ő', 'ö', '÷', 'ř', 'ů',
			'ú', 'ű', 'ü', 'ý', 'ţ', '\u02d9'
		};
	}
}
