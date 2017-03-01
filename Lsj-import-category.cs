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
        string fromlang = "en";
        Site fsite = new Site("https://"+fromlang+".wikipedia.org", botkonto, password);
        string tolang = "ceb";
        Site tsite = new Site("https://" + tolang + ".wikipedia.org", botkonto, password);
        PageList pl = new PageList(fsite);

        tsite.defaultEditComment = "Importing template category";
        tsite.minorEditByDefault = false;

        

        //Select how to get pages. Uncomment as needed.

        //Find articles from a category
        pl.FillAllFromCategoryTree("Geobox2");
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
        //pl.FillFromCategory("Samtliga artiklar föreslagna för sammanslagningar och delningar");

        //Find subcategories of a category
        //pl.FillSubsFromCategory("Svampars vetenskapliga namn");
        
        //Find articles from all the links to an article, mostly useful on very small wikis
        //pl.FillFromLinksToPage("Boidae");

        //Find articles containing a specific string
        //pl.FillFromSearchResults("cdata",9999);

        //Set specific article:
        //Page pp = new Page(site, "Citrontrogon");pl.Add(pp);

        //Skip all namespaces except articles:
        //pl.RemoveNamespaces(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,100,101});

        Dictionary<string, string> replacedict = new Dictionary<string, string>();

        if ( tolang == "ceb" )
            replacedict.Add("Template:", "Plantilya:");
        if (tolang == "war")
            replacedict.Add("Template:", "Batakan:");



        List<string> linkword = new List<string>();
        //linkword.Add("Catalogue of Life");

        //Require title to contain one in requiretitle list:
        List<string> requiretitle = new List<string>();
        //requiretitle.Add("Radioprogram nerlagda");

        //Require ALL in requireword list:
        List<string> requireword = new List<string>();
        requireword.Add("Template:");
        
        //Require AT LEAST ONE in requireone list:
        List<string> requireone = new List<string>();


        

        List<string> vetoword = new List<string>();
        //vetoword.Add("nedlagda");
        
        DateTime oldtime = DateTime.Now;
        oldtime = oldtime.AddSeconds(10);

        Console.WriteLine("Pages to change : " + pl.Count().ToString());

        int iremain = pl.Count();

		foreach(Page p in pl)
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

            if (p.IsRedirect())
            {
                p.title = p.RedirectsTo();
                if (!tryload(p, 2))
                    continue;
                if (!p.Exists())
                    continue;
            }

            //Check so all required strings actually present:

            bool allfound = true;
            foreach (string s in requireword)
                if (!p.title.Contains(s))
                    allfound = false;

            if (!allfound)
                continue;

            if (requireone.Count > 0)
            {
                bool onefound = false;
                foreach (string s in requireone)
                    if (p.text.Contains(s))
                        onefound = true;

                if (!onefound)
                    continue;
            }

            //Check so no vetoword are present:

            bool vetofound = false;
            foreach (string s in vetoword)
                if (p.text.Contains(s))
                    vetofound = true;

            if (vetofound)
                continue;

            //If redirect, go back to redirect page:

            //if (origtitle != p.title)
            //{
            //    p.title = origtitle;
            //    p.Load();
            //}

            //Do the actual replacement:

            string newtitle = origtitle;

            foreach (KeyValuePair<string,string> replacepair in replacedict)
            {
                newtitle = newtitle.Replace(replacepair.Key, replacepair.Value);
                
            }

            Page pt = new Page(tsite, newtitle);

            tryload(pt, 2);
            if (pt.Exists())
                continue;

            pt.text = p.text;

            //Save the result:

            //Bot.editComment = "Importing category";
            //isMinorEdit = false;

            if (trysave(pt, 4))
            {
                nedit++;
                DateTime newtime = DateTime.Now;
                while (newtime < oldtime)
                    newtime = DateTime.Now;
                oldtime = newtime.AddSeconds(5);
            }

            p.title += "/doc";

            tryload(p, 2);

            if (p.Exists())
            {
                string doctitle = p.title;
                foreach (KeyValuePair<string, string> replacepair in replacedict)
                {
                    doctitle = doctitle.Replace(replacepair.Key, replacepair.Value);

                }

                Page pd = new Page(tsite, newtitle);
                tryload(pd, 2);
                if (pd.Exists())
                    continue;

                pd.text = p.text;

                //Save the result:

                //Bot.editComment = "Importing category";
                //isMinorEdit = false;

                if (trysave(pd, 4))
                {
                    nedit++;
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



}
