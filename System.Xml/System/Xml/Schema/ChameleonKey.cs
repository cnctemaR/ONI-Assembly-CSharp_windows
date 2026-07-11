using System;

namespace System.Xml.Schema
{
	internal class ChameleonKey
	{
		public ChameleonKey(string ns, XmlSchema originalSchema)
		{
			this.targetNS = ns;
			this.chameleonLocation = originalSchema.BaseUri;
			if (this.chameleonLocation.OriginalString.Length == 0)
			{
				this.originalSchema = originalSchema;
			}
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				this.hashCode = this.targetNS.GetHashCode() + this.chameleonLocation.GetHashCode() + ((this.originalSchema == null) ? 0 : this.originalSchema.GetHashCode());
			}
			return this.hashCode;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			ChameleonKey chameleonKey = obj as ChameleonKey;
			return chameleonKey != null && (this.targetNS.Equals(chameleonKey.targetNS) && this.chameleonLocation.Equals(chameleonKey.chameleonLocation)) && this.originalSchema == chameleonKey.originalSchema;
		}

		internal string targetNS;

		internal Uri chameleonLocation;

		internal XmlSchema originalSchema;

		private int hashCode;
	}
}
