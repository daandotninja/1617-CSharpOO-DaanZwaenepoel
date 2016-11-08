using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Globals;
using DataAccessLayer;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class DataAccessLayerTests

    {
        #region auxiliary methods

        private void CheckProperty(Type t, string propName, Type[] propTypes, bool hasGetter, bool hasSetter)
        {
            var props = t.GetProperties();
            var prop = props.Where(p => p.Name == propName).FirstOrDefault();
            Assert.IsNotNull(prop, $"{t.GetType().Name} has no public \"{propName}\" property");
            Assert.IsTrue(Array.Exists(propTypes, p => p.Name == prop.PropertyType.Name),
                              $"{t.FullName}: property {propName} is a {prop.PropertyType.Name}");
            Assert.IsTrue((prop.CanRead == hasGetter), $"{t.Name}: property \"{propName}\" has {(prop.CanRead ? "a" : "no")} public Getter ");
            Assert.IsTrue((prop.CanWrite == hasSetter), $"{t.Name}: property \"{propName}\" has {(prop.CanWrite ? "a" : "no")} public Setter ");

        }

        private void CheckMethod(Type t, string methodName, Type[] returnTypes, Type[] parameterTypes)
        {
            var methods = t.GetMethods();
            // check if method exists with right signature
            var method = methods.Where(m =>
            {
                if (m.Name != methodName) return false;
                var parameters = m.GetParameters();
                if ((parameterTypes == null || parameterTypes.Length == 0)) return parameters.Length == 0;
                if (parameters.Length != parameterTypes.Length) return false;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    // if (parameters[i].ParameterType != parameterTypes[i])
                    if (!parameters[i].ParameterType.IsAssignableFrom(parameterTypes[i]))
                        return false;
                }
                return true;
            }).FirstOrDefault();
            Assert.IsNotNull(method, $"{t.FullName} has no public \"{methodName}\" method with the right signature");

            // check returnType
            Assert.IsTrue(Array.Exists(returnTypes, r => r.Name == method.ReturnType.Name),
                              $"{t.FullName}: method \"{methodName}\" returns a \"{method.ReturnType.Name}\"");
        }

        #endregion auxiliary methods

        [TestMethod, Timeout(500)]
        public void TestIDataAccessInterfaceDeclaredOK()
        {
            Type x = typeof(IDataAccess);
            Assert.IsNotNull(x, $"interface \"IDataAccess\" not declared!");
            CheckMethod(x, "ImportScores", new Type[] { typeof(void) }, new Type[] { typeof(string) });
            CheckMethod(x, "AddCourseScore", new Type[] { typeof(void) }, new Type[] { typeof(string), typeof(int) });
            CheckProperty(x, "SortedCourseList", new Type[] { typeof(List<CourseResult>) }, true, false);
        }

        [TestMethod, Timeout(500)]
        public void TestDataAccessImplementsIDataAccess()
        {
            IDataAccess dal = new DataAccessImplementation() as IDataAccess;
            Assert.IsNotNull(dal,
                $"DataAccessLayerTests - Class \"DataAccessImplementation\" does not implement interface \"IDataAccess\".");
        }

        [TestMethod, Timeout(500)]
        public void TestSortedCourseListIsInitialized()
        {
            IDataAccess dal = new DataAccessImplementation();
            Assert.IsNotNull(dal.SortedCourseList,
                $"DataAccessLayerTests - property \"SortedCourseList\" is null after instantiation.");
            Assert.IsTrue((dal.SortedCourseList.Count == 0),
                $"DataAccessLayerTests - property \"SortedCourseList\" is not empty after instantiation.");
        }

        [TestMethod, Timeout(500)]
        public void TestAddCourseScoreMethodWorks()
        {
            IDataAccess dal = new DataAccessImplementation();
            var testName1 = TestValues.GetUniqueName();
            var testScore1A = TestValues.GetRandomScore();
            var testScore1B = TestValues.GetRandomScore();
            var testName2 = TestValues.GetUniqueName();
            var testScore2 = TestValues.GetRandomScore();
            dal.AddCourseScore(testName1, testScore1A);
            Assert.IsTrue((dal.SortedCourseList.Count == 1),
                $"DataAccessLayerTests - property \"SortedCourseList\" has wrong number of entries after adding one course score: expected {1}, was {dal.SortedCourseList.Count()}.");
            dal.AddCourseScore(testName1, testScore1B);
            Assert.IsTrue((dal.SortedCourseList.Count == 1),
                $"DataAccessLayerTests - property \"SortedCourseList\" has wrong number of entries after adding two scores for same course: expected {1}, was {dal.SortedCourseList.Count()}.");
            Assert.IsTrue((dal.SortedCourseList.First().NrOfParticipants == 2),
                $"DataAccessLayerTests - property \"SortedCourseList\" has wrong number of participants in entry after adding two scores for same course: expected {2}, was {dal.SortedCourseList.First().NrOfParticipants}.");
            Assert.IsTrue(((dal.SortedCourseList.First().Score-(testScore1A+testScore1B)/2.0) < 0.001),
                $"DataAccessLayerTests - property \"SortedCourseList\" has wrong score in entry after adding two scores for same course: expected {(testScore1A + testScore1B) / 2.0:F1}, was {dal.SortedCourseList.First().Score:F1}.");
            dal.AddCourseScore(testName2, testScore2);
            Assert.IsTrue((dal.SortedCourseList.Count == 2),
                $"DataAccessLayerTests - property \"SortedCourseList\" has wrong number of entries after adding two different courses: expected {2}, was {dal.SortedCourseList.Count()}.");
        }

        [TestMethod, Timeout(500)]
        public void TestAddCourseScoreMethodIsCaseInsensitive()
        {
            IDataAccess dal = new DataAccessImplementation();
            var testName = TestValues.GetUniqueName().ToUpper();
            var testScore = TestValues.GetRandomScore();
            dal.AddCourseScore(testName, testScore);
            dal.AddCourseScore(testName.ToLower(), testScore);
            Assert.IsFalse((dal.SortedCourseList.Count == 2),
                $"DataAccessLayerTests - property \"SortedCourseList\" method \"AddCourseScore\" is case sensitive for course name.");
        }

        [TestMethod]
        public void TestImportScoresMethod()
        {
            const string filename = "testdata.csv";
            var testName = TestValues.GetUniqueName();
            var testScore = TestValues.GetRandomScore();
            if (testScore > 8) testScore -= 2;

            string[] lines = new string[10];
            lines[0] = $"{testName}, {testScore}";
            for (int i = 1; i < lines.Length; i++)
            {
                lines[i] = $"{TestValues.GetUniqueName()}, {TestValues.GetRandomScore()}";
            }
            lines[5] = $"{testName}, {testScore+2}";
            File.WriteAllLines(filename, lines);

            IDataAccess dal = new DataAccessImplementation();
            dal.ImportScores(filename);
            Assert.IsTrue((dal.SortedCourseList.Count == lines.Length-1),
                $"DataAccessLayerTests - property \"SortedCourseList\" has wrong number of entries after adding one course score: expected {lines.Length - 1}, was {dal.SortedCourseList.Count()}.");
            var names = dal.SortedCourseList.Select(r => r.Name.ToLower());
            for (int i = 0; i < lines.Length; i++)
            {
                var name = lines[i].Split(',')[0].ToLower();
                Assert.IsTrue((names.Contains(name)),
                            $"DataAccessLayerTests - after importing file, a course name is missing from property \"SortedCourseList\": expected {name}.");
            }
            var result = dal.SortedCourseList.Where(r => r.Name.ToLower() == testName.ToLower()).First();
            Assert.IsTrue((Math.Abs(result.Score - testScore - 1) < 0.001),
                $"DataAccessLayerTests - after importing file, property \"SortedCourseList\" returns wrong score for a course (\"{testName}\"): was {result.Score:F1}, expected {testScore + 1.0:F1}.");
        }

        [TestMethod, Timeout(500)]
        public void TestSortedCourseListpropertyIsSorted()
        {
            IDataAccess dal = new DataAccessImplementation();
            var testName = TestValues.GetUniqueName();
            var testScore = TestValues.GetRandomScore();
            if (testScore > 8) testScore -= 2;
            dal.AddCourseScore(testName, testScore);
            for (int i = 1; i < 10; i++)
            {
                dal.AddCourseScore(TestValues.GetUniqueName(), TestValues.GetRandomScore());
            }
            dal.AddCourseScore(testName, testScore+1);
                       
            var names = dal.SortedCourseList.Select(r => r.Name.ToLower()).ToArray();
            for (int i = 1; i < names.Length; i++)
            {
                Assert.IsTrue((names[i].CompareTo(names[i-1]) > 0),
                $"DataAccessLayerTests - property \"SortedCourseList\" is not sorted in ascending order.");
            }


        }

       
    }
}
