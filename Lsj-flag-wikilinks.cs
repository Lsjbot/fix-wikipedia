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
    public static List<string> forktemplates = new List<string>();


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
        }

    }


	public static void addpages(Site site,PageList pl)
	{
        //Page pp1 = new Page(site, "Aega monophthalma"); pl.Add(pp1);

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


    public static string remove_disambig(string title)
    {
        string tit = title;
        if (tit.IndexOf("(") > 0)
            tit = tit.Remove(tit.IndexOf("(")).Trim();
        else if (tit.IndexOf(",") > 0)
            tit = tit.Remove(tit.IndexOf(",")).Trim();
        //if (tit != title)
        //    Console.WriteLine(title + " |" + tit + "|");
        return tit;
    }

    public static bool is_disambig(string title)
    {
        return (title != remove_disambig(title));
    }


    public static void Main()
	{
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string botkonto = "Lsjbot";
        string makelang = "sv";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);
        site.defaultEditComment = "Ersätter och wikilänkar";
        site.minorEditByDefault = true;

        do
        {
            nedit = 0;
            PageList pl = new PageList(site);
            PageList pl1 = new PageList(site);

            //Select how to get pages. Uncomment as needed.

            //Add pages "by hand":
            //addpages(site,pl);
            //Find articles from a category
            //pl.FillAllFromCategoryTree("Phasmatodea");
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
            pl.FillAllFromCategory("Robotskapade artiklar 2016-05");

            //Find subcategories of a category
            //pl.FillSubsFromCategory("Svampars vetenskapliga namn");

            //Find articles from all the links to an article, mostly useful on very small wikis
            //pl.FillFromLinksToPage("Maltese Islands");

            //Find articles containing a specific string
            //pl.FillFromSearchResults("insource:\"http://www.itis.gov;http://\"", 4999);

            //Set specific article:
            //Page pp = new Page(site, "Citrontrogon");pl.Add(pp);
            
            //Skip all namespaces except articles:
            //pl.RemoveNamespaces(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,100,101});

            Dictionary<string, string> replacedict = new Dictionary<string, string>();


            List<string> linkword = new List<string>();
            //linkword.Add("Catalogue of Life");

            //Require title to contain one in requiretitle list:
            List<string> requiretitle = new List<string>();
            //requiretitle.Add("Radioprogram nerlagda");

            //Require ALL in requireword list:
            List<string> requireword = new List<string>();
            requireword.Add("botskapad");
            //requireword.Add("Burkina Faso");


            //Require AT LEAST ONE in requireone list:
            List<string> requireone = new List<string>();



            List<string> vetoword = new List<string>();
            //vetoword.Add("vitrea");

            DateTime oldtime = DateTime.Now;
            oldtime = oldtime.AddSeconds(5);

            Console.WriteLine("Pages to change : " + pl.Count().ToString());

            int iremain = pl.Count();

            bool resume = false;

            foreach (Page p in pl)
            {
                iremain--;
                //Skip start of alphabet:
                //if (String.Compare(p.title,"Sicydium") < 0 )
                //    continue;
                if (!resume)
                {
                    if (p.title == "Valhermoso")
                        resume = true;
                    else
                        continue;
                }

                if (is_disambig(p.title))
                    continue;
                
                if (!tryload(p, 2))
                    continue;
                if (!p.Exists())
                    continue;

                string origtitle = p.title;

                //Follow redirect:

                //if (p.IsRedirect())
                //{
                //    p.title = p.RedirectsTo();
                //    if (!tryload(p, 2))
                //        continue;
                //    if (!p.Exists())
                //        continue;
                //}

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

                //Find articles from all the links to an article, mostly useful on very small wikis
                PageList pllink = new PageList(site);
                try
                {
                    pllink.FillFromLinksToPage(p.title);
                }
                catch (WebException e)
                {
                }

                int nlink = 0;
                foreach (Page plink in pllink)
                {
                    if (plink.title.Contains("Lsjbot"))
                        continue;
                    if (!pl.Contains(plink.title))
                    {
                        if ( tryload(plink, 1))
                            if (!plink.IsRedirect() && !plink.text.Contains("obotskapad"))
                            {
                                Console.WriteLine("plink.title = " + plink.title);
                                nlink++;
                            }
                    }
                }

                Console.WriteLine(p.title + " :" + pllink.Count().ToString()+", "+nlink.ToString());

                if (nlink > 0)
                    p.AddToCategory("Kontrollbehov inkommande wikilänkar");
                else
                    p.RemoveFromCategory("Kontrollbehov inkommande wikilänkar");


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
                        if (nedit < 4)
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

        }
        while (false);// (nedit > 0);

	}



}
