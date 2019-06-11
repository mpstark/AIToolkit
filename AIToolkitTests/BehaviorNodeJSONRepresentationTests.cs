using AIToolkit.Resources;
using AIToolkit.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AIToolkitTests
{
    [TestClass]
    public class BehaviorNodeJSONRepresentationTests
    {
        [TestMethod]
        public void SerializeDeserializeRepReserializeTest()
        {
            var root = CoreAI_BT.InitRootNode(null, null, null);

            var repFirst = BehaviorNodeDef.FromNode(root);
            var jsonFirst = repFirst.ToJSONString();

            var repSecond = SerializeUtil.FromJSON<BehaviorNodeDef>(jsonFirst);
            var jsonSecond = repSecond.ToJSONString();

            Assert.AreEqual(jsonFirst, jsonSecond);
        }

        [TestMethod]
        public void SerializeDeserializeNodeReserializeTest()
        {
            var root = CoreAI_BT.InitRootNode(null, null, null);

            var repFirst = BehaviorNodeDef.FromNode(root);
            var jsonFirst = repFirst.ToJSONString();

            var reconstructedRoot = repFirst.ToNode(null, null);

            var repSecond = BehaviorNodeDef.FromNode(reconstructedRoot);
            var jsonSecond = repSecond.ToJSONString();

            Assert.AreEqual(jsonFirst, jsonSecond);
        }
    }
}
