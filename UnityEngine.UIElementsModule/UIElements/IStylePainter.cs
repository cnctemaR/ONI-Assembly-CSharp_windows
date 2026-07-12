using System;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
	internal interface IStylePainter
	{
		MeshWriteData DrawMesh(int vertexCount, int indexCount, Texture texture, Material material, MeshGenerationContext.MeshFlags flags);

		void DrawText(TextElement te);

		void DrawText(string text, Vector2 pos, float fontSize, Color color, FontAsset font);

		void DrawRectangle(MeshGenerationContextUtils.RectangleParams rectParams);

		void DrawBorder(MeshGenerationContextUtils.BorderParams borderParams);

		void DrawImmediate(Action callback, bool cullingEnabled);

		void DrawVectorImage(VectorImage vectorImage, Vector2 pos, Angle rotationAngle, Vector2 scale);

		VisualElement visualElement { get; }
	}
}
