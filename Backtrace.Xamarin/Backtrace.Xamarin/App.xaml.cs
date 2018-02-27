using Backtrace.Interfaces;
using Backtrace.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace Backtrace.Xamarin
{
	public partial class App : Application
	{
        private static BacktraceCredentials backtraceCredentials = new BacktraceCredentials(@"https://yolo.sp.backtrace.io:6098/", "328174ab5c377e2cdcb6c763ec2bbdf1f9aa5282c1f6bede693efe06a479db54");
        private static BacktraceClient backtraceClient = new BacktraceClient(backtraceCredentials);

        private static void DoSomething(int i = 0)
        {
            if (i == 2)
            {
                throw new ArgumentException("i");
            }
            Thread.Sleep(20);
            try
            {
                DoSomething(++i);

            }
            catch (Exception e)
            {
                backtraceClient.Send(e);
            }
        }

        public App ()
		{
			InitializeComponent();

			MainPage = new Backtrace.Xamarin.MainPage();


            //Report a new exception from current application
            try
            {
                Thread thread = new Thread(new ThreadStart(() => { DoSomething(0); }));
                thread.Start();
                thread.Join();
                var i = 0;
                var result = i / i;
            }
            catch (Exception exception)
            {
                var report = new BacktraceReport<object>(
                    exception: exception,
                    attributes: new Dictionary<string, object>() { { "AttributeString", "string" } },
                    attachmentPaths: new List<string>() { @"path to file attachment", @"patch to another file attachment" }
                );
                backtraceClient.Send(report);
            }

        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
