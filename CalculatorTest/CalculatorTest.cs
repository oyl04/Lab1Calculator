using Lab1Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NumberConverterTest
{
    [TestClass]
    public class CalculatorTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(Calculator.Evaluate("5 ^ 4"), 625);
            Assert.AreEqual(Calculator.Evaluate("100 ^ (1/2)"), 10);
            Assert.AreEqual(Calculator.Evaluate("(70 / 2) ^ 2 + 4"), 1229);
            Assert.AreEqual(Calculator.Evaluate("inc(5) + dec(3) + dec(2)"), 9);
            Assert.AreEqual(Calculator.Evaluate("mod(-5;3)"), 1);
            Assert.AreEqual(Calculator.Evaluate("mod(5;3)"), 2);
            Assert.AreEqual(Calculator.Evaluate("div(399, 8)"), 49);
            Assert.AreEqual(Calculator.Evaluate("div(399, 8) - div(300, 5)"), -11);
            Assert.AreEqual(Calculator.Evaluate("3 * 3 ^ 3"), 81);
            Assert.AreEqual(Calculator.Evaluate("(3 * 3) ^ 3"), 729);
            Assert.AreEqual(Calculator.Evaluate("div(500;3)-div(200;3)"), 100);
            Assert.AreEqual(Calculator.Evaluate("3 ^ 2 ^ 2"), 81);
            Assert.AreEqual(Calculator.Evaluate("inc(dec(inc(dec(inc(dec(5)))))"), 5);
        }
    }
}