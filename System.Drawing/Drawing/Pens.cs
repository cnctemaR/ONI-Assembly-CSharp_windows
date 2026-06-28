using System;

namespace System.Drawing
{
	public sealed class Pens
	{
		private Pens()
		{
		}

		public static Pen AliceBlue
		{
			get
			{
				if (Pens.aliceblue == null)
				{
					Pens.aliceblue = new Pen(Color.AliceBlue);
					Pens.aliceblue.isModifiable = false;
				}
				return Pens.aliceblue;
			}
		}

		public static Pen AntiqueWhite
		{
			get
			{
				if (Pens.antiquewhite == null)
				{
					Pens.antiquewhite = new Pen(Color.AntiqueWhite);
					Pens.antiquewhite.isModifiable = false;
				}
				return Pens.antiquewhite;
			}
		}

		public static Pen Aqua
		{
			get
			{
				if (Pens.aqua == null)
				{
					Pens.aqua = new Pen(Color.Aqua);
					Pens.aqua.isModifiable = false;
				}
				return Pens.aqua;
			}
		}

		public static Pen Aquamarine
		{
			get
			{
				if (Pens.aquamarine == null)
				{
					Pens.aquamarine = new Pen(Color.Aquamarine);
					Pens.aquamarine.isModifiable = false;
				}
				return Pens.aquamarine;
			}
		}

		public static Pen Azure
		{
			get
			{
				if (Pens.azure == null)
				{
					Pens.azure = new Pen(Color.Azure);
					Pens.azure.isModifiable = false;
				}
				return Pens.azure;
			}
		}

		public static Pen Beige
		{
			get
			{
				if (Pens.beige == null)
				{
					Pens.beige = new Pen(Color.Beige);
					Pens.beige.isModifiable = false;
				}
				return Pens.beige;
			}
		}

		public static Pen Bisque
		{
			get
			{
				if (Pens.bisque == null)
				{
					Pens.bisque = new Pen(Color.Bisque);
					Pens.bisque.isModifiable = false;
				}
				return Pens.bisque;
			}
		}

		public static Pen Black
		{
			get
			{
				if (Pens.black == null)
				{
					Pens.black = new Pen(Color.Black);
					Pens.black.isModifiable = false;
				}
				return Pens.black;
			}
		}

		public static Pen BlanchedAlmond
		{
			get
			{
				if (Pens.blanchedalmond == null)
				{
					Pens.blanchedalmond = new Pen(Color.BlanchedAlmond);
					Pens.blanchedalmond.isModifiable = false;
				}
				return Pens.blanchedalmond;
			}
		}

		public static Pen Blue
		{
			get
			{
				if (Pens.blue == null)
				{
					Pens.blue = new Pen(Color.Blue);
					Pens.blue.isModifiable = false;
				}
				return Pens.blue;
			}
		}

		public static Pen BlueViolet
		{
			get
			{
				if (Pens.blueviolet == null)
				{
					Pens.blueviolet = new Pen(Color.BlueViolet);
					Pens.blueviolet.isModifiable = false;
				}
				return Pens.blueviolet;
			}
		}

		public static Pen Brown
		{
			get
			{
				if (Pens.brown == null)
				{
					Pens.brown = new Pen(Color.Brown);
					Pens.brown.isModifiable = false;
				}
				return Pens.brown;
			}
		}

		public static Pen BurlyWood
		{
			get
			{
				if (Pens.burlywood == null)
				{
					Pens.burlywood = new Pen(Color.BurlyWood);
					Pens.burlywood.isModifiable = false;
				}
				return Pens.burlywood;
			}
		}

		public static Pen CadetBlue
		{
			get
			{
				if (Pens.cadetblue == null)
				{
					Pens.cadetblue = new Pen(Color.CadetBlue);
					Pens.cadetblue.isModifiable = false;
				}
				return Pens.cadetblue;
			}
		}

		public static Pen Chartreuse
		{
			get
			{
				if (Pens.chartreuse == null)
				{
					Pens.chartreuse = new Pen(Color.Chartreuse);
					Pens.chartreuse.isModifiable = false;
				}
				return Pens.chartreuse;
			}
		}

		public static Pen Chocolate
		{
			get
			{
				if (Pens.chocolate == null)
				{
					Pens.chocolate = new Pen(Color.Chocolate);
					Pens.chocolate.isModifiable = false;
				}
				return Pens.chocolate;
			}
		}

		public static Pen Coral
		{
			get
			{
				if (Pens.coral == null)
				{
					Pens.coral = new Pen(Color.Coral);
					Pens.coral.isModifiable = false;
				}
				return Pens.coral;
			}
		}

		public static Pen CornflowerBlue
		{
			get
			{
				if (Pens.cornflowerblue == null)
				{
					Pens.cornflowerblue = new Pen(Color.CornflowerBlue);
					Pens.cornflowerblue.isModifiable = false;
				}
				return Pens.cornflowerblue;
			}
		}

		public static Pen Cornsilk
		{
			get
			{
				if (Pens.cornsilk == null)
				{
					Pens.cornsilk = new Pen(Color.Cornsilk);
					Pens.cornsilk.isModifiable = false;
				}
				return Pens.cornsilk;
			}
		}

