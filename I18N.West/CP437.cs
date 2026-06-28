using System;
using System.Text;
using I18N.Common;

namespace I18N.West
{
	[Serializable]
	public class CP437 : ByteEncoding
	{
		public CP437()
			: base(437, CP437.ToChars, "OEM United States", "IBM437", "IBM437", "IBM437", false, false, false, false, 1252)
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
						num3 = 255;
						break;
					case 161:
						num3 = 173;
						break;
					case 162:
						num3 = 155;
						break;
					case 163:
						num3 = 156;
						break;
					case 164:
						num3 = 15;
						break;
					case 165:
						num3 = 157;
						break;
					case 166:
						num3 = 221;
						break;
					case 167:
						num3 = 21;
						break;
					case 168:
						num3 = 34;
						break;
					case 169:
						num3 = 99;
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
						num3 = 45;
						break;
					case 174:
						num3 = 114;
						break;
					case 175:
						num3 = 95;
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
						num3 = 51;
						break;
					case 180:
						num3 = 39;
						break;
					case 181:
						num3 = 230;
						break;
					case 182:
						num3 = 20;
						break;
					case 183:
						num3 = 250;
						break;
					case 184:
						num3 = 44;
						break;
					case 185:
						num3 = 49;
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
						num3 = 95;
						break;
					case 191:
						num3 = 168;
						break;
					case 192:
						num3 = 65;
						break;
					case 193:
						num3 = 65;
						break;
					case 194:
						num3 = 65;
						break;
					case 195:
						num3 = 65;
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
						num3 = 69;
						break;
					case 201:
						num3 = 144;
						break;
					case 202:
						num3 = 69;
						break;
					case 203:
						num3 = 69;
						break;
					case 204:
						num3 = 73;
						break;
					case 205:
						num3 = 73;
						break;
					case 206:
						num3 = 73;
						break;
					case 207:
						num3 = 73;
						break;
					case 208:
						num3 = 68;
						break;
					case 209:
						num3 = 165;
						break;
					case 210:
						num3 = 79;
						break;
					case 211:
						num3 = 79;
						break;
					case 212:
						num3 = 79;
						break;
					case 213:
						num3 = 79;
						break;
					case 214:
						num3 = 153;
						break;
					case 215:
						num3 = 120;
						break;
					case 216:
						num3 = 79;
						break;
					case 217:
						num3 = 85;
						break;
					case 218:
						num3 = 85;
						break;
					case 219:
						num3 = 85;
						break;
					case 220:
						num3 = 154;
						break;
					case 221:
						num3 = 89;
						break;
					case 222:
						num3 = 95;
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
						num3 = 97;
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
						num3 = 100;
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
						num3 = 111;
						break;
					case 246:
						num3 = 148;
						break;
					case 247:
						num3 = 246;
						break;
					case 248:
						num3 = 111;
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
						num3 = 121;
						break;
					case 254:
						num3 = 95;
						break;
					case 255:
						num3 = 152;
						break;
					case 256:
						num3 = 65;
						break;
					case 257:
						num3 = 97;
						break;
					case 258:
						num3 = 65;
						break;
					case 259:
						num3 = 97;
						break;
					case 260:
						num3 = 65;
						break;
					case 261:
						num3 = 97;
						break;
					case 262:
						num3 = 67;
						break;
					case 263:
						num3 = 99;
						break;
					case 264:
						num3 = 67;
						break;
					case 265:
						num3 = 99;
						break;
					case 266:
						num3 = 67;
						break;
					case 267:
						num3 = 99;
						break;
					case 268:
						num3 = 67;
						break;
					case 269:
						num3 = 99;
						break;
					case 270:
						num3 = 68;
						break;
					case 271:
						num3 = 100;
						break;
					case 272:
						num3 = 68;
						break;
					case 273:
						num3 = 100;
						break;
					case 274:
						num3 = 69;
						break;
					case 275:
						num3 = 101;
						break;
					case 276:
						num3 = 69;
						break;
					case 277:
						num3 = 101;
						break;
					case 278:
						num3 = 69;
						break;
					case 279:
						num3 = 101;
						break;
					case 280:
						num3 = 69;
						break;
					case 281:
						num3 = 101;
						break;
					case 282:
						num3 = 69;
						break;
					case 283:
						num3 = 101;
						break;
					case 284:
						num3 = 71;
						break;
					case 285:
						num3 = 103;
						break;
					case 286:
						num3 = 71;
						break;
					case 287:
						num3 = 103;
						break;
					case 288:
						num3 = 71;
						break;
					case 289:
						num3 = 103;
						break;
					case 290:
						num3 = 71;
						break;
					case 291:
						num3 = 103;
						break;
					case 292:
						num3 = 72;
						break;
					case 293:
						num3 = 104;
						break;
					case 294:
						num3 = 72;
						break;
					case 295:
						num3 = 104;
						break;
					case 296:
						num3 = 73;
						break;
					case 297:
						num3 = 105;
						break;
					case 298:
						num3 = 73;
						break;
					case 299:
						num3 = 105;
						break;
					case 300:
						num3 = 73;
						break;
					case 301:
						num3 = 105;
						break;
					case 302:
						num3 = 73;
						break;
					case 303:
						num3 = 105;
						break;
					case 304:
						num3 = 73;
						break;
					case 305:
						num3 = 105;
						break;
					default:
						switch (num4)
						{
						case 65281:
						case 65282:
						case 65283:
						case 65284:
						case 65285:
						case 65286:
						case 65287:
						case 65288:
						case 65289:
						case 65290:
						case 65291:
						case 65292:
						case 65293:
						case 65294:
						case 65295:
						case 65296:
						case 65297:
						case 65298:
						case 65299:
						case 65300:
						case 65301:
						case 65302:
						case 65303:
						case 65304:
						case 65305:
						case 65306:
						case 65307:
						case 65308:
						case 65309:
						case 65310:
							num3 -= 65248;
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
							case 9554:
								num3 = 213;
								break;
							case 9555:
								num3 = 214;
								break;
							case 9556:
								num3 = 201;
								break;
							case 9557:
								num3 = 184;
								break;
							case 9558:
								num3 = 183;
								break;
							case 9559:
								num3 = 187;
								break;
							case 9560:
								num3 = 212;
								break;
							case 9561:
								num3 = 211;
								break;
							case 9562:
								num3 = 200;
								break;
							case 9563:
								num3 = 190;
								break;
							case 9564:
								num3 = 189;
								break;
							case 9565:
								num3 = 188;
								break;
							case 9566:
								num3 = 198;
								break;
							case 9567:
								num3 = 199;
								break;
							case 9568:
								num3 = 204;
								break;
							case 9569:
								num3 = 181;
								break;
							case 9570:
								num3 = 182;
								break;
							case 9571:
								num3 = 185;
								break;
							case 9572:
								num3 = 209;
								break;
							case 9573:
								num3 = 210;
								break;
							case 9574:
								num3 = 203;
								break;
							case 9575:
								num3 = 207;
								break;
							case 9576:
								num3 = 208;
								break;
							case 9577:
								num3 = 202;
								break;
							case 9578:
								num3 = 216;
								break;
							case 9579:
								num3 = 215;
								break;
							case 9580:
								num3 = 206;
								break;
							default:
								switch (num4)
								{
								case 8450:
									num3 = 67;
									break;
								default:
									switch (num4)
									{
									case 8208:
										num3 = 45;
										break;
									case 8209:
										num3 = 45;
										break;
									default:
										switch (num4)
										{
										case 8304:
											num3 = 248;
											break;
										default:
											switch (num4)
											{
											case 8721:
												num3 = 228;
												break;
											case 8722:
												num3 = 45;
												break;
											case 8723:
												num3 = 241;
												break;
											default:
												switch (num4)
												{
												case 768:
													num3 = 96;
													break;
												case 769:
													num3 = 39;
													break;
												case 770:
													num3 = 94;
													break;
												case 771:
													num3 = 126;
													break;
												case 772:
													num3 = 196;
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
														case 12288:
															num3 = 32;
															break;
														default:
															switch (num4)
															{
															case 708:
																num3 = 94;
																break;
															default:
																switch (num4)
																{
																case 928:
																	num3 = 227;
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
																		case 913:
																			num3 = 224;
																			break;
																		default:
																			switch (num4)
																			{
																			case 960:
																				num3 = 227;
																				break;
																			default:
																				switch (num4)
																				{
																				case 8192:
																					num3 = 32;
																					break;
																				case 8193:
																					num3 = 32;
																					break;
																				case 8194:
																					num3 = 32;
																					break;
																				case 8195:
																					num3 = 32;
																					break;
																				case 8196:
																					num3 = 32;
																					break;
																				case 8197:
																					num3 = 32;
																					break;
																				case 8198:
																					num3 = 32;
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
																						case 945:
																							num3 = 224;
																							break;
																						case 946:
																							num3 = 225;
																							break;
																						default:
																							switch (num4)
																							{
																							case 8801:
																								num3 = 240;
																								break;
																							default:
																								switch (num4)
																								{
																								case 697:
																									num3 = 39;
																									break;
																								case 698:
																									num3 = 34;
																									break;
																								default:
																									switch (num4)
																									{
																									case 8249:
																										num3 = 60;
																										break;
																									case 8250:
																										num3 = 62;
																										break;
																									default:
																										switch (num4)
																										{
																										case 8356:
																											num3 = 156;
																											break;
																										default:
																											switch (num4)
																											{
																											case 730:
																												num3 = 248;
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
																													case 9658:
																														num3 = 16;
																														break;
																													default:
																														if (num4 != 817)
																														{
																															if (num4 != 818)
																															{
																																if (num4 != 8810)
																																{
																																	if (num4 != 8811)
																																	{
																																		if (num4 != 8962)
																																		{
																																			if (num4 != 8963)
																																			{
																																				if (num4 != 8992)
																																				{
																																					if (num4 != 8993)
																																					{
																																						if (num4 != 9001)
																																						{
																																							if (num4 != 9002)
																																							{
																																								if (num4 != 9688)
																																								{
																																									if (num4 != 9689)
																																									{
																																										if (num4 != 12314)
																																										{
																																											if (num4 != 12315)
																																											{
																																												if (num4 != 807)
																																												{
																																													if (num4 != 894)
																																													{
																																														if (num4 != 956)
																																														{
																																															if (num4 != 1211)
																																															{
																																																if (num4 != 1417)
																																																{
																																																	if (num4 != 1642)
																																																	{
																																																		if (num4 != 8260)
																																																		{
																																																			if (num4 != 8413)
																																																			{
																																																				if (num4 != 8616)
																																																				{
																																																					if (num4 != 8709)
																																																					{
																																																						if (num4 != 8758)
																																																						{
																																																							if (num4 != 8764)
																																																							{
																																																								if (num4 != 8776)
																																																								{
																																																									if (num4 != 8901)
																																																									{
																																																										if (num4 != 8976)
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
																																																																									if (num4 != 10003)
																																																																									{
																																																																										if (num4 != 10072)
																																																																										{
																																																																											if (num4 != 12539)
																																																																											{
																																																																												base.HandleFallback(ref encoderFallbackBuffer, chars, ref num, ref charCount, bytes, ref num2, ref byteCount);
																																																																											}
																																																																											else
																																																																											{
																																																																												num3 = 250;
																																																																											}
																																																																										}
																																																																										else
																																																																										{
																																																																											num3 = 124;
																																																																										}
																																																																									}
																																																																									else
																																																																									{
																																																																										num3 = 251;
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
																																																											num3 = 169;
																																																										}
																																																									}
																																																									else
																																																									{
																																																										num3 = 250;
																																																									}
																																																								}
																																																								else
																																																								{
																																																									num3 = 247;
																																																								}
																																																							}
																																																							else
																																																							{
																																																								num3 = 126;
																																																							}
																																																						}
																																																						else
																																																						{
																																																							num3 = 58;
																																																						}
																																																					}
																																																					else
																																																					{
																																																						num3 = 237;
																																																					}
																																																				}
																																																				else
																																																				{
																																																					num3 = 23;
																																																				}
																																																			}
																																																			else
																																																			{
																																																				num3 = 9;
																																																			}
																																																		}
																																																		else
																																																		{
																																																			num3 = 47;
																																																		}
																																																	}
																																																	else
																																																	{
																																																		num3 = 37;
																																																	}
																																																}
																																																else
																																																{
																																																	num3 = 58;
																																																}
																																															}
																																															else
																																															{
																																																num3 = 104;
																																															}
																																														}
																																														else
																																														{
																																															num3 = 230;
																																														}
																																													}
																																													else
																																													{
																																														num3 = 59;
																																													}
																																												}
																																												else
																																												{
																																													num3 = 44;
																																												}
																																											}
																																											else
																																											{
																																												num3 = 93;
																																											}
																																										}
																																										else
																																										{
																																											num3 = 91;
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
																																							}
																																							else
																																							{
																																								num3 = 62;
																																							}
																																						}
																																						else
																																						{
																																							num3 = 60;
																																						}
																																					}
																																					else
																																					{
																																						num3 = 245;
																																					}
																																				}
																																				else
																																				{
																																					num3 = 244;
																																				}
																																			}
																																			else
																																			{
																																				num3 = 94;
																																			}
																																		}
																																		else
																																		{
																																			num3 = 127;
																																		}
																																	}
																																	else
																																	{
																																		num3 = 175;
																																	}
																																}
																																else
																																{
																																	num3 = 174;
																																}
																															}
																															else
																															{
																																num3 = 95;
																															}
																														}
																														else
																														{
																															num3 = 95;
																														}
																														break;
																													case 9660:
																														num3 = 31;
																														break;
																													}
																													break;
																												case 9474:
																													num3 = 179;
																													break;
																												}
																												break;
																											case 732:
																												num3 = 126;
																												break;
																											}
																											break;
																										case 8359:
																											num3 = 158;
																											break;
																										}
																										break;
																									case 8252:
																										num3 = 19;
																										break;
																									}
																									break;
																								case 700:
																									num3 = 39;
																									break;
																								}
																								break;
																							case 8804:
																								num3 = 243;
																								break;
																							case 8805:
																								num3 = 242;
																								break;
																							}
																							break;
																						case 948:
																							num3 = 235;
																							break;
																						case 949:
																							num3 = 238;
																							break;
																						}
																						break;
																					}
																					break;
																				}
																				break;
																			case 963:
																				num3 = 229;
																				break;
																			case 964:
																				num3 = 231;
																				break;
																			case 966:
																				num3 = 237;
																				break;
																			}
																			break;
																		case 915:
																			num3 = 226;
																			break;
																		case 916:
																			num3 = 235;
																			break;
																		case 917:
																			num3 = 238;
																			break;
																		case 920:
																			num3 = 233;
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
																case 931:
																	num3 = 228;
																	break;
																case 932:
																	num3 = 231;
																	break;
																case 934:
																	num3 = 232;
																	break;
																case 937:
																	num3 = 234;
																	break;
																}
																break;
															case 710:
																num3 = 94;
																break;
															case 712:
																num3 = 39;
																break;
															case 713:
																num3 = 196;
																break;
															case 714:
																num3 = 39;
																break;
															case 715:
																num3 = 96;
																break;
															case 717:
																num3 = 95;
																break;
															}
															break;
														case 12295:
															num3 = 9;
															break;
														case 12296:
															num3 = 60;
															break;
														case 12297:
															num3 = 62;
															break;
														case 12298:
															num3 = 174;
															break;
														case 12299:
															num3 = 175;
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
													case 9834:
														num3 = 13;
														break;
													case 9835:
														num3 = 14;
														break;
													}
													break;
												case 776:
													num3 = 34;
													break;
												case 778:
													num3 = 248;
													break;
												case 782:
													num3 = 34;
													break;
												}
												break;
											case 8725:
												num3 = 47;
												break;
											case 8726:
												num3 = 92;
												break;
											case 8727:
												num3 = 42;
												break;
											case 8728:
												num3 = 248;
												break;
											case 8729:
												num3 = 249;
												break;
											case 8730:
												num3 = 251;
												break;
											case 8734:
												num3 = 236;
												break;
											case 8735:
												num3 = 28;
												break;
											case 8739:
												num3 = 124;
												break;
											case 8745:
												num3 = 239;
												break;
											}
											break;
										case 8308:
										case 8309:
										case 8310:
										case 8311:
										case 8312:
											num3 -= 8256;
											break;
										case 8319:
											num3 = 252;
											break;
										case 8320:
										case 8321:
										case 8322:
										case 8323:
										case 8324:
										case 8325:
										case 8326:
										case 8327:
										case 8328:
										case 8329:
											num3 -= 8272;
											break;
										}
										break;
									case 8211:
										num3 = 45;
										break;
									case 8212:
										num3 = 45;
										break;
									case 8215:
										num3 = 95;
										break;
									case 8216:
										num3 = 96;
										break;
									case 8217:
										num3 = 39;
										break;
									case 8218:
										num3 = 44;
										break;
									case 8220:
										num3 = 34;
										break;
									case 8221:
										num3 = 34;
										break;
									case 8222:
										num3 = 44;
										break;
									case 8224:
										num3 = 43;
										break;
									case 8225:
										num3 = 216;
										break;
									case 8226:
										num3 = 7;
										break;
									case 8228:
										num3 = 250;
										break;
									case 8230:
										num3 = 46;
										break;
									case 8240:
										num3 = 37;
										break;
									case 8242:
										num3 = 39;
										break;
									case 8245:
										num3 = 96;
										break;
									}
									break;
								case 8455:
									num3 = 69;
									break;
								case 8458:
									num3 = 103;
									break;
								case 8459:
									num3 = 72;
									break;
								case 8460:
									num3 = 72;
									break;
								case 8461:
									num3 = 72;
									break;
								case 8462:
									num3 = 104;
									break;
								case 8464:
									num3 = 73;
									break;
								case 8465:
									num3 = 73;
									break;
								case 8466:
									num3 = 76;
									break;
								case 8467:
									num3 = 108;
									break;
								case 8469:
									num3 = 78;
									break;
								case 8472:
									num3 = 80;
									break;
								case 8473:
									num3 = 80;
									break;
								case 8474:
									num3 = 81;
									break;
								case 8475:
									num3 = 82;
									break;
								case 8476:
									num3 = 82;
									break;
								case 8477:
									num3 = 82;
									break;
								case 8482:
									num3 = 84;
									break;
								case 8484:
									num3 = 90;
									break;
								case 8486:
									num3 = 234;
									break;
								case 8488:
									num3 = 90;
									break;
								case 8490:
									num3 = 75;
									break;
								case 8491:
									num3 = 143;
									break;
								case 8492:
									num3 = 66;
									break;
								case 8493:
									num3 = 67;
									break;
								case 8494:
									num3 = 101;
									break;
								case 8495:
									num3 = 101;
									break;
								case 8496:
									num3 = 69;
									break;
								case 8497:
									num3 = 70;
									break;
								case 8499:
									num3 = 77;
									break;
								case 8500:
									num3 = 111;
									break;
								}
								break;
							case 9600:
								num3 = 223;
								break;
							case 9604:
								num3 = 220;
								break;
							case 9608:
								num3 = 219;
								break;
							case 9612:
								num3 = 221;
								break;
							case 9616:
								num3 = 222;
								break;
							case 9617:
								num3 = 176;
								break;
							case 9618:
								num3 = 177;
								break;
							case 9619:
								num3 = 178;
								break;
							}
							break;
						case 65312:
						case 65313:
						case 65314:
						case 65315:
						case 65316:
						case 65317:
						case 65318:
						case 65319:
						case 65320:
						case 65321:
						case 65322:
						case 65323:
						case 65324:
						case 65325:
						case 65326:
						case 65327:
						case 65328:
						case 65329:
						case 65330:
						case 65331:
						case 65332:
						case 65333:
						case 65334:
						case 65335:
						case 65336:
						case 65337:
						case 65338:
						case 65339:
						case 65340:
						case 65341:
						case 65342:
						case 65343:
						case 65344:
						case 65345:
						case 65346:
						case 65347:
						case 65348:
						case 65349:
						case 65350:
						case 65351:
						case 65352:
						case 65353:
						case 65354:
						case 65355:
						case 65356:
						case 65357:
						case 65358:
						case 65359:
						case 65360:
						case 65361:
						case 65362:
						case 65363:
						case 65364:
						case 65365:
						case 65366:
						case 65367:
						case 65368:
						case 65369:
						case 65370:
						case 65371:
						case 65372:
						case 65373:
						case 65374:
							num3 -= 65248;
							break;
						}
						break;
					case 308:
						num3 = 74;
						break;
					case 309:
						num3 = 106;
						break;
					case 310:
						num3 = 75;
						break;
					case 311:
						num3 = 107;
						break;
					case 313:
						num3 = 76;
						break;
					case 314:
						num3 = 108;
						break;
					case 315:
						num3 = 76;
						break;
					case 316:
						num3 = 108;
						break;
					case 317:
						num3 = 76;
						break;
					case 318:
						num3 = 108;
						break;
					case 321:
						num3 = 76;
						break;
					case 322:
						num3 = 108;
						break;
					case 323:
						num3 = 78;
						break;
					case 324:
						num3 = 110;
						break;
					case 325:
						num3 = 78;
						break;
					case 326:
						num3 = 110;
						break;
					case 327:
						num3 = 78;
						break;
					case 328:
						num3 = 110;
						break;
					case 332:
						num3 = 79;
						break;
					case 333:
						num3 = 111;
						break;
					case 334:
						num3 = 79;
						break;
					case 335:
						num3 = 111;
						break;
					case 336:
						num3 = 79;
						break;
					case 337:
						num3 = 111;
						break;
					case 338:
						num3 = 79;
						break;
					case 339:
						num3 = 111;
						break;
					case 340:
						num3 = 82;
						break;
					case 341:
						num3 = 114;
						break;
					case 342:
						num3 = 82;
						break;
					case 343:
						num3 = 114;
						break;
					case 344:
						num3 = 82;
						break;
					case 345:
						num3 = 114;
						break;
					case 346:
						num3 = 83;
						break;
					case 347:
						num3 = 115;
						break;
					case 348:
						num3 = 83;
						break;
					case 349:
						num3 = 115;
						break;
					case 350:
						num3 = 83;
						break;
					case 351:
						num3 = 115;
						break;
					case 352:
						num3 = 83;
						break;
					case 353:
						num3 = 115;
						break;
					case 354:
						num3 = 84;
						break;
					case 355:
						num3 = 116;
						break;
					case 356:
						num3 = 84;
						break;
					case 357:
						num3 = 116;
						break;
					case 358:
						num3 = 84;
						break;
					case 359:
						num3 = 116;
						break;
					case 360:
						num3 = 85;
						break;
					case 361:
						num3 = 117;
						break;
					case 362:
						num3 = 85;
						break;
					case 363:
						num3 = 117;
						break;
					case 364:
						num3 = 85;
						break;
					case 365:
						num3 = 117;
						break;
					case 366:
						num3 = 85;
						break;
					case 367:
						num3 = 117;
						break;
					case 368:
						num3 = 85;
						break;
					case 369:
						num3 = 117;
						break;
					case 370:
						num3 = 85;
						break;
					case 371:
						num3 = 117;
						break;
					case 372:
						num3 = 87;
						break;
					case 373:
						num3 = 119;
						break;
					case 374:
						num3 = 89;
						break;
					case 375:
						num3 = 121;
						break;
					case 376:
						num3 = 89;
						break;
					case 377:
						num3 = 90;
						break;
					case 378:
						num3 = 122;
						break;
					case 379:
						num3 = 90;
						break;
					case 380:
						num3 = 122;
						break;
					case 381:
						num3 = 90;
						break;
					case 382:
						num3 = 122;
						break;
					case 384:
						num3 = 98;
						break;
					case 393:
						num3 = 68;
						break;
					case 401:
						num3 = 159;
						break;
					case 402:
						num3 = 159;
						break;
					case 407:
						num3 = 73;
						break;
					case 410:
						num3 = 108;
						break;
					case 415:
						num3 = 79;
						break;
					case 416:
						num3 = 79;
						break;
					case 417:
						num3 = 111;
						break;
					case 425:
						num3 = 228;
						break;
					case 427:
						num3 = 116;
						break;
					case 430:
						num3 = 84;
						break;
					case 431:
						num3 = 85;
						break;
					case 432:
						num3 = 117;
						break;
					case 438:
						num3 = 122;
						break;
					case 448:
						num3 = 124;
						break;
					case 451:
						num3 = 33;
						break;
					case 461:
						num3 = 65;
						break;
					case 462:
						num3 = 97;
						break;
					case 463:
						num3 = 73;
						break;
					case 464:
						num3 = 105;
						break;
					case 465:
						num3 = 79;
						break;
					case 466:
						num3 = 111;
						break;
					case 467:
						num3 = 85;
						break;
					case 468:
						num3 = 117;
						break;
					case 469:
						num3 = 85;
						break;
					case 470:
						num3 = 117;
						break;
					case 471:
						num3 = 85;
						break;
					case 472:
						num3 = 117;
						break;
					case 473:
						num3 = 85;
						break;
					case 474:
						num3 = 117;
						break;
					case 475:
						num3 = 85;
						break;
					case 476:
						num3 = 117;
						break;
					case 478:
						num3 = 65;
						break;
					case 479:
						num3 = 97;
						break;
					case 484:
						num3 = 71;
						break;
					case 485:
						num3 = 103;
						break;
					case 486:
						num3 = 71;
						break;
					case 487:
						num3 = 103;
						break;
					case 488:
						num3 = 75;
						break;
					case 489:
						num3 = 107;
						break;
					case 490:
						num3 = 79;
						break;
					case 491:
						num3 = 111;
						break;
					case 492:
						num3 = 79;
						break;
					case 493:
						num3 = 111;
						break;
					case 496:
						num3 = 106;
						break;
					case 609:
						num3 = 103;
						break;
					case 632:
						num3 = 237;
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
			'x', 'y', 'z', '{', '|', '}', '~', '\u007f', 'Ç', 'ü',
			'é', 'â', 'ä', 'à', 'å', 'ç', 'ê', 'ë', 'è', 'ï',
			'î', 'ì', 'Ä', 'Å', 'É', 'æ', 'Æ', 'ô', 'ö', 'ò',
			'û', 'ù', 'ÿ', 'Ö', 'Ü', '¢', '£', '¥', '₧', 'ƒ',
			'á', 'í', 'ó', 'ú', 'ñ', 'Ñ', 'ª', 'º', '¿', '⌐',
			'¬', '½', '¼', '¡', '«', '»', '░', '▒', '▓', '│',
			'┤', '╡', '╢', '╖', '╕', '╣', '║', '╗', '╝', '╜',
			'╛', '┐', '└', '┴', '┬', '├', '─', '┼', '╞', '╟',
			'╚', '╔', '╩', '╦', '╠', '═', '╬', '╧', '╨', '╤',
			'╥', '╙', '╘', '╒', '╓', '╫', '╪', '┘', '┌', '█',
			'▄', '▌', '▐', '▀', 'α', 'ß', 'Γ', 'π', 'Σ', 'σ',
			'µ', 'τ', 'Φ', 'Θ', 'Ω', 'δ', '∞', 'φ', 'ε', '∩',
			'≡', '±', '≥', '≤', '⌠', '⌡', '÷', '≈', '°', '∙',
			'·', '√', 'ⁿ', '²', '■', '\u00a0'
		};
	}
}
