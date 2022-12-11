using Lab1Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace CellTest
{
    [TestClass]
    public class CellTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Cell first = new Cell(5, 3);
            Assert.AreEqual(first.getName(), "D6");
            List <Cell> list1 = new List<Cell>();
            List <Cell> list2 = new List<Cell>();
            Cell second = new Cell(4, 8);
            list1.Add(first); 
            list1.Add(second); 
            list2.Add(second);
            first.setCell("4 + 3", "7", list1, list2);
            Assert.AreEqual(first.referencesFromThis.Count(), 2);
            Assert.AreEqual(first.DependenciesToThis.Count(), 1);
        }
    }
}