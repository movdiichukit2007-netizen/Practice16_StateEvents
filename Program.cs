using System;
using System.IO;
using System.Windows.Forms;

namespace Practice16_StateEvents;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        if (args.Length > 0 && args[0] == "--screenshots")
        {
            string outputDirectory = args.Length > 1
                ? args[1]
                : Directory.GetCurrentDirectory();

            Directory.CreateDirectory(outputDirectory);
            using MainForm form = new();
            form.CreateScreenshots(outputDirectory);
            return;
        }

        Application.Run(new MainForm());
    }
}
