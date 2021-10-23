using VRage.ObjectBuilders;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using System;
using Sandbox.ModAPI;

namespace PlantAndCook
{
	public class BlockReplace
	{
		public static void ReplaceBlock(IMyEntity Entity, MyDefinitionId newDefinitionId)
		{
			try
			{
				IMySlimBlock oldBlock = (Entity as IMyCubeBlock).SlimBlock;

				MyObjectBuilder_CubeBlock oldBuilder = oldBlock.GetObjectBuilder();

				MyObjectBuilder_CubeBlock newBuilder = MyObjectBuilderSerializer.CreateNewObject(newDefinitionId) as MyObjectBuilder_CubeBlock;

				newBuilder.BlockOrientation = oldBuilder.BlockOrientation;
				newBuilder.Min = oldBuilder.Min;
				newBuilder.ColorMaskHSV = oldBuilder.ColorMaskHSV;
				newBuilder.Owner = oldBuilder.Owner;

				IMyCubeGrid grid = oldBlock.CubeGrid;

				grid.RemoveBlock(oldBlock);
				grid.AddBlock(newBuilder, false);
			}
			catch (Exception e)
			{
				//MyAPIGateway.Utilities.ShowNotification("Exception: " + e.Message, 3000);
			}
		}
	}
}