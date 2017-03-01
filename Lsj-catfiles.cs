//Originally from [[Wikipedia:Projekt DotNetWikiBot Framework/Innocent bot/Ny parameter i Mall Ishockeyspelare]]
//Extensively modified by Lsj

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Threading;
using System.Net;

using DotNetWikiBot;

class MyBot : Bot
{
    public static void Main()
	{
        List<string> infiles = new List<string>();
        infiles.Add("artname-sv-20150811.txt");
        infiles.Add("missing-adm1-toartname.txt");

        string outfile = "artname-sv.txt";

        using (StreamWriter sw = new StreamWriter(outfile))
        {
            foreach (string file in infiles)
            {
                Console.WriteLine(file + "...");
                using (StreamReader sr = new StreamReader(file))
                {

                    while (!sr.EndOfStream)
                    {
                        sw.WriteLine(sr.ReadLine());
                    }
                }
                Console.WriteLine(file + " done!");
            }
        }

	}



}
