using System;
using System.Collections;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml
{
	internal class DTDEntityDeclaration : DTDEntityBase
	{
		internal DTDEntityDeclaration(DTDObjectModel root)
			: base(root)
		{
		}

		public string NotationName
		{
			get
			{
				return this.notationName;
			}
			set
			{
				this.notationName = value;
			}
		}

		public bool HasExternalReference
		{
			get
			{
				if (!this.scanned)
				{
					this.ScanEntityValue(new ArrayList());
				}
				return this.hasExternalReference;
			}
		}

		public string EntityValue
		{
			get
			{
				if (base.IsInvalid)
				{
					return string.Empty;
				}
				if (base.PublicId == null && base.SystemId == null && base.LiteralEntityValue == null)
				{
					return string.Empty;
				}
				if (this.entityValue == null)
				{
					if (this.NotationName != null)
					{
						this.entityValue = string.Empty;
					}
					else if (base.SystemId == null || base.SystemId == string.Empty)
					{
						this.entityValue = base.ReplacementText;
						if (this.entityValue == null)
						{
							this.entityValue = string.Empty;
						}
					}
					else
					{
						this.entityValue = base.ReplacementText;
					}
					this.ScanEntityValue(new ArrayList());
				}
				return this.entityValue;
			}
		}

		public void ScanEntityValue(ArrayList refs)
		{
			string text = this.EntityValue;
			if (base.SystemId != null)
			{
				this.hasExternalReference = true;
			}
			if (this.recursed)
			{
				throw base.NotWFError("Entity recursion was found.");
			}
			this.recursed = true;
			if (this.scanned)
			{
				foreach (object obj in refs)
				{
					string text2 = (string)obj;
					if (this.ReferencingEntities.Contains(text2))
					{
						throw base.NotWFError(string.Format("Nested entity was found between {0} and {1}", text2, base.Name));
					}
				}
				this.recursed = false;
				return;
			}
			int num = text.Length;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				char c = text[i];
				if (c != '&')
				{
					if (c == ';')
					{
						if (num2 != 0)
						{
							string text3 = text.Substring(num2, i - num2);
							if (text3.Length == 0)
							{
								throw base.NotWFError("Entity reference name is missing.");
							}
							if (text3[0] != '#')
							{
								if (XmlChar.GetPredefinedEntity(text3) < 0)
								{
									this.ReferencingEntities.Add(text3);
									DTDEntityDeclaration dtdentityDeclaration = base.Root.EntityDecls[text3];
									if (dtdentityDeclaration != null)
									{
										if (dtdentityDeclaration.SystemId != null)
										{
											this.hasExternalReference = true;
										}
										refs.Add(base.Name);
										dtdentityDeclaration.ScanEntityValue(refs);
										foreach (object obj2 in dtdentityDeclaration.ReferencingEntities)
										{
											string text4 = (string)obj2;
											this.ReferencingEntities.Add(text4);
										}
										refs.Remove(base.Name);
										text = text.Remove(num2 - 1, text3.Length + 2);
										text = text.Insert(num2 - 1, dtdentityDeclaration.EntityValue);
										i -= text3.Length + 1;
										num = text.Length;
									}
									num2 = 0;
								}
							}
						}
					}
				}
				else
				{
					num2 = i + 1;
				}
			}
			if (num2 != 0)
			{
				base.Root.AddError(new XmlSchemaException("Invalid reference character '&' is specified.", this.LineNumber, this.LinePosition, null, this.BaseURI, null));
			}
			this.scanned = true;
			this.recursed = false;
		}

		private string entityValue;

		private string notationName;

		private ArrayList ReferencingEntities = new ArrayList();

		private bool scanned;

		private bool recursed;

		private bool hasExternalReference;
	}
}
