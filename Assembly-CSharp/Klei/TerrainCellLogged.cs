using System;
using ProcGen;
using ProcGenGame;

namespace Klei
{
	public class TerrainCellLogged : TerrainCell
	{
		public TerrainCellLogged()
		{
		}

		public TerrainCellLogged(Node node, VoronoiDiagram.Site site)
			: base(node, site)
		{
		}

		public override void LogInfo(string evt, string param, float value)
		{
		}
	}
}
