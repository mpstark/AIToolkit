using EnhancedAI.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnhancedAITests
{
    [TestClass]
    public class BehaviorNodeFactoryTests
    {
        [TestMethod]
        public void BraceNodeTest()
        {
            var typeName = "BraceNode";
            var name = "braceNode0000";
            var braceNode = BehaviorNodeFactory.CreateBehaviorNode(typeName, name, null, null);

            Assert.IsNotNull(braceNode);
            Assert.AreEqual(typeName, braceNode.GetType().ToString());
            Assert.AreEqual(name, braceNode.GetName());
        }
    }
}
