using Sandbox.Game;
using Sandbox.Game.Components;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

// Code heavily borrowed from Bačiulis' awesome Drink Water mod. Thanks so much for pointing me towards these methods dude, very much appreciated.

namespace sleep
{
	[MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
	public class Session : MySessionComponentBase
	{

		public static ushort modId = 19008;
		public static int runCount = 0;
		public static Random rand = new Random();
		public static List<long> blinkList = new List<long>();
		public static int loadWait = 120;

		protected override void UnloadData()
		{
			if (!MyAPIGateway.Multiplayer.IsServer)
				MyAPIGateway.Multiplayer.UnregisterMessageHandler(modId, getPoke);
		}

		public override void UpdateAfterSimulation()
		{
			try
			{
				// Only run every quarter of a second
				if (++runCount % 15 > 0)
					return;

				// Blinking during load screen causes crash, don't load messagehandler on clients for 30s
				if (!MyAPIGateway.Multiplayer.IsServer && loadWait > 0)
				{
					if (--loadWait == 0)
						MyAPIGateway.Multiplayer.RegisterMessageHandler(modId, getPoke);
					return;
				}

				foreach (var playerId in blinkList)
					if (playerId == MyVisualScriptLogicProvider.GetLocalPlayerId())
						blink(false);
					else
						MyAPIGateway.Multiplayer.SendMessageTo(modId, Encoding.ASCII.GetBytes("unblink"), MyVisualScriptLogicProvider.GetSteamId(playerId), true);
				blinkList.Clear();

				// Check for recharging or using toilet every second
				if (runCount % 60 > 0)
					return;

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
						if (block.ToString().Contains("Toilet") || block.ToString().Contains("Bathroom"))
						{
							food.Decrease(5f, null);
							if (food.Value > 0)
								player.Character.GetInventory(0).AddItems((MyFixedPoint)0.05f, (MyObjectBuilder_PhysicalObject)MyObjectBuilderSerializer.CreateNewObject(new MyDefinitionId(typeof(MyObjectBuilder_Ore), "Organic")));
						}

					// Do remaining checks & stat updates every 5s
					if (runCount < 100)
						continue;

					if (sleep.Value > 20 || rand.Next((int)sleep.Value) > 0)
						continue;

					blinkList.Add(player.IdentityId);
					if (player.IdentityId == MyVisualScriptLogicProvider.GetLocalPlayerId())
						blink(true);
					else if (MyAPIGateway.Multiplayer.IsServer)
						MyAPIGateway.Multiplayer.SendMessageTo(modId, Encoding.ASCII.GetBytes("blink"), MyVisualScriptLogicProvider.GetSteamId(player.IdentityId), true);
				}

				if (runCount > 299)
					runCount = 0;
			}
			catch (Exception ex)
			{
				Echo("sleep exception", ex.ToString());
			}
		}

		public void getPoke(byte[] poke)
		{
			// To call blink action on clients
			try
			{
				var msg = ASCIIEncoding.ASCII.GetString(poke);
				blink(msg == "blink");
			}
			catch (Exception ex)
			{
				Echo("sleep exception", ex.ToString());
			}
		}

		public void blink(bool blink)
		{
			MyVisualScriptLogicProvider.ScreenColorFadingSetColor(Color.Black, 0L);
			MyVisualScriptLogicProvider.ScreenColorFadingMinimalizeHUD(false);
			MyVisualScriptLogicProvider.ScreenColorFadingStart(0.1f, blink, 0L);
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
