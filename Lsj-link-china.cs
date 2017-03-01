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
            catch (WikiBotException e)
            {
                string message = e.Message;
                Console.Error.WriteLine(message);
                itry++;
                if (itry > iattempt)
                    return false;
            }
            //catch (EditConflictException e)
            //{
            //    string message = e.Message;
            //    Console.Error.WriteLine(message);
            //    itry++;
            //    if (itry > iattempt)
            //        return false;
            //}
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
        string makelang = "sv";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);
        site.defaultEditComment = "Länkar Kinaartiklar";
        site.minorEditByDefault = true;
        Site zhsite = new Site("https://zh.wikipedia.org", botkonto, password);

        int nround = 1;
        do
        {
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
            pl.FillFromCategory("Sidnamn med kinesiska tecken");

            //Find subcategories of a category
            //pl.FillSubsFromCategory("Svampars vetenskapliga namn");

            //Find articles from all the links to an article, mostly useful on very small wikis
            //pl.FillFromLinksToPage("Användare:Lsjbot/Algoritmer");

            //Find articles containing a specific string
            //pl.FillFromSearchResults("insource:\"Användare:Lsjbot/Algoritmer\"", 4999);
            //pl.FillFromSearchResults("insource:\"http://www.itis.gov;http://\"", 4999);

            //Set specific article:
            //Page pp = new Page(site, "Citrontrogon");pl.Add(pp);

            //Skip all namespaces except articles:
            pl.RemoveNamespaces(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 100, 101 });

            Dictionary<string, string> replacedict = new Dictionary<string, string>();

            //replacedict.Add("[[Kategoriya:Kabukiran sa Awstralya nga mas taas kay sa 8000 metros ibabaw sa dagat nga lebel]]", "");

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
            requireword.Add("obotskapad");
            //requireword.Add("= -3\n");
            //requireword.Add("Brasilien");


            //Require AT LEAST ONE in requireone list:
            List<string> requireone = new List<string>();



            List<string> vetoword = new List<string>();
            vetoword.Add("[[zh:");
            vetoword.Add("förgrening");
            //vetoword.Add("isländska");

            Page pd = new Page(site, "Användare:Lsjbot/Kinadubletter");
            tryload(pd, 2);

            DateTime oldtime = DateTime.Now;
            oldtime = oldtime.AddSeconds(5);

            Console.WriteLine("Pages to check : " + pl.Count().ToString());

            int iremain = pl.Count();

            foreach (Page p in pl)
            {
                //Skip start of alphabet:
                if (String.Compare(p.title, "莫力庙") < 0)
                    continue;

                if (!tryload(p, 2))
                    continue;
                if (!p.Exists())
                    continue;

                string origtitle = p.title;

                //Follow redirect:

                if (p.IsRedirect())
                {
                    p.title = p.RedirectsTo();
                    Console.WriteLine("RedirectsTo = " + p.RedirectsTo());
                    Page p2 = new Page(site, p.RedirectsTo());
                    
                    if (!tryload(p2, 2))
                        continue;
                    if (!p2.Exists())
                        continue;
                    p.title = p2.title;
                    p.text = p2.text;
                    //Console.WriteLine(p2.text);
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

                //Check wikidata:

                List<string> wdlinks;
                try
                {
                    wdlinks = p.GetWikidataLinks();
                }
                catch (WebException e)
                {
                    string message = e.Message;
                    Console.Error.WriteLine("ts we " + message);
                    continue;
                }

                if (wdlinks.Count > 0)
                {
                    bool zhfound = false;
                    Console.WriteLine("wdlinks:");
                    foreach (string iwl in wdlinks)
                    {
                        Console.WriteLine(iwl);
                        if (iwl == "zh")
                            zhfound = true;
                    }

                    if (zhfound)
                        continue;
                    //Console.ReadLine();
                }

                //Find in Chinese Wikipedia:

                Page pzh = new Page(zhsite, origtitle);

                tryload(pzh, 2);

                if (!pzh.Exists())
                {
                    Console.WriteLine("Not found on zhwp");
                    continue;
                }

                if (pzh.text.Contains("disambig"))
                {
                    Console.WriteLine("Disambig on zhwp");
                    continue;
                }

                wdlinks = pzh.GetWikidataLinks();
                if (wdlinks.Count > 0)
                {
                    bool svfound = false;
                    Console.WriteLine("zh-wdlinks:");
                    foreach (string iwl in wdlinks)
                    {
                        Console.WriteLine(iwl);
                        if (iwl == "sv")
                            svfound = true;
                    }

                    if (svfound)
                    {
                        pd.text += "\n* [["+p.title+"]] - [[:zh:"+pzh.title+"]]";
                        trysave(pd, 1);
                        continue;
                    }
                    //Console.ReadLine();
                }

                string origtext = p.text;
                
                //Do the actual replacement:

                string zhlink = "[[zh:" + pzh.title + "]]";
                if (p.text.Contains("[[ceb:"))
                    p.text = p.text.Replace("[[ceb:", zhlink + "\n[[ceb:");
                else
                    p.text += "\n" + zhlink;

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
                iremain--;
                Console.WriteLine(iremain.ToString() + " remaining.");
            }

            Console.WriteLine("Total # edits = " + nedit.ToString());
            nround++;
        }
        while (nedit > 0);

	}



}
