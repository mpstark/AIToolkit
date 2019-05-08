using BattleTech;
using EnhancedAI.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnhancedAITests
{
    [TestClass]
    public class LeafFactoryTests
    {
        [TestMethod]
        public void BraceNodeTest()
        {
            var name = "braceNode0000";
            var types = new[] { typeof(string), typeof(BehaviorTree), typeof(AbstractActor)};
            var braceNode = LeafFactory.CreateInternalLeaf("BraceNode", types, name, null, null);

            Assert.IsNotNull(braceNode);
            Assert.AreEqual("BraceNode", braceNode.GetType().ToString());
            Assert.AreEqual(name, braceNode.GetName());
        }
    }
}