		public static Pen Crimson
		{
			get
			{
				if (Pens.crimson == null)
				{
					Pens.crimson = new Pen(Color.Crimson);
					Pens.crimson.isModifiable = false;
				}
				return Pens.crimson;
			}
		}

		public static Pen Cyan
		{
			get
			{
				if (Pens.cyan == null)
				{
					Pens.cyan = new Pen(Color.Cyan);
					Pens.cyan.isModifiable = false;
				}
				return Pens.cyan;
			}
		}

		public static Pen DarkBlue
		{
			get
			{
				if (Pens.darkblue == null)
				{
					Pens.darkblue = new Pen(Color.DarkBlue);
					Pens.darkblue.isModifiable = false;
				}
				return Pens.darkblue;
			}
		}

		public static Pen DarkCyan
		{
			get
			{
				if (Pens.darkcyan == null)
				{
					Pens.darkcyan = new Pen(Color.DarkCyan);
					Pens.darkcyan.isModifiable = false;
				}
				return Pens.darkcyan;
			}
		}

		public static Pen DarkGoldenrod
		{
			get
			{
				if (Pens.darkgoldenrod == null)
				{
					Pens.darkgoldenrod = new Pen(Color.DarkGoldenrod);
					Pens.darkgoldenrod.isModifiable = false;
				}
				return Pens.darkgoldenrod;
			}
		}

		public static Pen DarkGray
		{
			get
			{
				if (Pens.darkgray == null)
				{
					Pens.darkgray = new Pen(Color.DarkGray);
					Pens.darkgray.isModifiable = false;
				}
				return Pens.darkgray;
			}
		}

		public static Pen DarkGreen
		{
			get
			{
				if (Pens.darkgreen == null)
				{
					Pens.darkgreen = new Pen(Color.DarkGreen);
					Pens.darkgreen.isModifiable = false;
				}
				return Pens.darkgreen;
			}
		}

		public static Pen DarkKhaki
		{
			get
			{
				if (Pens.darkkhaki == null)
				{
					Pens.darkkhaki = new Pen(Color.DarkKhaki);
					Pens.darkkhaki.isModifiable = false;
				}
				return Pens.darkkhaki;
			}
		}

		public static Pen DarkMagenta
		{
			get
			{
				if (Pens.darkmagenta == null)
				{
					Pens.darkmagenta = new Pen(Color.DarkMagenta);
					Pens.darkmagenta.isModifiable = false;
				}
				return Pens.darkmagenta;
			}
		}

		public static Pen DarkOliveGreen
		{
			get
			{
				if (Pens.darkolivegreen == null)
				{
					Pens.darkolivegreen = new Pen(Color.DarkOliveGreen);
					Pens.darkolivegreen.isModifiable = false;
				}
				return Pens.darkolivegreen;
			}
		}

		public static Pen DarkOrange
		{
			get
			{
				if (Pens.darkorange == null)
				{
					Pens.darkorange = new Pen(Color.DarkOrange);
					Pens.darkorange.isModifiable = false;
				}
				return Pens.darkorange;
			}
		}

		public static Pen DarkOrchid
		{
			get
			{
				if (Pens.darkorchid == null)
				{
					Pens.darkorchid = new Pen(Color.DarkOrchid);
					Pens.darkorchid.isModifiable = false;
				}
				return Pens.darkorchid;
			}
		}

		public static Pen DarkRed
		{
			get
			{
				if (Pens.darkred == null)
				{
					Pens.darkred = new Pen(Color.DarkRed);
					Pens.darkred.isModifiable = false;
				}
				return Pens.darkred;
			}
		}

		public static Pen DarkSalmon
		{
			get
			{
				if (Pens.darksalmon == null)
				{
					Pens.darksalmon = new Pen(Color.DarkSalmon);
					Pens.darksalmon.isModifiable = false;
				}
				return Pens.darksalmon;
			}
		}

		public static Pen DarkSeaGreen
		{
			get
			{
				if (Pens.darkseagreen == null)
				{
					Pens.darkseagreen = new Pen(Color.DarkSeaGreen);
					Pens.darkseagreen.isModifiable = false;
				}
				return Pens.darkseagreen;
			}
		}

		public static Pen DarkSlateBlue
		{
			get
			{
				if (Pens.darkslateblue == null)
				{
					Pens.darkslateblue = new Pen(Color.DarkSlateBlue);
					Pens.darkslateblue.isModifiable = false;
				}
				return Pens.darkslateblue;
			}
		}

		public static Pen DarkSlateGray
		{
			get
			{
				if (Pens.darkslategray == null)
				{
					Pens.darkslategray = new Pen(Color.DarkSlateGray);
					Pens.darkslategray.isModifiable = false;
				}
				return Pens.darkslategray;
			}
		}

		public static Pen DarkTurquoise
		{
			get
			{
				if (Pens.darkturquoise == null)
				{
					Pens.darkturquoise = new Pen(Color.DarkTurquoise);
					Pens.darkturquoise.isModifiable = false;
				}
				return Pens.darkturquoise;
			}
		}

		public static Pen DarkViolet
		{
			get
			{
				if (Pens.darkviolet == null)
				{
					Pens.darkviolet = new Pen(Color.DarkViolet);
					Pens.darkviolet.isModifiable = false;
				}
				return Pens.darkviolet;
			}
		}

