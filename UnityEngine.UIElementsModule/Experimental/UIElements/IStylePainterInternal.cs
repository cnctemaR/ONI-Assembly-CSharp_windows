using System;

namespace UnityEngine.Experimental.UIElements
{
	internal interface IStylePainterInternal : IStylePainter
	{
		void DrawRect(RectStylePainterParameters painterParams);

		void DrawTexture(TextureStylePainterParameters painterParams);

		void DrawText(TextStylePainterParameters painterParams);

		void DrawMesh(MeshStylePainterParameters painterParameters);

		void DrawImmediate(Action callback);

		void DrawBackground();

		void DrawBorder();

		void DrawText(string text);

		float opacity { get; set; }
	}
}
