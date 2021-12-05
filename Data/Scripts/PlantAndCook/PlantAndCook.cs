using Sandbox.Game.Components;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace PlantAndCook
{
	[MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
	public class Session : MySessionComponentBase
	{
		private static int runCount = 0;

		public override void UpdateAfterSimulation()
		{
			try
			{
				if (runCount++ < 100)
				{
					return;
				}
				else
				{
					runCount = 0;
				}

				var players = new List<IMyPlayer>();
				MyAPIGateway.Multiplayer.Players.GetPlayers(players, p => p.Character != null && p.Character.ToString().Contains("Astronaut"));

				foreach (IMyPlayer player in players)
				{
					var statComp = player.Character?.Components.Get<MyEntityStatComponent>();
					if (statComp == null)
						continue;

					MyEntityStat sleep = GetPlayerStat(statComp, "Sleep");
					MyEntityStat food = GetPlayerStat(statComp, "Food");

					if (sleep == null || food == null)
						continue;

					var block = player.Controller?.ControlledEntity?.Entity as IMyCubeBlock;
					if (block != null)
					{
						string subtypeId = block.BlockDefinition.SubtypeId.ToLower();
						if (subtypeId.Contains("toilet") || subtypeId.Contains("bathroom"))
						{
							food.Decrease(5f, null);
							if (food.Value > 0)
								player.Character.GetInventory(0).AddItems((MyFixedPoint)0.05f, (MyObjectBuilder_PhysicalObject)MyObjectBuilderSerializer.CreateNewObject(new MyDefinitionId(typeof(MyObjectBuilder_Ore), "Organic")));
						}
					}
				}
			}
			catch (Exception ex)
			{
				Echo("Plant and Cook exception", ex.ToString());
			}
		}

		private MyEntityStat GetPlayerStat(MyEntityStatComponent statComp, string statName)
		{
			MyEntityStat stat;
			statComp.TryGetStat(MyStringHash.GetOrCompute(statName), out stat);
			return stat;
		}

		public static void Echo(string msg1, string msg2 = "")
		{
			MyLog.Default.WriteLineAndConsole(msg1 + ": " + msg2);
		}
	}
}
