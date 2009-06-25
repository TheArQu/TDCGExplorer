using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TAHdecrypt
{
    public class Pedo
    {
        private static float DegreeToRadian(float angle)
        {
           return (float)(Math.PI * angle / 180.0);
        }

        public static int UpdateTmo(string source_file) 
        {
            string dest_file = source_file + ".tmp";

            TMOFile tmo = new TMOFile();
            try
            {
                tmo.Load(source_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            if (tmo.nodes[0].ShortName != "W_Hips") {
                Console.WriteLine("Passed: root node is not W_Hips");
                return 1;
            }

            Dictionary<string, TMONode> nodes = new Dictionary<string, TMONode>();

            foreach(TMONode node in tmo.nodes)
            try {
                nodes.Add(node.ShortName, node);
            } catch (ArgumentException) {
                Console.WriteLine("node {0} already exists.", node.ShortName);
            }

            try {
                TMONode node;
node = nodes["W_Hips"];
node.Scale1(0.93F, 0.75F, 0.85F);
node = nodes["W_LeftUpLeg"];
node.Scale1(0.78F, 0.66F, 0.78F);
node.Move(-0.100F, 0.000F, 0.000F);
node = nodes["W_LeftUpLegRoll"];
node.Scale1(0.85F, 0.68F, 0.86F);
node = nodes["W_LeftLeg"];
node.Scale1(0.81F, 0.63F, 0.71F);
node = nodes["W_LeftLegRoll"];
node.Scale1(0.83F, 0.84F, 0.81F);
node = nodes["W_LeftFoot"];
node.Scale1(0.91F, 0.81F, 0.71F);
node = nodes["W_LeftToeBase"];
node.Scale1(0.80F, 0.80F, 0.80F);
node = nodes["W_RightUpLeg"];
node.Scale1(0.78F, 0.66F, 0.78F);
node.Move(0.100F, 0.000F, 0.000F);
node = nodes["W_RightUpLegRoll"];
node.Scale1(0.85F, 0.68F, 0.86F);
node = nodes["W_RightLeg"];
node.Scale1(0.81F, 0.63F, 0.71F);
node = nodes["W_RightLegRoll"];
node.Scale1(0.83F, 0.84F, 0.81F);
node = nodes["W_RightFoot"];
node.Scale1(0.91F, 0.81F, 0.71F);
node = nodes["W_RightToeBase"];
node.Scale1(0.80F, 0.80F, 0.80F);
node = nodes["W_Spine_Dummy"];
node.Scale1(0.95F, 0.70F, 0.90F);
node = nodes["W_Spine1"];
node.Scale1(0.95F, 0.70F, 1.00F);
node.Move(0.000F, -0.090F, 0.000F);
node = nodes["W_Spine2"];
node.Scale1(0.95F, 0.60F, 1.00F);
node.Move(0.000F, -0.100F, 0.000F);
node = nodes["W_Spine3"];
node.Scale1(0.90F, 1.00F, 0.95F);
node.Move(0.000F, -0.065F, 0.000F);
node = nodes["W_RightShoulder_Dummy"];
node.Scale1(0.73F, 0.96F, 0.84F);
node = nodes["W_RightShoulder"];
node.Scale1(1.00F, 0.75F, 1.00F);
node = nodes["W_RightArm_Dummy"];
node.Scale1(0.71F, 1.00F, 1.00F);
node = nodes["W_RightArm"];
node.Scale1(0.76F, 1.00F, 1.00F);
node = nodes["W_RightArmRoll"];
node.Scale1(0.76F, 1.00F, 1.00F);
node = nodes["W_RightForeArm"];
node.Scale1(0.70F, 0.90F, 0.90F);
node = nodes["W_RightForeArmRoll"];
node.Scale1(0.72F, 1.00F, 1.00F);
node = nodes["W_RightHand"];
node.Scale1(0.70F, 0.87F, 0.87F);
node = nodes["W_RightHandPinky1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandPinky2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandPinky3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandPinky4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandRing1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandRing2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandRing3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandRing4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandMiddle1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandMiddle2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandMiddle3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandMiddle4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandIndex1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandIndex2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandIndex3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandIndex4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandThumb1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandThumb2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandThumb3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_RightHandThumb4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftShoulder_Dummy"];
node.Scale1(0.73F, 0.96F, 0.84F);
node = nodes["W_LeftShoulder"];
node.Scale1(1.00F, 0.75F, 1.00F);
node = nodes["W_LeftArm_Dummy"];
node.Scale1(0.71F, 1.00F, 1.00F);
node = nodes["W_LeftArm"];
node.Scale1(0.76F, 1.00F, 1.00F);
node = nodes["W_LeftArmRoll"];
node.Scale1(0.76F, 1.00F, 1.00F);
node = nodes["W_LeftForeArm"];
node.Scale1(0.70F, 0.90F, 0.90F);
node = nodes["W_LeftForeArmRoll"];
node.Scale1(0.72F, 1.00F, 1.00F);
node = nodes["W_LeftHand"];
node.Scale1(0.70F, 0.87F, 0.87F);
node = nodes["W_LeftHandPinky1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandPinky2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandPinky3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandPinky4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandRing1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandRing2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandRing3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandRing4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandMiddle1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandMiddle2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandMiddle3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandMiddle4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandIndex1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandIndex2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandIndex3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandIndex4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandThumb1"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandThumb2"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandThumb3"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_LeftHandThumb4"];
node.Scale1(0.70F, 1.00F, 1.00F);
node = nodes["W_Neck"];
node.Scale1(1.00F, 0.72F, 1.00F);
node = nodes["face_oya"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["sitakuti_oya"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["Ha_Down"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["sitakuti_l_1"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["sitakuti_r_1"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["sita_01"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["sita_02"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["sita_03"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["kutiyoko_r"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["kutiyoko_l"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["uekuti_oya"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["Ha_UP"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["uekuti_l_1"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["uekuti_r_1"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["Kami_Oya"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["kami_Front_Mid1_L"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["kami_Front_Mid2_L"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["kami_Front_Mid3_L"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["kami_Front_Mid1_R"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["kami_Front_Mid2_R"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["kami_Front_Mid3_R"];
node.Scale1(1.00F, 0.94F, 1.00F);
node = nodes["Chichi_Right1"];
node.Scale1(0.90F, 1.00F, 0.91F);
node = nodes["Chichi_Left1"];
node.Scale1(0.90F, 1.00F, 0.91F);
            } catch (KeyNotFoundException) {
                Console.WriteLine("node not found.");
            }

            tmo.Save(dest_file);

            System.IO.File.Delete(source_file);
            System.IO.File.Move(dest_file, source_file);
            Console.WriteLine("Pedoed: " + source_file);

            return 0;
        }
    }
}
