using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Globals;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace UnitTests
{
    public static class TestValues
    {
        private static List<string> nodeList = new List<string>();
        private static Random random = new Random();

        public static string GetUniqueName()
        {
            string name;
            do
            {
                name = GetName();
            } while (nodeList.Contains(name));
            nodeList.Add(name);
            return name;
        }

        private static string GetName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            string textPart = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string digitPart = new string(Enumerable.Repeat(digits, 3).Select(s => s[random.Next(s.Length)]).ToArray());
            return textPart + digitPart;
        }

        public static int GetRandomScore()
        {
            return random.Next(11);
        }
    }


    [TestClass]
    public class GlobalTests
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


        [TestMethod, Timeout(100)]
        public void UnitTestsAreFunctioning()
        {
            //Dummy: 1 point for making unit tests work
        }

        [TestMethod, Timeout(500)]
        public void TestEnumGrades()
        {
            var expectedValues = new string[] { "Insufficient", "Poor", "Good", "Excellent" };
            var x = typeof(Grades);
            Assert.IsTrue((x.IsEnum), $"\"Grades\" is not an enum type.");
            var values = Enum.GetNames(x).ToList();
            Assert.IsTrue((values.Count == expectedValues.Length),
                              $"Enum type \"Grades\" should have {expectedValues.Length} values, not {values.Count}.");
            foreach (var value in expectedValues)
            {
                Assert.IsTrue((values.Contains(value)), $"Enum type \"Grades\" should contain \"{value}\" as a value.");
            }
        }

        [TestMethod, Timeout(500)]
        public void TestCourseResultCtor()
        {
            Type x = typeof(CourseResult);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"\"nCourseResult\" has a default constructor (not allowed)!");
            constructor = x.GetConstructor(new Type[] { typeof(string), typeof(int) });
            Assert.IsNotNull(constructor,
                $"\"nCourseResult\" does not contain an constructor with parameters of type \'string\' & \'int\'.");
            // check if can be instantiated with valid parameters;
            var dummy = new CourseResult("test", 1);
            // check if exceptions are thrown with invalid params
            try
            {
                dummy = new CourseResult("test", -1);
                Assert.Fail($"Constructor for \"nCourseResult\" does not throw ArgumentOutOfRangeException for negative scores.");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // this is expected, no problem                
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                Assert.Fail($"Constructor for \"nCourseResult\" with negative score throws {e.GetType().Name} instead of ArgumentOutOfRangeException !");
            }

            try
            {
                dummy = new CourseResult("test", 11);
                Assert.Fail($"Constructor for \"nCourseResult\" does not throw ArgumentOutOfRangeException for score > 10.");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // this is expected, no problem                
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                Assert.Fail($"Constructor for \"nCourseResult\" with score > 10 throws {e.GetType().Name} instead of ArgumentOutOfRangeException !");
            }

        }

        [TestMethod, Timeout(500)]
        public void TestCourseResultNameProperty()
        {
            var testName = TestValues.GetUniqueName();
            var dummy = new CourseResult(testName, 5);
            Assert.IsTrue((dummy.Name == testName),
                              $"\"Name\" property for \"CourseResult\" does not return the value passed to the constructor (expected \"{testName}\", was \"{dummy.Name}\" )");
        }

        [TestMethod, Timeout(500)]
        public void TestCourseResultScoreProperty()
        {
            var testName = TestValues.GetUniqueName();
            int testScore = TestValues.GetRandomScore();
            var dummy = new CourseResult(testName, testScore);
            Assert.IsTrue((dummy.Score is double),
                              $"\"Score\" property for \"CourseResult\" is of type \"{dummy.Score.GetType().Name}\", should be \"double\" )");
            Assert.IsTrue((Math.Abs(dummy.Score - testScore) < 0.001),
                              $"\"Score\" property for \"CourseResult\" does not return the value passed to the constructor (expected \"{1.0 * testScore:F1}\", was \"{dummy.Score:F1}\" )");
        }

        [TestMethod, Timeout(500)]
        public void TestCourseResultGradeProperty()
        {
            int[] scoreList = new int[] { 0, 4, 6, 7, 8, 9 };
            Grades[] gradeList = new Grades[] { Grades.Insufficient, Grades.Insufficient, Grades.Poor, Grades.Good, Grades.Good, Grades.Excellent };
            var testName = TestValues.GetUniqueName();
            for (var i = 0; i < scoreList.Length; i++)
            {

                var dummy = new CourseResult(testName, scoreList[i]);
                Assert.IsTrue((dummy.Grade == gradeList[i]),
                              $"\"Grade\" property for \"CourseResult\" has wrong value for score of {scoreList[i]}: is \"{dummy.Grade}\", should be \"{gradeList[i]}\" )");
            }
        }

        [TestMethod, Timeout(500)]
        public void TestCourseResultNrOfParticipantsProperty()
        {
            var testName = TestValues.GetUniqueName();
            var dummy = new CourseResult(testName, 5);
            Assert.IsTrue((dummy.NrOfParticipants == 1),
                              $"\"NrOfParticipants\" property for \"CourseResult\" has wrong value after instantiation: expected 1, was \"{dummy.NrOfParticipants}\" )");
        }

        [TestMethod, Timeout(500)]
        public void TestCourseResultAddScoreMethod()
        {
            int[] testScores = new int[10];
            for (int i = 0; i < testScores.Length; i++)
            {
                testScores[i] = TestValues.GetRandomScore();
            }
            var dummy = new CourseResult(TestValues.GetUniqueName(), testScores[0]);
            for (int i = 1; i < testScores.Length; i++)
            {
                dummy.AddScore(testScores[i]);
                Assert.IsTrue((dummy.NrOfParticipants == i + 1),
                              $"\"AddScore\" method for \"CourseResult\" does not increment NrOfParticipants: expected {i + 1}, was \"{dummy.NrOfParticipants}\" )");
            }
            Assert.IsTrue(((dummy.Score - testScores.Average(i => (float)i) < 0.001)),
                              $"\"Score\" property for \"CourseResult\" has wrong value after calling \"AddScore\": expected \"{testScores.Average(i => (float)i):F1}\", was \"{dummy.Score:F1}\" )");

        }

        [TestMethod, Timeout(500)]
        public void TestCourseResultCompareToMethod()
        {
            string[] testList = new string[10];
            for (int i = 0; i < testList.Length; i++)
            {
                testList[i] = TestValues.GetUniqueName();
            }
            var dummy = new CourseResult(testList[0], 5);
            for (int i = 0; i < testList.Length; i++)
            {
                var dummy2 = new CourseResult(testList[i], 5);
                Assert.IsTrue((dummy.CompareTo(dummy2) == testList[0].CompareTo(testList[i])),
                              $"\"CompareTo\" method for \"CourseResult\" returns wrong value: \"{dummy.Name}\" & \"{dummy2.Name}\" returns \"{dummy.CompareTo(dummy2)}\": expected \"{testList[0].CompareTo(testList[i])}\" )");
            }
        }

        [TestMethod, Timeout(500)]
        public void TestCourseResultToStringMethod()
        {
            var name = TestValues.GetUniqueName();
            int[] testScores = new int[12];
            for (int i = 0; i < testScores.Length; i++)
            {
                testScores[i] = TestValues.GetRandomScore();
            }
            var dummy = new CourseResult(name, testScores[0]);
            for (int i = 1; i < testScores.Length; i++)
            {
                dummy.AddScore(testScores[i]);
            }
            var text = dummy.ToString();
            Assert.IsTrue((text.ToLower().Contains(name.ToLower())),
                             $"result from \"ToString\" method for \"CourseResult\" does not contain name: \"{dummy.ToString()}\", name was \"{name}\".");
            Assert.IsTrue((text.Contains($"{dummy.Score:F1}")),
                             $"result from \"ToString\" method for \"CourseResult\" does not contain score: \"{dummy.ToString()}\", score was \"{dummy.Score:F1}\".");
            Assert.IsTrue((text.Contains($"{dummy.Grade}")),
                             $"result from \"ToString\" method for \"CourseResult\" does not contain grade: \"{dummy.ToString()}\", grade was \"{dummy.Grade}\".");
            Assert.IsTrue((text.Contains($"{dummy.NrOfParticipants}")),
                             $"result from \"ToString\" method for \"CourseResult\" does not contain number of particopants: \"{dummy.ToString()}\", is missing \"{dummy.NrOfParticipants}\".");
        }

       
    }
}
