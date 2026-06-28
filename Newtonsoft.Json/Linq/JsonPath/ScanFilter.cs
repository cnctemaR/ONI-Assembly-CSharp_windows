using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class ScanFilter : PathFilter
	{
		public string Name { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken root in current)
			{
				if (this.Name == null)
				{
					yield return root;
				}
				JToken value = root;
				JToken container = root;
				for (;;)
				{
					if (container != null && container.HasValues)
					{
						value = container.First;
					}
					else
					{
						while (value != null && value != root && value == value.Parent.Last)
						{
							value = value.Parent;
						}
						if (value == null || value == root)
						{
							break;
						}
						value = value.Next;
					}
					JProperty e = value as JProperty;
					if (e != null)
					{
						if (e.Name == this.Name)
						{
							yield return e.Value;
						}
					}
					else if (this.Name == null)
					{
						yield return value;
					}
					container = value as JContainer;
				}
			}
			yield break;
		}
	}
}
