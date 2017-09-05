using System;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace TaskyUITests
{
    [TestFixture (Platform.Android)]
    [TestFixture (Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        readonly Func<AppQuery,AppQuery> AddButton;
        readonly Func<AppQuery,AppQuery> NameField;
        readonly Func<AppQuery,AppQuery> DescriptionField;

        public Tests(Platform platform)
        {
            this.platform = platform;

            if (platform == Platform.iOS)
            {
                AddButton = c => c.Button("Add");
                NameField = c => c.Class("UITextField").Index(0);
                DescriptionField = c => c.Class("UITextField").Index(1);
            }
            else
            {
                AddButton = c => c.Marked("Add Task");
                NameField = c => c.Class("EditText").Index(0);
                DescriptionField = c => c.Class("EditText").Index(1);
            }
        }

        [SetUp]
        public void BeforeEachTest ()
        {
            app = AppInitializer.StartApp (platform);
        }

        void AddANewTask (string name, string description)
        {
            app.Screenshot("AddingTask: " + name);
            app.Tap(AddButton);

            app.WaitForElement(NameField);
            app.EnterText(NameField, name);
            app.EnterText(DescriptionField, description);
            app.Screenshot("AddingTask: details set, tapping Save");

            app.Tap ("Save");
            app.WaitForNoElement("Save");
            app.Screenshot("AddingTask: back to list.");
        }

        [Test]
        //[Ignore]
        public void AppLaunches()
        {
            app.Repl();
        }

        [Test]
        public void TaskyPro_CreatingATask_ShouldBeSuccessful()
        {
            AddANewTask ("Get Milk", "Pick up some milk");
            Assert.Greater (app.Query ("Get Milk").Length, 0);
        }

        [Test]
        public void TaskyPro_DeletingATask_ShouldBeSuccessful()
        {
            AddANewTask ("Test Delete", "This item should be deleted");
            app.Tap ("Test Delete");

            app.WaitForElement("Delete");
            app.Screenshot("DeleteTask: start");
            app.Tap ("Delete");

            app.WaitForNoElement("Delete");
            app.Screenshot("DeleteTask: back to list");
            Assert.AreEqual (0, app.Query ("Test Delete").Length);
        }
    }
}