		public static Pen DeepPink
		{
			get
			{
				if (Pens.deeppink == null)
				{
					Pens.deeppink = new Pen(Color.DeepPink);
					Pens.deeppink.isModifiable = false;
				}
				return Pens.deeppink;
			}
		}

		public static Pen DeepSkyBlue
		{
			get
			{
				if (Pens.deepskyblue == null)
				{
					Pens.deepskyblue = new Pen(Color.DeepSkyBlue);
					Pens.deepskyblue.isModifiable = false;
				}
				return Pens.deepskyblue;
			}
		}

		public static Pen DimGray
		{
			get
			{
				if (Pens.dimgray == null)
				{
					Pens.dimgray = new Pen(Color.DimGray);
					Pens.dimgray.isModifiable = false;
				}
				return Pens.dimgray;
			}
		}

		public static Pen DodgerBlue
		{
			get
			{
				if (Pens.dodgerblue == null)
				{
					Pens.dodgerblue = new Pen(Color.DodgerBlue);
					Pens.dodgerblue.isModifiable = false;
				}
				return Pens.dodgerblue;
			}
		}

		public static Pen Firebrick
		{
			get
			{
				if (Pens.firebrick == null)
				{
					Pens.firebrick = new Pen(Color.Firebrick);
					Pens.firebrick.isModifiable = false;
				}
				return Pens.firebrick;
			}
		}

		public static Pen FloralWhite
		{
			get
			{
				if (Pens.floralwhite == null)
				{
					Pens.floralwhite = new Pen(Color.FloralWhite);
					Pens.floralwhite.isModifiable = false;
				}
				return Pens.floralwhite;
			}
		}

		public static Pen ForestGreen
		{
			get
			{
				if (Pens.forestgreen == null)
				{
					Pens.forestgreen = new Pen(Color.ForestGreen);
					Pens.forestgreen.isModifiable = false;
				}
				return Pens.forestgreen;
			}
		}

		public static Pen Fuchsia
		{
			get
			{
				if (Pens.fuchsia == null)
				{
					Pens.fuchsia = new Pen(Color.Fuchsia);
					Pens.fuchsia.isModifiable = false;
				}
				return Pens.fuchsia;
			}
		}

		public static Pen Gainsboro
		{
			get
			{
				if (Pens.gainsboro == null)
				{
					Pens.gainsboro = new Pen(Color.Gainsboro);
					Pens.gainsboro.isModifiable = false;
				}
				return Pens.gainsboro;
			}
		}

		public static Pen GhostWhite
		{
			get
			{
				if (Pens.ghostwhite == null)
				{
					Pens.ghostwhite = new Pen(Color.GhostWhite);
					Pens.ghostwhite.isModifiable = false;
				}
				return Pens.ghostwhite;
			}
		}

		public static Pen Gold
		{
			get
			{
				if (Pens.gold == null)
				{
					Pens.gold = new Pen(Color.Gold);
					Pens.gold.isModifiable = false;
				}
				return Pens.gold;
			}
		}

		public static Pen Goldenrod
		{
			get
			{
				if (Pens.goldenrod == null)
				{
					Pens.goldenrod = new Pen(Color.Goldenrod);
					Pens.goldenrod.isModifiable = false;
				}
				return Pens.goldenrod;
			}
		}

		public static Pen Gray
		{
			get
			{
				if (Pens.gray == null)
				{
					Pens.gray = new Pen(Color.Gray);
					Pens.gray.isModifiable = false;
				}
				return Pens.gray;
			}
		}

		public static Pen Green
		{
			get
			{
				if (Pens.green == null)
				{
					Pens.green = new Pen(Color.Green);
					Pens.green.isModifiable = false;
				}
				return Pens.green;
			}
		}

		public static Pen GreenYellow
		{
			get
			{
				if (Pens.greenyellow == null)
				{
					Pens.greenyellow = new Pen(Color.GreenYellow);
					Pens.greenyellow.isModifiable = false;
				}
				return Pens.greenyellow;
			}
		}

		public static Pen Honeydew
		{
			get
			{
				if (Pens.honeydew == null)
				{
					Pens.honeydew = new Pen(Color.Honeydew);
					Pens.honeydew.isModifiable = false;
				}
				return Pens.honeydew;
			}
		}

		public static Pen HotPink
		{
			get
			{
				if (Pens.hotpink == null)
				{
					Pens.hotpink = new Pen(Color.HotPink);
					Pens.hotpink.isModifiable = false;
				}
				return Pens.hotpink;
			}
		}

		public static Pen IndianRed
		{
			get
			{
				if (Pens.indianred == null)
				{
					Pens.indianred = new Pen(Color.IndianRed);
					Pens.indianred.isModifiable = false;
				}
				return Pens.indianred;
			}
		}

		public static Pen Indigo
		{
			get
			{
				if (Pens.indigo == null)
				{
					Pens.indigo = new Pen(Color.Indigo);
					Pens.indigo.isModifiable = false;
				}
				return Pens.indigo;
			}
		}

		public static Pen Ivory
		{
			get
			{
				if (Pens.ivory == null)
				{
					Pens.ivory = new Pen(Color.Ivory);
					Pens.ivory.isModifiable = false;
				}
				return Pens.ivory;
			}
		}

