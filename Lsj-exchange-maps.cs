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
    public static int nedit = 0;

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


	public void MyFunction1()
	{
		// Write your own function here
	}
    public static void Main()
	{
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string botkonto = "Lsjbot";
        string fromlang = "ceb";
        Site fsite = new Site("https://"+fromlang+".wikipedia.org", botkonto, password);
        string tolang = "sv";
        Site tsite = new Site("https://" + tolang + ".wikipedia.org", botkonto, password);
        PageList pl = new PageList(fsite);

        Dictionary<string, string> sclist = new Dictionary<string, string>();
        Dictionary<string, string> cslist = new Dictionary<string, string>();

        cslist.Add("Cocos (Keeling) Islands", "Cocos Islands");




        cslist.Add("Australia Heard and McDonald Islands", "Heard- och McDonaldöarna");









//        cslist.Add("Indian Ocean", "Indiska oceanen");




        cslist.Add("Jersey", "Jersey");













        cslist.Add("United Kingdom Cayman Islands", "Caymanöarna");


        cslist.Add("Lebanon", "Libanon");
















        cslist.Add("Republic of Macedonia", "Makedonien");



        cslist.Add("Macau", "Macao");














        cslist.Add("Australia Norfolk Island", "Norfolkön");




        cslist.Add("Nepal", "Nepal");














        cslist.Add("Israel", "Israel");




        cslist.Add("Réunion", "Reunion");













        cslist.Add("Svalbard", "Svalbard");









        cslist.Add("Syria", "Syrien");



        cslist.Add("Indian Ocean", "Indiska oceanen");















        cslist.Add("Pacific Ocean", "Stilla havet");

//        {{#switch:{{{1}}}
//| name   = Andorra
//| top    = 42.675
//| bottom = 42.4
//| left   = 1.3875
//| right  = 1.8125
//| image  = Andorra location map.svg
//| image1 = Andorra relief location map.jpg
//}}<noinclude>
//{{Location map/Info}}
//{{Documentation}}
//</noinclude>

//{{#switch:{{{1}}}
//| namn   = Andorra
//| topp    = 42.675
//| botten = 42.4
//| vänster   = 1.3875
//| höger  = 1.8125
//| bild  = Andorra location map.svg
//| bild1 = Andorra relief location map.jpg
//}}<noinclude>{{Kartposition/Info}}

//[[Kategori:Koordinatmallar|Andorra]]

//</noinclude>

        Dictionary<string, string> pardict = new Dictionary<string, string>();
        pardict.Add("| name =", "| namn =");
        pardict.Add("| top =", "| topp =");
        pardict.Add("| bottom =", "| botten =");
        pardict.Add("| left =", "| vänster =");
        pardict.Add("| right =", "| höger =");
        pardict.Add("| image =", "| bild =");
        pardict.Add("| image1 =", "| bild1 =");
        //pardict.Add("{{documentation}}", "[[Kategori:Koordinatmallar]]");

        if (fromlang == "sv")
        {
            foreach (string s in sclist.Keys)
            {
                Page pf = new Page(fsite, "Mall:Kartposition "+s);
                if (tryload(pf, 2))
                {
                    if (pf.Exists())
                    {
                        Page pt = new Page(tsite, "Batakan:Location map " + sclist[s]);
                        tryload(pt, 2);
                        if (pt.Exists())
                            continue;
                        pt.text = pf.text;
                        while (pt.text.IndexOf("| ") > 0 )
                            pt.text = pt.text.Replace("| ", "|");
                        while (pt.text.IndexOf(" =") > 0)
                            pt.text = pt.text.Replace(" =", "=");
                        foreach (string sss in pardict.Keys)
                            pt.text = pt.text.Replace(pardict[sss].Replace(" ",""),sss);
                        while (pt.text.IndexOf("| name = ") > 0)
                            pt.text = pt.text.Replace("| name = ", "| name =");
                        pt.text = pt.text.Replace("| name =" + s, "| name = " + sclist[s]);

                        pt.text = pt.text.Replace("[[Kategori:Koordinatmallar|" + s + "]]", "{{documentation}}");
                        Console.WriteLine(pt.text);
                        Console.WriteLine("============================================");
                        trysave(pt, 2);
                    }
                }
            }
        }
        else
        {
            foreach (string s in cslist.Keys)
            {
                Page pf = new Page(fsite, "Plantilya:Location map " + s);
                if (tryload(pf, 2))
                {
                    if (pf.Exists())
                    {
                        Page pt = new Page(tsite, "Mall:Kartposition " + cslist[s]);
                        tryload(pt, 2);
                        if (pt.Exists())
                            continue;
                        pt.text = pf.text;
                        while (pt.text.IndexOf("| ") > 0)
                            pt.text = pt.text.Replace("| ", "|");
                        while (pt.text.IndexOf(" =") > 0)
                            pt.text = pt.text.Replace(" =", "=");
                        foreach (string sss in pardict.Keys)
                            pt.text = pt.text.Replace(sss.Replace(" ", ""), pardict[sss]);
                        while (pt.text.IndexOf("| namn = ") > 0)
                            pt.text = pt.text.Replace("| namn = ", "| namn =");
                        pt.text = pt.text.Replace("| namn =" + s, "| namn = " + cslist[s]);
                        pt.text = pt.text.Replace("{{Location map/Info}}", "{{Kartposition/Info}}");

                        pt.text = pt.text.Replace("{{documentation}}","\n[[Kategori:Koordinatmallar|" + cslist[s] + "]]");
                        pt.text = pt.text.Replace("{{Documentation}}", "\n[[Kategori:Koordinatmallar|" + cslist[s] + "]]");
                        if ( !pt.text.Contains("[[Kategori:"))
                            pt.text = pt.text.Replace("</noinclude>", "\n[[Kategori:Koordinatmallar|" + cslist[s] + "]]\n</noinclude>");

                        while (pt.text.Contains("[[Category:"))
                        {
                            int i1 = pt.text.IndexOf("[[Category:");
                            int i2 = pt.text.IndexOf("]]",i1);
                            pt.text = pt.text.Remove(i1, i2 - i1 + 2);
                        }

                        Console.WriteLine(pt.text);
                        Console.WriteLine("============================================");
                        trysave(pt, 2);
                        //Console.ReadLine();
                    }
                }
            }

        }
        
        

	}



}
