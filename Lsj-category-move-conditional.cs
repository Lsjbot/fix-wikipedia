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
        string makelang = "ceb";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);
        PageList pl = new PageList(site);


        //PageList pl1 = new PageList(site);

        //Select how to get pages. Uncomment as needed.

        //Find articles from a category
        //pl.FillAllFromCategoryTree("Nesomyidae");
        //pl1.FillAllFromCategoryTree("Siphonostomatoida");
        //foreach (Page p in pl1)
        //    pl.Add(p);
        //pl.FillFromCategory("Samtliga artiklar föreslagna för sammanslagningar och delningar");

        //Find subcategories of a category
        //pl.FillSubsFromCategory("Muridae (Rodentia)");

        Console.WriteLine("Move FROM category:");
        string fromcat = Console.ReadLine();
        Console.WriteLine("Move TO category:");
        string tocat = Console.ReadLine();
        Console.WriteLine("Move ONLY articles with title containing:");
        string required = Console.ReadLine();

        //Find subcategories and articles in a category
        pl.FillAllFromCategory(fromcat);

        //site.defaultEditComment = "Flyttar kategori " + fromcat + " till " + tocat;
        site.minorEditByDefault = false;
        site.defaultEditComment = "Moving category " + fromcat + " to " + tocat;

        //Find articles from all the links to a template, mostly useful on very small wikis
        //pl.FillFromLinksToPage("Hersilia (djur)");

        //Set specific article:
        //Page pp = new Page(site, "Citrontrogon");pl.Add(pp);

        //Skip all namespaces except articles:
        //pl.RemoveNamespaces(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,100,101});

        Dictionary<string, string> replacedict = new Dictionary<string, string>();

        replacedict.Add("[[Kategori:"+fromcat, "[[Kategori:"+tocat);
        replacedict.Add("[[Kategoriya:" + fromcat, "[[Kategoriya:" + tocat);
        replacedict.Add("[[Category:" + fromcat, "[[Kategoriya:" + tocat);
        //replacedict.Add("Sommarflicksländor", "Dammflicksländor");
        //replacedict.Add("sommarflicksländor", "dammflicksländor");
        
        
        List<string> linkword = new List<string>();
        //linkword.Add("Stillahavssluttningen");

        //Require ALL in requireword list:
        List<string> requireword = new List<string>();
        //requireword.Add("obotskapad");
        
        //Require AT LEAST ONE in requireone list:
        List<string> requireone = new List<string>();



        List<string> vetoword = new List<string>();
        //vetoword.Add("Kategori:Fungi");
        
        DateTime oldtime = DateTime.Now;
        oldtime = oldtime.AddSeconds(10);

        Console.WriteLine("Pages to change : " + pl.Count().ToString());

        int iremain = pl.Count();

		foreach(Page p in pl)
		{
            //Skip start of alphabet:
            //if (String.Compare(p.title,"Pseudanthessius") < 0 )
            //    continue;

            if (!p.title.Contains(required))
                continue;

            if (!tryload(p, 2))
                continue;
            if (!p.Exists())
                continue;

            string origtitle = p.title;

            
            //Check so all required strings actually present:

            bool allfound = true;
            foreach (string s in requireword)
                if (!p.text.Contains(s))
                    allfound = false;

            if (!allfound)
                continue;

            if (requireone.Count > 0)
            {
                bool onefound = false;
                foreach (string s in requireone)
                    if (p.title.Contains(s))
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

            string origtext = p.text;

            //Do the actual replacement:

            foreach (KeyValuePair<string,string> replacepair in replacedict)
            {
                p.text = p.text.Replace(replacepair.Key, replacepair.Value);
                
            }

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
                //Bot.editComment = "Byter kategori";
                //isMinorEdit = true;

                if (trysave(p, 4))
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
