using Sandbox.Common.ObjectBuilders;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.Game;

namespace PlantAndCook
{
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_Kitchen), false, new string[]
	{ "LargeBlockKitchen" })]
	public class KitchenReplaceLogicComponent : MyGameLogicComponent
	{
		public override void Init(MyObjectBuilder_EntityBase objectBuilder)
		{
			BlockReplace.ReplaceBlock(Entity, new MyDefinitionId(typeof(MyObjectBuilder_Assembler), "Kitchen"));
		}
	}
}