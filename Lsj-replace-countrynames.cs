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
                Console.Error.WriteLine("ts we " + message);
                itry++;
                if (itry > iattempt)
                    return false;
                else
                    Thread.Sleep(600000);//milliseconds
            }
            catch (WikiBotException e)
            {
                string message = e.Message;
                Console.Error.WriteLine("ts wbe " + message);
                if (message.Contains("Bad title"))
                    return false;
                itry++;
                if (itry > iattempt)
                    return false;
                else
                    Thread.Sleep(600000);//milliseconds
            }
            catch (IOException e)
            {
                string message = e.Message;
                Console.Error.WriteLine("ts ioe " + message);
                itry++;
                if (itry > iattempt)
                    return false;
                else
                    Thread.Sleep(600000);//milliseconds
            }
        }

    }


	public static void addpages(Site site,PageList pl)
	{
        //Page pp1 = new Page(site, "Aega monophthalma"); pl.Add(pp1);
        List<string> sl = new List<string>();
        //sl.Add("Lista över insjöar i Arjeplogs kommun (1-1000)");
        sl.Add("Ðakovića Kosa");
        sl.Add("Ðanuša");
        sl.Add("Ðaprovica");

        foreach (string s in sl)
        {
            Page pp = new Page(site, s); pl.Add(pp);
        }



	}

    public static string ReplaceOne(string textparam, string oldt, string newt, int position)
    {
        string text = textparam;
        int oldpos = text.IndexOf(oldt, position);
        if (oldpos < 0)
            return text;
        text = text.Remove(oldpos, oldt.Length);
        text = text.Insert(oldpos, newt);

        return text;
    }

    public static void Main()
	{
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string botkonto = "Lsjbot";
        string makelang = "ceb";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);
        site.defaultEditComment = "Fixing country categories";
        site.minorEditByDefault = true;

        int nround = 1;

        Dictionary<string, string> countrydict = new Dictionary<string, string>();
        //countrydict.Add("Myanmar", "Burma");
        //countrydict.Add("Norwega", "Noruwega");
        //countrydict.Add("Marwekos", "Maruwekos");
        //countrydict.Add("Habagatang Koreya", "Habagatang Korea");
        //countrydict.Add("Amihanang Koreya", "Amihanang Korea");
        //countrydict.Add("Malaysia", "Malasya");
        //countrydict.Add("Mosambike", "Mozambique");
        //countrydict.Add("Kuba", "Cuba");
        //countrydict.Add("Aserbayan", "Aserbaiyan");
        //countrydict.Add("Bruney", "Brunei");
        //countrydict.Add("Indonesya", "Indonesia");
        //countrydict.Add("Iraq", "Irak");
        //countrydict.Add("Bolivia", "Bolibya");
        //countrydict.Add("Chile", "Tsile");
        //countrydict.Add("Georgia (nasud)", "Heyorhiya");
        countrydict.Add("Ireland", "Irlanda");

        foreach (string fromcountry in countrydict.Keys)
        {
            string tocountry = countrydict[fromcountry];
            nedit = 0;
            PageList pl = new PageList(site);
            PageList pl1 = new PageList(site);

            //Select how to get pages. Uncomment as needed.

            //Add pages "by hand":
            //addpages(site,pl);

            //Find articles from a category
            //pl.FillAllFromCategoryTree("Geografi i Goiás");
            //pl1.FillAllFromCategoryTree("Eufriesea");
            //foreach (Page p in pl1)
            //    pl.Add(p);
            //pl1.FillAllFromCategoryTree("Euglossa");
            //foreach (Page p in pl1)
            //    pl.Add(p);
            //pl1.FillAllFromCategoryTree("Eulaema");
            //foreach (Page p in pl1)
            //    pl.Add(p);
            //pl1.FillAllFromCategoryTree("Exaerete");
            //foreach (Page p in pl1)
            //    pl.Add(p);
            //pl.FillAllFromCategory(fromcountry);

            //Find subcategories of a category
            //pl.FillSubsFromCategory("Svampars vetenskapliga namn");

            //Find articles from all the links to an article, mostly useful on very small wikis
            pl.FillFromLinksToPage(fromcountry);

            //Find articles containing a specific string
            //pl.FillFromSearchResults("insource:\"Användare:Lsjbot/Algoritmer\"", 4999);
            //pl.FillFromSearchResults("insource:\"http://www.itis.gov;http://\"", 4999);

            //Set specific article:
            //Page pp = new Page(site, "Citrontrogon");pl.Add(pp);

            //Skip all namespaces except articles:
            pl.RemoveNamespaces(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 100, 101 });

            Dictionary<string, string> replacedict = new Dictionary<string, string>();

            //replacedict.Add("Åboland-Turunmaa", "Åboland");
            //replacedict.Add("[[Kategoriya:" + fromcountry + "]]", "[[Kategoriya:" + tocountry + "]]");
            //replacedict.Add("[[kategoriya:" + fromcountry + "]]", "[[Kategoriya:" + tocountry + "]]");
            //replacedict.Add("[[Category:" + fromcountry + "]]", "[[Kategoriya:" + tocountry + "]]");
            //replacedict.Add("[[category:" + fromcountry + "]]", "[[Kategoriya:" + tocountry + "]]");
            replacedict.Add("[[" + fromcountry + "]]", "[[" + tocountry + "]]");

            Dictionary<string, string> regexdict = new Dictionary<string, string>();
            //regexdict.Add(@"\| timezone *= \[\[Fernando de Noronha Time\|FNT\]\]", "| timezone             = [[Brasilia Time|BRT]]");
            //regexdict.Add(@"\| timezone_DST *= \[\[Amazon Summer Time\|AMST\]\]", "| timezone_DST         = [[Brasilia Summer Time|BRST]]");
            //regexdict.Add(@"\| utc_offset *= -2", "| utc_offset           = -3");
            //regexdict.Add(@"\| utc_offset_DST *= -3", "| utc_offset_DST       = -2");




            List<string> linkword = new List<string>();
            //linkword.Add("Catalogue of Life");

            //Require title to contain one in requiretitle list:
            List<string> requiretitle = new List<string>();
            //requiretitle.Add("Radioprogram nerlagda");

            //Require ALL in requireword list:
            List<string> requireword = new List<string>();
            //requireword.Add("obotskapad");
            //requireword.Add("= -3\n");
            //requireword.Add("Brasilien");


            //Require AT LEAST ONE in requireone list:
            List<string> requireone = new List<string>();



            List<string> vetoword = new List<string>();
            //vetoword.Add("Argentina");
            //vetoword.Add("Island");
            //vetoword.Add("isländska");


            DateTime oldtime = DateTime.Now;
            oldtime = oldtime.AddSeconds(5);

            Console.WriteLine("Pages to change : " + pl.Count().ToString());

            int iremain = pl.Count();

            bool resume = true;

            foreach (Page p in pl)
            {
                iremain--;
                //Skip start of alphabet:
                //if (String.Compare(p.title,"Sicydium") < 0 )
                //    continue;
                if (!resume)
                {
                    if (p.title == "Moylan Lough")
                        resume = true;
                    else
                        continue;
                }

                if (!tryload(p, 2))
                    continue;
                if (!p.Exists())
                    continue;


                string origtitle = p.title;

                //Follow redirect:

                if (p.IsRedirect())
                {
                    p.title = p.RedirectsTo();
                    if (!tryload(p, 2))
                        continue;
                    if (!p.Exists())
                        continue;
                }

                //Check so required title actually present:

                if (requiretitle.Count > 0)
                {
                    bool onefound = false;
                    foreach (string s in requiretitle)
                        if (p.title.Contains(s))
                            onefound = true;

                    if (!onefound)
                    {
                        Console.WriteLine("requiretitle not found");
                        continue;
                    }
                }


                //Check so all required strings actually present:

                bool allfound = true;
                foreach (string s in requireword)
                    if (!p.text.Contains(s))
                        allfound = false;

                if (!allfound)
                {
                    Console.WriteLine("requireword not found");
                    continue;
                }

                if (requireone.Count > 0)
                {
                    bool onefound = false;
                    foreach (string s in requireone)
                        if (p.text.Contains(s))
                            onefound = true;

                    if (!onefound)
                    {
                        Console.WriteLine("requireone not found");
                        continue;
                    }
                }

                //Check so no vetoword are present:

                bool vetofound = false;
                foreach (string s in vetoword)
                    if (p.text.Contains(s))
                        vetofound = true;

                if (vetofound)
                {
                    Console.WriteLine("vetoword found");
                    continue;
                }

                //If redirect, go back to redirect page:

                //if (origtitle != p.title)
                //{
                //    p.title = origtitle;
                //    p.Load();
                //}

                string origtext = p.text;

                //Do the actual replacement:

                foreach (KeyValuePair<string, string> replacepair in replacedict)
                {
                    p.text = p.text.Replace(replacepair.Key, replacepair.Value);

                }

                foreach (KeyValuePair<string, string> replacepair in regexdict)
                {
                    p.text = Regex.Replace(p.text, replacepair.Key, replacepair.Value);

                }


                //special for mismatching tags:
                //int itag = p.text.ToLower().IndexOf("<i>");
                //int refend = p.text.IndexOf("</ref>", itag);
                //int bend = p.text.ToLower().IndexOf("</b>", itag);

                //if (refend < 0)
                //    refend = 999999;
                //if (bend < 0)
                //    bend = 999999;
                //if (refend < bend)
                //{
                //    p.text = ReplaceOne(p.text, "</ref>", "''</ref>", itag);
                //    p.text = p.text.Replace("<i>", "''").Replace("<I>", "''");
                //}
                //else if (bend < refend)
                //{
                //    p.text = ReplaceOne(p.text, "</b>", "''</b>", itag);
                //    p.text = ReplaceOne(p.text, "</B>", "''</B>", itag);
                //    p.text = p.text.Replace("<i>", "''").Replace("<I>", "''");
                //}
                //else
                //    p.text = p.text.Replace("<i>", "").Replace("<I>", "");

                //Wikilink first occurrence of each word, if not linked already:

                foreach (string s in linkword)
                {
                    if (p.text.IndexOf(s) < 0)
                        continue;
                    string slinked = "[[" + s + "]]";
                    if (p.text.IndexOf(slinked) < 0)
                    {
                        p.text = p.text.Insert(p.text.IndexOf(s), "[[");
                        p.text = p.text.Replace("[[" + s, slinked);
                    }
                }

                //Save the result:

                if (p.text != origtext)
                {
                    //Bot.editComment = "Ersätter och wikilänkar";
                    //isMinorEdit = true;

                    if (trysave(p, 4))
                    {
                        nedit++;
                        if ((nedit < 4) && (nround == 1))
                        {
                            Console.Write("<ret>");
                            Console.ReadLine();
                        }
                        DateTime newtime = DateTime.Now;
                        while (newtime < oldtime)
                            newtime = DateTime.Now;
                        oldtime = newtime.AddSeconds(5);
                    }
                }
                Console.WriteLine(iremain.ToString() + " remaining.");
            }

            Console.WriteLine("Total # edits = " + nedit.ToString());
            nround++;
        }
        //while (nedit > 0);

	}



}
