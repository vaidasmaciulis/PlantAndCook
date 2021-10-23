using Sandbox.Common.ObjectBuilders;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.Game;
using VRage.Game.ModAPI;

namespace PressurizeYourRoom
{
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_Kitchen), false, new string[]
	{ "LargeBlockKitchen" })]
	public class KitchenReplaceLogicComponent : MyGameLogicComponent
	{
		public override void Init(MyObjectBuilder_EntityBase objectBuilder)
		{
			try
			{
				IMySlimBlock oldKitchenSlimBlock = (Entity as IMyCubeBlock).SlimBlock;

				MyObjectBuilder_CubeBlock oldKitchenBuilder = oldKitchenSlimBlock.GetObjectBuilder();

				MyDefinitionId newKitchenId = new MyDefinitionId(typeof(MyObjectBuilder_Assembler), "Kitchen");
				MyObjectBuilder_CubeBlock newKitchenBuilder = MyObjectBuilderSerializer.CreateNewObject(newKitchenId) as MyObjectBuilder_CubeBlock;

				newKitchenBuilder.BlockOrientation = oldKitchenBuilder.BlockOrientation;
				newKitchenBuilder.Min = oldKitchenBuilder.Min;
				newKitchenBuilder.ColorMaskHSV = oldKitchenBuilder.ColorMaskHSV;
				newKitchenBuilder.Owner = oldKitchenBuilder.Owner;

				IMyCubeGrid grid = oldKitchenSlimBlock.CubeGrid;

				grid.RemoveBlock(oldKitchenSlimBlock);
				grid.AddBlock(newKitchenBuilder, false);
			}
			catch
			{
			}
		}
	}
}