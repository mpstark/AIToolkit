using BattleTech;
using EnhancedAI.Selectors.Unit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnhancedAITests.Selectors
{
    [TestClass]
    public class SelectorTests
    {
        public static bool AlwaysTrue(AbstractActor unit)
        {
            return true;
        }

        public static bool AlwaysFalse(AbstractActor unit)
        {
            return false;
        }

        public static bool IsUnitNull(AbstractActor unit)
        {
            return unit == null;
        }

        [TestMethod]
        public void CustomSelectorAlwaysTrueTest()
        {
            var selector = new CustomUnitSelector();
            var value = selector.Select("SelectorTests.AlwaysTrue", null);

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void CustomSelectorAlwaysFalseTest()
        {
            var selector = new CustomUnitSelector();
            var value = selector.Select("SelectorTests.AlwaysFalse", null);

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void CustomSelectorIsUnitNullTest()
        {
            var selector = new CustomUnitSelector();
            var value = selector.Select("SelectorTests.IsUnitNull", null);

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void CustomSelectorMissingFunctionTest()
        {
            var selector = new CustomUnitSelector();
            var value = selector.Select("SelectorTests.FunctionMissing", null);

            Assert.IsFalse(value);
        }
    }
}