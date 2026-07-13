using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Unity.VectorGraphics
{
	internal class SVGAttribParser
	{
		public static List<BezierContour> ParsePath(XmlReaderIterator.Node node)
		{
			string text = node["d"];
			bool flag = string.IsNullOrEmpty(text);
			List<BezierContour> list;
			if (flag)
			{
				list = null;
			}
			else
			{
				try
				{
					list = new SVGAttribParser(text, SVGAttribParser.AttribPath.Path).contours;
				}
				catch (Exception ex)
				{
					throw node.GetException(ex.Message);
				}
			}
			return list;
		}

		public static Matrix2D ParseTransform(XmlReaderIterator.Node node)
		{
			return SVGAttribParser.ParseTransform(node, "transform");
		}

		public static Matrix2D ParseTransform(XmlReaderIterator.Node node, string attribName)
		{
			string text = node[attribName];
			bool flag = string.IsNullOrEmpty(text);
			Matrix2D identity;
			if (flag)
			{
				identity = Matrix2D.identity;
			}
			else
			{
				try
				{
					identity = new SVGAttribParser(text, attribName, SVGAttribParser.AttribTransform.Transform).transform;
				}
				catch (Exception ex)
				{
					throw node.GetException(ex.Message);
				}
			}
			return identity;
		}

		public static IFill ParseFill(XmlReaderIterator.Node node, SVGDictionary dict, SVGPostponedFills postponedFills, SVGStyleResolver styles, Inheritance inheritance = Inheritance.Inherited)
		{
			bool flag;
			return SVGAttribParser.ParseFill(node, dict, postponedFills, styles, inheritance, out flag);
		}

		public static IFill ParseFill(XmlReaderIterator.Node node, SVGDictionary dict, SVGPostponedFills postponedFills, SVGStyleResolver styles, Inheritance inheritance, out bool isDefaultFill)
		{
			string text = styles.Evaluate("fill-opacity", inheritance);
			float num = ((text != null) ? SVGAttribParser.ParseFloat(text) : 1f);
			string text2 = styles.Evaluate("fill-rule", inheritance);
			FillMode fillMode = FillMode.NonZero;
			bool flag = text2 != null;
			if (flag)
			{
				bool flag2 = text2 == "nonzero";
				if (flag2)
				{
					fillMode = FillMode.NonZero;
				}
				else
				{
					bool flag3 = text2 == "evenodd";
					if (!flag3)
					{
						throw new Exception("Unknown fill-rule: " + text2);
					}
					fillMode = FillMode.OddEven;
				}
			}
			IFill fill;
			try
			{
				string text3 = styles.Evaluate("fill", inheritance);
				isDefaultFill = text3 == null && text == null;
				fill = new SVGAttribParser(text3, "fill", num, fillMode, dict, postponedFills, true).fill;
			}
			catch (Exception ex)
			{
				throw node.GetException(ex.Message);
			}
			return fill;
		}

		public static Stroke ParseStrokeAndOpacity(XmlReaderIterator.Node node, SVGDictionary dict, SVGStyleResolver styles, Inheritance inheritance = Inheritance.Inherited)
		{
			string text = styles.Evaluate("stroke", inheritance);
			bool flag = string.IsNullOrEmpty(text);
			Stroke stroke;
			if (flag)
			{
				stroke = null;
			}
			else
			{
				string text2 = styles.Evaluate("stroke-opacity", inheritance);
				float num = ((text2 != null) ? SVGAttribParser.ParseFloat(text2) : 1f);
				IFill fill = null;
				try
				{
					fill = new SVGAttribParser(text, "stroke", num, FillMode.NonZero, dict, null, true).fill;
				}
				catch (Exception ex)
				{
					throw node.GetException(ex.Message);
				}
				bool flag2 = fill == null;
				if (flag2)
				{
					stroke = null;
				}
				else
				{
					stroke = new Stroke
					{
						Fill = fill
					};
				}
			}
			return stroke;
		}

		public static Color ParseColor(string colorString)
		{
			bool flag = colorString[0] == '#';
			Color color;
			if (flag)
			{
				uint num = uint.Parse(colorString.Substring(1), NumberStyles.HexNumber);
				bool flag2 = colorString.Length == 4;
				if (flag2)
				{
					color = new Color((((num >> 8) & 15U) | (((num >> 8) & 15U) << 4)) / 255f, (((num >> 4) & 15U) | (((num >> 4) & 15U) << 4)) / 255f, ((num & 15U) | ((num & 15U) << 4)) / 255f);
				}
				else
				{
					color = new Color(((num >> 16) & 255U) / 255f, ((num >> 8) & 255U) / 255f, (num & 255U) / 255f);
				}
			}
			else
			{
				bool flag3 = colorString.StartsWith("rgb(") && colorString.EndsWith(")");
				if (flag3)
				{
					string text = colorString.Substring(4, colorString.Length - 5);
					string[] array = text.Split(new char[] { ',', '%' }, StringSplitOptions.RemoveEmptyEntries);
					bool flag4 = array.Length != 3;
					if (flag4)
					{
						throw new Exception("Invalid rgb() color specification");
					}
					float num2 = (colorString.Contains("%") ? 100f : 255f);
					color = new Color((float)byte.Parse(array[0]) / num2, (float)byte.Parse(array[1]) / num2, (float)byte.Parse(array[2]) / num2);
				}
				else
				{
					bool flag5 = colorString.StartsWith("rgba(") && colorString.EndsWith(")");
					if (flag5)
					{
						string text2 = colorString.Substring(5, colorString.Length - 6);
						string[] array2 = text2.Split(new char[] { ',', '%' }, StringSplitOptions.RemoveEmptyEntries);
						bool flag6 = array2.Length != 4;
						if (flag6)
						{
							throw new Exception("Invalid rgba() color specification");
						}
						float num3 = (colorString.Contains("%") ? 100f : 255f);
						color = new Color((float)byte.Parse(array2[0]) / num3, (float)byte.Parse(array2[1]) / num3, (float)byte.Parse(array2[2]) / num3, (num3 == 100f) ? ((float)byte.Parse(array2[3]) / num3) : SVGAttribParser.ParseFloat(array2[3]));
					}
					else
					{
						bool flag7 = colorString.StartsWith("hsl(") && colorString.EndsWith(")");
						if (flag7)
						{
							string text3 = colorString.Substring(4, colorString.Length - 5);
							string[] array3 = text3.Split(new char[] { ',', '%' }, StringSplitOptions.RemoveEmptyEntries);
							bool flag8 = array3.Length != 3;
							if (flag8)
							{
								throw new Exception("Invalid hsl() color specification");
							}
							float num4 = SVGAttribParser.ParseFloat(array3[0]) / 360f;
							float num5 = SVGAttribParser.ParseFloat(array3[1]) / 100f;
							float num6 = SVGAttribParser.ParseFloat(array3[2]) / 100f;
							color = SVGAttribParser.HSLToRGB(num4, num5, num6);
						}
						else
						{
							bool flag9 = SVGAttribParser.namedColors == null;
							if (flag9)
							{
								SVGAttribParser.namedColors = new NamedWebColorDictionary();
							}
							color = SVGAttribParser.namedColors[colorString.ToLower()];
						}
					}
				}
			}
			return color;
		}

		private static float HueToValue(float p, float q, float t)
		{
			bool flag = t < 0f;
			if (flag)
			{
				t += 1f;
			}
			bool flag2 = t > 1f;
			if (flag2)
			{
				t -= 1f;
			}
			bool flag3 = t < 0.16666667f;
			float num;
			if (flag3)
			{
				num = p + (q - p) * 6f * t;
			}
			else
			{
				bool flag4 = t < 0.5f;
				if (flag4)
				{
					num = q;
				}
				else
				{
					bool flag5 = t < 0.6666667f;
					if (flag5)
					{
						num = p + (q - p) * (0.6666667f - t) * 6f;
					}
					else
					{
						num = p;
					}
				}
			}
			return num;
		}

		private static Color HSLToRGB(float hue, float saturation, float lightness)
		{
			float num = ((lightness < 0.5f) ? (lightness * (1f + saturation)) : (lightness + saturation - lightness * saturation));
			float num2 = 2f * lightness - num;
			float num3 = SVGAttribParser.HueToValue(num2, num, hue + 0.33333334f);
			float num4 = SVGAttribParser.HueToValue(num2, num, hue);
			float num5 = SVGAttribParser.HueToValue(num2, num, hue - 0.33333334f);
			return new Color(num3, num4, num5);
		}

		public static string ParseURLRef(string url)
		{
			bool flag = url.StartsWith("url(") && url.EndsWith(")");
			string text;
			if (flag)
			{
				text = url.Substring(4, url.Length - 5);
			}
			else
			{
				text = null;
			}
			return text;
		}

		public static object ParseRelativeRef(string iri, SVGDictionary dict)
		{
			bool flag = iri == null;
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				bool flag2 = !iri.StartsWith("#");
				if (flag2)
				{
					throw new Exception("Unsupported reference type (" + iri + ")");
				}
				iri = iri.Substring(1);
				object obj2;
				dict.TryGetValue(iri, out obj2);
				obj = obj2;
			}
			return obj;
		}

		public static string CleanIri(string iri)
		{
			bool flag = iri == null;
			string text;
			if (flag)
			{
				text = null;
			}
			else
			{
				bool flag2 = !iri.StartsWith("#");
				if (flag2)
				{
					throw new Exception("Unsupported reference type (" + iri + ")");
				}
				iri = iri.Substring(1);
				text = iri;
			}
			return text;
		}

		private SVGAttribParser(string attrib, SVGAttribParser.AttribPath attribPath)
		{
			this.attribName = "path";
			this.attribString = attrib;
			this.NextPathCommand(true);
			bool flag = this.pathCommand != 'm' && this.pathCommand != 'M';
			if (flag)
			{
				throw new Exception("Path must start with a MoveTo pathCommand");
			}
			char c = '\0';
			Vector2 vector = Vector2.zero;
			while (this.NextPathCommand(false) > '\0')
			{
				bool flag2 = this.pathCommand >= 'a' && this.pathCommand <= 'z';
				char c2 = char.ToLower(this.pathCommand);
				bool flag3 = c2 == 'm';
				if (flag3)
				{
					this.penPos = this.NextVector2(flag2);
					this.pathCommand = (flag2 ? 'l' : 'L');
					this.ConcludePath(false);
				}
				else
				{
					bool flag4 = c2 == 'z';
					if (flag4)
					{
						bool flag5 = this.currentContour.First != null;
						if (flag5)
						{
							this.penPos = this.currentContour.First.Value.P0;
						}
						this.ConcludePath(true);
					}
					else
					{
						bool flag6 = c2 == 'l';
						if (flag6)
						{
							Vector2 vector2 = this.NextVector2(flag2);
							bool flag7 = (vector2 - this.penPos).magnitude > VectorUtils.Epsilon;
							if (flag7)
							{
								this.currentContour.AddLast(VectorUtils.MakeLine(this.penPos, vector2));
							}
							this.penPos = vector2;
						}
						else
						{
							bool flag8 = c2 == 'h';
							if (flag8)
							{
								float num = (flag2 ? (this.penPos.x + this.NextFloat()) : this.NextFloat());
								Vector2 vector3 = new Vector2(num, this.penPos.y);
								bool flag9 = (vector3 - this.penPos).magnitude > VectorUtils.Epsilon;
								if (flag9)
								{
									this.currentContour.AddLast(VectorUtils.MakeLine(this.penPos, vector3));
								}
								this.penPos = vector3;
							}
							else
							{
								bool flag10 = c2 == 'v';
								if (flag10)
								{
									float num2 = (flag2 ? (this.penPos.y + this.NextFloat()) : this.NextFloat());
									Vector2 vector4 = new Vector2(this.penPos.x, num2);
									bool flag11 = (vector4 - this.penPos).magnitude > VectorUtils.Epsilon;
									if (flag11)
									{
										this.currentContour.AddLast(VectorUtils.MakeLine(this.penPos, vector4));
									}
									this.penPos = vector4;
								}
								else
								{
									bool flag12 = c2 == 'c' || c2 == 'q';
									if (flag12)
									{
										BezierSegment bezierSegment = default(BezierSegment);
										bezierSegment.P0 = this.penPos;
										bezierSegment.P1 = this.NextVector2(flag2);
										bool flag13 = c2 == 'c';
										if (flag13)
										{
											bezierSegment.P2 = this.NextVector2(flag2);
										}
										bezierSegment.P3 = this.NextVector2(flag2);
										bool flag14 = c2 == 'q';
										if (flag14)
										{
											vector = bezierSegment.P1;
											float num3 = 0.6666667f;
											bezierSegment.P1 = bezierSegment.P0 + num3 * (vector - bezierSegment.P0);
											bezierSegment.P2 = bezierSegment.P3 + num3 * (vector - bezierSegment.P3);
										}
										this.penPos = bezierSegment.P3;
										bool flag15 = !VectorUtils.IsEmptySegment(bezierSegment);
										if (flag15)
										{
											this.currentContour.AddLast(bezierSegment);
										}
									}
									else
									{
										bool flag16 = c2 == 's' || c2 == 't';
										if (flag16)
										{
											Vector2 vector5 = this.penPos;
											bool flag17 = this.currentContour.Count > 0 && (c == 'c' || c == 'q' || c == 's' || c == 't');
											if (flag17)
											{
												vector5 += this.currentContour.Last.Value.P3 - ((c == 'q' || c == 't') ? vector : this.currentContour.Last.Value.P2);
											}
											BezierSegment bezierSegment2 = default(BezierSegment);
											bezierSegment2.P0 = this.penPos;
											bezierSegment2.P1 = vector5;
											bool flag18 = c2 == 's';
											if (flag18)
											{
												bezierSegment2.P2 = this.NextVector2(flag2);
											}
											bezierSegment2.P3 = this.NextVector2(flag2);
											bool flag19 = c2 == 't';
											if (flag19)
											{
												vector = bezierSegment2.P1;
												float num4 = 0.6666667f;
												bezierSegment2.P1 = bezierSegment2.P0 + num4 * (vector - bezierSegment2.P0);
												bezierSegment2.P2 = bezierSegment2.P3 + num4 * (vector - bezierSegment2.P3);
											}
											this.penPos = bezierSegment2.P3;
											bool flag20 = !VectorUtils.IsEmptySegment(bezierSegment2);
											if (flag20)
											{
												this.currentContour.AddLast(bezierSegment2);
											}
										}
										else
										{
											bool flag21 = c2 == 'a';
											if (flag21)
											{
												Vector2 vector6 = this.NextVector2(false);
												float num5 = this.NextFloat();
												bool flag22 = this.NextBool();
												bool flag23 = this.NextBool();
												Vector2 vector7 = this.NextVector2(flag2);
												bool flag24 = vector6.magnitude <= VectorUtils.Epsilon;
												if (flag24)
												{
													bool flag25 = (vector7 - this.penPos).magnitude > VectorUtils.Epsilon;
													if (flag25)
													{
														this.currentContour.AddLast(VectorUtils.MakeLine(this.penPos, vector7));
													}
												}
												else
												{
													BezierPathSegment[] array = VectorUtils.BuildEllipsePath(this.penPos, vector7, -num5 * 0.017453292f, vector6.x, vector6.y, flag22, flag23);
													foreach (BezierSegment bezierSegment3 in VectorUtils.SegmentsInPath(array, false))
													{
														this.currentContour.AddLast(bezierSegment3);
													}
												}
												this.penPos = vector7;
											}
										}
									}
								}
							}
						}
					}
				}
				c = c2;
			}
			this.ConcludePath(false);
		}

		private SVGAttribParser(string attrib, string attribNameVal, SVGAttribParser.AttribTransform attribTransform)
		{
			this.attribString = attrib;
			this.attribName = attribNameVal;
			this.transform = Matrix2D.identity;
			while (this.stringPos < this.attribString.Length)
			{
				int num = this.stringPos;
				string text = this.NextStringCommand();
				bool flag = string.IsNullOrEmpty(text);
				if (flag)
				{
					break;
				}
				this.SkipSymbol('(');
				bool flag2 = text == "matrix";
				if (flag2)
				{
					Matrix2D matrix2D = default(Matrix2D);
					matrix2D.m00 = this.NextFloat();
					matrix2D.m10 = this.NextFloat();
					matrix2D.m01 = this.NextFloat();
					matrix2D.m11 = this.NextFloat();
					matrix2D.m02 = this.NextFloat();
					matrix2D.m12 = this.NextFloat();
					this.transform *= matrix2D;
				}
				else
				{
					bool flag3 = text == "translate";
					if (flag3)
					{
						float num2 = this.NextFloat();
						float num3 = 0f;
						bool flag4 = !this.PeekSymbol(')');
						if (flag4)
						{
							num3 = this.NextFloat();
						}
						this.transform *= Matrix2D.Translate(new Vector2(num2, num3));
					}
					else
					{
						bool flag5 = text == "scale";
						if (flag5)
						{
							float num4 = this.NextFloat();
							float num5 = num4;
							bool flag6 = !this.PeekSymbol(')');
							if (flag6)
							{
								num5 = this.NextFloat();
							}
							this.transform *= Matrix2D.Scale(new Vector2(num4, num5));
						}
						else
						{
							bool flag7 = text == "rotate";
							if (flag7)
							{
								float num6 = this.NextFloat() * 0.017453292f;
								float num7 = 0f;
								float num8 = 0f;
								bool flag8 = !this.PeekSymbol(')');
								if (flag8)
								{
									num7 = this.NextFloat();
									num8 = this.NextFloat();
								}
								this.transform *= Matrix2D.Translate(new Vector2(num7, num8)) * Matrix2D.RotateLH(-num6) * Matrix2D.Translate(new Vector2(-num7, -num8));
							}
							else
							{
								bool flag9 = text == "skewX" || text == "skewY";
								if (!flag9)
								{
									throw new Exception("Unknown transform command at " + num.ToString() + " in trasform specification");
								}
								float num9 = Mathf.Tan(this.NextFloat() * 0.017453292f);
								Matrix2D identity = Matrix2D.identity;
								bool flag10 = text == "skewY";
								if (flag10)
								{
									identity.m10 = num9;
								}
								else
								{
									identity.m01 = num9;
								}
								this.transform *= identity;
							}
						}
					}
				}
				this.SkipSymbol(')');
			}
		}

		private SVGAttribParser(string attrib, string attribName, float opacity, FillMode mode, SVGDictionary dict, SVGPostponedFills postponedFills, bool allowReference = true)
		{
			this.attribName = attribName;
			bool flag = string.IsNullOrEmpty(attrib);
			if (flag)
			{
				bool flag2 = opacity < 1f;
				if (flag2)
				{
					this.fill = new SolidFill
					{
						Color = new Color(0f, 0f, 0f, opacity)
					};
				}
				else
				{
					this.fill = dict[(mode == FillMode.NonZero) ? SVGDocument.StockBlackNonZeroFillName : SVGDocument.StockBlackOddEvenFillName] as IFill;
				}
			}
			else
			{
				bool flag3 = attrib == "none" || attrib == "transparent";
				if (!flag3)
				{
					bool flag4 = attrib == "currentColor";
					if (flag4)
					{
						Debug.LogError("currentColor is not supported as a " + attribName + " value");
					}
					else
					{
						string[] array = attrib.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						if (allowReference)
						{
							string text = SVGAttribParser.ParseURLRef(array[0]);
							bool flag5 = text != null;
							if (flag5)
							{
								this.fill = SVGAttribParser.ParseRelativeRef(text, dict) as IFill;
								bool flag6 = this.fill == null;
								if (flag6)
								{
									bool flag7 = array.Length > 1;
									if (flag7)
									{
										this.fill = new SVGAttribParser(array[1], attribName, opacity, mode, dict, postponedFills, false).fill;
									}
									else
									{
										bool flag8 = postponedFills != null;
										if (flag8)
										{
											this.fill = new SolidFill
											{
												Color = Color.clear
											};
											postponedFills[this.fill] = text;
										}
									}
								}
								bool flag9 = this.fill != null;
								if (flag9)
								{
									this.fill.Opacity = opacity;
								}
								return;
							}
						}
						Color color = SVGAttribParser.ParseColor(string.Join("", array));
						color.a *= opacity;
						bool flag10 = array.Length > 1;
						if (flag10)
						{
						}
						this.fill = new SolidFill
						{
							Color = color,
							Mode = mode
						};
					}
				}
			}
		}

		private void ConcludePath(bool joinEnds)
		{
			bool flag = this.currentContour.Count > 0;
			if (flag)
			{
				BezierContour bezierContour = default(BezierContour);
				bezierContour.Closed = joinEnds && this.currentContour.Count >= 1;
				bezierContour.Segments = new BezierPathSegment[this.currentContour.Count + 1];
				int num = 0;
				foreach (BezierSegment bezierSegment in this.currentContour)
				{
					bezierContour.Segments[num++] = new BezierPathSegment
					{
						P0 = bezierSegment.P0,
						P1 = bezierSegment.P1,
						P2 = bezierSegment.P2
					};
				}
				BezierSegment bezierSegment2 = VectorUtils.MakeLine(this.currentContour.Last.Value.P3, bezierContour.Segments[0].P0);
				bezierContour.Segments[num] = new BezierPathSegment
				{
					P0 = bezierSegment2.P0,
					P1 = bezierSegment2.P1,
					P2 = bezierSegment2.P2
				};
				this.contours.Add(bezierContour);
			}
			this.currentContour.Clear();
		}

		private Vector2 NextVector2(bool relative = false)
		{
			Vector2 vector = new Vector2(this.NextFloat(), this.NextFloat());
			return relative ? (vector + this.penPos) : vector;
		}

		private float NextFloat()
		{
			this.SkipWhitespaces();
			bool flag = this.stringPos >= this.attribString.Length;
			if (flag)
			{
				throw new Exception(this.attribName + " specification ended before sufficing numbers required by the last pathCommand");
			}
			int num = this.stringPos;
			bool flag2 = this.attribString[this.stringPos] == '-' || this.attribString[this.stringPos] == '+';
			if (flag2)
			{
				this.stringPos++;
			}
			bool flag3 = false;
			bool flag4 = false;
			while (this.stringPos < this.attribString.Length)
			{
				char c = this.attribString[this.stringPos];
				bool flag5 = !flag3 && c == '.';
				if (flag5)
				{
					flag3 = true;
					this.stringPos++;
				}
				else
				{
					bool flag6 = !flag4 && (c == 'e' || c == 'E');
					if (flag6)
					{
						flag4 = true;
						this.stringPos++;
						bool flag7 = this.stringPos < this.attribString.Length && this.attribString[this.stringPos] == '-';
						if (flag7)
						{
							this.stringPos++;
						}
					}
					else
					{
						bool flag8 = !char.IsDigit(c);
						if (flag8)
						{
							break;
						}
						this.stringPos++;
					}
				}
			}
			bool flag9 = this.stringPos - num == 0 || (this.stringPos - num == 1 && this.attribString[num] == '-');
			if (flag9)
			{
				throw new Exception(string.Concat(new string[]
				{
					"Missing number at ",
					num.ToString(),
					" in ",
					this.attribName,
					" specification"
				}));
			}
			return SVGAttribParser.ParseFloat(this.attribString.Substring(num, this.stringPos - num));
		}

		internal static float ParseFloat(string s)
		{
			return float.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture);
		}

		private bool NextBool()
		{
			bool flag = false;
			bool flag2 = false;
			this.SkipWhitespaces();
			bool flag3 = this.stringPos < this.attribString.Length;
			if (flag3)
			{
				char c = this.attribString[this.stringPos];
				this.stringPos++;
				bool flag4 = c != '0' && c != '1';
				if (flag4)
				{
					flag2 = true;
				}
				else
				{
					flag = c == '1';
				}
			}
			else
			{
				flag2 = true;
			}
			bool flag5 = flag2;
			if (flag5)
			{
				throw new Exception(string.Concat(new string[]
				{
					"Expected bool at ",
					this.stringPos.ToString(),
					" of ",
					this.attribName,
					" specification"
				}));
			}
			return flag;
		}

		private char NextPathCommand(bool noCommandInheritance = false)
		{
			this.SkipWhitespaces();
			bool flag = this.stringPos >= this.attribString.Length;
			char c;
			if (flag)
			{
				c = '\0';
			}
			else
			{
				char c2 = this.attribString[this.stringPos];
				bool flag2 = (c2 >= 'a' && c2 <= 'z') || (c2 >= 'A' && c2 <= 'Z');
				if (flag2)
				{
					this.pathCommand = c2;
					this.stringPos++;
					c = c2;
				}
				else
				{
					bool flag3 = !noCommandInheritance && (char.IsDigit(c2) || c2 == '.' || c2 == '-');
					if (!flag3)
					{
						throw new Exception("Unexpected character at " + this.stringPos.ToString() + " in path specification");
					}
					c = this.pathCommand;
				}
			}
			return c;
		}

		private string NextStringCommand()
		{
			this.SkipWhitespaces();
			bool flag = this.stringPos >= this.attribString.Length;
			string text;
			if (flag)
			{
				text = null;
			}
			else
			{
				int num = this.stringPos;
				while (this.stringPos < this.attribString.Length)
				{
					char c = this.attribString[this.stringPos];
					bool flag2 = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
					if (!flag2)
					{
						break;
					}
					this.stringPos++;
				}
				bool flag3 = this.stringPos - num == 0;
				if (flag3)
				{
					throw new Exception(string.Concat(new string[]
					{
						"Unexpected character at ",
						this.stringPos.ToString(),
						" in ",
						this.attribName,
						" specification"
					}));
				}
				text = this.attribString.Substring(num, this.stringPos - num);
			}
			return text;
		}

		private void SkipSymbol(char s)
		{
			this.SkipWhitespaces();
			bool flag = this.stringPos >= this.attribString.Length || this.attribString[this.stringPos] != s;
			if (flag)
			{
				throw new Exception(string.Concat(new string[]
				{
					"Expected ",
					s.ToString(),
					" at ",
					this.stringPos.ToString(),
					" of ",
					this.attribName,
					" specification"
				}));
			}
			this.stringPos++;
		}

		private bool PeekSymbol(char s)
		{
			this.SkipWhitespaces();
			return this.stringPos < this.attribString.Length && this.attribString[this.stringPos] == s;
		}

		private void SkipWhitespaces()
		{
			while (this.stringPos < this.attribString.Length)
			{
				char c = this.attribString[this.stringPos];
				char c2 = c;
				switch (c2)
				{
				case '\t':
				case '\n':
				case '\r':
					break;
				case '\v':
				case '\f':
					goto IL_0055;
				default:
					if (c2 != ' ' && c2 != ',')
					{
						goto IL_0055;
					}
					break;
				}
				this.stringPos++;
				continue;
				IL_0055:
				break;
			}
		}

		private LinkedList<BezierSegment> currentContour = new LinkedList<BezierSegment>();

		private List<BezierContour> contours = new List<BezierContour>();

		private Vector2 penPos;

		private string attribString;

		private char pathCommand;

		private Matrix2D transform;

		private IFill fill;

		private string attribName;

		private int stringPos;

		private static NamedWebColorDictionary namedColors;

		private enum AttribPath
		{
			Path
		}

		private enum AttribTransform
		{
			Transform
		}

		private enum AttribStroke
		{
			Stroke
		}
	}
}