		public static Pen Khaki
		{
			get
			{
				if (Pens.khaki == null)
				{
					Pens.khaki = new Pen(Color.Khaki);
					Pens.khaki.isModifiable = false;
				}
				return Pens.khaki;
			}
		}

		public static Pen Lavender
		{
			get
			{
				if (Pens.lavender == null)
				{
					Pens.lavender = new Pen(Color.Lavender);
					Pens.lavender.isModifiable = false;
				}
				return Pens.lavender;
			}
		}

		public static Pen LavenderBlush
		{
			get
			{
				if (Pens.lavenderblush == null)
				{
					Pens.lavenderblush = new Pen(Color.LavenderBlush);
					Pens.lavenderblush.isModifiable = false;
				}
				return Pens.lavenderblush;
			}
		}

		public static Pen LawnGreen
		{
			get
			{
				if (Pens.lawngreen == null)
				{
					Pens.lawngreen = new Pen(Color.LawnGreen);
					Pens.lawngreen.isModifiable = false;
				}
				return Pens.lawngreen;
			}
		}

		public static Pen LemonChiffon
		{
			get
			{
				if (Pens.lemonchiffon == null)
				{
					Pens.lemonchiffon = new Pen(Color.LemonChiffon);
					Pens.lemonchiffon.isModifiable = false;
				}
				return Pens.lemonchiffon;
			}
		}

		public static Pen LightBlue
		{
			get
			{
				if (Pens.lightblue == null)
				{
					Pens.lightblue = new Pen(Color.LightBlue);
					Pens.lightblue.isModifiable = false;
				}
				return Pens.lightblue;
			}
		}

		public static Pen LightCoral
		{
			get
			{
				if (Pens.lightcoral == null)
				{
					Pens.lightcoral = new Pen(Color.LightCoral);
					Pens.lightcoral.isModifiable = false;
				}
				return Pens.lightcoral;
			}
		}

		public static Pen LightCyan
		{
			get
			{
				if (Pens.lightcyan == null)
				{
					Pens.lightcyan = new Pen(Color.LightCyan);
					Pens.lightcyan.isModifiable = false;
				}
				return Pens.lightcyan;
			}
		}

		public static Pen LightGoldenrodYellow
		{
			get
			{
				if (Pens.lightgoldenrodyellow == null)
				{
					Pens.lightgoldenrodyellow = new Pen(Color.LightGoldenrodYellow);
					Pens.lightgoldenrodyellow.isModifiable = false;
				}
				return Pens.lightgoldenrodyellow;
			}
		}

		public static Pen LightGray
		{
			get
			{
				if (Pens.lightgray == null)
				{
					Pens.lightgray = new Pen(Color.LightGray);
					Pens.lightgray.isModifiable = false;
				}
				return Pens.lightgray;
			}
		}

		public static Pen LightGreen
		{
			get
			{
				if (Pens.lightgreen == null)
				{
					Pens.lightgreen = new Pen(Color.LightGreen);
					Pens.lightgreen.isModifiable = false;
				}
				return Pens.lightgreen;
			}
		}

		public static Pen LightPink
		{
			get
			{
				if (Pens.lightpink == null)
				{
					Pens.lightpink = new Pen(Color.LightPink);
					Pens.lightpink.isModifiable = false;
				}
				return Pens.lightpink;
			}
		}

		public static Pen LightSalmon
		{
			get
			{
				if (Pens.lightsalmon == null)
				{
					Pens.lightsalmon = new Pen(Color.LightSalmon);
					Pens.lightsalmon.isModifiable = false;
				}
				return Pens.lightsalmon;
			}
		}

		public static Pen LightSeaGreen
		{
			get
			{
				if (Pens.lightseagreen == null)
				{
					Pens.lightseagreen = new Pen(Color.LightSeaGreen);
					Pens.lightseagreen.isModifiable = false;
				}
				return Pens.lightseagreen;
			}
		}

		public static Pen LightSkyBlue
		{
			get
			{
				if (Pens.lightskyblue == null)
				{
					Pens.lightskyblue = new Pen(Color.LightSkyBlue);
					Pens.lightskyblue.isModifiable = false;
				}
				return Pens.lightskyblue;
			}
		}

		public static Pen LightSlateGray
		{
			get
			{
				if (Pens.lightslategray == null)
				{
					Pens.lightslategray = new Pen(Color.LightSlateGray);
					Pens.lightslategray.isModifiable = false;
				}
				return Pens.lightslategray;
			}
		}

		public static Pen LightSteelBlue
		{
			get
			{
				if (Pens.lightsteelblue == null)
				{
					Pens.lightsteelblue = new Pen(Color.LightSteelBlue);
					Pens.lightsteelblue.isModifiable = false;
				}
				return Pens.lightsteelblue;
			}
		}

		public static Pen LightYellow
		{
			get
			{
				if (Pens.lightyellow == null)
				{
					Pens.lightyellow = new Pen(Color.LightYellow);
					Pens.lightyellow.isModifiable = false;
				}
				return Pens.lightyellow;
			}
		}

		public static Pen Lime
		{
			get
			{
				if (Pens.lime == null)
				{
					Pens.lime = new Pen(Color.Lime);
					Pens.lime.isModifiable = false;
				}
				return Pens.lime;
			}
		}

