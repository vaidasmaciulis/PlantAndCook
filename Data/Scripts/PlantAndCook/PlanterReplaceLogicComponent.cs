using Sandbox.Common.ObjectBuilders;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.Game;

namespace PlantAndCook
{
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_Planter), false, new string[]
	{ "LargeBlockPlanters" })]
	public class PlanterReplaceLogicComponent : MyGameLogicComponent
	{
		public override void Init(MyObjectBuilder_EntityBase objectBuilder)
		{
			BlockReplace.ReplaceBlock(Entity, new MyDefinitionId(typeof(MyObjectBuilder_Assembler), "Planters"));
		}
	}
}