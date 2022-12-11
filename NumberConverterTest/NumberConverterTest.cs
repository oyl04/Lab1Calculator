using Lab1Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NumberConverterTest
{
    [TestClass]
    public class NumberConverterTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            CellCoordinates cell = new CellCoordinates();
            cell.row = 288; cell.column = 24;
            Assert.AreEqual(cell, NumberConverter.From26System("Y289"));
            cell.row = 248; cell.column = 50;
            Assert.AreEqual(cell, NumberConverter.From26System("AY249"));
            cell.row = 5; cell.column = 24;
            Assert.AreEqual(cell, NumberConverter.From26System("Y6"));
            cell.row = 0; cell.column = 702;
            Assert.AreEqual(cell, NumberConverter.From26System("AAA1"));
            ++cell.row; ++cell.column;
            Assert.AreEqual(cell, NumberConverter.From26System("AAB2"));
            ++cell.row; cell.column += 52;
            Assert.AreEqual(cell, NumberConverter.From26System("ACB3"));
            ++cell.row; cell.column += 26 * 21;
            Assert.AreEqual(cell, NumberConverter.From26System("AXB4"));
            cell.row += 2091;
            Assert.AreEqual(cell, NumberConverter.From26System("AXB2095"));
            cell.row -= 20;
            Assert.AreEqual(cell, NumberConverter.From26System("AXB2075"));
        }
    }
}