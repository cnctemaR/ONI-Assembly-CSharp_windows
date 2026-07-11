using System;
using System.Collections.Generic;

public class SnapOn : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.kanimController = base.GetComponent<KAnimControllerBase>();
	}

	protected override void OnSpawn()
	{
		foreach (SnapOn.SnapPoint snapPoint in this.snapPoints)
		{
			if (snapPoint.automatic)
			{
				this.DoAttachSnapOn(snapPoint);
			}
		}
	}

	public void AttachSnapOnByName(string name)
	{
		foreach (SnapOn.SnapPoint snapPoint in this.snapPoints)
		{
			if (snapPoint.pointName == name)
			{
				HashedString context = base.GetComponent<AnimEventHandler>().GetContext();
				if (!context.IsValid || !snapPoint.context.IsValid || context == snapPoint.context)
				{
					this.DoAttachSnapOn(snapPoint);
				}
			}
		}
	}

	public void DetachSnapOnByName(string name)
	{
		foreach (SnapOn.SnapPoint snapPoint in this.snapPoints)
		{
			if (snapPoint.pointName == name)
			{
				HashedString context = base.GetComponent<AnimEventHandler>().GetContext();
				if (!context.IsValid || !snapPoint.context.IsValid || context == snapPoint.context)
				{
					base.GetComponent<SymbolOverrideController>().RemoveSymbolOverride(snapPoint.overrideSymbol, 5);
					this.kanimController.SetSymbolVisiblity(snapPoint.overrideSymbol, false);
					break;
				}
			}
		}
	}

	private void DoAttachSnapOn(SnapOn.SnapPoint point)
	{
		SnapOn.OverrideEntry overrideEntry = null;
		KAnimFile kanimFile = point.buildFile;
		string text = "";
		if (this.overrideMap.TryGetValue(point.pointName, out overrideEntry))
		{
			kanimFile = overrideEntry.buildFile;
			text = overrideEntry.symbolName;
		}
		KAnim.Build.Symbol symbol = SnapOn.GetSymbol(kanimFile, text);
		base.GetComponent<SymbolOverrideController>().AddSymbolOverride(point.overrideSymbol, symbol, 5);
		this.kanimController.SetSymbolVisiblity(point.overrideSymbol, true);
	}

	private static KAnim.Build.Symbol GetSymbol(KAnimFile anim_file, string symbol_name)
	{
		KAnim.Build.Symbol symbol = anim_file.GetData().build.symbols[0];
		KAnimHashedString kanimHashedString = new KAnimHashedString(symbol_name);
		foreach (KAnim.Build.Symbol symbol2 in anim_file.GetData().build.symbols)
		{
			if (symbol2.hash == kanimHashedString)
			{
				symbol = symbol2;
				break;
			}
		}
		return symbol;
	}

	public void AddOverride(string point_name, KAnimFile build_override, string symbol_name)
	{
		this.overrideMap[point_name] = new SnapOn.OverrideEntry
		{
			buildFile = build_override,
			symbolName = symbol_name
		};
	}

	public void RemoveOverride(string point_name)
	{
		this.overrideMap.Remove(point_name);
	}

	private KAnimControllerBase kanimController;

	public List<SnapOn.SnapPoint> snapPoints = new List<SnapOn.SnapPoint>();

	private Dictionary<string, SnapOn.OverrideEntry> overrideMap = new Dictionary<string, SnapOn.OverrideEntry>();

	[Serializable]
	public class SnapPoint
	{
		public string pointName;

		public bool automatic = true;

		public HashedString context;

		public KAnimFile buildFile;

		public HashedString overrideSymbol;
	}

	public class OverrideEntry
	{
		public KAnimFile buildFile;

		public string symbolName;
	}
}
