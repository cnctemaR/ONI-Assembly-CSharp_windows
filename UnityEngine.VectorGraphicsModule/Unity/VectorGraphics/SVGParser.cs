using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

namespace Unity.VectorGraphics
{
	public class SVGParser
	{
		public static SVGParser.SceneInfo ImportSVG(TextReader textReader, float dpi = 0f, float pixelsPerUnit = 1f, int windowWidth = 0, int windowHeight = 0, bool clipViewport = false)
		{
			ViewportOptions viewportOptions = (clipViewport ? ViewportOptions.PreserveViewport : ViewportOptions.DontPreserve);
			return SVGParser.ImportSVG(textReader, viewportOptions, dpi, pixelsPerUnit, windowWidth, windowHeight);
		}

		public static SVGParser.SceneInfo ImportSVG(TextReader textReader, ViewportOptions viewportOptions, float dpi = 0f, float pixelsPerUnit = 1f, int windowWidth = 0, int windowHeight = 0)
		{
			Scene scene = new Scene();
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreProcessingInstructions = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;
			xmlReaderSettings.ValidationFlags = XmlSchemaValidationFlags.None;
			xmlReaderSettings.ValidationType = ValidationType.None;
			xmlReaderSettings.XmlResolver = null;
			bool flag = dpi == 0f;
			if (flag)
			{
				dpi = Screen.dpi;
			}
			SVGDocument svgdocument;
			Dictionary<SceneNode, float> nodeOpacities;
			Dictionary<string, SceneNode> nodeIDs;
			using (XmlReader xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
			{
				bool flag2 = viewportOptions == ViewportOptions.PreserveViewport || viewportOptions == ViewportOptions.OnlyApplyRootViewBox;
				svgdocument = new SVGDocument(xmlReader, dpi, scene, windowWidth, windowHeight, flag2);
				svgdocument.Import();
				nodeOpacities = svgdocument.NodeOpacities;
				nodeIDs = svgdocument.NodeIDs;
			}
			float num = 1f / pixelsPerUnit;
			bool flag3 = num != 1f && scene != null && scene.Root != null;
			if (flag3)
			{
				scene.Root.Transform = scene.Root.Transform * Matrix2D.Scale(new Vector2(num, num));
			}
			bool flag4 = viewportOptions == ViewportOptions.PreserveViewport && scene != null && scene.Root != null;
			if (flag4)
			{
				Rect rect = VectorUtils.SceneNodeBounds(scene.Root);
				bool flag5 = !svgdocument.sceneViewport.Contains(rect.min) || !svgdocument.sceneViewport.Contains(rect.max);
				if (flag5)
				{
					Shape shape = new Shape();
					VectorUtils.MakeRectangleShape(shape, svgdocument.sceneViewport);
					scene.Root = new SceneNode
					{
						Children = new List<SceneNode> { scene.Root },
						Clipper = new SceneNode
						{
							Shapes = new List<Shape> { shape }
						}
					};
				}
			}
			return new SVGParser.SceneInfo(scene, svgdocument.sceneViewport, nodeOpacities, nodeIDs);
		}

		public struct SceneInfo
		{
			internal SceneInfo(Scene scene, Rect sceneViewport, Dictionary<SceneNode, float> nodeOpacities, Dictionary<string, SceneNode> nodeIDs)
			{
				this.Scene = scene;
				this.SceneViewport = sceneViewport;
				this.NodeOpacity = nodeOpacities;
				this.NodeIDs = nodeIDs;
			}

			public readonly Scene Scene { get; }

			public readonly Rect SceneViewport { get; }

			public readonly Dictionary<SceneNode, float> NodeOpacity { get; }

			public readonly Dictionary<string, SceneNode> NodeIDs { get; }
		}
	}
}