		public static Pen LimeGreen
		{
			get
			{
				if (Pens.limegreen == null)
				{
					Pens.limegreen = new Pen(Color.LimeGreen);
					Pens.limegreen.isModifiable = false;
				}
				return Pens.limegreen;
			}
		}

		public static Pen Linen
		{
			get
			{
				if (Pens.linen == null)
				{
					Pens.linen = new Pen(Color.Linen);
					Pens.linen.isModifiable = false;
				}
				return Pens.linen;
			}
		}

		public static Pen Magenta
		{
			get
			{
				if (Pens.magenta == null)
				{
					Pens.magenta = new Pen(Color.Magenta);
					Pens.magenta.isModifiable = false;
				}
				return Pens.magenta;
			}
		}

		public static Pen Maroon
		{
			get
			{
				if (Pens.maroon == null)
				{
					Pens.maroon = new Pen(Color.Maroon);
					Pens.maroon.isModifiable = false;
				}
				return Pens.maroon;
			}
		}

		public static Pen MediumAquamarine
		{
			get
			{
				if (Pens.mediumaquamarine == null)
				{
					Pens.mediumaquamarine = new Pen(Color.MediumAquamarine);
					Pens.mediumaquamarine.isModifiable = false;
				}
				return Pens.mediumaquamarine;
			}
		}

		public static Pen MediumBlue
		{
			get
			{
				if (Pens.mediumblue == null)
				{
					Pens.mediumblue = new Pen(Color.MediumBlue);
					Pens.mediumblue.isModifiable = false;
				}
				return Pens.mediumblue;
			}
		}

		public static Pen MediumOrchid
		{
			get
			{
				if (Pens.mediumorchid == null)
				{
					Pens.mediumorchid = new Pen(Color.MediumOrchid);
					Pens.mediumorchid.isModifiable = false;
				}
				return Pens.mediumorchid;
			}
		}

		public static Pen MediumPurple
		{
			get
			{
				if (Pens.mediumpurple == null)
				{
					Pens.mediumpurple = new Pen(Color.MediumPurple);
					Pens.mediumpurple.isModifiable = false;
				}
				return Pens.mediumpurple;
			}
		}

		public static Pen MediumSeaGreen
		{
			get
			{
				if (Pens.mediumseagreen == null)
				{
					Pens.mediumseagreen = new Pen(Color.MediumSeaGreen);
					Pens.mediumseagreen.isModifiable = false;
				}
				return Pens.mediumseagreen;
			}
		}

		public static Pen MediumSlateBlue
		{
			get
			{
				if (Pens.mediumslateblue == null)
				{
					Pens.mediumslateblue = new Pen(Color.MediumSlateBlue);
					Pens.mediumslateblue.isModifiable = false;
				}
				return Pens.mediumslateblue;
			}
		}

		public static Pen MediumSpringGreen
		{
			get
			{
				if (Pens.mediumspringgreen == null)
				{
					Pens.mediumspringgreen = new Pen(Color.MediumSpringGreen);
					Pens.mediumspringgreen.isModifiable = false;
				}
				return Pens.mediumspringgreen;
			}
		}

		public static Pen MediumTurquoise
		{
			get
			{
				if (Pens.mediumturquoise == null)
				{
					Pens.mediumturquoise = new Pen(Color.MediumTurquoise);
					Pens.mediumturquoise.isModifiable = false;
				}
				return Pens.mediumturquoise;
			}
		}

		public static Pen MediumVioletRed
		{
			get
			{
				if (Pens.mediumvioletred == null)
				{
					Pens.mediumvioletred = new Pen(Color.MediumVioletRed);
					Pens.mediumvioletred.isModifiable = false;
				}
				return Pens.mediumvioletred;
			}
		}

		public static Pen MidnightBlue
		{
			get
			{
				if (Pens.midnightblue == null)
				{
					Pens.midnightblue = new Pen(Color.MidnightBlue);
					Pens.midnightblue.isModifiable = false;
				}
				return Pens.midnightblue;
			}
		}

		public static Pen MintCream
		{
			get
			{
				if (Pens.mintcream == null)
				{
					Pens.mintcream = new Pen(Color.MintCream);
					Pens.mintcream.isModifiable = false;
				}
				return Pens.mintcream;
			}
		}

		public static Pen MistyRose
		{
			get
			{
				if (Pens.mistyrose == null)
				{
					Pens.mistyrose = new Pen(Color.MistyRose);
					Pens.mistyrose.isModifiable = false;
				}
				return Pens.mistyrose;
			}
		}

		public static Pen Moccasin
		{
			get
			{
				if (Pens.moccasin == null)
				{
					Pens.moccasin = new Pen(Color.Moccasin);
					Pens.moccasin.isModifiable = false;
				}
				return Pens.moccasin;
			}
		}

		public static Pen NavajoWhite
		{
			get
			{
				if (Pens.navajowhite == null)
				{
					Pens.navajowhite = new Pen(Color.NavajoWhite);
					Pens.navajowhite.isModifiable = false;
				}
				return Pens.navajowhite;
			}
		}

		public static Pen Navy
		{
			get
			{
				if (Pens.navy == null)
				{
					Pens.navy = new Pen(Color.Navy);
					Pens.navy.isModifiable = false;
				}
				return Pens.navy;
			}
		}

