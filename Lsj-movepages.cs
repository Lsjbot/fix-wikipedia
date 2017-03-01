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

    public static void movepage(Site site, string frompage, string topage)
    {
        string magnitud = site.GetWebPage(site.address + "/wiki/Special:MovePage/" + HttpUtility.UrlEncode(frompage));
        magnitud = magnitud.Substring(magnitud.IndexOf("\"editToken\":\"") + 13);
        magnitud = magnitud.Substring(0, magnitud.IndexOf("\"") - 1);
        string tmpStr = site.PostDataAndGetResult(site.address + "/w/api.php", "action=move&from=" + HttpUtility.UrlEncode(frompage) + "&to=" + HttpUtility.UrlEncode(topage) + "&token=" + HttpUtility.UrlEncode(magnitud) + "&reason=Flyttar artikel&redirect=true");
        Console.WriteLine(tmpStr);
        make_redirect(site, frompage, topage, "");

    }

    public static void make_redirect(Site site, string frompage, string topage, string cat)
    {
        Page pred = new Page(site, frompage);
        if (tryload(pred, 1))
        {
            if (!pred.Exists())
            {
                pred.text = "#REDIRECT [[" + topage + "]]";
                if (!String.IsNullOrEmpty(cat))
                    pred.AddToCategory(cat);
                trysave(pred, 2);
            }

        }

    }

    public static void Main()
	{
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string botkonto = "Lsjbot";
        string makelang = "sv";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);
        PageList pl = new PageList(site);
        PageList pl1 = new PageList(site);

        site.defaultEditComment = "Flyttar sidor";
        site.minorEditByDefault = true;

        //Select how to get pages. Uncomment as needed.

        //Find articles from a category
        //pl.FillAllFromCategory("Robotskapade artiklar 2014-10");
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

        //Set specific article:
        //Page pp = new Page(site, "Användare:Lsjbot/Flytt-test");pl.Add(pp);

        List<string> sl = new List<string>();
        //sl.Add("Lista över insjöar i Arjeplogs kommun (1-1000)");
        sl.Add("Ðakovića Kosa");
        sl.Add("Ðanuša");
        sl.Add("Ðaprovica");
        sl.Add("Ðatla");
        sl.Add("Ðatlo");
        sl.Add("Ðatlo (bergstopp)");
        sl.Add("Ðatlo (grotta i Bosnien och Hercegovina, Republika Srpska, lat 43,06, long 18,49)");
        sl.Add("Ðavat");
        sl.Add("Ðavato");
        sl.Add("Ðavolica");
        sl.Add("Ðavor-Konak");
        sl.Add("Ðed");
        sl.Add("Ðed (berg i Bosnien och Hercegovina, Republika Srpska, lat 43,11, long 18,41)");
        sl.Add("Ðed (kulle)");
        sl.Add("Ðed (ås)");
        sl.Add("Ðedov Do");
        sl.Add("Ðedov Do (dal i Bosnien och Hercegovina)");
        sl.Add("Ðedovac");
        sl.Add("Ðedovac (kulle)");
        sl.Add("Ðedovac (källa)");
        sl.Add("Ðedovića Voda");
        sl.Add("Ðedovo Brdo");
        sl.Add("Ðekića Brdo");
        sl.Add("Ðekića Vis");
        sl.Add("Ðenića Brdo");
        sl.Add("Ðera");
        sl.Add("Ðerina Voda");
        sl.Add("Ðerinac");
        sl.Add("Ðerinac (bergstopp)");
        sl.Add("Ðermašica");
        sl.Add("Ðernovača");
        sl.Add("Ðeropa");
        sl.Add("Ðerzelovica");
        sl.Add("Ðeva");
        sl.Add("Ðevice");
        sl.Add("Ðevigrad");
        sl.Add("Ðidovi");
        sl.Add("Ðilas");
        sl.Add("Ðipovac");
        sl.Add("Ðipuša");
        sl.Add("Ðogat");
        sl.Add("Ðon");
        sl.Add("Ðorđe Stratimirović");
        sl.Add("Ðorđo Lavrnić");
        sl.Add("Ðubino Brdo");
        sl.Add("Ðukanov Vis");
        sl.Add("Ðukanov Vrh");
        sl.Add("Ðukina Voda");
        sl.Add("Ðukino Brdo");
        sl.Add("Ðukića Brdo");
        sl.Add("Ðukića Brdo (bergstopp)");
        sl.Add("Ðukića Glavica");
        sl.Add("Ðulanova Rijeka");
        sl.Add("Ðulanovo Brdo");
        sl.Add("Ðuletske Kose");
        sl.Add("Ðulina Kosa");
        sl.Add("Ðulina Rupa");
        sl.Add("Ðulinac");
        sl.Add("Ðupska Čuka");
        sl.Add("Ðupska Čuka (kulle i Makedonien)");
        sl.Add("Ðuranovac");
        sl.Add("Ðuranovina");
        sl.Add("Ðuranđa");
        sl.Add("Ðuraš");
        sl.Add("Ðuraš (kulle)");
        sl.Add("Ðurendića Vis");
        sl.Add("Ðurevac");
        sl.Add("Ðurevac (vattendrag i Bosnien och Hercegovina)");
        sl.Add("Ðurica");
        sl.Add("Ðurin Sjek");
        sl.Add("Ðurina Voda");
        sl.Add("Ðurinovača");
        sl.Add("Ðurinovača (bergstopp)");
        sl.Add("Ðurinovača (utlöpare)");
        sl.Add("Ðurića Brdo");
        sl.Add("Ðurića Brdo (bergstopp)");
        sl.Add("Ðurića Brdo (kulle)");
        sl.Add("Ðurića Kuk");
        sl.Add("Ðurića Vis");
        sl.Add("Ðurića Vrelo");
        sl.Add("Ðuričin Do");
        sl.Add("Ðurkelina Jama");
        sl.Add("Ðurkovac");
        sl.Add("Ðurkovac (berg i Bosnien och Hercegovina)");
        sl.Add("Ðuroje");
        sl.Add("Ðurov");
        sl.Add("Ðurov Ras");
        sl.Add("Ðurđev Do");
        sl.Add("Ðurđeva Glava");
        sl.Add("Ðurđeva Glavica");
        sl.Add("Ðurđeva Glavica (berg i Bosnien och Hercegovina)");
        sl.Add("Ðurđevac");
        sl.Add("Ðurđevac (utlöpare)");
        sl.Add("Ðurđevica");
        sl.Add("Ðurđevica (berg)");
        sl.Add("Ðurđeviča Vis");
        sl.Add("Ðurđevo Brdo");
        sl.Add("Ðurđevo Brdo (berg i Bosnien och Hercegovina, Republika Srpska, lat 43,58, long 19,20)");
        sl.Add("Ðurđevo Brdo (berg i Bosnien och Hercegovina, Republika Srpska, lat 44,04, long 19,58)");
        sl.Add("Ðurđevo Brdo (kulle i Bosnien och Hercegovina, Republika Srpska, lat 42,76, long 18,27)");
        sl.Add("Ðurđevo Brdo (kulle i Bosnien och Hercegovina, Republika Srpska, lat 43,49, long 18,82)");
        sl.Add("Ðurđovac");
        sl.Add("Ðusin Vrh");
        sl.Add("Ðuvića Vrh");
        sl.Add("Ðvogrla Jama");

        foreach (string s in sl)
        {
            Page pp = new Page(site, s); pl.Add(pp);
        }


        //Skip all namespaces except articles:
        //pl.RemoveNamespaces(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,100,101});

        Dictionary<string, string> replacedict = new Dictionary<string, string>();

        //replacedict.Add("-", "–");
        replacedict.Add("Ð", "Đ"); //från isländskt Đ till bosniskt Đ

        //Require ALL in requireword list:
        List<string> requireword = new List<string>();
        //requireword.Add("obotskapad");
        
        //Require AT LEAST ONE in requireone list:
        List<string> requireone = new List<string>();


        
        //Vetowords should NOT be in article
        List<string> vetoword = new List<string>();
        vetoword.Add("Island");
        vetoword.Add("isländska");
        vetoword.Add("OMDIRIGERING");
        vetoword.Add("Đ");


        
        DateTime oldtime = DateTime.Now;
        oldtime = oldtime.AddSeconds(10);

        Console.WriteLine("Pages to change : " + pl.Count().ToString());

        int iremain = pl.Count();

        bool first = true;

		foreach(Page p in pl)
		{
            //Skip start of alphabet:
            string skipuntil = "";

            if ((skipuntil != "" ) && String.Compare(p.title,skipuntil) < 0 )
                continue;

            if (!tryload(p, 2))
                continue;
            if (!p.Exists())
                continue;

            string origtitle = p.title;

            ////Follow redirect:

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
                if (!p.text.Contains(s))
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

            //string origtext = p.text;

            //Do the actual replacement:

            string frompage = p.title;
            string topage = p.title;

            foreach (KeyValuePair<string,string> replacepair in replacedict)
            {
                topage = topage.Replace(replacepair.Key, replacepair.Value);
            }

            //Do the actual move:
            movepage(site, frompage, topage);

            //Move discussion too:
            bool movedisk = true;
            if (movedisk)
            {
                Page pd = new Page(site, "Diskussion:" + frompage);
                tryload(pd, 1);
                if ( pd.Exists())
                    movepage(site, "Diskussion:" + frompage, "Diskussion:" + topage);
            }

            //Wait:
            if (first)
            {
                Console.WriteLine("<ret");
                Console.ReadLine();
                first = false;
            }
            DateTime newtime = DateTime.Now;
            while (newtime < oldtime)
                newtime = DateTime.Now;
            oldtime = newtime.AddSeconds(10);

            iremain--;
            Console.WriteLine(iremain.ToString() + " remaining.");
        }

        Console.WriteLine("Total # edits = " + nedit.ToString());
        
        

	}



}
