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
            var typeName = "BraceNode";
            var name = "braceNode0000";
            var braceNode = LeafFactory.CreateInternalLeaf(typeName, name, null, null);

            Assert.IsNotNull(braceNode);
            Assert.AreEqual(typeName, braceNode.GetType().ToString());
            Assert.AreEqual(name, braceNode.GetName());
        }
    }
}