		public static Pen OldLace
		{
			get
			{
				if (Pens.oldlace == null)
				{
					Pens.oldlace = new Pen(Color.OldLace);
					Pens.oldlace.isModifiable = false;
				}
				return Pens.oldlace;
			}
		}

		public static Pen Olive
		{
			get
			{
				if (Pens.olive == null)
				{
					Pens.olive = new Pen(Color.Olive);
					Pens.olive.isModifiable = false;
				}
				return Pens.olive;
			}
		}

		public static Pen OliveDrab
		{
			get
			{
				if (Pens.olivedrab == null)
				{
					Pens.olivedrab = new Pen(Color.OliveDrab);
					Pens.olivedrab.isModifiable = false;
				}
				return Pens.olivedrab;
			}
		}

		public static Pen Orange
		{
			get
			{
				if (Pens.orange == null)
				{
					Pens.orange = new Pen(Color.Orange);
					Pens.orange.isModifiable = false;
				}
				return Pens.orange;
			}
		}

		public static Pen OrangeRed
		{
			get
			{
				if (Pens.orangered == null)
				{
					Pens.orangered = new Pen(Color.OrangeRed);
					Pens.orangered.isModifiable = false;
				}
				return Pens.orangered;
			}
		}

		public static Pen Orchid
		{
			get
			{
				if (Pens.orchid == null)
				{
					Pens.orchid = new Pen(Color.Orchid);
					Pens.orchid.isModifiable = false;
				}
				return Pens.orchid;
			}
		}

		public static Pen PaleGoldenrod
		{
			get
			{
				if (Pens.palegoldenrod == null)
				{
					Pens.palegoldenrod = new Pen(Color.PaleGoldenrod);
					Pens.palegoldenrod.isModifiable = false;
				}
				return Pens.palegoldenrod;
			}
		}

		public static Pen PaleGreen
		{
			get
			{
				if (Pens.palegreen == null)
				{
					Pens.palegreen = new Pen(Color.PaleGreen);
					Pens.palegreen.isModifiable = false;
				}
				return Pens.palegreen;
			}
		}

		public static Pen PaleTurquoise
		{
			get
			{
				if (Pens.paleturquoise == null)
				{
					Pens.paleturquoise = new Pen(Color.PaleTurquoise);
					Pens.paleturquoise.isModifiable = false;
				}
				return Pens.paleturquoise;
			}
		}

		public static Pen PaleVioletRed
		{
			get
			{
				if (Pens.palevioletred == null)
				{
					Pens.palevioletred = new Pen(Color.PaleVioletRed);
					Pens.palevioletred.isModifiable = false;
				}
				return Pens.palevioletred;
			}
		}

		public static Pen PapayaWhip
		{
			get
			{
				if (Pens.papayawhip == null)
				{
					Pens.papayawhip = new Pen(Color.PapayaWhip);
					Pens.papayawhip.isModifiable = false;
				}
				return Pens.papayawhip;
			}
		}

		public static Pen PeachPuff
		{
			get
			{
				if (Pens.peachpuff == null)
				{
					Pens.peachpuff = new Pen(Color.PeachPuff);
					Pens.peachpuff.isModifiable = false;
				}
				return Pens.peachpuff;
			}
		}

		public static Pen Peru
		{
			get
			{
				if (Pens.peru == null)
				{
					Pens.peru = new Pen(Color.Peru);
					Pens.peru.isModifiable = false;
				}
				return Pens.peru;
			}
		}

		public static Pen Pink
		{
			get
			{
				if (Pens.pink == null)
				{
					Pens.pink = new Pen(Color.Pink);
					Pens.pink.isModifiable = false;
				}
				return Pens.pink;
			}
		}

		public static Pen Plum
		{
			get
			{
				if (Pens.plum == null)
				{
					Pens.plum = new Pen(Color.Plum);
					Pens.plum.isModifiable = false;
				}
				return Pens.plum;
			}
		}

		public static Pen PowderBlue
		{
			get
			{
				if (Pens.powderblue == null)
				{
					Pens.powderblue = new Pen(Color.PowderBlue);
					Pens.powderblue.isModifiable = false;
				}
				return Pens.powderblue;
			}
		}

		public static Pen Purple
		{
			get
			{
				if (Pens.purple == null)
				{
					Pens.purple = new Pen(Color.Purple);
					Pens.purple.isModifiable = false;
				}
				return Pens.purple;
			}
		}

		public static Pen Red
		{
			get
			{
				if (Pens.red == null)
				{
					Pens.red = new Pen(Color.Red);
					Pens.red.isModifiable = false;
				}
				return Pens.red;
			}
		}

		public static Pen RosyBrown
		{
			get
			{
				if (Pens.rosybrown == null)
				{
					Pens.rosybrown = new Pen(Color.RosyBrown);
					Pens.rosybrown.isModifiable = false;
				}
				return Pens.rosybrown;
			}
		}

		public static Pen RoyalBlue
		{
			get
			{
				if (Pens.royalblue == null)
				{
					Pens.royalblue = new Pen(Color.RoyalBlue);
					Pens.royalblue.isModifiable = false;
				}
				return Pens.royalblue;
			}
		}

