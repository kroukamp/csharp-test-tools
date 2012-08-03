using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace TestRunner
{
    class Program
    {
        static Hashtable GetTestFiles(string path) {      
            Hashtable fileInfo = new Hashtable(); 
            try {
                var files = from file in Directory.EnumerateFiles(path,"*.r", SearchOption.AllDirectories)
                        from line in File.ReadLines(file)
                        where line.Contains("Tags")
                        select new {
                            File = file,
                            Line = line
                        };
                foreach (var f in files) {fileInfo[f.File] = f.Line;}
                return fileInfo;
            }
            catch (UnauthorizedAccessException UAEx) { Console.WriteLine(UAEx.Message); }
            catch (PathTooLongException PathEx) { Console.WriteLine(PathEx.Message); }
            return fileInfo;
        }
        static void PrintMenu() {
            Console.WriteLine("Test Runner version 1");
            Console.WriteLine("Menu:");
            Console.WriteLine("----------------------");
            Console.WriteLine("1. Run all tests ");
            Console.WriteLine("2. Exit ");
            Console.WriteLine("----------------------");        
        }
        static int PrintMenuAndGetOption() {
            // Print out menu, get user input
            PrintMenu();
            string optString = Console.ReadLine();
            try {
                int opt = Convert.ToInt16(optString); 
                return opt;
            } catch (FormatException e) {Console.WriteLine("Please enter a valid number!");}
            return 0;
        }
        static string runRBatchCommand(string rBatchFileName) {
            Process p = new Process(); 
            ProcessStartInfo info = new ProcessStartInfo(@"C:\Program Files\R\R-2.15.1\bin\R.exe");
            info.Arguments = "CMD BATCH " + rBatchFileName;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            //info.FileName = rBatchFileName;
            p.StartInfo = info;
            //Console.WriteLine(" CMD BATCH " + rBatchFileName);
            try {
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                return output;
            }  catch (Exception e)  { Console.WriteLine(e); } 
            return "";
        }
        static void Main(string[] args)
        {
            // Print menu. Read in option. Execute options. Print results. 
            int option = 0;
            while (option != 2) {
                option = PrintMenuAndGetOption();
                if (option == 1) {
                    Hashtable testFiles = GetTestFiles(@"c:\dev\r\table_regression\");
                    foreach (string file in testFiles.Keys) {
                        Console.WriteLine("Testing {0} with properties {1}", file, testFiles[file]);
                        runRBatchCommand(file); 
                    }
                } else if (option == 2) { 
                    Console.WriteLine("byebye, dear!");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
    }
}
