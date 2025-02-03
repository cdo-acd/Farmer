using System;
using System.IO;
using System.Linq;

namespace Farmer
{
    class Program
    {
        static void ShowBanner()
        {
            Console.WriteLine(Config.banner);
            Console.WriteLine("Author: @domchell - MDSec ActiveBreach\n\n");
        }
        static void ShowHelp()
        {
            ShowBanner();
            Console.WriteLine("farmer.exe <port> [seconds] [output] [--disable-ess]");
        }
        static void ParseArgs(string[] args)
        {

            if (args.Length <= 4)
            {
                if (args.Length == 1)
                {
                    Console.WriteLine("[*] Opening server on port {0}", args[0]);
                    Config.port = int.Parse(args[0]);
                    return;
                }
                
                for (int i = 0; i < args.Length; i++)
                {
                    if (!string.IsNullOrEmpty(args[i]))
                    {
                        if (args[i].ToLower().Contains("--disable-ess")) {
                            Config.ess = false;
                            Console.WriteLine("[*] Disabling ESS");
                            args = args.Where(w => w != args[i]).ToArray();
                            if (args.Length == 1)
                            {
                                return;
                            }
                            break;
                        }
                    }
                }

                Config.port = int.Parse(args[0]);
                Config.timer = int.Parse(args[1]);

                Console.WriteLine("[*] Opening server on port {0}", args[0]);
                Console.WriteLine("[*] Farming for {0} seconds", args[1]);

                if (args.Length == 3)
                {
                    Config.output = args[2];
                    Console.WriteLine("[*] Writing output to {0}", args[2]);
                }

            }
        }
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 4)
            {
                ShowHelp();
                return;
            }
            ShowBanner();

            ParseArgs(args);


            try
            {
                // if file output is required, open a streamwriter handler
                if(Config.output != null)
                {
                    Config.sw = File.AppendText(Config.output);
                }

                Farmer farm = new Farmer();
                farm.Initialize(Config.port);

                if (Config.timer == 0)
                {
                    // loop indefinitely until soemthing stops
                    while (Config.timer != -1)
                        System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    // loop until the timer runs out
                    while (Config.timer > 0)
                    {
                        System.Threading.Thread.Sleep(1000);
                        Config.timer--;
                    }
                }

                // clean up, stop the listener and close file handle with time for final connection to close
                Console.WriteLine("\n[*] Shutting down");
                System.Threading.Thread.Sleep(10000);
                farm.Stop();
                if (Config.output != null)
                {
                    Config.sw.Close();
                }

                Console.WriteLine("[*] Harvesting complete, hope you had a good crop");

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
