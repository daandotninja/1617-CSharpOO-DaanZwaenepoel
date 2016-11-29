using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using System.Drawing;
using System.IO;
using Globals;
using LogicLayer;
using FilterGUIMain;

namespace UnitTests
{
    [TestClass]
    public class TestArchitectuur
    {
        [TestMethod, Timeout(100)]
        public void TestIfMainFormConstructorTakesLogicInterfaceParameter()
        {
            Type x = typeof(MainForm);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { typeof(IFilter) });
            Assert.IsNotNull(constructor,
                $"Architectuur - constructor for \'MainForm\' does not take IFilter parameter!");
            constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"Architectuur - \'MainForm\' has a default constructor (not allowed)!");
        }

       
        [TestMethod, Timeout(100)]
        public void CheckMainFormhasNoFieldsOfTypeLogicImplementation()
        {
            Type x = typeof(MainForm);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { typeof(IFilter) });
            Assert.IsNotNull(constructor,
                $"Architectuur - constructor for \'MainForm\' does not take \'IFilter\' parameter!");
            constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"Architectuur - \'MainForm\' has a default constructor (not allowed)!");
            var fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
            var OK = (fields.Where(f => f.FieldType == typeof(ImageFilter)).Count() == 0);
            Assert.IsTrue(OK, $"Architectuur - \'MainForm\' uses a private instance of \'ImageFilter\' (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields();
            OK = (fields.Where(f => f.FieldType == typeof(ImageFilter)).Count() == 0);
            Assert.IsTrue(OK, $"Architectuur - \'MainForm\' uses a public instance of \'ImageFilter\' (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
        }
    }

    [TestClass]

    public class TestImageFilter
    {
        static private Random random;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            random = new Random();
        }


        public static Bitmap GetTestBitmap()
        {
            int w = random.Next(50) + 1;
            int h = random.Next(50) + 1;
            lock (random)
            {
                var bitmap = new Bitmap(w, h);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        bitmap.SetPixel(x, y, Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)));
                    }
                }
                return bitmap;
            }
        }

        private void SaveBitmap(Bitmap bitmap, string filename)
        {
            using (var bm = new Bitmap(bitmap))
            {
                bm.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
            }
        }


        [TestMethod, Timeout(1000)]
        public void TestImageFilterImplementsIFilter()
        {
            var f = new ImageFilter();
            Assert.IsTrue((f is IFilter), $"ImageFilter does not implement \'IFilter\' interface.");
        }

        [TestMethod, Timeout(1000)]
        public void TestLoad()
        {
            var filename = "test.png";
            IFilter filter = new ImageFilter();
            var testBitmap = GetTestBitmap();
            SaveBitmap(testBitmap, filename);
            filter.Load(filename);
            try
            {
                TestLoadHelper(filter, testBitmap);
            }
            finally
            {
                filter.OriginalImage.Dispose();
                filter.FilteredImage.Dispose();
            }
        }

        private void TestLoadHelper(IFilter filter, Bitmap original)
        {
            var filteredBitmap = filter.FilteredImage;
            Assert.IsNotNull(filteredBitmap, $"ImageFilter - after call to \'Load\', property \'FilteredImage\' is null.");
            Assert.IsTrue(((original.Width == filteredBitmap.Width) && (original.Height == filteredBitmap.Height)),
                         $"ImageFilter - after call to \'Load\', \'filteredImage\' has wrong size.");
            for (int x = 0; x < filteredBitmap.Width; x++)
            {
                for (int y = 0; y < filteredBitmap.Height; y++)
                {
                    var t = original.GetPixel(x, y);
                    var c = filteredBitmap.GetPixel(x, y);
                    Assert.IsTrue(
                        ((c.R == t.R) &&
                         (c.G == t.G) &&
                         (c.B == t.B)), $"ImageFilter - after call to \'Load\', property \'FilteredImage\' contains wrong bitmap.");
                }
            }
        }

        private void AttachDummyEventHandler(ImageFilter filter)
        {
            EventInfo eventInfo = typeof(ImageFilter).GetEvents().FirstOrDefault();
            if (eventInfo != null)
            {
                var x = eventInfo.EventHandlerType;
                var parameters = x.GetMethod("Invoke").GetParameters();
                Assert.IsTrue((parameters.Length == 1), $"\'ImageFilter\' event \'{eventInfo.Name}\' is not of type '\'Action<int>\'.");
                Assert.IsTrue((parameters[0].ParameterType.Name == "Int32"), $"\'ImageFilter\' event \'{eventInfo.Name}\' is not of type '\'Action<int>\'.");
                MethodInfo addHandler = eventInfo.GetAddMethod();
                Action<int> dummy = (i) => { };
                Object[] addHandlerArgs = { dummy };
                addHandler.Invoke(filter, addHandlerArgs);
            };
        }


        [TestMethod, Timeout(1000)]
        public void TestExecuteFilter()
        {

            var filename = "test1.png";
            ImageFilter filter = new ImageFilter();
            var testBitmap = GetTestBitmap();
            SaveBitmap(testBitmap, filename);
            filter.Load(filename);
            try
            {


                TestLoadHelper(filter, testBitmap);
                AttachDummyEventHandler(filter);
                filter.ExecuteFilter((p) => { return Color.FromArgb(1, 2, 3); });
                var filteredBitmap = filter.FilteredImage;
                Assert.IsNotNull(filteredBitmap, $"ImageFilter - upon calling \'ExecuteFilter\', property \'FilteredImage\' is null.");
                var originalBitmap = filter.OriginalImage;
                Assert.IsTrue(((originalBitmap.Width == filteredBitmap.Width) && (originalBitmap.Height == filteredBitmap.Height)),
                             $"ImageFilter - upon calling \'ExecuteFilter\', property \'filteredImage\' has wrong size.");
                for (int x = 0; x < filteredBitmap.Width; x++)
                {
                    for (int y = 0; y < filteredBitmap.Height; y++)
                    {
                        var c = filteredBitmap.GetPixel(x, y);
                        Assert.IsTrue(
                            ((c.R == 1) &&
                             (c.G == 2) &&
                             (c.B == 3)), $"ImageFilter - upon calling \'ExecuteFilter\', property \'FilteredImage\' contains wrong bitmap.");
                    }
                }
            }
            finally
            {
                filter.OriginalImage.Dispose();
                filter.FilteredImage.Dispose();
            }

        }


        [TestMethod, Timeout(1000)]
        public void TestApplyGreyscaleFilter()
        {
            var filename = "test2.png";
            ImageFilter filter = new ImageFilter();
            var testBitmap = GetTestBitmap();
            SaveBitmap(testBitmap, filename);
            filter.Load(filename);
            try
            {
                TestLoadHelper(filter, testBitmap);
                AttachDummyEventHandler(filter);


                filter.ApplyFilter(Filter.GreyScale);
                var filteredBitmap = filter.FilteredImage;
                Assert.IsNotNull(filteredBitmap, $"ImageFilter - upon calling \'ApplyFilter\', property \'FilteredImage\' is null.");
                var originalBitmap = filter.OriginalImage;
                Assert.IsTrue(((originalBitmap.Width == filteredBitmap.Width) && (originalBitmap.Height == filteredBitmap.Height)),
                             $"ImageFilter - upon calling \'ApplyFilter\', property \'FilteredImage\' has wrong size.");
                for (int x = 0; x < filteredBitmap.Width; x++)
                {
                    for (int y = 0; y < filteredBitmap.Height; y++)
                    {
                        var c = filteredBitmap.GetPixel(x, y);
                        Assert.IsTrue(
                            ((c.R == c.G) && (c.B == c.R)),
                            $"ImageFilter - upon calling \'ApplyFilter(Filter.Greyscale)\', property \'FilteredImage\' contains wrong bitmap.");
                    }
                }
            }
            finally
            {
                filter.OriginalImage.Dispose();
                filter.FilteredImage.Dispose();
            }
        }

        [TestMethod, Timeout(1000)]
        public void TestApplyRedFilter()
        {
            var filename = "test3.png";
            ImageFilter filter = new ImageFilter();
            var testBitmap = GetTestBitmap();
            SaveBitmap(testBitmap, filename);
            filter.Load(filename);
            try
            {
                TestLoadHelper(filter, testBitmap);
                AttachDummyEventHandler(filter);
                filter.ApplyFilter(Filter.Red);
                var filteredBitmap = filter.FilteredImage;
                Assert.IsNotNull(filteredBitmap, $"ImageFilter - upon calling \'ApplyFilter\', property \'FilteredImage\' is null.");
                var originalBitmap = filter.OriginalImage;
                Assert.IsTrue(((originalBitmap.Width == filteredBitmap.Width) && (originalBitmap.Height == filteredBitmap.Height)),
                             $"ImageFilter - upon calling \'ApplyFilter\', property \'filteredImage\' has wrong size.");
                for (int x = 0; x < filteredBitmap.Width; x++)
                {
                    for (int y = 0; y < filteredBitmap.Height; y++)
                    {
                        var c = filteredBitmap.GetPixel(x, y);
                        Assert.IsTrue(
                            ((c.G == 0) && (c.B == 0)),
                            $"ImageFilter - upon calling \'ApplyFilter(Filter.Red)\', property \'FilteredImage\' contains wrong bitmap.");
                    }
                }
            }
            finally
            {
                filter.OriginalImage.Dispose();
                filter.FilteredImage.Dispose();
            }
        }

        [TestMethod, Timeout(1000)]
        public void TestApplyGreenFilter()
        {
            var filename = "test4.png";
            ImageFilter filter = new ImageFilter();
            var testBitmap = GetTestBitmap();
            SaveBitmap(testBitmap, filename);
            filter.Load(filename);
            try
            {
                TestLoadHelper(filter, testBitmap);
                AttachDummyEventHandler(filter);
                filter.ApplyFilter(Filter.Green);
                var filteredBitmap = filter.FilteredImage;
                Assert.IsNotNull(filteredBitmap, $"ImageFilter - upon calling \'ApplyFilter\', property \'FilteredImage\' is null.");
                var originalBitmap = filter.OriginalImage;
                Assert.IsTrue(((originalBitmap.Width == filteredBitmap.Width) && (originalBitmap.Height == filteredBitmap.Height)),
                             $"ImageFilter - upon calling \'ApplyFilter\', property \'filteredImage\' has wrong size.");
                for (int x = 0; x < filteredBitmap.Width; x++)
                {
                    for (int y = 0; y < filteredBitmap.Height; y++)
                    {
                        var c = filteredBitmap.GetPixel(x, y);
                        Assert.IsTrue(
                            ((c.R == 0) && (c.B == 0)),
                            $"ImageFilter - upon calling \'ApplyFilter(Filter.Green)\', property \'FilteredImage\' contains wrong bitmap.");
                    }
                }
            }
            finally
            {
                filter.OriginalImage.Dispose();
                filter.FilteredImage.Dispose();
            }
        }

        [TestMethod, Timeout(1000)]
        public void TestApplyBlueFilter()
        {
            var filename = "test5.png";
            ImageFilter filter = new ImageFilter();
            var testBitmap = GetTestBitmap();
            SaveBitmap(testBitmap, filename);
            filter.Load(filename);
            try
            {
                TestLoadHelper(filter, testBitmap);
                AttachDummyEventHandler(filter);
                filter.ApplyFilter(Filter.Blue);
                var filteredBitmap = filter.FilteredImage;
                Assert.IsNotNull(filteredBitmap, $"ImageFilter - upon calling \'ApplyFilter\', property \'FilteredImage\' is null.");
                var originalBitmap = filter.OriginalImage;
                Assert.IsTrue(((originalBitmap.Width == filteredBitmap.Width) && (originalBitmap.Height == filteredBitmap.Height)),
                             $"ImageFilter - upon calling \'ApplyFilter\', property \'filteredImage\' has wrong size.");
                for (int x = 0; x < filteredBitmap.Width; x++)
                {
                    for (int y = 0; y < filteredBitmap.Height; y++)
                    {
                        var c = filteredBitmap.GetPixel(x, y);
                        Assert.IsTrue(
                            ((c.R == 0) && (c.G == 0)),
                            $"ImageFilter - upon calling \'ApplyFilter(Filter.Blue)\', property \'FilteredImage\' contains wrong bitmap.");
                    }
                }
            }
            finally
            {
                filter.OriginalImage.Dispose();
                filter.FilteredImage.Dispose();
            }
        }

        [TestMethod, Timeout(1000)]
        public void TestIfIFilterContainsEvent()
        {
            var eventInfo = typeof(IFilter).GetEvents().FirstOrDefault();
            Assert.IsNotNull(eventInfo, $"\'IFilter\' interface does not declare any events.");
            if (eventInfo != null)
            {
                var x = eventInfo.EventHandlerType;
                var parameters = x.GetMethod("Invoke").GetParameters();
                Assert.IsTrue((parameters.Length == 1), $"\'IFilter\' event \'{eventInfo.Name}\' is not of type '\'Action<int>\'.");
                Assert.IsTrue((parameters[0].ParameterType.Name == "Int32"), $"\'IFilter\' event \'{eventInfo.Name}\' is not of type '\'Action<int>\'.");
            };
        }

        private void TestIfImageFilterContainsEvent()
        {
            var eventInfo = typeof(ImageFilter).GetEvents().FirstOrDefault();
            Assert.IsNotNull(eventInfo, $"Class \'ImageFilter\' does not declare any events.");
            if (eventInfo != null)
            {
                var x = eventInfo.EventHandlerType;
                var parameters = x.GetMethod("Invoke").GetParameters();
                Assert.IsTrue((parameters.Length == 1), $"Class \'ImageFilter\' event \'{eventInfo.Name}\' is not of type '\'Action<int>\'.");
                Assert.IsTrue((parameters[0].ParameterType.Name == "Int32"), $"Class \'ImageFilter\' event \'{eventInfo.Name}\' is not of type '\'Action<int>\'.");
            };
        }

        [TestMethod, Timeout(1000)]
        public void TestIfExecuteFilterCallsEvent()
        {
            var filename = "test6.png";
            TestIfExecuteFilterCallsEventHelper(filename);
        }

        private void TestIfExecuteFilterCallsEventHelper(string filename)
        {

            TestIfImageFilterContainsEvent();

            ImageFilter filter = new ImageFilter();
            var testBitmap = GetTestBitmap();
            SaveBitmap(testBitmap, filename);
            filter.Load(filename);
            try
            {
                TestLoadHelper(filter, testBitmap);

                int eventCounter = 0;
                int eventValue = -1;

                EventInfo eventInfo = typeof(ImageFilter).GetEvents().FirstOrDefault();
                if (eventInfo != null)
                {
                    var parameters = eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters();
                    MethodInfo addHandler = eventInfo.GetAddMethod();
                    Action<int> dummy = (i) =>
                    {
                        eventCounter++;
                        eventValue = i;
                    };
                    Object[] addHandlerArgs = { dummy };
                    addHandler.Invoke(filter, addHandlerArgs);
                };
                filter.ExecuteFilter((p) => { return p; });
                Assert.IsTrue((eventCounter > 0), $"Method \'ExecuteFilter\' does not call its \'{eventInfo.Name}\' event.");
                Assert.IsTrue((eventCounter > Math.Min(testBitmap.Height, testBitmap.Width) / 2),
                    $"Method \'ExecuteFilter\' calls its \'{eventInfo.Name}\' event only {eventCounter} times for bitmap with {testBitmap.Height} lines of {testBitmap.Width} pixels.");
                Assert.IsTrue((eventValue >= 90), $"ExecuteFilter - Last value from \'{eventInfo.Name}\' event is {eventValue}, expected 100.");
            }
            finally
            {
                filter.OriginalImage.Dispose();
                filter.FilteredImage.Dispose();
            }
        }

        [TestMethod]
        public void TestIfExecuteFilterTestsNullEvent()
        {
            var filename = "test7a.png";
            TestIfExecuteFilterCallsEventHelper(filename);
            filename = "test7b.png";
            var filter = new ImageFilter();
            var testBitmap = GetTestBitmap();
            SaveBitmap(testBitmap, filename);
            filter.Load(filename);

            try
            {
                TestLoadHelper(filter, testBitmap);
                try
                {
                    filter.ExecuteFilter((p) => { return p; });
                }
                catch (Exception e)
                {
                    Assert.Fail($"Method \'ExecuteFilter\' calls event even if it is null: \'{e.GetType().ToString()}\' is thrown.");
                }
            }
            finally
            {
                filter.OriginalImage.Dispose();
                filter.FilteredImage.Dispose();
            }
        }

    }


}
