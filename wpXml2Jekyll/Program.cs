﻿using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace wpXml2Jekyll
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                FreeConsole();
                Application.Run(new UIForm());
            }
            else
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Usage: wpXml2Jekyll [wordpress export file] [output folder]");
                    Environment.Exit(1);
                }
                var wordpressXmlFile = args[0];
                var outputFolder = args[1];

                var posts = new PostImporter().ReadWpPosts(wordpressXmlFile);
                int count = new PostWriter().WritePost(posts, outputFolder);
                Console.WriteLine("Saved " + count + " posts");
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();
    }
}
