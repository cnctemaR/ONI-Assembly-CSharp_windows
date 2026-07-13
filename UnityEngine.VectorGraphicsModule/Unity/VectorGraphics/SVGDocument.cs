using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Unity.VectorGraphics
{
	internal class SVGDocument
	{
		public SVGDocument(XmlReader docReader, float dpi, Scene scene, int windowWidth, int windowHeight, bool applyRootViewBox)
		{
			this.allElems = new SVGDocument.ElemHandler[]
			{
				new SVGDocument.ElemHandler(this.circle),
				new SVGDocument.ElemHandler(this.defs),
				new SVGDocument.ElemHandler(this.ellipse),
				new SVGDocument.ElemHandler(this.g),
				new SVGDocument.ElemHandler(this.image),
				new SVGDocument.ElemHandler(this.line),
				new SVGDocument.ElemHandler(this.linearGradient),
				new SVGDocument.ElemHandler(this.path),
				new SVGDocument.ElemHandler(this.polygon),
				new SVGDocument.ElemHandler(this.polyline),
				new SVGDocument.ElemHandler(this.radialGradient),
				new SVGDocument.ElemHandler(this.clipPath),
				new SVGDocument.ElemHandler(this.pattern),
				new SVGDocument.ElemHandler(this.mask),
				new SVGDocument.ElemHandler(this.rect),
				new SVGDocument.ElemHandler(this.symbol),
				new SVGDocument.ElemHandler(this.use),
				new SVGDocument.ElemHandler(this.style)
			};
			this.elemsToAddToHierarchy = new HashSet<SVGDocument.ElemHandler>(new SVGDocument.ElemHandler[]
			{
				new SVGDocument.ElemHandler(this.circle),
				new SVGDocument.ElemHandler(this.ellipse),
				new SVGDocument.ElemHandler(this.g),
				new SVGDocument.ElemHandler(this.image),
				new SVGDocument.ElemHandler(this.line),
				new SVGDocument.ElemHandler(this.path),
				new SVGDocument.ElemHandler(this.polygon),
				new SVGDocument.ElemHandler(this.polyline),
				new SVGDocument.ElemHandler(this.rect),
				new SVGDocument.ElemHandler(this.svg),
				new SVGDocument.ElemHandler(this.use)
			});
			this.docReader = new XmlReaderIterator(docReader);
			this.scene = scene;
			this.dpiScale = dpi / 90f;
			this.windowWidth = windowWidth;
			this.windowHeight = windowHeight;
			this.applyRootViewBox = applyRootViewBox;
			this.svgObjects[SVGDocument.StockBlackNonZeroFillName] = new SolidFill
			{
				Color = new Color(0f, 0f, 0f),
				Mode = FillMode.NonZero
			};
			this.svgObjects[SVGDocument.StockBlackOddEvenFillName] = new SolidFill
			{
				Color = new Color(0f, 0f, 0f),
				Mode = FillMode.OddEven
			};
		}

		public void Import()
		{
			bool flag = this.scene == null;
			if (flag)
			{
				throw new ArgumentNullException();
			}
			bool flag2 = !this.docReader.GoToRoot("svg");
			if (flag2)
			{
				throw new SVGFormatException("Document doesn't have 'svg' root");
			}
			this.currentContainerSize.Push(new Vector2((float)this.windowWidth, (float)this.windowHeight));
			this.svg();
			this.currentContainerSize.Pop();
			bool flag3 = this.currentContainerSize.Count > 0;
			if (flag3)
			{
				throw SVGFormatException.StackError;
			}
			this.PostProcess(this.scene.Root);
			this.RemoveInvisibleNodes();
		}

		public Dictionary<SceneNode, float> NodeOpacities
		{
			get
			{
				return this.nodeOpacity;
			}
		}

		public Dictionary<string, SceneNode> NodeIDs
		{
			get
			{
				return this.nodeIDs;
			}
		}

		internal static string StockBlackNonZeroFillName
		{
			get
			{
				return "unity_internal_black_nz";
			}
		}

		internal static string StockBlackOddEvenFillName
		{
			get
			{
				return "unity_internal_black_oe";
			}
		}

		private void ParseChildren(XmlReaderIterator.Node node, string nodeName)
		{
			SceneNode sceneNode = this.currentSceneNode.Peek();
			SVGDocument.Handlers handlers = this.subTags[nodeName];
			while (this.docReader.GoToNextChild(node))
			{
				XmlReaderIterator.Node node2 = this.docReader.VisitCurrent();
				SVGDocument.ElemHandler elemHandler;
				bool flag = !handlers.TryGetValue(node2.Name, out elemHandler);
				if (flag)
				{
					this.docReader.SkipCurrentChildTree(node2);
				}
				else
				{
					bool flag2 = this.elemsToAddToHierarchy.Contains(elemHandler);
					SceneNode sceneNode2 = null;
					bool flag3 = flag2;
					if (flag3)
					{
						bool flag4 = sceneNode.Children == null;
						if (flag4)
						{
							sceneNode.Children = new List<SceneNode>();
						}
						sceneNode2 = new SceneNode();
						this.nodeGlobalSceneState[sceneNode2] = new SVGDocument.NodeGlobalSceneState
						{
							ContainerSize = this.currentContainerSize.Peek()
						};
						sceneNode.Children.Add(sceneNode2);
						this.currentSceneNode.Push(sceneNode2);
					}
					this.styles.PushNode(node2);
					bool flag5 = sceneNode2 != null;
					if (flag5)
					{
						this.styles.SaveLayerForSceneNode(sceneNode2);
						bool flag6 = this.styles.Evaluate("display", Inheritance.None) == "none";
						if (flag6)
						{
							this.invisibleNodes.Add(new SVGDocument.NodeWithParent
							{
								node = sceneNode2,
								parent = sceneNode
							});
						}
					}
					elemHandler();
					this.ParseChildren(node2, node2.Name);
					this.styles.PopNode();
					bool flag7 = flag2 && this.currentSceneNode.Pop() != sceneNode2;
					if (flag7)
					{
						throw SVGFormatException.StackError;
					}
				}
			}
		}

		private void circle()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			IFill fill = SVGAttribParser.ParseFill(node, this.svgObjects, this.postponedFills, this.styles, Inheritance.Inherited);
			PathCorner pathCorner;
			PathEnding pathEnding;
			Stroke stroke = this.ParseStrokeAttributeSet(node, out pathCorner, out pathEnding, Inheritance.Inherited);
			float num = this.AttribLengthVal(node, "cx", 0f, SVGDocument.DimType.Width);
			float num2 = this.AttribLengthVal(node, "cy", 0f, SVGDocument.DimType.Height);
			float num3 = this.AttribLengthVal(node, "r", 0f, SVGDocument.DimType.Length);
			Shape shape = new Shape();
			VectorUtils.MakeCircleShape(shape, new Vector2(num, num2), num3);
			shape.PathProps = new PathProperties
			{
				Stroke = stroke,
				Head = pathEnding,
				Tail = pathEnding,
				Corners = pathCorner
			};
			shape.Fill = fill;
			sceneNode.Shapes = new List<Shape>(1);
			sceneNode.Shapes.Add(shape);
			this.ParseClipAndMask(node, sceneNode);
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag = this.ShouldDeclareSupportedChildren(node);
			if (flag)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void defs()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = new SceneNode();
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag = this.ShouldDeclareSupportedChildren(node);
			if (flag)
			{
				this.SupportElems(node, this.allElems);
			}
			this.currentSceneNode.Push(sceneNode);
			this.ParseChildren(node, node.Name);
			bool flag2 = this.currentSceneNode.Pop() != sceneNode;
			if (flag2)
			{
				throw SVGFormatException.StackError;
			}
		}

		private void ellipse()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			IFill fill = SVGAttribParser.ParseFill(node, this.svgObjects, this.postponedFills, this.styles, Inheritance.Inherited);
			PathCorner pathCorner;
			PathEnding pathEnding;
			Stroke stroke = this.ParseStrokeAttributeSet(node, out pathCorner, out pathEnding, Inheritance.Inherited);
			float num = this.AttribLengthVal(node, "cx", 0f, SVGDocument.DimType.Width);
			float num2 = this.AttribLengthVal(node, "cy", 0f, SVGDocument.DimType.Height);
			float num3 = this.AttribLengthVal(node, "rx", 0f, SVGDocument.DimType.Length);
			float num4 = this.AttribLengthVal(node, "ry", 0f, SVGDocument.DimType.Length);
			Shape shape = new Shape();
			VectorUtils.MakeEllipseShape(shape, new Vector2(num, num2), num3, num4);
			shape.PathProps = new PathProperties
			{
				Stroke = stroke,
				Corners = pathCorner,
				Head = pathEnding,
				Tail = pathEnding
			};
			shape.Fill = fill;
			sceneNode.Shapes = new List<Shape>(1);
			sceneNode.Shapes.Add(shape);
			this.ParseClipAndMask(node, sceneNode);
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag = this.ShouldDeclareSupportedChildren(node);
			if (flag)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void g()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			this.ParseClipAndMask(node, sceneNode);
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag = this.ShouldDeclareSupportedChildren(node);
			if (flag)
			{
				this.SupportElems(node, this.allElems);
			}
		}

		private void image()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			string text = node["xlink:href"];
			bool flag = text != null;
			if (flag)
			{
				TextureFill textureFill = new TextureFill();
				textureFill.Mode = FillMode.NonZero;
				textureFill.Addressing = AddressMode.Clamp;
				string text2 = text.ToLower();
				bool flag2 = text2.StartsWith("data:");
				if (flag2)
				{
					textureFill.Texture = this.DecodeTextureData(text);
				}
				else
				{
					Debug.LogWarning("Unsupported URL scheme for <image>: " + text);
				}
				bool flag3 = textureFill.Texture != null;
				if (flag3)
				{
					this.ParseID(node, sceneNode);
					this.ParseOpacity(sceneNode);
					sceneNode.Transform = SVGAttribParser.ParseTransform(node);
					Rect rect = this.ParseViewport(node, sceneNode, this.currentContainerSize.Peek());
					sceneNode.Transform *= Matrix2D.Translate(rect.position);
					SVGDocument.ViewBoxInfo viewBoxInfo = default(SVGDocument.ViewBoxInfo);
					viewBoxInfo.ViewBox = new Rect(0f, 0f, (float)textureFill.Texture.width, (float)textureFill.Texture.height);
					this.ParseViewBoxAspectRatio(node, ref viewBoxInfo);
					this.ApplyViewBox(sceneNode, viewBoxInfo, rect);
					Shape shape = new Shape();
					VectorUtils.MakeRectangleShape(shape, new Rect(0f, 0f, (float)textureFill.Texture.width, (float)textureFill.Texture.height));
					shape.Fill = textureFill;
					sceneNode.Shapes = new List<Shape>(1);
					sceneNode.Shapes.Add(shape);
					this.ParseClipAndMask(node, sceneNode);
				}
			}
			string text3 = node["id"];
			bool flag4 = !string.IsNullOrEmpty(text3);
			if (flag4)
			{
				List<SVGDocument.NodeReferenceData> list;
				bool flag5 = this.postponedSymbolData.TryGetValue(text3, out list);
				if (flag5)
				{
					foreach (SVGDocument.NodeReferenceData nodeReferenceData in list)
					{
						this.ResolveReferencedNode(sceneNode, nodeReferenceData, true);
					}
				}
			}
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag6 = this.ShouldDeclareSupportedChildren(node);
			if (flag6)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void line()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			PathCorner pathCorner;
			PathEnding pathEnding;
			Stroke stroke = this.ParseStrokeAttributeSet(node, out pathCorner, out pathEnding, Inheritance.Inherited);
			float num = this.AttribLengthVal(node, "x1", 0f, SVGDocument.DimType.Width);
			float num2 = this.AttribLengthVal(node, "y1", 0f, SVGDocument.DimType.Height);
			float num3 = this.AttribLengthVal(node, "x2", 0f, SVGDocument.DimType.Width);
			float num4 = this.AttribLengthVal(node, "y2", 0f, SVGDocument.DimType.Height);
			Shape shape = new Shape();
			shape.PathProps = new PathProperties
			{
				Stroke = stroke,
				Head = pathEnding,
				Tail = pathEnding
			};
			shape.Contours = new BezierContour[]
			{
				new BezierContour
				{
					Segments = VectorUtils.BezierSegmentToPath(VectorUtils.MakeLine(new Vector2(num, num2), new Vector2(num3, num4)))
				}
			};
			sceneNode.Shapes = new List<Shape>(1);
			sceneNode.Shapes.Add(shape);
			this.ParseClipAndMask(node, sceneNode);
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag = this.ShouldDeclareSupportedChildren(node);
			if (flag)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void linearGradient()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			string text = node["xlink:href"];
			GradientFill gradientFill = SVGAttribParser.ParseRelativeRef(text, this.svgObjects) as GradientFill;
			SVGDocument.LinearGradientExData linearGradientExData = ((gradientFill != null) ? (this.gradientExInfo[gradientFill] as SVGDocument.LinearGradientExData) : null);
			bool flag = linearGradientExData != null && linearGradientExData.WorldRelative;
			string text2 = node["gradientUnits"];
			string text3 = text2;
			if (text3 != null)
			{
				if (!(text3 == "objectBoundingBox"))
				{
					if (!(text3 == "userSpaceOnUse"))
					{
						throw node.GetUnsupportedAttribValException("gradientUnits");
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			AddressMode addressMode = ((gradientFill != null) ? gradientFill.Addressing : AddressMode.Clamp);
			string text4 = node["spreadMethod"];
			string text5 = text4;
			if (text5 != null)
			{
				if (!(text5 == "pad"))
				{
					if (!(text5 == "reflect"))
					{
						if (!(text5 == "repeat"))
						{
							throw node.GetUnsupportedAttribValException("spreadMethod");
						}
						addressMode = AddressMode.Wrap;
					}
					else
					{
						addressMode = AddressMode.Mirror;
					}
				}
				else
				{
					addressMode = AddressMode.Clamp;
				}
			}
			Matrix2D matrix2D = SVGAttribParser.ParseTransform(node, "gradientTransform");
			GradientFill gradientFill2 = this.CloneGradientFill(gradientFill);
			bool flag2 = gradientFill2 == null;
			if (flag2)
			{
				gradientFill2 = new GradientFill
				{
					Addressing = addressMode,
					Type = GradientFillType.Linear
				};
			}
			gradientFill2.Type = GradientFillType.Linear;
			SVGDocument.LinearGradientExData linearGradientExData2 = new SVGDocument.LinearGradientExData
			{
				WorldRelative = flag,
				FillTransform = matrix2D
			};
			this.gradientExInfo[gradientFill2] = linearGradientExData2;
			this.currentContainerSize.Push(Vector2.one);
			linearGradientExData2.X1 = node["x1"];
			linearGradientExData2.Y1 = node["y1"];
			linearGradientExData2.X2 = node["x2"];
			linearGradientExData2.Y2 = node["y2"];
			this.AttribLengthVal(linearGradientExData2.X1, node, "x1", 0f, SVGDocument.DimType.Width);
			this.AttribLengthVal(linearGradientExData2.Y1, node, "y1", 0f, SVGDocument.DimType.Height);
			this.AttribLengthVal(linearGradientExData2.X2, node, "x2", 1f, SVGDocument.DimType.Width);
			this.AttribLengthVal(linearGradientExData2.Y2, node, "y2", 0f, SVGDocument.DimType.Height);
			this.currentContainerSize.Pop();
			this.currentGradientFill = gradientFill2;
			this.currentGradientId = node["id"];
			this.currentGradientLink = SVGAttribParser.CleanIri(text);
			bool flag3 = !string.IsNullOrEmpty(text) && !this.svgObjects.ContainsKey(text);
			if (flag3)
			{
				bool flag4 = !this.postponedStopData.ContainsKey(this.currentGradientLink);
				if (flag4)
				{
					this.postponedStopData.Add(this.currentGradientLink, new List<SVGDocument.PostponedStopData>());
				}
				this.postponedStopData[this.currentGradientLink].Add(new SVGDocument.PostponedStopData
				{
					fill = gradientFill2
				});
			}
			this.AddToSVGDictionaryIfPossible(node, gradientFill2);
			bool flag5 = this.ShouldDeclareSupportedChildren(node);
			if (flag5)
			{
				this.SupportElems(node, new SVGDocument.ElemHandler[]
				{
					new SVGDocument.ElemHandler(this.stop)
				});
			}
		}

		private void path()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			IFill fill = SVGAttribParser.ParseFill(node, this.svgObjects, this.postponedFills, this.styles, Inheritance.Inherited);
			PathCorner pathCorner;
			PathEnding pathEnding;
			Stroke stroke = this.ParseStrokeAttributeSet(node, out pathCorner, out pathEnding, Inheritance.Inherited);
			PathProperties pathProperties = new PathProperties
			{
				Stroke = stroke,
				Corners = pathCorner,
				Head = pathEnding,
				Tail = pathEnding
			};
			List<BezierContour> list = SVGAttribParser.ParsePath(node);
			bool flag = list != null && list.Count > 0;
			if (flag)
			{
				sceneNode.Shapes = new List<Shape>(1);
				sceneNode.Shapes.Add(new Shape
				{
					Contours = list.ToArray(),
					Fill = fill,
					PathProps = pathProperties
				});
				this.AddToSVGDictionaryIfPossible(node, sceneNode);
			}
			this.ParseClipAndMask(node, sceneNode);
			bool flag2 = this.ShouldDeclareSupportedChildren(node);
			if (flag2)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void polygon()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			IFill fill = SVGAttribParser.ParseFill(node, this.svgObjects, this.postponedFills, this.styles, Inheritance.Inherited);
			PathCorner pathCorner;
			PathEnding pathEnding;
			Stroke stroke = this.ParseStrokeAttributeSet(node, out pathCorner, out pathEnding, Inheritance.Inherited);
			string text = node["points"];
			string[] array = ((text != null) ? text.Split(SVGDocument.whiteSpaceNumberChars, StringSplitOptions.RemoveEmptyEntries) : null);
			bool flag = array != null;
			if (flag)
			{
				bool flag2 = (array.Length & 1) == 1;
				if (flag2)
				{
					throw node.GetException("polygon 'points' must specify x,y for each coordinate");
				}
				bool flag3 = array.Length < 4;
				if (flag3)
				{
					throw node.GetException("polygon 'points' do not even specify one triangle");
				}
				PathProperties pathProperties = new PathProperties
				{
					Stroke = stroke,
					Corners = pathCorner,
					Head = pathEnding,
					Tail = pathEnding
				};
				BezierContour bezierContour = new BezierContour
				{
					Closed = true
				};
				Vector2 vector = new Vector2(this.AttribLengthVal(array[0], node, "points", 0f, SVGDocument.DimType.Width), this.AttribLengthVal(array[1], node, "points", 0f, SVGDocument.DimType.Height));
				int num = array.Length / 2;
				List<BezierPathSegment> list = new List<BezierPathSegment>(num);
				for (int i = 1; i < num; i++)
				{
					Vector2 vector2 = new Vector2(this.AttribLengthVal(array[i * 2], node, "points", 0f, SVGDocument.DimType.Width), this.AttribLengthVal(array[i * 2 + 1], node, "points", 0f, SVGDocument.DimType.Height));
					bool flag4 = vector2 == vector;
					if (!flag4)
					{
						BezierSegment bezierSegment = VectorUtils.MakeLine(vector, vector2);
						list.Add(new BezierPathSegment
						{
							P0 = bezierSegment.P0,
							P1 = bezierSegment.P1,
							P2 = bezierSegment.P2
						});
						vector = vector2;
					}
				}
				bool flag5 = list.Count > 0;
				if (flag5)
				{
					BezierSegment bezierSegment2 = VectorUtils.MakeLine(vector, list[0].P0);
					list.Add(new BezierPathSegment
					{
						P0 = bezierSegment2.P0,
						P1 = bezierSegment2.P1,
						P2 = bezierSegment2.P2
					});
					bezierContour.Segments = list.ToArray();
					Shape shape = new Shape
					{
						Contours = new BezierContour[] { bezierContour },
						PathProps = pathProperties,
						Fill = fill
					};
					sceneNode.Shapes = new List<Shape>(1);
					sceneNode.Shapes.Add(shape);
				}
			}
			this.ParseClipAndMask(node, sceneNode);
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag6 = this.ShouldDeclareSupportedChildren(node);
			if (flag6)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void polyline()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			IFill fill = SVGAttribParser.ParseFill(node, this.svgObjects, this.postponedFills, this.styles, Inheritance.Inherited);
			PathCorner pathCorner;
			PathEnding pathEnding;
			Stroke stroke = this.ParseStrokeAttributeSet(node, out pathCorner, out pathEnding, Inheritance.Inherited);
			string text = node["points"];
			string[] array = ((text != null) ? text.Split(SVGDocument.whiteSpaceNumberChars, StringSplitOptions.RemoveEmptyEntries) : null);
			bool flag = array != null;
			if (flag)
			{
				bool flag2 = (array.Length & 1) == 1;
				if (flag2)
				{
					throw node.GetException("polyline 'points' must specify x,y for each coordinate");
				}
				bool flag3 = array.Length < 4;
				if (flag3)
				{
					throw node.GetException("polyline 'points' do not even specify one line");
				}
				Shape shape = new Shape
				{
					Fill = fill
				};
				shape.PathProps = new PathProperties
				{
					Stroke = stroke,
					Corners = pathCorner,
					Head = pathEnding,
					Tail = pathEnding
				};
				Vector2 vector = new Vector2(this.AttribLengthVal(array[0], node, "points", 0f, SVGDocument.DimType.Width), this.AttribLengthVal(array[1], node, "points", 0f, SVGDocument.DimType.Height));
				int num = array.Length / 2;
				List<BezierPathSegment> list = new List<BezierPathSegment>(num);
				for (int i = 1; i < num; i++)
				{
					Vector2 vector2 = new Vector2(this.AttribLengthVal(array[i * 2], node, "points", 0f, SVGDocument.DimType.Width), this.AttribLengthVal(array[i * 2 + 1], node, "points", 0f, SVGDocument.DimType.Height));
					bool flag4 = vector2 == vector;
					if (!flag4)
					{
						BezierSegment bezierSegment = VectorUtils.MakeLine(vector, vector2);
						list.Add(new BezierPathSegment
						{
							P0 = bezierSegment.P0,
							P1 = bezierSegment.P1,
							P2 = bezierSegment.P2
						});
						vector = vector2;
					}
				}
				bool flag5 = list.Count > 0;
				if (flag5)
				{
					BezierSegment bezierSegment2 = VectorUtils.MakeLine(vector, list[0].P0);
					list.Add(new BezierPathSegment
					{
						P0 = bezierSegment2.P0,
						P1 = bezierSegment2.P1,
						P2 = bezierSegment2.P2
					});
					shape.Contours = new BezierContour[]
					{
						new BezierContour
						{
							Segments = list.ToArray()
						}
					};
					sceneNode.Shapes = new List<Shape>(1);
					sceneNode.Shapes.Add(shape);
				}
			}
			this.ParseClipAndMask(node, sceneNode);
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag6 = this.ShouldDeclareSupportedChildren(node);
			if (flag6)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void radialGradient()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			string text = node["xlink:href"];
			GradientFill gradientFill = SVGAttribParser.ParseRelativeRef(text, this.svgObjects) as GradientFill;
			SVGDocument.RadialGradientExData radialGradientExData = ((gradientFill != null) ? (this.gradientExInfo[gradientFill] as SVGDocument.RadialGradientExData) : null);
			bool flag = radialGradientExData != null && radialGradientExData.WorldRelative;
			string text2 = node["gradientUnits"];
			string text3 = text2;
			if (text3 != null)
			{
				if (!(text3 == "objectBoundingBox"))
				{
					if (!(text3 == "userSpaceOnUse"))
					{
						throw node.GetUnsupportedAttribValException("gradientUnits");
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			AddressMode addressMode = ((gradientFill != null) ? gradientFill.Addressing : AddressMode.Clamp);
			string text4 = node["spreadMethod"];
			string text5 = text4;
			if (text5 != null)
			{
				if (!(text5 == "pad"))
				{
					if (!(text5 == "reflect"))
					{
						if (!(text5 == "repeat"))
						{
							throw node.GetUnsupportedAttribValException("spreadMethod");
						}
						addressMode = AddressMode.Wrap;
					}
					else
					{
						addressMode = AddressMode.Mirror;
					}
				}
				else
				{
					addressMode = AddressMode.Clamp;
				}
			}
			Matrix2D matrix2D = SVGAttribParser.ParseTransform(node, "gradientTransform");
			GradientFill gradientFill2 = this.CloneGradientFill(gradientFill);
			bool flag2 = gradientFill2 == null;
			if (flag2)
			{
				gradientFill2 = new GradientFill
				{
					Addressing = addressMode,
					Type = GradientFillType.Radial
				};
			}
			gradientFill2.Type = GradientFillType.Radial;
			SVGDocument.RadialGradientExData radialGradientExData2 = new SVGDocument.RadialGradientExData
			{
				WorldRelative = flag,
				FillTransform = matrix2D
			};
			this.gradientExInfo[gradientFill2] = radialGradientExData2;
			this.currentContainerSize.Push(Vector2.one);
			radialGradientExData2.Cx = node["cx"];
			radialGradientExData2.Cy = node["cy"];
			radialGradientExData2.Fx = node["fx"];
			radialGradientExData2.Fy = node["fy"];
			radialGradientExData2.R = node["r"];
			this.AttribLengthVal(radialGradientExData2.Cx, node, "cx", 0.5f, SVGDocument.DimType.Width);
			this.AttribLengthVal(radialGradientExData2.Cy, node, "cy", 0.5f, SVGDocument.DimType.Height);
			this.AttribLengthVal(radialGradientExData2.Fx, node, "fx", 0.5f, SVGDocument.DimType.Width);
			this.AttribLengthVal(radialGradientExData2.Fy, node, "fy", 0.5f, SVGDocument.DimType.Height);
			this.AttribLengthVal(radialGradientExData2.R, node, "r", 0.5f, SVGDocument.DimType.Length);
			this.currentContainerSize.Pop();
			this.currentGradientFill = gradientFill2;
			this.currentGradientId = node["id"];
			this.currentGradientLink = SVGAttribParser.CleanIri(text);
			bool flag3 = !string.IsNullOrEmpty(text) && !this.svgObjects.ContainsKey(text);
			if (flag3)
			{
				bool flag4 = !this.postponedStopData.ContainsKey(this.currentGradientLink);
				if (flag4)
				{
					this.postponedStopData.Add(this.currentGradientLink, new List<SVGDocument.PostponedStopData>());
				}
				this.postponedStopData[this.currentGradientLink].Add(new SVGDocument.PostponedStopData
				{
					fill = gradientFill2
				});
			}
			this.AddToSVGDictionaryIfPossible(node, gradientFill2);
			bool flag5 = this.ShouldDeclareSupportedChildren(node);
			if (flag5)
			{
				this.SupportElems(node, new SVGDocument.ElemHandler[]
				{
					new SVGDocument.ElemHandler(this.stop)
				});
			}
		}

		private void clipPath()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			string text = node["id"];
			SceneNode sceneNode = new SceneNode
			{
				Transform = SVGAttribParser.ParseTransform(node)
			};
			string text2 = node["clipPathUnits"];
			string text3 = text2;
			bool flag;
			if (text3 != null && !(text3 == "userSpaceOnUse"))
			{
				if (!(text3 == "objectBoundingBox"))
				{
					throw node.GetUnsupportedAttribValException("clipPathUnits");
				}
				flag = false;
			}
			else
			{
				flag = true;
			}
			this.clipData[sceneNode] = new SVGDocument.ClipData
			{
				WorldRelative = flag
			};
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag2 = this.ShouldDeclareSupportedChildren(node);
			if (flag2)
			{
				this.SupportElems(node, this.allElems);
			}
			this.currentSceneNode.Push(sceneNode);
			this.ParseChildren(node, node.Name);
			bool flag3 = this.currentSceneNode.Pop() != sceneNode;
			if (flag3)
			{
				throw SVGFormatException.StackError;
			}
			bool flag4 = !string.IsNullOrEmpty(text);
			if (flag4)
			{
				List<SVGDocument.PostponedClip> list;
				bool flag5 = this.postponedClip.TryGetValue(text, out list);
				if (flag5)
				{
					foreach (SVGDocument.PostponedClip postponedClip in list)
					{
						this.ApplyClipper(sceneNode, postponedClip.node, flag);
					}
				}
			}
		}

		private void pattern()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = new SceneNode
			{
				Transform = Matrix2D.identity
			};
			string text = node["patternUnits"];
			string text2 = text;
			bool flag;
			if (text2 != null && !(text2 == "objectBoundingBox"))
			{
				if (!(text2 == "userSpaceOnUse"))
				{
					throw node.GetUnsupportedAttribValException("patternUnits");
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			string text3 = node["patternContentUnits"];
			string text4 = text3;
			bool flag2;
			if (text4 != null && !(text4 == "userSpaceOnUse"))
			{
				if (!(text4 == "objectBoundingBox"))
				{
					throw node.GetUnsupportedAttribValException("patternContentUnits");
				}
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
			float num = this.AttribLengthVal(node["x"], node, "x", 0f, SVGDocument.DimType.Width);
			float num2 = this.AttribLengthVal(node["y"], node, "y", 0f, SVGDocument.DimType.Height);
			float num3 = this.AttribLengthVal(node["width"], node, "width", 0f, SVGDocument.DimType.Width);
			float num4 = this.AttribLengthVal(node["height"], node, "height", 0f, SVGDocument.DimType.Height);
			Matrix2D matrix2D = SVGAttribParser.ParseTransform(node, "patternTransform");
			this.patternData[sceneNode] = new SVGDocument.PatternData
			{
				WorldRelative = flag,
				ContentWorldRelative = flag2,
				PatternTransform = matrix2D
			};
			PatternFill patternFill = new PatternFill
			{
				Pattern = sceneNode,
				Rect = new Rect(num, num2, num3, num4)
			};
			this.AddToSVGDictionaryIfPossible(node, patternFill);
			bool flag3 = this.ShouldDeclareSupportedChildren(node);
			if (flag3)
			{
				this.SupportElems(node, this.allElems);
			}
			this.currentSceneNode.Push(sceneNode);
			this.ParseChildren(node, node.Name);
			bool flag4 = this.currentSceneNode.Pop() != sceneNode;
			if (flag4)
			{
				throw SVGFormatException.StackError;
			}
		}

		private void mask()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = new SceneNode
			{
				Transform = Matrix2D.identity
			};
			string text = node["maskUnits"];
			string text2 = text;
			bool flag;
			if (text2 != null && !(text2 == "userSpaceOnUse"))
			{
				if (!(text2 == "objectBoundingBox"))
				{
					throw node.GetUnsupportedAttribValException("maskUnits");
				}
				flag = false;
			}
			else
			{
				flag = true;
			}
			string text3 = node["maskContentUnits"];
			string text4 = text3;
			bool flag2;
			if (text4 != null && !(text4 == "userSpaceOnUse"))
			{
				if (!(text4 == "objectBoundingBox"))
				{
					throw node.GetUnsupportedAttribValException("maskContentUnits");
				}
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
			this.maskData[sceneNode] = new SVGDocument.MaskData
			{
				WorldRelative = flag,
				ContentWorldRelative = flag2
			};
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag3 = this.ShouldDeclareSupportedChildren(node);
			if (flag3)
			{
				this.SupportElems(node, this.allElems);
			}
			this.currentSceneNode.Push(sceneNode);
			this.ParseChildren(node, node.Name);
			bool flag4 = this.currentSceneNode.Pop() != sceneNode;
			if (flag4)
			{
				throw SVGFormatException.StackError;
			}
		}

		private void rect()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			IFill fill = SVGAttribParser.ParseFill(node, this.svgObjects, this.postponedFills, this.styles, Inheritance.Inherited);
			PathCorner pathCorner;
			PathEnding pathEnding;
			Stroke stroke = this.ParseStrokeAttributeSet(node, out pathCorner, out pathEnding, Inheritance.Inherited);
			float num = this.AttribLengthVal(node, "x", 0f, SVGDocument.DimType.Width);
			float num2 = this.AttribLengthVal(node, "y", 0f, SVGDocument.DimType.Height);
			float num3 = this.AttribLengthVal(node, "rx", -1f, SVGDocument.DimType.Length);
			float num4 = this.AttribLengthVal(node, "ry", -1f, SVGDocument.DimType.Length);
			float num5 = this.AttribLengthVal(node, "width", 0f, SVGDocument.DimType.Length);
			float num6 = this.AttribLengthVal(node, "height", 0f, SVGDocument.DimType.Length);
			bool flag = num3 < 0f && num4 >= 0f;
			if (flag)
			{
				num3 = num4;
			}
			else
			{
				bool flag2 = num4 < 0f && num3 >= 0f;
				if (flag2)
				{
					num4 = num3;
				}
				else
				{
					bool flag3 = num4 < 0f && num3 < 0f;
					if (flag3)
					{
						num4 = (num3 = 0f);
					}
				}
			}
			num3 = Mathf.Min(num3, num5 * 0.5f);
			num4 = Mathf.Min(num4, num6 * 0.5f);
			Vector2 vector = new Vector2(num3, num4);
			Shape shape = new Shape();
			VectorUtils.MakeRectangleShape(shape, new Rect(num, num2, num5, num6), vector, vector, vector, vector);
			shape.Fill = fill;
			shape.PathProps = new PathProperties
			{
				Stroke = stroke,
				Head = pathEnding,
				Tail = pathEnding,
				Corners = pathCorner
			};
			sceneNode.Shapes = new List<Shape>(1);
			sceneNode.Shapes.Add(shape);
			this.ParseClipAndMask(node, sceneNode);
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag4 = this.ShouldDeclareSupportedChildren(node);
			if (flag4)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void stop()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			GradientStop gradientStop = default(GradientStop);
			string text = this.styles.Evaluate("stop-color", Inheritance.None);
			Color color = ((text != null) ? SVGAttribParser.ParseColor(text) : Color.black);
			color.a = this.AttribFloatVal("stop-opacity", 1f);
			gradientStop.Color = color;
			string text2 = this.styles.Evaluate("offset", Inheritance.None);
			bool flag = !string.IsNullOrEmpty(text2);
			if (flag)
			{
				bool flag2 = text2.EndsWith("%");
				bool flag3 = flag2;
				if (flag3)
				{
					text2 = text2.Substring(0, text2.Length - 1);
				}
				gradientStop.StopPercentage = SVGAttribParser.ParseFloat(text2);
				bool flag4 = flag2;
				if (flag4)
				{
					gradientStop.StopPercentage /= 100f;
				}
				gradientStop.StopPercentage = Mathf.Max(0f, gradientStop.StopPercentage);
				gradientStop.StopPercentage = Mathf.Min(1f, gradientStop.StopPercentage);
			}
			bool flag5 = this.currentGradientFill.Stops == null || this.currentGradientFill.Stops.Length == 0;
			GradientStop[] array;
			if (flag5)
			{
				array = new GradientStop[1];
			}
			else
			{
				array = new GradientStop[this.currentGradientFill.Stops.Length + 1];
				this.currentGradientFill.Stops.CopyTo(array, 0);
			}
			array[array.Length - 1] = gradientStop;
			this.currentGradientFill.Stops = array;
			bool flag6 = !string.IsNullOrEmpty(this.currentGradientId) && this.postponedStopData.ContainsKey(this.currentGradientId);
			if (flag6)
			{
				foreach (SVGDocument.PostponedStopData postponedStopData in this.postponedStopData[this.currentGradientId])
				{
					postponedStopData.fill.Stops = array;
				}
			}
			bool flag7 = !string.IsNullOrEmpty(this.currentGradientLink) && this.postponedStopData.ContainsKey(this.currentGradientLink);
			if (flag7)
			{
				List<SVGDocument.PostponedStopData> list = this.postponedStopData[this.currentGradientLink];
				foreach (SVGDocument.PostponedStopData postponedStopData2 in list)
				{
					bool flag8 = postponedStopData2.fill == this.currentGradientFill;
					if (flag8)
					{
						list.Remove(postponedStopData2);
						break;
					}
				}
			}
			bool flag9 = this.ShouldDeclareSupportedChildren(node);
			if (flag9)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void svg()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = new SceneNode();
			bool flag = this.scene.Root == null;
			if (flag)
			{
				this.scene.Root = sceneNode;
			}
			this.styles.PushNode(node);
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			this.sceneViewport = this.ParseViewport(node, sceneNode, new Vector2((float)this.windowWidth, (float)this.windowHeight));
			SVGDocument.ViewBoxInfo viewBoxInfo = this.ParseViewBox(node, sceneNode, this.sceneViewport);
			bool flag2 = this.applyRootViewBox;
			if (flag2)
			{
				this.ApplyViewBox(sceneNode, viewBoxInfo, this.sceneViewport);
			}
			this.currentContainerSize.Push(this.sceneViewport.size);
			bool flag3 = !viewBoxInfo.IsEmpty;
			if (flag3)
			{
				this.currentViewBoxSize.Push(viewBoxInfo.ViewBox.size);
			}
			this.currentSceneNode.Push(sceneNode);
			this.nodeGlobalSceneState[sceneNode] = new SVGDocument.NodeGlobalSceneState
			{
				ContainerSize = this.currentContainerSize.Peek()
			};
			bool flag4 = this.ShouldDeclareSupportedChildren(node);
			if (flag4)
			{
				this.SupportElems(node, this.allElems);
			}
			this.ParseChildren(node, "svg");
			bool flag5 = this.currentSceneNode.Pop() != sceneNode;
			if (flag5)
			{
				throw SVGFormatException.StackError;
			}
			bool flag6 = !viewBoxInfo.IsEmpty;
			if (flag6)
			{
				this.currentViewBoxSize.Pop();
			}
			this.currentContainerSize.Pop();
			this.styles.PopNode();
		}

		private void symbol()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = new SceneNode();
			string text = node["id"];
			this.ParseID(node, sceneNode);
			this.ParseOpacity(sceneNode);
			sceneNode.Transform = Matrix2D.identity;
			Rect rect = new Rect(Vector2.zero, this.currentContainerSize.Peek());
			SVGDocument.ViewBoxInfo viewBoxInfo = this.ParseViewBox(node, sceneNode, rect);
			bool flag = !viewBoxInfo.IsEmpty;
			if (flag)
			{
				this.currentViewBoxSize.Push(viewBoxInfo.ViewBox.size);
			}
			this.symbolViewBoxes[sceneNode] = viewBoxInfo;
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag2 = this.ShouldDeclareSupportedChildren(node);
			if (flag2)
			{
				this.SupportElems(node, this.allElems);
			}
			this.currentSceneNode.Push(sceneNode);
			this.ParseChildren(node, node.Name);
			bool flag3 = this.currentSceneNode.Pop() != sceneNode;
			if (flag3)
			{
				throw SVGFormatException.StackError;
			}
			bool flag4 = !viewBoxInfo.IsEmpty;
			if (flag4)
			{
				this.currentViewBoxSize.Pop();
			}
			this.ParseClipAndMask(node, sceneNode);
			bool flag5 = !string.IsNullOrEmpty(text);
			if (flag5)
			{
				List<SVGDocument.NodeReferenceData> list;
				bool flag6 = this.postponedSymbolData.TryGetValue(text, out list);
				if (flag6)
				{
					foreach (SVGDocument.NodeReferenceData nodeReferenceData in list)
					{
						this.ResolveReferencedNode(sceneNode, nodeReferenceData, true);
					}
				}
			}
		}

		private void use()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			SceneNode sceneNode = this.currentSceneNode.Peek();
			this.ParseOpacity(sceneNode);
			Rect rect = this.ParseViewport(node, sceneNode, Vector2.zero);
			SVGDocument.NodeReferenceData nodeReferenceData = new SVGDocument.NodeReferenceData
			{
				node = sceneNode,
				viewport = rect,
				id = node["id"]
			};
			string text = node["xlink:href"];
			SceneNode sceneNode2 = SVGAttribParser.ParseRelativeRef(text, this.svgObjects) as SceneNode;
			bool flag = sceneNode2 == null && !string.IsNullOrEmpty(text) && text.StartsWith("#");
			if (flag)
			{
				text = text.Substring(1);
				List<SVGDocument.NodeReferenceData> list;
				bool flag2 = !this.postponedSymbolData.TryGetValue(text, out list);
				if (flag2)
				{
					list = new List<SVGDocument.NodeReferenceData>();
					this.postponedSymbolData[text] = list;
				}
				list.Add(nodeReferenceData);
			}
			sceneNode.Transform = SVGAttribParser.ParseTransform(node);
			sceneNode.Transform *= Matrix2D.Translate(rect.position);
			bool flag3 = sceneNode2 != null;
			if (flag3)
			{
				this.ResolveReferencedNode(sceneNode2, nodeReferenceData, false);
			}
			this.ParseClipAndMask(node, sceneNode);
			this.AddToSVGDictionaryIfPossible(node, sceneNode);
			bool flag4 = this.ShouldDeclareSupportedChildren(node);
			if (flag4)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void style()
		{
			XmlReaderIterator.Node node = this.docReader.VisitCurrent();
			string text = this.docReader.ReadTextWithinElement();
			bool flag = text.Length > 0;
			if (flag)
			{
				this.styles.SetGlobalStyleSheet(SVGStyleSheetUtils.Parse(text));
			}
			bool flag2 = this.ShouldDeclareSupportedChildren(node);
			if (flag2)
			{
				this.SupportElems(node, Array.Empty<SVGDocument.ElemHandler>());
			}
		}

		private void ResolveReferencedNode(SceneNode referencedNode, SVGDocument.NodeReferenceData refData, bool isDeferred)
		{
			SVGDocument.ViewBoxInfo viewBoxInfo;
			bool flag = this.symbolViewBoxes.TryGetValue(referencedNode, out viewBoxInfo);
			if (flag)
			{
				this.ApplyViewBox(refData.node, viewBoxInfo, refData.viewport);
			}
			bool flag2 = refData.node.Children == null;
			if (flag2)
			{
				refData.node.Children = new List<SceneNode>();
			}
			SVGStyleResolver.StyleLayer styleLayer = null;
			if (isDeferred)
			{
				styleLayer = this.styles.GetLayerForScenNode(refData.node);
				bool flag3 = styleLayer != null;
				if (flag3)
				{
					this.styles.PushLayer(styleLayer);
				}
			}
			SVGStyleResolver.StyleLayer styleLayer2 = this.nodeStyleLayers[referencedNode];
			bool flag4 = styleLayer2 != null;
			if (flag4)
			{
				this.styles.PushLayer(styleLayer2);
			}
			List<SceneNode> list = new List<SceneNode>(10);
			foreach (SceneNode sceneNode in VectorUtils.SceneNodes(referencedNode))
			{
				list.Add(sceneNode);
			}
			SceneNode sceneNode2 = this.CloneSceneNode(referencedNode);
			int num = 0;
			foreach (SceneNode sceneNode3 in VectorUtils.SceneNodes(sceneNode2))
			{
				int num2 = num++;
				bool flag5 = sceneNode3.Shapes == null;
				if (!flag5)
				{
					SceneNode sceneNode4 = list[num2];
					SVGStyleResolver.StyleLayer layerForScenNode = this.styles.GetLayerForScenNode(sceneNode4);
					bool flag6 = layerForScenNode != null;
					if (flag6)
					{
						this.styles.PushLayer(layerForScenNode);
					}
					bool flag7;
					IFill fill = SVGAttribParser.ParseFill(null, this.svgObjects, this.postponedFills, this.styles, Inheritance.Inherited, out flag7);
					PathCorner pathCorner;
					PathEnding pathEnding;
					Stroke stroke = this.ParseStrokeAttributeSet(null, out pathCorner, out pathEnding, Inheritance.Inherited);
					foreach (Shape shape in sceneNode3.Shapes)
					{
						PathProperties pathProps = shape.PathProps;
						pathProps.Stroke = stroke;
						pathProps.Corners = pathCorner;
						pathProps.Head = pathEnding;
						shape.PathProps = pathProps;
						shape.Fill = (flag7 ? shape.Fill : fill);
					}
					bool flag8 = layerForScenNode != null;
					if (flag8)
					{
						this.styles.PopLayer();
					}
				}
			}
			bool flag9 = styleLayer2 != null;
			if (flag9)
			{
				this.styles.PopLayer();
			}
			bool flag10 = styleLayer != null;
			if (flag10)
			{
				this.styles.PopLayer();
			}
			bool flag11 = !string.IsNullOrEmpty(refData.id);
			if (flag11)
			{
				this.nodeIDs[refData.id] = sceneNode2;
			}
			refData.node.Children.Add(sceneNode2);
		}

		private SceneNode CloneSceneNode(SceneNode node)
		{
			bool flag = node == null;
			SceneNode sceneNode;
			if (flag)
			{
				sceneNode = null;
			}
			else
			{
				List<SceneNode> list = null;
				bool flag2 = node.Children != null;
				if (flag2)
				{
					list = new List<SceneNode>(node.Children.Count);
					foreach (SceneNode sceneNode2 in node.Children)
					{
						list.Add(this.CloneSceneNode(sceneNode2));
					}
				}
				List<Shape> list2 = null;
				bool flag3 = node.Shapes != null;
				if (flag3)
				{
					list2 = new List<Shape>(node.Shapes.Count);
					foreach (Shape shape in node.Shapes)
					{
						list2.Add(this.CloneShape(shape));
					}
				}
				SceneNode sceneNode3 = new SceneNode
				{
					Children = list,
					Shapes = list2,
					Transform = node.Transform,
					Clipper = this.CloneSceneNode(node.Clipper)
				};
				bool flag4 = this.nodeGlobalSceneState.ContainsKey(node);
				if (flag4)
				{
					this.nodeGlobalSceneState[sceneNode3] = this.nodeGlobalSceneState[node];
				}
				bool flag5 = this.nodeOpacity.ContainsKey(node);
				if (flag5)
				{
					this.nodeOpacity[sceneNode3] = this.nodeOpacity[node];
				}
				sceneNode = sceneNode3;
			}
			return sceneNode;
		}

		private Shape CloneShape(Shape shape)
		{
			bool flag = shape == null;
			Shape shape2;
			if (flag)
			{
				shape2 = null;
			}
			else
			{
				BezierContour[] array = null;
				bool flag2 = shape.Contours != null;
				if (flag2)
				{
					array = new BezierContour[shape.Contours.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this.CloneContour(shape.Contours[i]);
					}
				}
				shape2 = new Shape
				{
					Fill = this.CloneFill(shape.Fill),
					FillTransform = shape.FillTransform,
					PathProps = this.ClonePathProps(shape.PathProps),
					Contours = array,
					IsConvex = shape.IsConvex
				};
			}
			return shape2;
		}

		private BezierContour CloneContour(BezierContour c)
		{
			BezierPathSegment[] array = null;
			bool flag = c.Segments != null;
			if (flag)
			{
				array = new BezierPathSegment[c.Segments.Length];
				for (int i = 0; i < array.Length; i++)
				{
					BezierPathSegment bezierPathSegment = c.Segments[i];
					array[i] = new BezierPathSegment
					{
						P0 = bezierPathSegment.P0,
						P1 = bezierPathSegment.P1,
						P2 = bezierPathSegment.P2
					};
				}
			}
			return new BezierContour
			{
				Segments = array,
				Closed = c.Closed
			};
		}

		private IFill CloneFill(IFill fill)
		{
			bool flag = fill == null;
			IFill fill2;
			if (flag)
			{
				fill2 = null;
			}
			else
			{
				IFill fill3 = null;
				bool flag2 = fill is SolidFill;
				if (flag2)
				{
					SolidFill solidFill = fill as SolidFill;
					fill3 = new SolidFill
					{
						Color = solidFill.Color,
						Opacity = solidFill.Opacity,
						Mode = solidFill.Mode
					};
				}
				else
				{
					bool flag3 = fill is GradientFill;
					if (flag3)
					{
						GradientFill gradientFill = fill as GradientFill;
						GradientStop[] array = null;
						bool flag4 = gradientFill.Stops != null;
						if (flag4)
						{
							array = new GradientStop[gradientFill.Stops.Length];
							for (int i = 0; i < array.Length; i++)
							{
								GradientStop gradientStop = gradientFill.Stops[i];
								array[i] = new GradientStop
								{
									Color = gradientStop.Color,
									StopPercentage = gradientStop.StopPercentage
								};
							}
						}
						GradientFill gradientFill2 = new GradientFill
						{
							Type = gradientFill.Type,
							Stops = array,
							Mode = gradientFill.Mode,
							Opacity = gradientFill.Opacity,
							Addressing = gradientFill.Addressing,
							RadialFocus = gradientFill.RadialFocus
						};
						this.gradientExInfo[gradientFill2] = this.gradientExInfo[gradientFill];
						fill3 = gradientFill2;
					}
					else
					{
						bool flag5 = fill is TextureFill;
						if (flag5)
						{
							TextureFill textureFill = fill as TextureFill;
							fill3 = new TextureFill
							{
								Texture = textureFill.Texture,
								Mode = textureFill.Mode,
								Opacity = textureFill.Opacity,
								Addressing = textureFill.Addressing
							};
						}
						else
						{
							bool flag6 = fill is PatternFill;
							if (flag6)
							{
								PatternFill patternFill = fill as PatternFill;
								fill3 = new PatternFill
								{
									Mode = patternFill.Mode,
									Opacity = patternFill.Opacity,
									Pattern = this.CloneSceneNode(patternFill.Pattern),
									Rect = patternFill.Rect
								};
							}
						}
					}
				}
				fill2 = fill3;
			}
			return fill2;
		}

		private PathProperties ClonePathProps(PathProperties props)
		{
			Stroke stroke = null;
			bool flag = props.Stroke != null;
			if (flag)
			{
				float[] array = null;
				bool flag2 = props.Stroke.Pattern != null;
				if (flag2)
				{
					array = new float[props.Stroke.Pattern.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = props.Stroke.Pattern[i];
					}
				}
				stroke = new Stroke
				{
					Fill = this.CloneFill(props.Stroke.Fill),
					FillTransform = props.Stroke.FillTransform,
					HalfThickness = props.Stroke.HalfThickness,
					Pattern = array,
					PatternOffset = props.Stroke.PatternOffset,
					TippedCornerLimit = props.Stroke.TippedCornerLimit
				};
			}
			return new PathProperties
			{
				Stroke = stroke,
				Head = props.Head,
				Tail = props.Tail,
				Corners = props.Corners
			};
		}

		private GradientFill CloneGradientFill(GradientFill other)
		{
			bool flag = other == null;
			GradientFill gradientFill;
			if (flag)
			{
				gradientFill = null;
			}
			else
			{
				gradientFill = new GradientFill
				{
					Type = other.Type,
					Stops = other.Stops,
					Mode = other.Mode,
					Opacity = other.Opacity,
					Addressing = other.Addressing,
					RadialFocus = other.RadialFocus
				};
			}
			return gradientFill;
		}

		private int AttribIntVal(string attribName)
		{
			return this.AttribIntVal(attribName, 0);
		}

		private int AttribIntVal(string attribName, int defaultVal)
		{
			string text = this.styles.Evaluate(attribName, Inheritance.None);
			return (text != null) ? int.Parse(text) : defaultVal;
		}

		private float AttribFloatVal(string attribName)
		{
			return this.AttribFloatVal(attribName, 0f);
		}

		private float AttribFloatVal(string attribName, float defaultVal)
		{
			string text = this.styles.Evaluate(attribName, Inheritance.None);
			return (text != null) ? SVGAttribParser.ParseFloat(text) : defaultVal;
		}

		private float AttribLengthVal(XmlReaderIterator.Node node, string attribName, SVGDocument.DimType dimType)
		{
			return this.AttribLengthVal(node, attribName, 0f, dimType);
		}

		private float AttribLengthVal(XmlReaderIterator.Node node, string attribName, float defaultUnitVal, SVGDocument.DimType dimType)
		{
			string text = this.styles.Evaluate(attribName, Inheritance.None);
			return this.AttribLengthVal(text, node, attribName, defaultUnitVal, dimType);
		}

		private float AttribLengthVal(string val, XmlReaderIterator.Node node, string attribName, float defaultUnitVal, SVGDocument.DimType dimType)
		{
			bool flag = val == null;
			float num;
			if (flag)
			{
				num = defaultUnitVal;
			}
			else
			{
				val = val.Trim();
				string text = "px";
				char c = val[val.Length - 1];
				bool flag2 = c == '%';
				if (flag2)
				{
					float num2 = SVGAttribParser.ParseFloat(val.Substring(0, val.Length - 1));
					bool flag3 = num2 < 0f;
					if (flag3)
					{
						throw node.GetException("Number in " + attribName + " cannot be negative");
					}
					num2 /= 100f;
					Vector2 vector = ((this.currentViewBoxSize.Count > 0) ? this.currentViewBoxSize.Peek() : this.currentContainerSize.Peek());
					switch (dimType)
					{
					case SVGDocument.DimType.Width:
						return num2 * vector.x;
					case SVGDocument.DimType.Height:
						return num2 * vector.y;
					case SVGDocument.DimType.Length:
						return num2 * vector.magnitude / 1.4142135f;
					}
				}
				else
				{
					bool flag4 = val.Length >= 2;
					if (flag4)
					{
						text = val.Substring(val.Length - 2);
					}
				}
				bool flag5 = char.IsDigit(c) || c == '.';
				if (!flag5)
				{
					float num3 = SVGAttribParser.ParseFloat(val.Substring(0, val.Length - 2));
					string text2 = text;
					string text3 = text2;
					uint num4 = global::<PrivateImplementationDetails>.ComputeStringHash(text3);
					if (num4 <= 1313756516U)
					{
						if (num4 <= 1094220446U)
						{
							if (num4 != 1075471351U)
							{
								if (num4 == 1094220446U)
								{
									if (text3 == "in")
									{
										return 90f * num3 * this.dpiScale;
									}
								}
							}
							else if (text3 == "em")
							{
								throw new NotImplementedException();
							}
						}
						else if (num4 != 1260025160U)
						{
							if (num4 == 1313756516U)
							{
								if (text3 == "pc")
								{
									return 15f * num3 * this.dpiScale;
								}
							}
						}
						else if (text3 == "ex")
						{
							throw new NotImplementedException();
						}
					}
					else if (num4 <= 1565420801U)
					{
						if (num4 != 1498310325U)
						{
							if (num4 == 1565420801U)
							{
								if (text3 == "pt")
								{
									return 1.25f * num3 * this.dpiScale;
								}
							}
						}
						else if (text3 == "px")
						{
							return num3;
						}
					}
					else if (num4 != 1613635087U)
					{
						if (num4 == 1680451373U)
						{
							if (text3 == "cm")
							{
								return 35.43307f * num3 * this.dpiScale;
							}
						}
					}
					else if (text3 == "mm")
					{
						return 3.543307f * num3 * this.dpiScale;
					}
					throw new FormatException("Unknown length unit type (" + text + ")");
				}
				num = SVGAttribParser.ParseFloat(val);
			}
			return num;
		}

		private void AddToSVGDictionaryIfPossible(XmlReaderIterator.Node node, object vectorElement)
		{
			string text = node["id"];
			bool flag = !string.IsNullOrEmpty(text);
			if (flag)
			{
				this.svgObjects[text] = vectorElement;
			}
		}

		private Rect ParseViewport(XmlReaderIterator.Node node, SceneNode sceneNode, Vector2 defaultViewportSize)
		{
			this.scenePos.x = this.AttribLengthVal(node, "x", SVGDocument.DimType.Width);
			this.scenePos.y = this.AttribLengthVal(node, "y", SVGDocument.DimType.Height);
			this.sceneSize.x = this.AttribLengthVal(node, "width", defaultViewportSize.x, SVGDocument.DimType.Width);
			this.sceneSize.y = this.AttribLengthVal(node, "height", defaultViewportSize.y, SVGDocument.DimType.Height);
			return new Rect(this.scenePos, this.sceneSize);
		}

		private SVGDocument.ViewBoxInfo ParseViewBox(XmlReaderIterator.Node node, SceneNode sceneNode, Rect sceneViewport)
		{
			SVGDocument.ViewBoxInfo viewBoxInfo = new SVGDocument.ViewBoxInfo
			{
				IsEmpty = true
			};
			string text = node["viewBox"];
			text = ((text != null) ? text.Trim() : null);
			bool flag = string.IsNullOrEmpty(text);
			SVGDocument.ViewBoxInfo viewBoxInfo2;
			if (flag)
			{
				viewBoxInfo2 = viewBoxInfo;
			}
			else
			{
				string[] array = text.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
				bool flag2 = array.Length != 4;
				if (flag2)
				{
					throw node.GetException("Invalid viewBox specification");
				}
				Vector2 vector = new Vector2(this.AttribLengthVal(array[0], node, "viewBox", 0f, SVGDocument.DimType.Width), this.AttribLengthVal(array[1], node, "viewBox", 0f, SVGDocument.DimType.Height));
				Vector2 vector2 = new Vector2(this.AttribLengthVal(array[2], node, "viewBox", sceneViewport.width, SVGDocument.DimType.Width), this.AttribLengthVal(array[3], node, "viewBox", sceneViewport.height, SVGDocument.DimType.Height));
				viewBoxInfo.ViewBox = new Rect(vector, vector2);
				this.ParseViewBoxAspectRatio(node, ref viewBoxInfo);
				viewBoxInfo.IsEmpty = false;
				viewBoxInfo2 = viewBoxInfo;
			}
			return viewBoxInfo2;
		}

		private void ParseViewBoxAspectRatio(XmlReaderIterator.Node node, ref SVGDocument.ViewBoxInfo viewBoxInfo)
		{
			viewBoxInfo.AspectRatio = SVGDocument.ViewBoxAspectRatio.FitLargestDim;
			viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Mid;
			viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Mid;
			string text = node["preserveAspectRatio"];
			text = ((text != null) ? text.Trim() : null);
			bool flag = false;
			bool flag2 = !string.IsNullOrEmpty(text);
			if (flag2)
			{
				string[] array = text.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text2 in array)
				{
					string text3 = text2;
					string text4 = text3;
					uint num = global::<PrivateImplementationDetails>.ComputeStringHash(text4);
					if (num <= 1603253636U)
					{
						if (num <= 692744732U)
						{
							if (num != 436337467U)
							{
								if (num != 492693232U)
								{
									if (num == 692744732U)
									{
										if (text4 == "xMidYMid")
										{
											viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Mid;
											viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Mid;
										}
									}
								}
								else if (text4 == "xMidYMax")
								{
									viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Mid;
									viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Max;
								}
							}
							else if (!(text4 == "defer"))
							{
							}
						}
						else if (num != 860520922U)
						{
							if (num != 1502587922U)
							{
								if (num == 1603253636U)
								{
									if (text4 == "xMinYMin")
									{
										viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Min;
										viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Min;
									}
								}
							}
							else if (text4 == "xMinYMid")
							{
								viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Min;
								viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Mid;
							}
						}
						else if (text4 == "xMidYMin")
						{
							viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Mid;
							viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Min;
						}
					}
					else if (num <= 2295560534U)
					{
						if (num != 1737076817U)
						{
							if (num != 1839420230U)
							{
								if (num == 2295560534U)
								{
									if (text4 == "meet")
									{
										viewBoxInfo.AspectRatio = SVGDocument.ViewBoxAspectRatio.FitLargestDim;
									}
								}
							}
							else if (text4 == "xMinYMax")
							{
								viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Min;
								viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Max;
							}
						}
						else if (text4 == "slice")
						{
							viewBoxInfo.AspectRatio = SVGDocument.ViewBoxAspectRatio.FitSmallestDim;
						}
					}
					else if (num <= 3805251044U)
					{
						if (num != 2913447899U)
						{
							if (num == 3805251044U)
							{
								if (text4 == "xMaxYMax")
								{
									viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Max;
									viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Max;
								}
							}
						}
						else if (text4 == "none")
						{
							flag = true;
						}
					}
					else if (num != 4038857782U)
					{
						if (num == 4139523496U)
						{
							if (text4 == "xMaxYMid")
							{
								viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Max;
								viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Mid;
							}
						}
					}
					else if (text4 == "xMaxYMin")
					{
						viewBoxInfo.AlignX = SVGDocument.ViewBoxAlign.Max;
						viewBoxInfo.AlignY = SVGDocument.ViewBoxAlign.Min;
					}
				}
			}
			bool flag3 = flag;
			if (flag3)
			{
				viewBoxInfo.AspectRatio = SVGDocument.ViewBoxAspectRatio.DontPreserve;
			}
		}

		private void ApplyViewBox(SceneNode sceneNode, SVGDocument.ViewBoxInfo viewBoxInfo, Rect sceneViewport)
		{
			bool flag = viewBoxInfo.ViewBox.size == Vector2.zero || sceneViewport.size == Vector2.zero;
			if (!flag)
			{
				Vector2 vector = Vector2.one;
				Vector2 vector2 = -viewBoxInfo.ViewBox.position;
				bool flag2 = viewBoxInfo.AspectRatio == SVGDocument.ViewBoxAspectRatio.DontPreserve;
				if (flag2)
				{
					vector = sceneViewport.size / viewBoxInfo.ViewBox.size;
				}
				else
				{
					vector.x = (vector.y = sceneViewport.width / viewBoxInfo.ViewBox.width);
					bool flag3 = viewBoxInfo.AspectRatio == SVGDocument.ViewBoxAspectRatio.FitLargestDim;
					bool flag4;
					if (flag3)
					{
						flag4 = viewBoxInfo.ViewBox.height * vector.y <= sceneViewport.height;
					}
					else
					{
						flag4 = viewBoxInfo.ViewBox.height * vector.y > sceneViewport.height;
					}
					Vector2 zero = Vector2.zero;
					bool flag5 = flag4;
					if (flag5)
					{
						bool flag6 = viewBoxInfo.AlignY == SVGDocument.ViewBoxAlign.Mid;
						if (flag6)
						{
							zero.y = (sceneViewport.height - viewBoxInfo.ViewBox.height * vector.y) * 0.5f;
						}
						else
						{
							bool flag7 = viewBoxInfo.AlignY == SVGDocument.ViewBoxAlign.Max;
							if (flag7)
							{
								zero.y = sceneViewport.height - viewBoxInfo.ViewBox.height * vector.y;
							}
						}
					}
					else
					{
						vector.x = (vector.y = sceneViewport.height / viewBoxInfo.ViewBox.height);
						bool flag8 = viewBoxInfo.AlignX == SVGDocument.ViewBoxAlign.Mid;
						if (flag8)
						{
							zero.x = (sceneViewport.width - viewBoxInfo.ViewBox.width * vector.x) * 0.5f;
						}
						else
						{
							bool flag9 = viewBoxInfo.AlignX == SVGDocument.ViewBoxAlign.Max;
							if (flag9)
							{
								zero.x = sceneViewport.width - viewBoxInfo.ViewBox.width * vector.x;
							}
						}
					}
					vector2 += zero / vector;
				}
				sceneNode.Transform = sceneNode.Transform * Matrix2D.Scale(vector) * Matrix2D.Translate(vector2);
			}
		}

		private Stroke ParseStrokeAttributeSet(XmlReaderIterator.Node node, out PathCorner strokeCorner, out PathEnding strokeEnding, Inheritance inheritance = Inheritance.Inherited)
		{
			Stroke stroke = SVGAttribParser.ParseStrokeAndOpacity(node, this.svgObjects, this.styles, inheritance);
			strokeCorner = PathCorner.Tipped;
			strokeEnding = PathEnding.Chop;
			bool flag = stroke != null;
			if (flag)
			{
				string text = this.styles.Evaluate("stroke-width", inheritance);
				stroke.HalfThickness = this.AttribLengthVal(text, node, "stroke-width", 1f, SVGDocument.DimType.Length) * 0.5f;
				string text2 = this.styles.Evaluate("stroke-linecap", inheritance);
				string text3 = text2;
				if (!(text3 == "butt"))
				{
					if (!(text3 == "square"))
					{
						if (text3 == "round")
						{
							strokeEnding = PathEnding.Round;
						}
					}
					else
					{
						strokeEnding = PathEnding.Square;
					}
				}
				else
				{
					strokeEnding = PathEnding.Chop;
				}
				string text4 = this.styles.Evaluate("stroke-linejoin", inheritance);
				string text5 = text4;
				if (!(text5 == "miter"))
				{
					if (!(text5 == "round"))
					{
						if (text5 == "bevel")
						{
							strokeCorner = PathCorner.Beveled;
						}
					}
					else
					{
						strokeCorner = PathCorner.Round;
					}
				}
				else
				{
					strokeCorner = PathCorner.Tipped;
				}
				string text6 = this.styles.Evaluate("stroke-dasharray", inheritance);
				bool flag2 = text6 != null && text6 != "none";
				if (flag2)
				{
					string[] array = text6.Split(SVGDocument.whiteSpaceNumberChars, StringSplitOptions.RemoveEmptyEntries);
					int num = (((array.Length & 1) == 1) ? (array.Length * 2) : array.Length);
					stroke.Pattern = new float[num];
					for (int i = 0; i < array.Length; i++)
					{
						stroke.Pattern[i] = this.AttribLengthVal(array[i], node, "stroke-dasharray", 0f, SVGDocument.DimType.Length);
					}
					bool flag3 = num > array.Length;
					if (flag3)
					{
						for (int j = 0; j < array.Length; j++)
						{
							stroke.Pattern[j + array.Length] = stroke.Pattern[j];
						}
					}
					string text7 = this.styles.Evaluate("stroke-dashoffset", inheritance);
					stroke.PatternOffset = this.AttribLengthVal(text7, node, "stroke-dashoffset", 0f, SVGDocument.DimType.Length);
				}
				string text8 = this.styles.Evaluate("stroke-miterlimit", inheritance);
				stroke.TippedCornerLimit = this.AttribLengthVal(text8, node, "stroke-miterlimit", 4f, SVGDocument.DimType.Length);
				bool flag4 = stroke.TippedCornerLimit < 1f;
				if (flag4)
				{
					throw node.GetException("'stroke-miterlimit' should be greater or equal to 1");
				}
			}
			return stroke;
		}

		private void ParseID(XmlReaderIterator.Node node, SceneNode sceneNode)
		{
			string text = node["id"];
			bool flag = !string.IsNullOrEmpty(text);
			if (flag)
			{
				this.nodeIDs[text] = sceneNode;
				this.nodeStyleLayers[sceneNode] = this.styles.PeekLayer();
			}
		}

		private float ParseOpacity(SceneNode sceneNode)
		{
			float num = this.AttribFloatVal("opacity", 1f);
			bool flag = num != 1f && sceneNode != null;
			if (flag)
			{
				this.nodeOpacity[sceneNode] = num;
			}
			return num;
		}

		private void ParseClipAndMask(XmlReaderIterator.Node node, SceneNode sceneNode)
		{
			this.ParseClip(node, sceneNode);
			this.ParseMask(node, sceneNode);
		}

		private void ParseClip(XmlReaderIterator.Node node, SceneNode sceneNode)
		{
			string text = null;
			string text2 = this.styles.Evaluate("clip-path", Inheritance.None);
			bool flag = text2 != null;
			if (flag)
			{
				text = SVGAttribParser.ParseURLRef(text2);
			}
			bool flag2 = text == null;
			if (!flag2)
			{
				SceneNode sceneNode2 = SVGAttribParser.ParseRelativeRef(text, this.svgObjects) as SceneNode;
				bool flag3 = sceneNode2 == null && text.Length > 1 && text.StartsWith("#");
				if (flag3)
				{
					List<SVGDocument.PostponedClip> list;
					bool flag4 = !this.postponedClip.TryGetValue(text, out list);
					if (flag4)
					{
						list = new List<SVGDocument.PostponedClip>(1);
					}
					list.Add(new SVGDocument.PostponedClip
					{
						node = sceneNode
					});
					this.postponedClip[text.Substring(1)] = list;
				}
				else
				{
					bool flag5 = true;
					SVGDocument.ClipData clipData;
					bool flag6 = this.clipData.TryGetValue(sceneNode2, out clipData);
					if (flag6)
					{
						flag5 = clipData.WorldRelative;
					}
					this.ApplyClipper(sceneNode2, sceneNode, flag5);
				}
			}
		}

		private void ApplyClipper(SceneNode clipper, SceneNode target, bool worldRelative)
		{
			SceneNode sceneNode = clipper;
			bool flag = !worldRelative;
			if (flag)
			{
				Rect rect = VectorUtils.SceneNodeBounds(target);
				Matrix2D matrix2D = Matrix2D.Translate(rect.position) * Matrix2D.Scale(rect.size);
				sceneNode = new SceneNode
				{
					Children = new List<SceneNode> { clipper },
					Transform = matrix2D
				};
			}
			target.Clipper = sceneNode;
		}

		private void ParseMask(XmlReaderIterator.Node node, SceneNode sceneNode)
		{
			string text = null;
			string text2 = node["mask"];
			bool flag = text2 != null;
			if (flag)
			{
				text = SVGAttribParser.ParseURLRef(text2);
			}
			bool flag2 = text == null;
			if (!flag2)
			{
				SceneNode sceneNode2 = SVGAttribParser.ParseRelativeRef(text, this.svgObjects) as SceneNode;
				SceneNode sceneNode3 = sceneNode2;
				SVGDocument.MaskData maskData;
				bool flag3 = this.maskData.TryGetValue(sceneNode2, out maskData) && !maskData.ContentWorldRelative;
				if (flag3)
				{
					Rect rect = VectorUtils.SceneNodeBounds(sceneNode);
					Matrix2D matrix2D = Matrix2D.Translate(rect.position) * Matrix2D.Scale(rect.size);
					sceneNode3 = new SceneNode
					{
						Children = new List<SceneNode> { sceneNode2 },
						Transform = matrix2D
					};
				}
				sceneNode.Clipper = sceneNode3;
			}
		}

		private Texture2D DecodeTextureData(string dataURI)
		{
			int num = 5;
			int length = dataURI.Length;
			int num2 = num;
			while (num < length && dataURI[num] != ';' && dataURI[num] != ',')
			{
				num++;
			}
			string text = dataURI.Substring(num2, num - num2).ToLower();
			bool flag = text != "image/png" && text != "image/jpeg";
			Texture2D texture2D;
			if (flag)
			{
				texture2D = null;
			}
			else
			{
				while (num < length && dataURI[num] != ',')
				{
					num++;
				}
				num++;
				bool flag2 = num >= length;
				if (flag2)
				{
					texture2D = null;
				}
				else
				{
					byte[] array = Convert.FromBase64String(dataURI.Substring(num));
					Texture2D texture2D2 = new Texture2D(1, 1);
					bool flag3 = texture2D2.LoadImage(array);
					if (flag3)
					{
						texture2D = texture2D2;
					}
					else
					{
						texture2D = null;
					}
				}
			}
			return texture2D;
		}

		private void PostProcess(SceneNode root)
		{
			this.AdjustFills(root);
		}

		private void AdjustFills(SceneNode root)
		{
			List<SVGDocument.HierarchyUpdate> list = new List<SVGDocument.HierarchyUpdate>();
			foreach (VectorUtils.SceneNodeWorldTransform sceneNodeWorldTransform in VectorUtils.WorldTransformedSceneNodes(root, this.nodeOpacity))
			{
				bool flag = sceneNodeWorldTransform.Node.Shapes == null;
				if (!flag)
				{
					foreach (Shape shape in sceneNodeWorldTransform.Node.Shapes)
					{
						bool flag2 = shape.Fill != null;
						if (flag2)
						{
							string text;
							bool flag3 = this.postponedFills.TryGetValue(shape.Fill, out text);
							if (flag3)
							{
								IFill fill = SVGAttribParser.ParseRelativeRef(text, this.svgObjects) as IFill;
								bool flag4 = fill != null;
								if (flag4)
								{
									shape.Fill = fill;
								}
							}
						}
						Stroke stroke = shape.PathProps.Stroke;
						bool flag5 = stroke != null && stroke.Fill is GradientFill;
						if (flag5)
						{
							Matrix2D identity = Matrix2D.identity;
							this.AdjustGradientFill(sceneNodeWorldTransform.Node, sceneNodeWorldTransform.WorldTransform, stroke.Fill, shape.Contours, ref identity);
							stroke.FillTransform = identity;
						}
						bool flag6 = shape.Fill is GradientFill;
						if (flag6)
						{
							Matrix2D identity2 = Matrix2D.identity;
							this.AdjustGradientFill(sceneNodeWorldTransform.Node, sceneNodeWorldTransform.WorldTransform, shape.Fill, shape.Contours, ref identity2);
							shape.FillTransform = identity2;
						}
						else
						{
							bool flag7 = shape.Fill is PatternFill;
							if (flag7)
							{
								SceneNode sceneNode = this.AdjustPatternFill(sceneNodeWorldTransform.Node, sceneNodeWorldTransform.WorldTransform, shape);
								bool flag8 = sceneNode != null;
								if (flag8)
								{
									list.Add(new SVGDocument.HierarchyUpdate
									{
										Parent = sceneNodeWorldTransform.Parent,
										NewNode = sceneNode,
										ReplaceNode = sceneNodeWorldTransform.Node
									});
								}
							}
						}
					}
				}
			}
			foreach (SVGDocument.HierarchyUpdate hierarchyUpdate in list)
			{
				int num = hierarchyUpdate.Parent.Children.IndexOf(hierarchyUpdate.ReplaceNode);
				hierarchyUpdate.Parent.Children.RemoveAt(num);
				hierarchyUpdate.Parent.Children.Insert(num, hierarchyUpdate.NewNode);
			}
		}

		private void AdjustGradientFill(SceneNode node, Matrix2D worldTransform, IFill fill, BezierContour[] contours, ref Matrix2D computedTransform)
		{
			GradientFill gradientFill = fill as GradientFill;
			bool flag = fill == null || contours == null || contours.Length == 0;
			if (!flag)
			{
				Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
				foreach (BezierContour bezierContour in contours)
				{
					Rect rect = VectorUtils.Bounds(bezierContour.Segments);
					vector = Vector2.Min(vector, rect.min);
					vector2 = Vector2.Max(vector2, rect.max);
				}
				Rect rect2 = new Rect(vector, vector2 - vector);
				SVGDocument.GradientExData gradientExData = this.gradientExInfo[gradientFill];
				Vector2 containerSize = this.nodeGlobalSceneState[node].ContainerSize;
				Matrix2D matrix2D = Matrix2D.identity;
				this.currentContainerSize.Push(gradientExData.WorldRelative ? containerSize : Vector2.one);
				bool flag2 = gradientExData is SVGDocument.LinearGradientExData;
				if (flag2)
				{
					SVGDocument.LinearGradientExData linearGradientExData = (SVGDocument.LinearGradientExData)gradientExData;
					Vector2 vector3 = new Vector2(this.AttribLengthVal(linearGradientExData.X1, null, null, 0f, SVGDocument.DimType.Width), this.AttribLengthVal(linearGradientExData.Y1, null, null, 0f, SVGDocument.DimType.Height));
					Vector2 vector4 = new Vector2(this.AttribLengthVal(linearGradientExData.X2, null, null, this.currentContainerSize.Peek().x, SVGDocument.DimType.Width), this.AttribLengthVal(linearGradientExData.Y2, null, null, 0f, SVGDocument.DimType.Height));
					Vector2 vector5 = vector4 - vector3;
					float num = 1f / vector5.magnitude;
					Matrix2D matrix2D2 = Matrix2D.Scale(new Vector2(rect2.width * num, rect2.height * num));
					Matrix2D matrix2D3 = Matrix2D.RotateLH(Mathf.Atan2(vector5.y, vector5.x));
					Matrix2D matrix2D4 = Matrix2D.Translate(-vector3);
					matrix2D = matrix2D2 * matrix2D3 * matrix2D4;
				}
				else
				{
					bool flag3 = gradientExData is SVGDocument.RadialGradientExData;
					if (flag3)
					{
						SVGDocument.RadialGradientExData radialGradientExData = (SVGDocument.RadialGradientExData)gradientExData;
						Vector2 vector6 = this.currentContainerSize.Peek() * 0.5f;
						Vector2 vector7 = new Vector2(this.AttribLengthVal(radialGradientExData.Cx, null, null, vector6.x, SVGDocument.DimType.Width), this.AttribLengthVal(radialGradientExData.Cy, null, null, vector6.y, SVGDocument.DimType.Height));
						Vector2 vector8 = new Vector2(this.AttribLengthVal(radialGradientExData.Fx, null, null, vector7.x, SVGDocument.DimType.Width), this.AttribLengthVal(radialGradientExData.Fy, null, null, vector7.y, SVGDocument.DimType.Height));
						float num2 = this.AttribLengthVal(radialGradientExData.R, null, null, vector6.magnitude / 1.4142135f, SVGDocument.DimType.Length);
						bool flag4 = !radialGradientExData.Parsed;
						if (flag4)
						{
							gradientFill.RadialFocus = (vector8 - vector7) / num2;
							bool flag5 = gradientFill.RadialFocus.sqrMagnitude > 1f - VectorUtils.Epsilon;
							if (flag5)
							{
								gradientFill.RadialFocus = gradientFill.RadialFocus.normalized * (1f - VectorUtils.Epsilon);
							}
							radialGradientExData.Parsed = true;
						}
						matrix2D = Matrix2D.Scale(rect2.size * 0.5f / num2) * Matrix2D.Translate(new Vector2(num2, num2) - vector7);
					}
					else
					{
						string text = "Unsupported gradient type: ";
						SVGDocument.GradientExData gradientExData2 = gradientExData;
						Debug.LogError(text + ((gradientExData2 != null) ? gradientExData2.ToString() : null));
					}
				}
				this.currentContainerSize.Pop();
				Matrix2D matrix2D5 = (gradientExData.WorldRelative ? (Matrix2D.Translate(rect2.min) * Matrix2D.Scale(rect2.size)) : Matrix2D.identity);
				Vector2 vector9 = new Vector2(1f / rect2.width, 1f / rect2.height);
				computedTransform = Matrix2D.Scale(vector9) * matrix2D * gradientExData.FillTransform.Inverse() * matrix2D5;
			}
		}

		private SceneNode AdjustPatternFill(SceneNode node, Matrix2D worldTransform, Shape shape)
		{
			PatternFill patternFill = shape.Fill as PatternFill;
			bool flag = patternFill == null || Mathf.Abs(patternFill.Rect.width) < VectorUtils.Epsilon || Mathf.Abs(patternFill.Rect.height) < VectorUtils.Epsilon;
			SceneNode sceneNode;
			if (flag)
			{
				sceneNode = null;
			}
			else
			{
				SVGDocument.PatternData patternData = this.patternData[patternFill.Pattern];
				Rect rect = VectorUtils.SceneNodeBounds(node);
				Rect rect2 = patternFill.Rect;
				bool flag2 = !patternData.WorldRelative;
				if (flag2)
				{
					rect2.position *= rect.size;
					rect2.size *= rect.size;
				}
				SceneNode sceneNode2 = new SceneNode
				{
					Transform = node.Transform,
					Children = new List<SceneNode>(2)
				};
				node.Transform = Matrix2D.identity;
				SceneNode sceneNode3 = patternFill.Pattern;
				bool flag3 = !patternData.ContentWorldRelative;
				if (flag3)
				{
					sceneNode3 = new SceneNode
					{
						Transform = Matrix2D.Scale(rect.size),
						Children = new List<SceneNode> { patternFill.Pattern }
					};
				}
				this.PostProcess(sceneNode3);
				SceneNode sceneNode4 = new SceneNode
				{
					Transform = patternData.PatternTransform,
					Children = new List<SceneNode>(20)
				};
				SceneNode sceneNode5 = new SceneNode
				{
					Transform = Matrix2D.identity,
					Children = new List<SceneNode> { sceneNode4 },
					Clipper = node
				};
				Shape shape2 = new Shape();
				VectorUtils.MakeRectangleShape(shape2, new Rect(0f, 0f, rect2.width, rect2.height));
				SceneNode sceneNode6 = new SceneNode
				{
					Transform = Matrix2D.identity,
					Shapes = new List<Shape> { shape2 }
				};
				Rect rect3 = VectorUtils.SceneNodeBounds(node);
				Matrix2D matrix2D = patternData.PatternTransform.Inverse();
				Vector2[] array = new Vector2[]
				{
					matrix2D * new Vector2(rect3.xMin, rect3.yMin),
					matrix2D * new Vector2(rect3.xMax, rect3.yMin),
					matrix2D * new Vector2(rect3.xMax, rect3.yMax),
					matrix2D * new Vector2(rect3.xMin, rect3.yMax)
				};
				rect3 = VectorUtils.Bounds(array);
				float num = rect3.xMax / rect2.width;
				float num2 = rect3.yMax / rect2.height;
				bool flag4 = Mathf.Abs(rect2.width) < VectorUtils.Epsilon || Mathf.Abs(rect2.height) < VectorUtils.Epsilon || num * num2 > 5000f;
				if (flag4)
				{
					Debug.LogWarning("Ignoring pattern which would result in too many repetitions");
					sceneNode = null;
				}
				else
				{
					Vector2 position = rect2.position;
					float num3 = (float)((int)(rect3.x / rect2.width)) * rect2.width - rect2.width;
					float num4 = (float)((int)(rect3.y / rect2.height)) * rect2.height - rect2.height;
					for (float num5 = num4; num5 < rect3.yMax; num5 += rect2.height)
					{
						for (float num6 = num3; num6 < rect3.xMax; num6 += rect2.width)
						{
							SceneNode sceneNode7 = new SceneNode
							{
								Transform = Matrix2D.Translate(new Vector2(num6, num5) + position),
								Children = new List<SceneNode> { sceneNode3 },
								Clipper = sceneNode6
							};
							sceneNode4.Children.Add(sceneNode7);
						}
					}
					sceneNode2.Children.Add(sceneNode5);
					sceneNode2.Children.Add(node);
					sceneNode = sceneNode2;
				}
			}
			return sceneNode;
		}

		private void RemoveInvisibleNodes()
		{
			foreach (SVGDocument.NodeWithParent nodeWithParent in this.invisibleNodes)
			{
				bool flag = nodeWithParent.parent.Children != null;
				if (flag)
				{
					nodeWithParent.parent.Children.Remove(nodeWithParent.node);
				}
			}
		}

		private bool ShouldDeclareSupportedChildren(XmlReaderIterator.Node node)
		{
			return !this.subTags.ContainsKey(node.Name);
		}

		private void SupportElems(XmlReaderIterator.Node node, params SVGDocument.ElemHandler[] handlers)
		{
			SVGDocument.Handlers handlers2 = new SVGDocument.Handlers(handlers.Length);
			foreach (SVGDocument.ElemHandler elemHandler in handlers)
			{
				handlers2[elemHandler.Method.Name] = elemHandler;
			}
			this.subTags[node.Name] = handlers2;
		}

		internal const float SVGLengthFactor = 1.4142135f;

		private static char[] whiteSpaceNumberChars = " \r\n\t,".ToCharArray();

		private XmlReaderIterator docReader;

		private Scene scene;

		private float dpiScale;

		private int windowWidth;

		private int windowHeight;

		private Vector2 scenePos;

		private Vector2 sceneSize;

		private SVGDictionary svgObjects = new SVGDictionary();

		private Dictionary<string, SVGDocument.Handlers> subTags = new Dictionary<string, SVGDocument.Handlers>();

		private Dictionary<GradientFill, SVGDocument.GradientExData> gradientExInfo = new Dictionary<GradientFill, SVGDocument.GradientExData>();

		private Dictionary<SceneNode, SVGDocument.ViewBoxInfo> symbolViewBoxes = new Dictionary<SceneNode, SVGDocument.ViewBoxInfo>();

		private Dictionary<SceneNode, SVGDocument.NodeGlobalSceneState> nodeGlobalSceneState = new Dictionary<SceneNode, SVGDocument.NodeGlobalSceneState>();

		private Dictionary<SceneNode, float> nodeOpacity = new Dictionary<SceneNode, float>();

		private Dictionary<string, SceneNode> nodeIDs = new Dictionary<string, SceneNode>();

		private Dictionary<SceneNode, SVGStyleResolver.StyleLayer> nodeStyleLayers = new Dictionary<SceneNode, SVGStyleResolver.StyleLayer>();

		private Dictionary<SceneNode, SVGDocument.ClipData> clipData = new Dictionary<SceneNode, SVGDocument.ClipData>();

		private Dictionary<SceneNode, SVGDocument.PatternData> patternData = new Dictionary<SceneNode, SVGDocument.PatternData>();

		private Dictionary<SceneNode, SVGDocument.MaskData> maskData = new Dictionary<SceneNode, SVGDocument.MaskData>();

		private Dictionary<string, List<SVGDocument.NodeReferenceData>> postponedSymbolData = new Dictionary<string, List<SVGDocument.NodeReferenceData>>();

		private Dictionary<string, List<SVGDocument.PostponedStopData>> postponedStopData = new Dictionary<string, List<SVGDocument.PostponedStopData>>();

		private Dictionary<string, List<SVGDocument.PostponedClip>> postponedClip = new Dictionary<string, List<SVGDocument.PostponedClip>>();

		private SVGPostponedFills postponedFills = new SVGPostponedFills();

		private List<SVGDocument.NodeWithParent> invisibleNodes = new List<SVGDocument.NodeWithParent>();

		private Stack<Vector2> currentContainerSize = new Stack<Vector2>();

		private Stack<Vector2> currentViewBoxSize = new Stack<Vector2>();

		private Stack<SceneNode> currentSceneNode = new Stack<SceneNode>();

		private GradientFill currentGradientFill;

		private string currentGradientId;

		private string currentGradientLink;

		private SVGDocument.ElemHandler[] allElems;

		private HashSet<SVGDocument.ElemHandler> elemsToAddToHierarchy;

		private SVGStyleResolver styles = new SVGStyleResolver();

		private bool applyRootViewBox;

		internal Rect sceneViewport;

		private enum ViewBoxAlign
		{
			Min,
			Mid,
			Max
		}

		private enum ViewBoxAspectRatio
		{
			DontPreserve,
			FitLargestDim,
			FitSmallestDim
		}

		private struct ViewBoxInfo
		{
			public Rect ViewBox;

			public SVGDocument.ViewBoxAspectRatio AspectRatio;

			public SVGDocument.ViewBoxAlign AlignX;

			public SVGDocument.ViewBoxAlign AlignY;

			public bool IsEmpty;
		}

		private struct HierarchyUpdate
		{
			public SceneNode Parent;

			public SceneNode NewNode;

			public SceneNode ReplaceNode;
		}

		private delegate void ElemHandler();

		private class Handlers : Dictionary<string, SVGDocument.ElemHandler>
		{
			public Handlers(int capacity)
				: base(capacity)
			{
			}
		}

		private enum DimType
		{
			Width,
			Height,
			Length
		}

		private struct NodeGlobalSceneState
		{
			public Vector2 ContainerSize;
		}

		private class GradientExData
		{
			public bool WorldRelative;

			public Matrix2D FillTransform;
		}

		private class LinearGradientExData : SVGDocument.GradientExData
		{
			public string X1;

			public string Y1;

			public string X2;

			public string Y2;
		}

		private class RadialGradientExData : SVGDocument.GradientExData
		{
			public bool Parsed;

			public string Cx;

			public string Cy;

			public string Fx;

			public string Fy;

			public string R;
		}

		private struct ClipData
		{
			public bool WorldRelative;
		}

		private struct PatternData
		{
			public bool WorldRelative;

			public bool ContentWorldRelative;

			public Matrix2D PatternTransform;
		}

		private struct MaskData
		{
			public bool WorldRelative;

			public bool ContentWorldRelative;
		}

		private struct NodeWithParent
		{
			public SceneNode node;

			public SceneNode parent;
		}

		private struct NodeReferenceData
		{
			public SceneNode node;

			public Rect viewport;

			public string id;
		}

		private struct PostponedStopData
		{
			public GradientFill fill;
		}

		private struct PostponedClip
		{
			public SceneNode node;
		}
	}
}
