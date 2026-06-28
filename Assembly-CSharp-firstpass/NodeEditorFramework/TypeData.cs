using System;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public class TypeData
	{
		internal TypeData(IConnectionTypeDeclaration typeDecl)
		{
			this.Identifier = typeDecl.Identifier;
			this.declaration = typeDecl;
			this.Type = this.declaration.Type;
			this.Color = this.declaration.Color;
			this.InKnobTex = ResourceManager.GetTintedTexture(this.declaration.InKnobTex, this.Color);
			this.OutKnobTex = ResourceManager.GetTintedTexture(this.declaration.OutKnobTex, this.Color);
			if (!this.isValid())
			{
				throw new DataMisalignedException("Type Declaration " + typeDecl.Identifier + " contains invalid data!");
			}
		}

		public TypeData(Type type)
		{
			this.Identifier = type.Name;
			this.declaration = null;
			this.Type = type;
			this.Color = Color.white;
			int hashCode = type.GetHashCode();
			byte[] bytes = BitConverter.GetBytes(hashCode);
			this.Color = new Color(Mathf.Pow((float)bytes[0] / 255f, 0.5f), Mathf.Pow((float)bytes[1] / 255f, 0.5f), Mathf.Pow((float)bytes[2] / 255f, 0.5f));
			this.InKnobTex = ResourceManager.GetTintedTexture("Textures/In_Knob.png", this.Color);
			this.OutKnobTex = ResourceManager.GetTintedTexture("Textures/Out_Knob.png", this.Color);
		}

		public string Identifier { get; private set; }

		public Type Type { get; private set; }

		public Color Color { get; private set; }

		public Texture2D InKnobTex { get; private set; }

		public Texture2D OutKnobTex { get; private set; }

		public bool isValid()
		{
			return this.Type != null && this.InKnobTex != null && this.OutKnobTex != null;
		}

		private IConnectionTypeDeclaration declaration;
	}
}
