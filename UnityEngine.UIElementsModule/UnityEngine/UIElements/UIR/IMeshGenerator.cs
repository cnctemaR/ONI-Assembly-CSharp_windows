using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements.UIR
{
	internal interface IMeshGenerator
	{
		VisualElement currentElement { get; set; }

		TextJobSystem textJobSystem { get; set; }

		void DrawText(List<NativeSlice<Vertex>> vertices, List<NativeSlice<ushort>> indices, List<Texture2D> atlases, List<GlyphRenderMode> renderModes, List<float> sdfScales);

		void DrawText(List<NativeSlice<Vertex>> vertices, List<NativeSlice<ushort>> indices, List<Material> materials, List<GlyphRenderMode> renderModes);

		void DrawText(string text, Vector2 pos, float fontSize, Color color, FontAsset font);

		void DrawRectangle(MeshGenerator.RectangleParams rectParams);

		void DrawBorder(MeshGenerator.BorderParams borderParams);

		void DrawVectorImage(VectorImage vectorImage, Vector2 offset, Angle rotationAngle, Vector2 scale);

		void DrawRectangleRepeat(MeshGenerator.RectangleParams rectParams, Rect totalRect, float scaledPixelsPerPoint);

		void ScheduleJobs(MeshGenerationContext mgc);
	}
}
