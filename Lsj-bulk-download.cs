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
using System.Web;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;


using DotNetWikiBot;

class MyBot : Bot
{
    public static int nedit = 0;
    public static string tabstring = "\t";
    public static char tabchar = tabstring[0];

    public static Dictionary<double, List<double>> lldone = new Dictionary<double, List<double>>();

    public static List<string> coordparams = new List<string>(); //possible template parameters for latitude/longitude

    public static bool tryload(Page p, int iattempt)
    {
        int itry = 1;


        while (true)
        {

            try
            {
                p.Load();
                return true;
            }
            catch (WebException e)
            {
                string message = e.Message;
                Console.Error.WriteLine(message);
                itry++;
                if (itry > iattempt)
                    return false;
            }
        }

    }

    public static bool trysave(Page p, int iattempt)
    {
        int itry = 1;


        while (true)
        {

            try
            {
                p.Save();

                return true;
            }
            catch (WebException e)
            {
                string message = e.Message;
                Console.Error.WriteLine(message);
                itry++;
                if (itry > iattempt)
                    return false;
            }
        }

    }

    public static double tryconvertdouble(string word)
    {
        double i = 9999.9;

        try
        {
            i = Convert.ToDouble(word);
        }
        catch (OverflowException)
        {
            Console.WriteLine("i Outside the range of the Double type: " + word);
        }
        catch (FormatException)
        {
            try
            {
                i = Convert.ToDouble(word.Replace(".", ","));
            }
            catch (FormatException)
            {
                Console.WriteLine("i Not in a recognizable double format: " + word.Replace(".", ","));
            }
            //Console.WriteLine("i Not in a recognizable double format: " + word);
        }

        return i;

    }

    public static void get_webfile(string url, string filename)
    {
        //WebClient client = new WebClient();

        // Add a user agent header in case the
        // requested URI contains a query.

        //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

        try
        {
            Console.WriteLine("Downloading " + url + filename);
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url+filename, filename);
            }
            //using (Stream SourceStream = client.OpenRead(url+filename))
            //{
            //    int maxlen = 100000000;
            //    byte[] buffer = new byte[maxlen];
            //    int len = SourceStream.Read(buffer, 0, maxlen);
            //    Console.WriteLine("len = " + len.ToString());
            //    using (Stream DestinationStream = File.Create(filename))
            //    {
            //        DestinationStream.Write(buffer, 0, len);
            //        DestinationStream.Close();
            //    }
            //    SourceStream.Close();
            //}

            Console.WriteLine("Done. Sleeping for 1 minute.");
            Thread.Sleep(60000);
        }
        catch (WebException e)
        {
            string message = e.Message;
            Console.Error.WriteLine(message);
        }
        return;
    }




    public static void Main()
	{
        Console.WriteLine("<ret>");
        Console.ReadLine();

        //string url = "http://www.viewfinderpanoramas.org/dem3/";

        //for (char c = 'E'; c < 'V'; c++)
        //{
        //    for (int i = 1; i < 61; i++)
        //    {
        //        string filename = "A00.zip";
        //        filename = filename.Replace('A',c);
        //        string nr = i.ToString();
        //        while (nr.Length < 2)
        //            nr = "0" + nr;
        //        filename = filename.Replace("00", nr);
        //        get_webfile(url, filename);
        //    }
        //}

        string url = "https://www.uhr.se/globalassets/_uhr.se/studier-och-antagning/antagningsstatistik/aldre-statistik/urval-2/";
        //https://www.uhr.se/globalassets/_uhr.se/studier-och-antagning/antagningsstatistik/aldre-statistik/urval-2/vt2001_antagna_urval2_larosate.xls
        for (int year = 2001; year < 2009; year++)
        {
            string filename = "vt2010_antagna_urval2_larosate.xls";
            filename = filename.Replace("2010", year.ToString());
            get_webfile(url, filename);
            filename = filename.Replace("vt", "ht");
            get_webfile(url, filename);

        }

    }



}
