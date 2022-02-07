using System;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Celestials;
using Kirali.Framework;


namespace KiraliConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("KIRALI CONSOLE TESTING");
            Console.WriteLine("Hello Universe!\n\n");

            while (true)
            {
                Console.Write("KIRALI_>> ");
                string inp = Console.ReadLine();
                Console.WriteLine("\n");
                

                if (inp.StartsWith("statm_prob "))
                {
                    int pnum = 0;
                    Int32.TryParse(inp.Replace("statm_prob", ""), out pnum);
                    Console.WriteLine("Solving Stat_Mech problem " + pnum + ".");
                    bool iffound = true;
                    
                    switch (pnum)
                    {
                        case 21:
                            Console.WriteLine("\nProblem 2.1 Helper Consists of making a series of bins for molecules to fall into.");
                            bool do21loop = true;
                            while (do21loop)
                            {
                                int molct = 0;
                                while (true)
                                {
                                    Console.Write("\nHow many molecules to toss into 10 bins? : ");
                                    Int32.TryParse(Console.ReadLine(), out molct);
                                    if (molct != 0)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid or failed parse of molecule count.");
                                    }
                                }
                                Console.WriteLine("Tossing " + molct + " molecules...");
                                int[] bins = ArrayHandler.SetAll(10, 0);
                                for (int m = 0; m < molct; m++)
                                {
                                    int binFall = Kirali.Framework.Random.Int(0, 10);
                                    bins[binFall]++;
                                }
                                Console.WriteLine("Resulting Bins:");
                                for (int tr21tptb = 0; tr21tptb < 10 * 8 + 11; tr21tptb++) { Console.Write("█"); }
                                Console.Write("\n█        █        █        █        █        █        █        █        █        █        █");
                                Console.Write("\n");
                                for (int binc = 0; binc < bins.Length; binc++)
                                {
                                    Console.Write("█" + " Bin " + JustNumberCell(binc + 1, '0', 2) + " ");

                                    //Console.WriteLine("Bin " + JustNumberCell(binc + 1, '0', 2) + "  contains " + JustNumberCell(bins[binc], '0', (int)Math.Round(Math.Log10(molct),0)) + " molecules; " + JustNumberCell((int)Math.Round(100.0 * (double)(bins[binc]) / molct, 2), '0', 2) + "% of the whole.");
                                }
                                Console.Write("█\n");
                                for (int binc = 0; binc < bins.Length; binc++)
                                {
                                    if(molct >= 100 && molct < 1000) { Console.Write("█   " + JustNumberCell(bins[binc], '0', 2) + "   "); }
                                    else if(molct >= 1000) { Console.Write("█  " + JustNumberCell(bins[binc], '0', 4) + "  "); }
                                    
                                    //Console.WriteLine(" + JustNumberCell((int)Math.Round(100.0 * (double)(bins[binc]) / molct, 2), '0', 2) + "% of the whole.");
                                }
                                Console.Write("█\n");
                                for (int binc = 0; binc < bins.Length; binc++)
                                {
                                    Console.Write("█   " + JustNumberCell((int)Math.Round(100.0 * (double)(bins[binc]) / molct, 2), '0', 2) + "%  ");

                                    //Console.WriteLine(JustNumberCell((int)Math.Round(100.0 * (double)(bins[binc]) / molct, 2), '0', 2) + "% of the whole.");
                                }
                                Console.Write("█\n█        █        █        █        █        █        █        █        █        █        █\n"); 
                                for (int tr21tptb = 0; tr21tptb < 10 * 8 + 11; tr21tptb++) { Console.Write("█"); }
                                Console.Write("\n");

                                //again?
                                if (!GetDoAgain()) { do21loop = false; break; }
                            }

                            break;
                        case 25:
                            //PV = NRT
                            double V25  = 0.3;
                            double N25  = 10E+24;
                            double T25i = 400;
                            double T25f = 403;


                            break;
                        default:
                            Console.WriteLine("Stat_Mech problem not found. Please try again.");
                            iffound = false;
                            break;
                    }
                    if (iffound)
                    {
                        Console.WriteLine("Completed program for problem # " + pnum);
                    }
                }


                //kill loop command
                if(inp.StartsWith("endprog") || inp.StartsWith("close") || inp.StartsWith("kill") || inp.StartsWith("break"))
                {
                    break;
                }
            }
        }

        private static string JustNumberCell(int num, char fill, int size)
        {
            double numsize = 1;
            for (int p = 1; p < 32; p++)
            {
                if(num >= Math.Pow(10, p)) { numsize++; }
                else { break; }
            }
            string cell = "";
            if(numsize <= size)
            {
                for (int pc = 0; pc < size - numsize; pc++) { cell += fill; }
            }
            else
            {
                return num.ToString();
            }
            return cell + num.ToString();
        }

        private static bool GetDoAgain()
        {
            bool doa = true;
            
            while (true)
            {
                Console.Write("Do again? (y/n): ");
                bool inpv = true;
                switch (Console.ReadKey().KeyChar)
                {
                    case 'Y':
                    case 'y':
                        doa = true;
                        break;
                    case 'N':
                    case 'n':
                        doa = false;
                        break;
                    default:
                        inpv = false;
                        Console.WriteLine("Input not understood.");
                        break;
                }
                Console.Write("\n");
                if (inpv)
                {
                    break;
                }
            }
            return doa;
        }
    }
}
