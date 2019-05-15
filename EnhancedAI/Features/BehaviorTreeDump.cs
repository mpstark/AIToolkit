using System.IO;
using EnhancedAI.Util;

namespace EnhancedAI.Features
{
    public static class BehaviorTreeDump
    {
        public enum DumpType
        {
            JSON,
            Text,
            Both
        }

        public static void DumpTree(BehaviorNode root, string directory, string name, DumpType type)
        {
            // dump to json
            if (type == DumpType.JSON || type == DumpType.Both)
            {
                BehaviorNodeJSONRepresentation.FromNode(root)
                    .ToJSONFile(Path.Combine(directory, $"{name}.json"));
            }

            // dump to text
            if (type == DumpType.Text || type == DumpType.Both)
            {
                root.DumpTree(Path.Combine(directory, $"{name}.txt"));
            }
        }

        public static void DumpTrees(DumpType type)
        {
            var dirPath = Path.Combine(Main.Directory, "Dump");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            DumpTree(AlwaysPass_BT.InitRootNode(null, null, null), dirPath, "AlwaysPass_BT", type);
            DumpTree(CoreAI_BT.InitRootNode(null, null, null), dirPath, "CoreAI_BT", type);
            DumpTree(PatrolAI_BT.InitRootNode(null, null, null), dirPath, "PatrolAI_BT", type);
            DumpTree(FleeAI_BT.InitRootNode(null, null, null), dirPath, "FleeAI_BT", type);
            DumpTree(PatrolAndShoot_BT.InitRootNode(null, null, null), dirPath, "PatrolAndShoot_BT", type);
            DumpTree(TutorialSprint_BT.InitRootNode(null, null, null), dirPath, "TutorialSprint_BT", type);
            DumpTree(PatrolOppAI_BT.InitRootNode(null, null, null), dirPath, "PatrolOppAI_BT", type);
            DumpTree(PanzyrAI_BT.InitRootNode(null, null, null), dirPath, "PanzyrAI_BT", type);
        }
    }
}
