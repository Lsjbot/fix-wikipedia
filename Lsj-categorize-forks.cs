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
using System.Web;

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


    public static DateTime pagecreation(Page p, Site iwsite)
    {
        //string magnitud = HttpUtility.UrlEncode(p.title);

        string xmlSrc;
        try
        {
            //xmlSrc = iwsite.PostDataAndGetResultHTM(iwsite.site + "/w/api.php", "action=query&format=xml&prop=revisions&titles=" + HttpUtility.UrlEncode(p.title) + "&rvlimit=1&rvprop=timestamp&rvdir=newer");
            xmlSrc = iwsite.PostDataAndGetResult(iwsite.address + "/w/api.php", "action=query&format=xml&prop=revisions&titles=" + HttpUtility.UrlEncode(p.title) + "&rvlimit=1&rvprop=timestamp&rvdir=newer");
        }
        catch (WebException e)
        {
            string message = e.Message;
            Console.Error.WriteLine(message);
            return DateTime.Now;
        }

        //Console.WriteLine(xmlSrc);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlSrc);
        DateTime timestamp = new DateTime();
        string ts = doc.GetElementsByTagName("rev")[0].Attributes.GetNamedItem("timestamp").Value;
        //Console.WriteLine("ts = "+ts);
        timestamp = DateTime.Parse(ts);
        timestamp = timestamp.ToUniversalTime();
        return timestamp;
    }

    public static string yearmonth(Page p, Site site)
    {
        DateTime pagedate = pagecreation(p, site);
        string monthstring = pagedate.Month.ToString();
        while (monthstring.Length < 2)
            monthstring = "0" + monthstring;
        return pagedate.Year.ToString() + "-" + monthstring;
    }

    private static List<string> getheaders(string inputString)
    {
        List<string> rl = new List<string>();
        Match m;
        Regex HeaderPattern = new Regex("== (.+) ==");

        try
        {
            m = HeaderPattern.Match(inputString);
            while (m.Success)
            {
                Console.WriteLine("Found header " + m.Groups[1] + " at "
                   + m.Groups[1].Index);
                rl.Add(m.Groups[1].Value);
                m = m.NextMatch();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return rl;
    }

    public static void Main()
	{
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string botkonto = "Lsjbot";
        string makelang = "sv";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);
        site.defaultEditComment = "Kategoriserar förgreningar";
        site.minorEditByDefault = true;

        List<string> donecat = new List<string>();

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
            pl.FillFromCategory("Robotskapade förgreningssidor");

            //Find subcategories of a category
            //pl.FillSubsFromCategory("Svampars vetenskapliga namn");

            //Find articles from all the links to an article, mostly useful on very small wikis
            //pl.FillFromLinksToPage("Brčko");

            //Find articles containing a specific string
            //pl.FillFromSearchResults("insource:\"http://www.itis.gov;http://\"", 4999);

            //Set specific article:
            //Page pp = new Page(site, "Citrontrogon");pl.Add(pp);

            //Skip all namespaces except articles:
            //pl.RemoveNamespaces(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,100,101});

            Dictionary<string, string> replacedict = new Dictionary<string, string>();

            //replacedict.Add("En underart finns: ''", "Utöver nominatformen finns också underarten ''");
            

            List<string> linkword = new List<string>();
            //linkword.Add("Catalogue of Life");

            //Require title to contain one in requiretitle list:
            List<string> requiretitle = new List<string>();
            //requiretitle.Add("Radioprogram nerlagda");

            //Require ALL in requireword list:
            List<string> requireword = new List<string>();
            requireword.Add("obotskapad");
            //requireword.Add("Burkina Faso");


            //Require AT LEAST ONE in requireone list:
            List<string> requireone = new List<string>();



            List<string> vetoword = new List<string>();
            vetoword.Add("[[Kategori:Robotskapade auktorsförkortningar]]");

            DateTime oldtime = DateTime.Now;
            oldtime = oldtime.AddSeconds(5);

            Console.WriteLine("Pages to change : " + pl.Count().ToString());

            int iremain = pl.Count();

            foreach (Page p in pl)
            {
                //Skip start of alphabet:
                //if (String.Compare(p.title,"Sicydium") < 0 )
                //    continue;

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

                //Do the actual replacement:

                string datecat = "Robotskapade förgreningar " + yearmonth(p, site);
                string catstring = "Kategori";
                if (makelang == "ceb")
                {
                    datecat = "Pagklaro paghimo ni bot " + yearmonth(p, site);
                    catstring = "Kategoriya:";
                }
                p.AddToCategory(datecat);

                if (!donecat.Contains(datecat))
                {

                    Page pcat = new Page(site, catstring + datecat);
                    tryload(pcat, 1);
                    if (!pcat.Exists())
                    {
                        pcat.text = "";
                        if ( makelang == "sv" )
                            pcat.AddToCategory("Robotskapade förgreningar efter datum");
                        else if ( makelang == "ceb" )
                            pcat.AddToCategory("Pagklaro paghimo ni bot");
                        trysave(pcat, 1);
                        donecat.Add(datecat);
                    }
                }

                foreach (string country in getheaders(p.text))
                {
                    string countrycat = "Robotskapade " + country + "förgreningar";
                    if (makelang == "ceb")
                        countrycat = "Pagklaro paghimo ni bot " + country;
                    p.AddToCategory(countrycat);
                    if (!donecat.Contains(countrycat))
                    {
                        Page pcat = new Page(site, catstring + countrycat);
                        tryload(pcat, 1);
                        if (!pcat.Exists())
                        {
                            pcat.text = "";
                            if ( makelang == "sv" )
                                pcat.AddToCategory("Robotskapade förgreningar efter land|"+country);
                            else if (makelang == "ceb")
                                pcat.AddToCategory("Pagklaro paghimo ni bot sa nasud");
                            trysave(pcat, 1);
                            donecat.Add(countrycat);
                        }
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
                iremain--;
                Console.WriteLine(iremain.ToString() + " remaining.");
            }

            Console.WriteLine("Total # edits = " + nedit.ToString());

        }
        while (false);// (nedit > 0);

	}



}
