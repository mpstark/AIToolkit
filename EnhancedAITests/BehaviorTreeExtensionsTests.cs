using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnhancedAI.Util;

namespace EnhancedAITests
{
    [TestClass]
    public class BehaviorTreeExtensionsTests
    {
        [TestMethod]
        public void FindPresentChildUnderCompositeNode()
        {
            var root = CoreAI_BT.InitRootNode(null, null, null) as CompositeBehaviorNode;

            var foundChild = root.FindChildByName("isShutdown0000", out var foundParent);

            var realParent = root?.Children.Find(node => node.GetName() == "if_shutdown__restart") as CompositeBehaviorNode;
            var realChild = realParent?.Children.Find(node => node.GetName() == "isShutdown0000");

            Assert.AreEqual(realChild, foundChild);
            Assert.AreEqual(realParent, foundParent);
        }

        [TestMethod]
        public void FindPresentChildUnderDecoratorNode()
        {
            var root = CoreAI_BT.InitRootNode(null, null, null) as CompositeBehaviorNode;

            var foundChild = root.FindChildByName("clearSensorLock0000", out var foundParent);

            var realParentParent = root?.Children.Find(node => node.GetName() == "startup_cleanup") as DecoratorBehaviorNode;
            var realParent = realParentParent?.ChildNode as DecoratorBehaviorNode;
            var realChild = realParent?.ChildNode;

            Assert.AreEqual(realChild, foundChild);
            Assert.AreEqual(realParent, foundParent);
        }

        [TestMethod]
        public void FindNonPresentChild()
        {
            var root = CoreAI_BT.InitRootNode(null, null, null);
            var foundChild = root.FindChildByName("NOT HERE", out var foundParent);

            Assert.IsNull(foundParent);
            Assert.IsNull(foundChild);
        }

        [TestMethod]
        public void RemoveChildFromCompositeByNameTest()
        {
            var root = CoreAI_BT.InitRootNode(null, null, null);
            var childName = "canMove";

            var foundChild = root.FindChildByName(childName, out var foundParent);
            Assert.IsNotNull(foundChild);
            Assert.IsNotNull(foundParent);

            var removed = root.RemoveChildByName(childName);
            Assert.IsNotNull(removed);
            Assert.AreEqual(foundChild, removed);

            var notFound = root.FindChildByName(childName, out var notFoundParent);
            Assert.IsNull(notFound);
            Assert.IsNull(notFoundParent);

            var parent = root.FindChildByName(foundParent.GetName(), out _);
            Assert.IsNotNull(parent);
        }

        [TestMethod]
        public void RemoveChildFromDecoratorNode()
        {
            var root = CoreAI_BT.InitRootNode(null, null, null);
            var childName = "clearSensorLock0000";

            var foundChild = root.FindChildByName(childName, out var foundParent);
            Assert.IsNotNull(foundChild);
            Assert.IsNotNull(foundParent);

            var removed = root.RemoveChildByName(childName);
            Assert.IsNotNull(removed);
            Assert.AreEqual(foundChild, removed);

            var notFound = root.FindChildByName(childName, out var notFoundParent);
            Assert.IsNull(notFound);
            Assert.IsNull(notFoundParent);

            Assert.IsNotNull(root.FindChildByName(foundParent.GetName(), out _));
        }

        [TestMethod]
        public void AddRemoveNodeTest()
        {
            var root = CoreAI_BT.InitRootNode(null, null, null);
            var childName = "if_inspire_available__maybe_inspire";

            var foundChild = root.FindChildByName(childName, out var foundParent);
            Assert.IsNotNull(foundChild);
            Assert.AreEqual(root, foundParent);

            var removedChild = root.RemoveChildByName(childName);
            Assert.IsNotNull(removedChild);
            Assert.AreEqual(foundChild, removedChild);

            Assert.IsNull(root.FindChildByName(childName, out _));

            var parentName = "free_engage";
            root.AddNode(removedChild, parentName, "lanceDetectsEnemies0000", true);
            Assert.AreEqual(removedChild, root.FindChildByName(childName, out var addedParent));
            Assert.AreEqual(parentName, addedParent.GetName());
            Assert.AreEqual(0, (addedParent as CompositeBehaviorNode).Children.FindIndex(child => child.Equals(removedChild)));
        }
    }
}