		public static Pen SaddleBrown
		{
			get
			{
				if (Pens.saddlebrown == null)
				{
					Pens.saddlebrown = new Pen(Color.SaddleBrown);
					Pens.saddlebrown.isModifiable = false;
				}
				return Pens.saddlebrown;
			}
		}

		public static Pen Salmon
		{
			get
			{
				if (Pens.salmon == null)
				{
					Pens.salmon = new Pen(Color.Salmon);
					Pens.salmon.isModifiable = false;
				}
				return Pens.salmon;
			}
		}

		public static Pen SandyBrown
		{
			get
			{
				if (Pens.sandybrown == null)
				{
					Pens.sandybrown = new Pen(Color.SandyBrown);
					Pens.sandybrown.isModifiable = false;
				}
				return Pens.sandybrown;
			}
		}

		public static Pen SeaGreen
		{
			get
			{
				if (Pens.seagreen == null)
				{
					Pens.seagreen = new Pen(Color.SeaGreen);
					Pens.seagreen.isModifiable = false;
				}
				return Pens.seagreen;
			}
		}

		public static Pen SeaShell
		{
			get
			{
				if (Pens.seashell == null)
				{
					Pens.seashell = new Pen(Color.SeaShell);
					Pens.seashell.isModifiable = false;
				}
				return Pens.seashell;
			}
		}

		public static Pen Sienna
		{
			get
			{
				if (Pens.sienna == null)
				{
					Pens.sienna = new Pen(Color.Sienna);
					Pens.sienna.isModifiable = false;
				}
				return Pens.sienna;
			}
		}

		public static Pen Silver
		{
			get
			{
				if (Pens.silver == null)
				{
					Pens.silver = new Pen(Color.Silver);
					Pens.silver.isModifiable = false;
				}
				return Pens.silver;
			}
		}

		public static Pen SkyBlue
		{
			get
			{
				if (Pens.skyblue == null)
				{
					Pens.skyblue = new Pen(Color.SkyBlue);
					Pens.skyblue.isModifiable = false;
				}
				return Pens.skyblue;
			}
		}

		public static Pen SlateBlue
		{
			get
			{
				if (Pens.slateblue == null)
				{
					Pens.slateblue = new Pen(Color.SlateBlue);
					Pens.slateblue.isModifiable = false;
				}
				return Pens.slateblue;
			}
		}

		public static Pen SlateGray
		{
			get
			{
				if (Pens.slategray == null)
				{
					Pens.slategray = new Pen(Color.SlateGray);
					Pens.slategray.isModifiable = false;
				}
				return Pens.slategray;
			}
		}

		public static Pen Snow
		{
			get
			{
				if (Pens.snow == null)
				{
					Pens.snow = new Pen(Color.Snow);
					Pens.snow.isModifiable = false;
				}
				return Pens.snow;
			}
		}

		public static Pen SpringGreen
		{
			get
			{
				if (Pens.springgreen == null)
				{
					Pens.springgreen = new Pen(Color.SpringGreen);
					Pens.springgreen.isModifiable = false;
				}
				return Pens.springgreen;
			}
		}

		public static Pen SteelBlue
		{
			get
			{
				if (Pens.steelblue == null)
				{
					Pens.steelblue = new Pen(Color.SteelBlue);
					Pens.steelblue.isModifiable = false;
				}
				return Pens.steelblue;
			}
		}

		public static Pen Tan
		{
			get
			{
				if (Pens.tan == null)
				{
					Pens.tan = new Pen(Color.Tan);
					Pens.tan.isModifiable = false;
				}
				return Pens.tan;
			}
		}

		public static Pen Teal
		{
			get
			{
				if (Pens.teal == null)
				{
					Pens.teal = new Pen(Color.Teal);
					Pens.teal.isModifiable = false;
				}
				return Pens.teal;
			}
		}

		public static Pen Thistle
		{
			get
			{
				if (Pens.thistle == null)
				{
					Pens.thistle = new Pen(Color.Thistle);
					Pens.thistle.isModifiable = false;
				}
				return Pens.thistle;
			}
		}

		public static Pen Tomato
		{
			get
			{
				if (Pens.tomato == null)
				{
					Pens.tomato = new Pen(Color.Tomato);
					Pens.tomato.isModifiable = false;
				}
				return Pens.tomato;
			}
		}

		public static Pen Transparent
		{
			get
			{
				if (Pens.transparent == null)
				{
					Pens.transparent = new Pen(Color.Transparent);
					Pens.transparent.isModifiable = false;
				}
				return Pens.transparent;
			}
		}

		public static Pen Turquoise
		{
			get
			{
				if (Pens.turquoise == null)
				{
					Pens.turquoise = new Pen(Color.Turquoise);
					Pens.turquoise.isModifiable = false;
				}
				return Pens.turquoise;
			}
		}

		public static Pen Violet
		{
			get
			{
				if (Pens.violet == null)
				{
					Pens.violet = new Pen(Color.Violet);
					Pens.violet.isModifiable = false;
				}
				return Pens.violet;
			}
		}

		public static Pen Wheat
		{
			get
			{
				if (Pens.wheat == null)
				{
					Pens.wheat = new Pen(Color.Wheat);
					Pens.wheat.isModifiable = false;
				}
				return Pens.wheat;
			}
		}

		public static Pen White
		{
			get
			{
				if (Pens.white == null)
				{
					Pens.white = new Pen(Color.White);
					Pens.white.isModifiable = false;
				}
				return Pens.white;
			}
		}

