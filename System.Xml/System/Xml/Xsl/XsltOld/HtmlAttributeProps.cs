using System;
using System.Collections;

namespace System.Xml.Xsl.XsltOld
{
	internal class HtmlAttributeProps
	{
		public static HtmlAttributeProps Create(bool abr, bool uri, bool name)
		{
			return new HtmlAttributeProps
			{
				abr = abr,
				uri = uri,
				name = name
			};
		}

		public bool Abr
		{
			get
			{
				return this.abr;
			}
		}

		public bool Uri
		{
			get
			{
				return this.uri;
			}
		}

		public bool Name
		{
			get
			{
				return this.name;
			}
		}

		public static HtmlAttributeProps GetProps(string name)
		{
			return (HtmlAttributeProps)HtmlAttributeProps.s_table[name];
		}

		private static Hashtable CreatePropsTable()
		{
			bool flag = false;
			bool flag2 = true;
			return new Hashtable(26, StringComparer.OrdinalIgnoreCase)
			{
				{
					"action",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"checked",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"cite",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"classid",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"codebase",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"compact",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"data",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"datasrc",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"declare",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"defer",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"disabled",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"for",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"href",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"ismap",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"longdesc",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"multiple",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"name",
					HtmlAttributeProps.Create(flag, flag, flag2)
				},
				{
					"nohref",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"noresize",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"noshade",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"nowrap",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"profile",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"readonly",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"selected",
					HtmlAttributeProps.Create(flag2, flag, flag)
				},
				{
					"src",
					HtmlAttributeProps.Create(flag, flag2, flag)
				},
				{
					"usemap",
					HtmlAttributeProps.Create(flag, flag2, flag)
				}
			};
		}

		private bool abr;

		private bool uri;

		private bool name;

		private static Hashtable s_table = HtmlAttributeProps.CreatePropsTable();
	}
}
