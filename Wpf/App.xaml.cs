using System;
using System.Diagnostics;

namespace MyGraph
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
           // PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorListener());

        }
    }
    public class BindingErrorListener : TraceListener
    {
        public override void Write(string message)
        {
            try
            {
                throw new Exception(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public override void WriteLine(string message)
        {
            try
            {
                throw new Exception(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