		public static Pen WhiteSmoke
		{
			get
			{
				if (Pens.whitesmoke == null)
				{
					Pens.whitesmoke = new Pen(Color.WhiteSmoke);
					Pens.whitesmoke.isModifiable = false;
				}
				return Pens.whitesmoke;
			}
		}

		public static Pen Yellow
		{
			get
			{
				if (Pens.yellow == null)
				{
					Pens.yellow = new Pen(Color.Yellow);
					Pens.yellow.isModifiable = false;
				}
				return Pens.yellow;
			}
		}

		public static Pen YellowGreen
		{
			get
			{
				if (Pens.yellowgreen == null)
				{
					Pens.yellowgreen = new Pen(Color.YellowGreen);
					Pens.yellowgreen.isModifiable = false;
				}
				return Pens.yellowgreen;
			}
		}

		private static Pen aliceblue;

		private static Pen antiquewhite;

		private static Pen aqua;

		private static Pen aquamarine;

		private static Pen azure;

		private static Pen beige;

		private static Pen bisque;

		private static Pen black;

		private static Pen blanchedalmond;

		private static Pen blue;

		private static Pen blueviolet;

		private static Pen brown;

		private static Pen burlywood;

		private static Pen cadetblue;

		private static Pen chartreuse;

		private static Pen chocolate;

		private static Pen coral;

		private static Pen cornflowerblue;

		private static Pen cornsilk;

		private static Pen crimson;

		private static Pen cyan;

		private static Pen darkblue;

		private static Pen darkcyan;

		private static Pen darkgoldenrod;

		private static Pen darkgray;

		private static Pen darkgreen;

		private static Pen darkkhaki;

		private static Pen darkmagenta;

		private static Pen darkolivegreen;

		private static Pen darkorange;

		private static Pen darkorchid;

		private static Pen darkred;

		private static Pen darksalmon;

		private static Pen darkseagreen;

		private static Pen darkslateblue;

		private static Pen darkslategray;

		private static Pen darkturquoise;

		private static Pen darkviolet;

		private static Pen deeppink;

		private static Pen deepskyblue;

		private static Pen dimgray;

		private static Pen dodgerblue;

		private static Pen firebrick;

		private static Pen floralwhite;

		private static Pen forestgreen;

		private static Pen fuchsia;

		private static Pen gainsboro;

		private static Pen ghostwhite;

		private static Pen gold;

		private static Pen goldenrod;

		private static Pen gray;

		private static Pen green;

		private static Pen greenyellow;

		private static Pen honeydew;

		private static Pen hotpink;

		private static Pen indianred;

		private static Pen indigo;

		private static Pen ivory;

		private static Pen khaki;

		private static Pen lavender;

		private static Pen lavenderblush;

		private static Pen lawngreen;

		private static Pen lemonchiffon;

		private static Pen lightblue;

		private static Pen lightcoral;

		private static Pen lightcyan;

		private static Pen lightgoldenrodyellow;

		private static Pen lightgray;

		private static Pen lightgreen;

		private static Pen lightpink;

		private static Pen lightsalmon;

		private static Pen lightseagreen;

		private static Pen lightskyblue;

		private static Pen lightslategray;

		private static Pen lightsteelblue;

		private static Pen lightyellow;

		private static Pen lime;

		private static Pen limegreen;

		private static Pen linen;

		private static Pen magenta;

		private static Pen maroon;

		private static Pen mediumaquamarine;

		private static Pen mediumblue;

		private static Pen mediumorchid;

		private static Pen mediumpurple;

		private static Pen mediumseagreen;

		private static Pen mediumslateblue;

		private static Pen mediumspringgreen;

		private static Pen mediumturquoise;

		private static Pen mediumvioletred;

		private static Pen midnightblue;

		private static Pen mintcream;

		private static Pen mistyrose;

		private static Pen moccasin;

		private static Pen navajowhite;

		private static Pen navy;

		private static Pen oldlace;

		private static Pen olive;

		private static Pen olivedrab;

		private static Pen orange;

		private static Pen orangered;

		private static Pen orchid;

		private static Pen palegoldenrod;

		private static Pen palegreen;

		private static Pen paleturquoise;

		private static Pen palevioletred;

		private static Pen papayawhip;

		private static Pen peachpuff;

		private static Pen peru;

		private static Pen pink;

		private static Pen plum;

		private static Pen powderblue;

		private static Pen purple;

		private static Pen red;

		private static Pen rosybrown;

		private static Pen royalblue;

		private static Pen saddlebrown;

		private static Pen salmon;

		private static Pen sandybrown;

		private static Pen seagreen;

		private static Pen seashell;

		private static Pen sienna;

		private static Pen silver;

		private static Pen skyblue;

		private static Pen slateblue;

		private static Pen slategray;

		private static Pen snow;

		private static Pen springgreen;

		private static Pen steelblue;

		private static Pen tan;

		private static Pen teal;

		private static Pen thistle;

		private static Pen tomato;

		private static Pen transparent;

		private static Pen turquoise;

		private static Pen violet;

		private static Pen wheat;

		private static Pen white;

		private static Pen whitesmoke;

		private static Pen yellow;

		private static Pen yellowgreen;
	}
}
