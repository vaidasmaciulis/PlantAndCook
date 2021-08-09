using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using System;
using VRage.Utils;
using VRageMath;

namespace IndustrialAstromech_Swapsie
{

  [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
  public class Swapsie_Sesh : MySessionComponentBase
  { 
  
    public static bool isInit = false;
  
    public override void UpdateBeforeSimulation()
    {
      if (!MyAPIGateway.Multiplayer.IsServer)
        return;
      if (!isInit && (MyAPIGateway.Session.Player!=null || MyAPIGateway.Utilities.IsDedicated))
      {
        MyAPIGateway.Entities.OnEntityAdd += EntityAdded;
        isInit = true;
      }
    }
  
    public void EntityAdded(IMyEntity ent)
    {
      if (ent is IMyCubeGrid && ent.Physics!=null)
        swapsie(ent as IMyCubeGrid);
    }    

    public void swapsie(IMyCubeGrid grid)    
    {
      try
      {
        var slimList = new List<IMySlimBlock>();
        grid.GetBlocks(slimList, s => !s.ToString().Contains("Assembler") && (s.ToString().Contains("MyKitchen") || s.ToString().Contains("MyPlanter")));
        foreach (var slim in slimList)
        {
          var builder = new MyObjectBuilder_Assembler();
          builder.EntityId = 0;
          builder.BlockOrientation = slim.Orientation;
          builder.BlockOrientation.Forward = Base6Directions.GetOppositeDirection(builder.BlockOrientation.Forward);
          if (slim.ToString().Contains("MyKitchen"))
            builder.SubtypeName = "Kitchen";
          else
            builder.SubtypeName = "Planters";
          builder.Min = slim.Min;
          builder.ColorMaskHSV = slim.GetColorMask();
          builder.Owner = slim.OwnerId;
          grid.RemoveBlock(slim);
          var newSlim = grid.AddBlock(builder, true);
        }
      }
      catch (Exception ex)
      {
        Echo("Swapsie exception", ex.ToString()); 
      }
    }
        

    public void Echo(string msg1, string msg2 = "")
    {
      MyAPIGateway.Utilities.ShowMessage(msg1, msg2);
      MyLog.Default.WriteLineAndConsole(msg1 + ": " + msg2);
    }    

  }
}

