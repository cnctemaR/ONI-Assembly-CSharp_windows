using System;
using System.Text;
using I18N.Common;

namespace I18N.West
{
	[Serializable]
	public class CP850 : ByteEncoding
	{
		public CP850()
			: base(850, CP850.ToChars, "Western European (DOS)", "ibm850", "ibm850", "ibm850", false, false, false, false, 1252)
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
				if (num3 >= 26)
				{
					int num4 = num3;
					switch (num4)
					{
					case 26:
						num3 = 127;
						break;
					case 27:
					case 29:
					case 30:
					case 31:
					case 32:
					case 33:
					case 34:
					case 35:
					case 36:
					case 37:
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
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 57:
					case 58:
					case 59:
					case 60:
					case 61:
					case 62:
					case 63:
					case 64:
					case 65:
					case 66:
					case 67:
					case 68:
					case 69:
					case 70:
					case 71:
					case 72:
					case 73:
					case 74:
					case 75:
					case 76:
					case 77:
					case 78:
					case 79:
					case 80:
					case 81:
					case 82:
					case 83:
					case 84:
					case 85:
					case 86:
					case 87:
					case 88:
					case 89:
					case 90:
					case 91:
					case 92:
					case 93:
					case 94:
					case 95:
					case 96:
					case 97:
					case 98:
					case 99:
					case 100:
					case 101:
					case 102:
					case 103:
					case 104:
					case 105:
					case 106:
					case 107:
					case 108:
					case 109:
					case 110:
					case 111:
					case 112:
					case 113:
					case 114:
					case 115:
					case 116:
					case 117:
					case 118:
					case 119:
					case 120:
					case 121:
					case 122:
					case 123:
					case 124:
					case 125:
					case 126:
						break;
					case 28:
						num3 = 26;
						break;
					case 127:
						num3 = 28;
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
								num3 = 205;
								break;
							case 9553:
								num3 = 186;
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
										num3 = 179;
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
									case 65517:
										num3 = 254;
										break;
									case 65518:
										num3 = 9;
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
												num3 = 200;
												break;
											default:
												switch (num4)
												{
												case 9568:
													num3 = 204;
													break;
												default:
													switch (num4)
													{
													case 9574:
														num3 = 203;
														break;
													default:
														switch (num4)
														{
														case 8252:
															num3 = 19;
															break;
														default:
															switch (num4)
															{
															case 9472:
																num3 = 196;
																break;
															default:
																switch (num4)
																{
																case 9617:
																	num3 = 176;
																	break;
																case 9618:
																	num3 = 177;
																	break;
																case 9619:
																	num3 = 178;
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
																					if (num4 != 8215)
																					{
																						if (num4 != 8226)
																						{
																							if (num4 != 8616)
																							{
																								if (num4 != 8735)
																								{
																									if (num4 != 8962)
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
																																							if (num4 != 9632)
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
																																								num3 = 254;
																																							}
																																						}
																																						else
																																						{
																																							num3 = 219;
																																						}
																																					}
																																					else
																																					{
																																						num3 = 220;
																																					}
																																				}
																																				else
																																				{
																																					num3 = 223;
																																				}
																																			}
																																			else
																																			{
																																				num3 = 206;
																																			}
																																		}
																																		else
																																		{
																																			num3 = 197;
																																		}
																																	}
																																	else
																																	{
																																		num3 = 193;
																																	}
																																}
																																else
																																{
																																	num3 = 194;
																																}
																															}
																															else
																															{
																																num3 = 180;
																															}
																														}
																														else
																														{
																															num3 = 195;
																														}
																													}
																													else
																													{
																														num3 = 217;
																													}
																												}
																												else
																												{
																													num3 = 192;
																												}
																											}
																											else
																											{
																												num3 = 191;
																											}
																										}
																										else
																										{
																											num3 = 218;
																										}
																									}
																									else
																									{
																										num3 = 127;
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
																							num3 = 7;
																						}
																					}
																					else
																					{
																						num3 = 242;
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
																num3 = 179;
																break;
															}
															break;
														case 8254:
															num3 = 238;
															break;
														}
														break;
													case 9577:
														num3 = 202;
														break;
													}
													break;
												case 9571:
													num3 = 185;
													break;
												}
												break;
											case 9565:
												num3 = 188;
												break;
											}
											break;
										}
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
								num3 = 201;
								break;
							case 9559:
								num3 = 187;
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
					case 160:
						num3 = 255;
						break;
					case 161:
						num3 = 173;
						break;
					case 162:
						num3 = 189;
						break;
					case 163:
						num3 = 156;
						break;
					case 164:
						num3 = 207;
						break;
					case 165:
						num3 = 190;
						break;
					case 166:
						num3 = 221;
						break;
					case 167:
						num3 = 245;
						break;
					case 168:
						num3 = 249;
						break;
					case 169:
						num3 = 184;
						break;
					case 170:
						num3 = 166;
						break;
					case 171:
						num3 = 174;
						break;
					case 172:
						num3 = 170;
						break;
					case 173:
						num3 = 240;
						break;
					case 174:
						num3 = 169;
						break;
					case 175:
						num3 = 238;
						break;
					case 176:
						num3 = 248;
						break;
					case 177:
						num3 = 241;
						break;
					case 178:
						num3 = 253;
						break;
					case 179:
						num3 = 252;
						break;
					case 180:
						num3 = 239;
						break;
					case 181:
						num3 = 230;
						break;
					case 182:
						num3 = 244;
						break;
					case 183:
						num3 = 250;
						break;
					case 184:
						num3 = 247;
						break;
					case 185:
						num3 = 251;
						break;
					case 186:
						num3 = 167;
						break;
					case 187:
						num3 = 175;
						break;
					case 188:
						num3 = 172;
						break;
					case 189:
						num3 = 171;
						break;
					case 190:
						num3 = 243;
						break;
					case 191:
						num3 = 168;
						break;
					case 192:
						num3 = 183;
						break;
					case 193:
						num3 = 181;
						break;
					case 194:
						num3 = 182;
						break;
					case 195:
						num3 = 199;
						break;
					case 196:
						num3 = 142;
						break;
					case 197:
						num3 = 143;
						break;
					case 198:
						num3 = 146;
						break;
					case 199:
						num3 = 128;
						break;
					case 200:
						num3 = 212;
						break;
					case 201:
						num3 = 144;
						break;
					case 202:
						num3 = 210;
						break;
					case 203:
						num3 = 211;
						break;
					case 204:
						num3 = 222;
						break;
					case 205:
						num3 = 214;
						break;
					case 206:
						num3 = 215;
						break;
					case 207:
						num3 = 216;
						break;
					case 208:
						num3 = 209;
						break;
					case 209:
						num3 = 165;
						break;
					case 210:
						num3 = 227;
						break;
					case 211:
						num3 = 224;
						break;
					case 212:
						num3 = 226;
						break;
					case 213:
						num3 = 229;
						break;
					case 214:
						num3 = 153;
						break;
					case 215:
						num3 = 158;
						break;
					case 216:
						num3 = 157;
						break;
					case 217:
						num3 = 235;
						break;
					case 218:
						num3 = 233;
						break;
					case 219:
						num3 = 234;
						break;
					case 220:
						num3 = 154;
						break;
					case 221:
						num3 = 237;
						break;
					case 222:
						num3 = 232;
						break;
					case 223:
						num3 = 225;
						break;
					case 224:
						num3 = 133;
						break;
					case 225:
						num3 = 160;
						break;
					case 226:
						num3 = 131;
						break;
					case 227:
						num3 = 198;
						break;
					case 228:
						num3 = 132;
						break;
					case 229:
						num3 = 134;
						break;
					case 230:
						num3 = 145;
						break;
					case 231:
						num3 = 135;
						break;
					case 232:
						num3 = 138;
						break;
					case 233:
						num3 = 130;
						break;
					case 234:
						num3 = 136;
						break;
					case 235:
						num3 = 137;
						break;
					case 236:
						num3 = 141;
						break;
					case 237:
						num3 = 161;
						break;
					case 238:
						num3 = 140;
						break;
					case 239:
						num3 = 139;
						break;
					case 240:
						num3 = 208;
						break;
					case 241:
						num3 = 164;
						break;
					case 242:
						num3 = 149;
						break;
					case 243:
						num3 = 162;
						break;
					case 244:
						num3 = 147;
						break;
					case 245:
						num3 = 228;
						break;
					case 246:
						num3 = 148;
						break;
					case 247:
						num3 = 246;
						break;
					case 248:
						num3 = 155;
						break;
					case 249:
						num3 = 151;
						break;
					case 250:
						num3 = 163;
						break;
					case 251:
						num3 = 150;
						break;
					case 252:
						num3 = 129;
						break;
					case 253:
						num3 = 236;
						break;
					case 254:
						num3 = 231;
						break;
					case 255:
						num3 = 152;
						break;
					case 272:
						num3 = 209;
						break;
					case 305:
						num3 = 213;
						break;
					case 402:
						num3 = 159;
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
			'\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019', '\u001c', '\u001b', '\u007f', '\u001d',
			'\u001e', '\u001f', ' ', '!', '"', '#', '$', '%', '&', '\'',
			'(', ')', '*', '+', ',', '-', '.', '/', '0', '1',
			'2', '3', '4', '5', '6', '7', '8', '9', ':', ';',
			'<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E',
			'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
			'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y',
			'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b', 'c',
			'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
			'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w',
			'x', 'y', 'z', '{', '|', '}', '~', '\u001a', 'Ç', 'ü',
			'é', 'â', 'ä', 'à', 'å', 'ç', 'ê', 'ë', 'è', 'ï',
			'î', 'ì', 'Ä', 'Å', 'É', 'æ', 'Æ', 'ô', 'ö', 'ò',
			'û', 'ù', 'ÿ', 'Ö', 'Ü', 'ø', '£', 'Ø', '×', 'ƒ',
			'á', 'í', 'ó', 'ú', 'ñ', 'Ñ', 'ª', 'º', '¿', '®',
			'¬', '½', '¼', '¡', '«', '»', '░', '▒', '▓', '│',
			'┤', 'Á', 'Â', 'À', '©', '╣', '║', '╗', '╝', '¢',
			'¥', '┐', '└', '┴', '┬', '├', '─', '┼', 'ã', 'Ã',
			'╚', '╔', '╩', '╦', '╠', '═', '╬', '¤', 'ð', 'Ð',
			'Ê', 'Ë', 'È', 'ı', 'Í', 'Î', 'Ï', '┘', '┌', '█',
			'▄', '¦', 'Ì', '▀', 'Ó', 'ß', 'Ô', 'Ò', 'õ', 'Õ',
			'µ', 'þ', 'Þ', 'Ú', 'Û', 'Ù', 'ý', 'Ý', '\u00af', '\u00b4',
			'\u00ad', '±', '‗', '¾', '¶', '§', '÷', '\u00b8', '°', '\u00a8',
			'·', '¹', '³', '²', '■', '\u00a0'
		};
	}
}
