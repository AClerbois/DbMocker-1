using Apps72.Dev.Data;
using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Linq;

namespace DbMocker.Tests
{
    [TestClass]
    public class DatabaseCommandTests
    {
        [TestMethod]
        public void Mock_ContainsSql_IntegerScalar_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(14);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                cmd.AddParameter("@ID", 1);
                var result = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, result);
            }
        }

        [TestMethod]
        public void Mock_ContainsSql_And_Parameter_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT") &&
                           c.Parameters.Any(p => p.ParameterName == "@ID"))
                .ReturnsScalar(14);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                cmd.AddParameter("@ID", 1);
                var result = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, result);
            }
        }

        [TestMethod]
        public void Mock_ExecuteNonQuery_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("INSERT"))
                .ReturnsScalar(14);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("INSERT ...");
                cmd.AddParameter("@ID", 1);
                var result = cmd.ExecuteNonQuery();

                Assert.AreEqual(14, result);
            }
        }

        [TestMethod]
        public void Mock_ExecuteTable_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(null)
                .ReturnsTable(new MockTable()
                {
                    Columns = new[] { "Col1", "Col2", "Col3" },
                    Rows = new object[,]
                    {
                        { 0,      1,      2 },
                        { 9,      8,      7 },
                        { 4,      5,      6 },
                    }
                });

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteTable(new
                {
                    Col1 = 0,
                    Col2 = 0,
                    Col3 = 0
                });

                Assert.AreEqual(3, result.Count());          // 2 rows
                Assert.AreEqual(1, result.First().Col2);     // First row / Col2
            }

        }

    }
}