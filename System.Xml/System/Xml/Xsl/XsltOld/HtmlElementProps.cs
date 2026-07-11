using System;
using System.Collections;

namespace System.Xml.Xsl.XsltOld
{
	internal class HtmlElementProps
	{
		public static HtmlElementProps Create(bool empty, bool abrParent, bool uriParent, bool noEntities, bool blockWS, bool head, bool nameParent)
		{
			return new HtmlElementProps
			{
				empty = empty,
				abrParent = abrParent,
				uriParent = uriParent,
				noEntities = noEntities,
				blockWS = blockWS,
				head = head,
				nameParent = nameParent
			};
		}

		public bool Empty
		{
			get
			{
				return this.empty;
			}
		}

		public bool AbrParent
		{
			get
			{
				return this.abrParent;
			}
		}

		public bool UriParent
		{
			get
			{
				return this.uriParent;
			}
		}

		public bool NoEntities
		{
			get
			{
				return this.noEntities;
			}
		}

		public bool BlockWS
		{
			get
			{
				return this.blockWS;
			}
		}

		public bool Head
		{
			get
			{
				return this.head;
			}
		}

		public bool NameParent
		{
			get
			{
				return this.nameParent;
			}
		}

		public static HtmlElementProps GetProps(string name)
		{
			return (HtmlElementProps)HtmlElementProps.s_table[name];
		}

		private static Hashtable CreatePropsTable()
		{
			bool flag = false;
			bool flag2 = true;
			return new Hashtable(71, StringComparer.OrdinalIgnoreCase)
			{
				{
					"a",
					HtmlElementProps.Create(flag, flag, flag2, flag, flag, flag, flag2)
				},
				{
					"address",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"applet",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"area",
					HtmlElementProps.Create(flag2, flag2, flag2, flag, flag2, flag, flag)
				},
				{
					"base",
					HtmlElementProps.Create(flag2, flag, flag2, flag, flag2, flag, flag)
				},
				{
					"basefont",
					HtmlElementProps.Create(flag2, flag, flag, flag, flag2, flag, flag)
				},
				{
					"blockquote",
					HtmlElementProps.Create(flag, flag, flag2, flag, flag2, flag, flag)
				},
				{
					"body",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"br",
					HtmlElementProps.Create(flag2, flag, flag, flag, flag, flag, flag)
				},
				{
					"button",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag, flag, flag)
				},
				{
					"caption",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"center",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"col",
					HtmlElementProps.Create(flag2, flag, flag, flag, flag2, flag, flag)
				},
				{
					"colgroup",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"dd",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"del",
					HtmlElementProps.Create(flag, flag, flag2, flag, flag2, flag, flag)
				},
				{
					"dir",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"div",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"dl",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"dt",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"fieldset",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"font",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"form",
					HtmlElementProps.Create(flag, flag, flag2, flag, flag2, flag, flag)
				},
				{
					"frame",
					HtmlElementProps.Create(flag2, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"frameset",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"h1",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"h2",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"h3",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"h4",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"h5",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"h6",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"head",
					HtmlElementProps.Create(flag, flag, flag2, flag, flag2, flag2, flag)
				},
				{
					"hr",
					HtmlElementProps.Create(flag2, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"html",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"iframe",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"img",
					HtmlElementProps.Create(flag2, flag2, flag2, flag, flag, flag, flag)
				},
				{
					"input",
					HtmlElementProps.Create(flag2, flag2, flag2, flag, flag, flag, flag)
				},
				{
					"ins",
					HtmlElementProps.Create(flag, flag, flag2, flag, flag2, flag, flag)
				},
				{
					"isindex",
					HtmlElementProps.Create(flag2, flag, flag, flag, flag2, flag, flag)
				},
				{
					"legend",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"li",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"link",
					HtmlElementProps.Create(flag2, flag, flag2, flag, flag2, flag, flag)
				},
				{
					"map",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"menu",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"meta",
					HtmlElementProps.Create(flag2, flag, flag, flag, flag2, flag, flag)
				},
				{
					"noframes",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"noscript",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"object",
					HtmlElementProps.Create(flag, flag2, flag2, flag, flag, flag, flag)
				},
				{
					"ol",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"optgroup",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"option",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"p",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"param",
					HtmlElementProps.Create(flag2, flag, flag, flag, flag2, flag, flag)
				},
				{
					"pre",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"q",
					HtmlElementProps.Create(flag, flag, flag2, flag, flag, flag, flag)
				},
				{
					"s",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"script",
					HtmlElementProps.Create(flag, flag2, flag2, flag2, flag, flag, flag)
				},
				{
					"select",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag, flag, flag)
				},
				{
					"strike",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"style",
					HtmlElementProps.Create(flag, flag, flag, flag2, flag2, flag, flag)
				},
				{
					"table",
					HtmlElementProps.Create(flag, flag, flag2, flag, flag2, flag, flag)
				},
				{
					"tbody",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"td",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"textarea",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag, flag, flag)
				},
				{
					"tfoot",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"th",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"thead",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"title",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"tr",
					HtmlElementProps.Create(flag, flag, flag, flag, flag2, flag, flag)
				},
				{
					"ul",
					HtmlElementProps.Create(flag, flag2, flag, flag, flag2, flag, flag)
				},
				{
					"xmp",
					HtmlElementProps.Create(flag, flag, flag, flag, flag, flag, flag)
				}
			};
		}

		private bool empty;

		private bool abrParent;

		private bool uriParent;

		private bool noEntities;

		private bool blockWS;

		private bool head;

		private bool nameParent;

		private static Hashtable s_table = HtmlElementProps.CreatePropsTable();
	}
}
