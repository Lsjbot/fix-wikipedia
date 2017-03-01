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
                nedit++;
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


	public static void movecat(string fromcat,string tocat,Site site)
	{
        Console.WriteLine("Moving from " + fromcat + " to " + tocat);
        PageList pl = new PageList(site);

        //Find subcategories and articles in a category
        pl.FillAllFromCategory(fromcat);

        Dictionary<string, string> replacedict = new Dictionary<string, string>();

        replacedict.Add("[[Kategori:" + fromcat.Replace("Kategori:", ""), "[[Kategori:" + tocat.Replace("Kategori:", ""));
        //replacedict.Add("Sommarflicksländor", "Dammflicksländor");
        //replacedict.Add("sommarflicksländor", "dammflicksländor");

        DateTime oldtime = DateTime.Now;
        oldtime = oldtime.AddSeconds(10);

        Console.WriteLine("Pages to change : " + pl.Count().ToString());

        int iremain = pl.Count();

        foreach (Page p in pl)
        {
            //Skip start of alphabet:
            //if (String.Compare(p.title,"Pseudanthessius") < 0 )
            //    continue;

            if (!tryload(p, 2))
                continue;
            if (!p.Exists())
                continue;

            string origtitle = p.title;



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

            //Save the result:

            if (p.text != origtext)
            {
                //Bot.editComment = "Byter kategori";
                //isMinorEdit = true;

                if (trysave(p, 4))
                {
                    //nedit++;
                    DateTime newtime = DateTime.Now;
                    while (newtime < oldtime)
                        newtime = DateTime.Now;
                    oldtime = newtime.AddSeconds(5);
                }
            }
            iremain--;
            Console.WriteLine(iremain.ToString() + " remaining.");
        }


	}

    public static void Main()
	{
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string botkonto = "Lsjbot";
        string makelang = "sv";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);

        site.defaultEditComment = "Flyttar Litauens provinskategorier till län";
        site.minorEditByDefault = false;

        PageList pl = new PageList(site);

        //Select how to get pages. Uncomment as needed.

        //Find articles from a category
        //pl.FillAllFromCategoryTree("Nesomyidae");
        //pl1.FillAllFromCategoryTree("Siphonostomatoida");
        //foreach (Page p in pl1)
        //    pl.Add(p);
        //pl.FillFromCategory("Samtliga artiklar föreslagna för sammanslagningar och delningar");

        //Find subcategories of a category
        pl.FillAllFromCategoryTree("Litauens län");
        //Skip all namespaces except categories:
        pl.RemoveNamespaces(new int[] {0,1,2,3,4,5,6,7,8,9,10,11,12,13,15,100,101});


        Console.WriteLine(pl.Count().ToString() + " kategorier att kolla.");
        int ncat = pl.Count();
        foreach (Page p in pl)
        {
            if ( p.title.Contains("provins"))
            {
                Page pnew = new Page(site, p.title.Replace("provins", "län"));
                tryload(p, 1);
                tryload(pnew,1);
                if (!pnew.Exists())
                {
                    pnew.text = p.text;
                    trysave(pnew, 1);
                }
                movecat(p.title, pnew.title, site);
                
            }
            ncat--;
            Console.WriteLine("Categories remaining: " + ncat.ToString());
        }




        //Find articles from all the links to a template, mostly useful on very small wikis
        //pl.FillFromLinksToPage("Hersilia (djur)");

        //Set specific article:
        //Page pp = new Page(site, "Citrontrogon");pl.Add(pp);

        //Skip all namespaces except articles:
        //pl.RemoveNamespaces(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,100,101});

        Console.WriteLine("Total # edits = " + nedit.ToString());
        
        

	}



}
