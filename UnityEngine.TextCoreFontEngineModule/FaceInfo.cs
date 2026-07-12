using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore
{
	[UsedByNativeCode]
	[Serializable]
	public struct FaceInfo
	{
		internal int faceIndex
		{
			get
			{
				return this.m_FaceIndex;
			}
			set
			{
				this.m_FaceIndex = value;
			}
		}

		public string familyName
		{
			get
			{
				return this.m_FamilyName;
			}
			set
			{
				this.m_FamilyName = value;
			}
		}

		public string styleName
		{
			get
			{
				return this.m_StyleName;
			}
			set
			{
				this.m_StyleName = value;
			}
		}

		public int pointSize
		{
			get
			{
				return this.m_PointSize;
			}
			set
			{
				this.m_PointSize = value;
			}
		}

		public float scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				this.m_Scale = value;
			}
		}

		internal int unitsPerEM
		{
			get
			{
				return this.m_UnitsPerEM;
			}
			set
			{
				this.m_UnitsPerEM = value;
			}
		}

		public float lineHeight
		{
			get
			{
				return this.m_LineHeight;
			}
			set
			{
				this.m_LineHeight = value;
			}
		}

		public float ascentLine
		{
			get
			{
				return this.m_AscentLine;
			}
			set
			{
				this.m_AscentLine = value;
			}
		}

		public float capLine
		{
			get
			{
				return this.m_CapLine;
			}
			set
			{
				this.m_CapLine = value;
			}
		}

		public float meanLine
		{
			get
			{
				return this.m_MeanLine;
			}
			set
			{
				this.m_MeanLine = value;
			}
		}

		public float baseline
		{
			get
			{
				return this.m_Baseline;
			}
			set
			{
				this.m_Baseline = value;
			}
		}

		public float descentLine
		{
			get
			{
				return this.m_DescentLine;
			}
			set
			{
				this.m_DescentLine = value;
			}
		}

		public float superscriptOffset
		{
			get
			{
				return this.m_SuperscriptOffset;
			}
			set
			{
				this.m_SuperscriptOffset = value;
			}
		}

		public float superscriptSize
		{
			get
			{
				return this.m_SuperscriptSize;
			}
			set
			{
				this.m_SuperscriptSize = value;
			}
		}

		public float subscriptOffset
		{
			get
			{
				return this.m_SubscriptOffset;
			}
			set
			{
				this.m_SubscriptOffset = value;
			}
		}

		public float subscriptSize
		{
			get
			{
				return this.m_SubscriptSize;
			}
			set
			{
				this.m_SubscriptSize = value;
			}
		}

		public float underlineOffset
		{
			get
			{
				return this.m_UnderlineOffset;
			}
			set
			{
				this.m_UnderlineOffset = value;
			}
		}

		public float underlineThickness
		{
			get
			{
				return this.m_UnderlineThickness;
			}
			set
			{
				this.m_UnderlineThickness = value;
			}
		}

		public float strikethroughOffset
		{
			get
			{
				return this.m_StrikethroughOffset;
			}
			set
			{
				this.m_StrikethroughOffset = value;
			}
		}

		public float strikethroughThickness
		{
			get
			{
				return this.m_StrikethroughThickness;
			}
			set
			{
				this.m_StrikethroughThickness = value;
			}
		}

		public float tabWidth
		{
			get
			{
				return this.m_TabWidth;
			}
			set
			{
				this.m_TabWidth = value;
			}
		}

		internal FaceInfo(string familyName, string styleName, int pointSize, float scale, int unitsPerEM, float lineHeight, float ascentLine, float capLine, float meanLine, float baseline, float descentLine, float superscriptOffset, float superscriptSize, float subscriptOffset, float subscriptSize, float underlineOffset, float underlineThickness, float strikethroughOffset, float strikethroughThickness, float tabWidth)
		{
			this.m_FaceIndex = 0;
			this.m_FamilyName = familyName;
			this.m_StyleName = styleName;
			this.m_PointSize = pointSize;
			this.m_Scale = scale;
			this.m_UnitsPerEM = unitsPerEM;
			this.m_LineHeight = lineHeight;
			this.m_AscentLine = ascentLine;
			this.m_CapLine = capLine;
			this.m_MeanLine = meanLine;
			this.m_Baseline = baseline;
			this.m_DescentLine = descentLine;
			this.m_SuperscriptOffset = superscriptOffset;
			this.m_SuperscriptSize = superscriptSize;
			this.m_SubscriptOffset = subscriptOffset;
			this.m_SubscriptSize = subscriptSize;
			this.m_UnderlineOffset = underlineOffset;
			this.m_UnderlineThickness = underlineThickness;
			this.m_StrikethroughOffset = strikethroughOffset;
			this.m_StrikethroughThickness = strikethroughThickness;
			this.m_TabWidth = tabWidth;
		}

		public bool Compare(FaceInfo other)
		{
			return this.familyName == other.familyName && this.styleName == other.styleName && this.faceIndex == other.faceIndex && this.pointSize == other.pointSize && FontEngineUtilities.Approximately(this.scale, other.scale) && FontEngineUtilities.Approximately((float)this.unitsPerEM, (float)other.unitsPerEM) && FontEngineUtilities.Approximately(this.lineHeight, other.lineHeight) && FontEngineUtilities.Approximately(this.ascentLine, other.ascentLine) && FontEngineUtilities.Approximately(this.capLine, other.capLine) && FontEngineUtilities.Approximately(this.meanLine, other.meanLine) && FontEngineUtilities.Approximately(this.baseline, other.baseline) && FontEngineUtilities.Approximately(this.descentLine, other.descentLine) && FontEngineUtilities.Approximately(this.superscriptOffset, other.superscriptOffset) && FontEngineUtilities.Approximately(this.superscriptSize, other.superscriptSize) && FontEngineUtilities.Approximately(this.subscriptOffset, other.subscriptOffset) && FontEngineUtilities.Approximately(this.subscriptSize, other.subscriptSize) && FontEngineUtilities.Approximately(this.underlineOffset, other.underlineOffset) && FontEngineUtilities.Approximately(this.underlineThickness, other.underlineThickness) && FontEngineUtilities.Approximately(this.strikethroughOffset, other.strikethroughOffset) && FontEngineUtilities.Approximately(this.strikethroughThickness, other.strikethroughThickness) && FontEngineUtilities.Approximately(this.tabWidth, other.tabWidth);
		}

		[SerializeField]
		[NativeName("faceIndex")]
		private int m_FaceIndex;

		[SerializeField]
		[NativeName("familyName")]
		private string m_FamilyName;

		[SerializeField]
		[NativeName("styleName")]
		private string m_StyleName;

		[SerializeField]
		[NativeName("pointSize")]
		private int m_PointSize;

		[NativeName("scale")]
		[SerializeField]
		private float m_Scale;

		[SerializeField]
		[NativeName("unitsPerEM")]
		private int m_UnitsPerEM;

		[NativeName("lineHeight")]
		[SerializeField]
		private float m_LineHeight;

		[NativeName("ascentLine")]
		[SerializeField]
		private float m_AscentLine;

		[NativeName("capLine")]
		[SerializeField]
		private float m_CapLine;

		[SerializeField]
		[NativeName("meanLine")]
		private float m_MeanLine;

		[SerializeField]
		[NativeName("baseline")]
		private float m_Baseline;

		[NativeName("descentLine")]
		[SerializeField]
		private float m_DescentLine;

		[NativeName("superscriptOffset")]
		[SerializeField]
		private float m_SuperscriptOffset;

		[SerializeField]
		[NativeName("superscriptSize")]
		private float m_SuperscriptSize;

		[SerializeField]
		[NativeName("subscriptOffset")]
		private float m_SubscriptOffset;

		[SerializeField]
		[NativeName("subscriptSize")]
		private float m_SubscriptSize;

		[NativeName("underlineOffset")]
		[SerializeField]
		private float m_UnderlineOffset;

		[SerializeField]
		[NativeName("underlineThickness")]
		private float m_UnderlineThickness;

		[SerializeField]
		[NativeName("strikethroughOffset")]
		private float m_StrikethroughOffset;

		[SerializeField]
		[NativeName("strikethroughThickness")]
		private float m_StrikethroughThickness;

		[SerializeField]
		[NativeName("tabWidth")]
		private float m_TabWidth;
	}
}
