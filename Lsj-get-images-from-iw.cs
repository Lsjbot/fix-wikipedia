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
    public static DateTime oldtime = DateTime.Now;

    public static void FillAllFromCategoryTreeExceptDone(string categoryName, Site site, PageList pl, List<string> doneCats)
    {
        pl.Clear();
        categoryName = site.CorrectNsPrefix(categoryName);
        //List<string> doneCats = new List<string>();
        Console.WriteLine("doneCats " + doneCats.Count.ToString());
        pl.FillAllFromCategory(categoryName);
        doneCats.Add(categoryName);
        for (int i = 0; i < pl.Count(); i++)
            if (pl.pages[i].GetNamespace() == 14 && !doneCats.Contains(pl.pages[i].title))
            {
                //Console.WriteLine(pl.pages[i].title);
                pl.FillAllFromCategory(pl.pages[i].title);
                doneCats.Add(pl.pages[i].title);
            }
        pl.RemoveRecurring();
    }


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
                DateTime newtime = DateTime.Now;
                while (newtime < oldtime)
                {
                    Thread.Sleep(500);//milliseconds
                    newtime = DateTime.Now;
                }
                oldtime = newtime.AddSeconds(6);

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

    public static int tryconvert(string word)
    {
        int i = -1;

        try
        {
            i = Convert.ToInt32(word);
        }
        catch (OverflowException)
        {
            Console.WriteLine("i Outside the range of the Int32 type: " + word);
        }
        catch (FormatException)
        {
            if (!String.IsNullOrEmpty(word))
                Console.WriteLine("i Not in a recognizable format: " + word);
        }

        return i;

    }


    //public static List<string> Interwiki(Site site, string title)

    ////Borrowed from http://sv.wikipedia.org/wiki/Wikipedia:Projekt_DotNetWikiBot_Framework/Innocent_bot/Addbotkopia
    //{
    //    List<string> r = new List<string>();
    //    XmlDocument doc = new XmlDocument();

    //    string url = "action=wbgetentities&sites=svwiki&titles=" + HttpUtility.UrlEncode(title) + "&languages=sv&format=xml";
    //    //string tmpStr = site.PostDataAndGetResultHTM(site.site+"/w/api.php", url);
    //    try
    //    {
    //        string tmpStr = site.PostDataAndGetResultHTM(site.site + "/w/api.php", url);
    //        doc.LoadXml(tmpStr);
    //        for (int i = 0; i < doc.GetElementsByTagName("sitelink").Count; i++)
    //        {
    //            string s = doc.GetElementsByTagName("sitelink")[i].Attributes.GetNamedItem("site").Value;
    //            string t = doc.GetElementsByTagName("sitelink")[i].Attributes.GetNamedItem("title").Value;
    //            s = s.Replace("_", "-");
    //            string t2 = s.Substring(0, s.Length - 4) + ":" + t;
    //            //Console.WriteLine(t2);
    //            r.Add(t2);
    //        }
    //    }
    //    catch (WebException e)
    //    {
    //        string message = e.Message;
    //        Console.Error.WriteLine(message);
    //    }

    //    return r;
    //}

    public static List<string> GetImageParams(Page p, string image)
    {
        List<string> paramlist = new List<string>();

        int start = p.text.IndexOf(image);
        if ( start <= 0 )
            return paramlist;

        start += image.Length;

        int end = p.text.IndexOf("]]", start);
        if (end < start)
            return paramlist;

        string paramstring = p.text.Substring(start,end-start);

        foreach (string pp in paramstring.Split('|'))
            if (!String.IsNullOrEmpty(pp))
                paramlist.Add(pp);

        return paramlist;
    }

    public static List<string> GetImagesInTemplates(Site site, Page p)
    {
        List<string> imagelist = new List<string>();
        List<string> paramlist = new List<string>();
        paramlist.Add("image");
        paramlist.Add("imagen");
        paramlist.Add("immagine");
        paramlist.Add("afbeelding");
        paramlist.Add("bild");
        paramlist.Add("bildname");

        foreach (string template in p.GetTemplates(true, false))
        {
            Dictionary<string, string> parameters = site.ParseTemplate(template);
            foreach (string param in parameters.Keys)
                if (paramlist.Contains(param.ToLower()))
                    if ( !String.IsNullOrEmpty(parameters[param].Trim()))
                        imagelist.Add(parameters[param].Trim());
        }
        return imagelist;
    }
	    
	public static void Main()
	{
        string makelang = "sv";
        string botaccount = "Lsjbot";
        Console.Write("Password: ");
        string password = Console.ReadLine();
        Site svsite = new Site("https://"+makelang+".wikipedia.org", botaccount, password);
        Site cmsite = new Site("https://commons.wikimedia.org", botaccount, password);
        //Site wdsite = new Site("http://wikidata.org", botaccount, password);

        //while (true)
        //{
        //    string fn = Console.ReadLine();
        //    Page ppp = new Page(cmsite, fn);
        //    tryload(ppp, 1);
        //    Console.WriteLine(ppp.text);
        //}

        //string cattodo = "Persoon naar beroep";
        string cattodo = "Robotskapade svampartiklar";
        string editcomment = "Fixar bilder från iw";
        string logpage = "Användare:Lsjbot/imagelog";
        string resume_at = "";
        //string resume_at = "";

        List<string> doneCats = new List<string>();
        //doneCats.Add("Svedesi");
        //doneCats.Add("Tedeschi");
        //doneCats.Add("Spagnoli");
        
        switch (makelang)
        {
            case "sv":
                editcomment = "Fixar bilder från iw, Kategori:"+cattodo;
                break;
            case "ceb":
                editcomment = "Galeriya sa hulagway";
                break;
            case "nl":
                editcomment = "Fotogalerij van interwiki, Categorie:"+cattodo;
                break;
            case "it":
                editcomment = "Galleria di immagini da interwiki, Categoria:" + cattodo;
                break;
            default:
                editcomment = "Image gallery from interwiki";
                break;
        }

        svsite.defaultEditComment = editcomment;
        svsite.minorEditByDefault = false;
        Console.WriteLine("apipath = "+svsite.apiPath);

        


        //Skip images in blacklist:
        List<string> blacklist = new List<string>();
        List<string> vetocatlist = new List<string>();

        bool blackwrite = false;
        bool blackread = true;

        if (blackread)
        {
            int nblack = 0;
            using (StreamReader sr = new StreamReader("blacklist.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    blacklist.Add(s);
                    nblack++;
                }
            }
            Console.WriteLine("nblack=" + nblack.ToString());

        }
        else
        {
            vetocatlist.Add("Image placeholders");
            vetocatlist.Add("Icons by subject");
            vetocatlist.Add("Logos of Eurovision");
            vetocatlist.Add("Flags by country");
            vetocatlist.Add("Audio files");
            //vetocatlist.Add("");

            foreach (string vc in vetocatlist)
            {
                PageList pldummy = new PageList(cmsite);
                bool loaded = false;
                do
                {
                    try
                    {
                        pldummy.FillFromCategoryTree(vc);
                        loaded = true;
                    }
                    catch (WebException e)
                    {
                        string message = e.Message;
                        Console.Error.WriteLine(message);
                    }
                }
                while (!loaded);
                foreach (Page pd in pldummy)
                {
                    //Console.WriteLine(pd.title);
                    blacklist.Add(pd.title.Replace("File:", "").Replace(" ", "_"));
                }
                pldummy.Clear();
            }
            //Console.ReadLine();

            if (blackwrite)
            {
                using (StreamWriter sw = new StreamWriter("blacklist.txt"))
                {

                    foreach (string s in blacklist)
                    {
                        sw.WriteLine(s);
                    }
                }
            }
        }


        //Skip pages in watchlist:
        svsite.watchList = new PageList(svsite);
        svsite.watchList.FillFromWatchList();
        Console.WriteLine("Watchlist pages: " + svsite.watchList.Count());
        
        List<string> blacktype = new List<string>();
        //blacktype.Add(".svg");
        //blacktype.Add(".png");
    
        PageList pl = new PageList(svsite);
            
        ////////////////////////////////////
        //Select how to get pages. Uncomment as needed.
        ////////////////////////////////////
        
        //Find articles from a category
        bool loaded2 = false;
        do
        {
            try
            {
                FillAllFromCategoryTreeExceptDone(cattodo,svsite,pl,doneCats);
                loaded2 = true;
            }
            catch (WebException e)
            {
                string message = e.Message;
                Console.Error.WriteLine(message);
            }
        }
        while (!loaded2);

        //Find articles from all the links to a template, mostly useful on very small wikis
        //        pl.FillFromLinksToPage("Mall:Taxobox");

        //Set specific article:
        //Page ppp = new Page(svsite, "Dina Tersago");pl.Add(ppp);

        //Skip all namespaces except regular articles:
        pl.RemoveNamespaces(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,100,101});

        ///////////////////////////////////////
        //Choose what to do with the pix that are found:
        // nchoice = 0: do nothing, except list on standard output
        // nchoice = 1: add as gallery in target article 
        // nchoice = 2: add as separate pix in target article
        // nchoice = 3: add in article discussion
        // nchoice = 4: list in separate workpage "Användare:Botaccount/Gallery"
        ////////////////////////////////////////
        int nchoice = 1;
        
        // If ntop is non-zero, don't use all pix but only the ntop most used ones.
        int ntop = 20;

        // Skip pictures with size smaller than minsize.
        int minsize = 50;

        //Skip articles that already have at least one pic:
        bool skipillustrated = true;

        Page pwork = new Page(svsite, "Användare:" + botaccount + "/Gallery");
        if (nchoice == 4)
        {
            pwork.Load();
        }

        Dictionary<string, Site> sitedict = new Dictionary<string,Site>();

        string sbrack = "[]'† ?";
        char[] brackets = sbrack.ToCharArray();
        
        //int nfound = 0;

        DateTime oldtime = DateTime.Now;

        int nedit = 0;

        int iremain = pl.Count();

        //Console.ReadLine();

		foreach(Page p in pl)
		{

            iremain--;
            Console.WriteLine(iremain.ToString() + " remaining.");

            //DateTime nexttime = oldtime.AddSeconds(7);
            //Skip start of alphabet:
            //if (String.Compare(p.title,"Acacia tortilis") < 0 )
            //    continue;

            //skip until specific article
            if (resume_at != "")
            {
                if (resume_at == p.title)
                    resume_at = "";
                else
                    continue;
            }

            //Skip pages in watchlist
            if (p.watched)
            {
                Console.WriteLine("Skip watched");
                continue;
            }

            if (svsite.watchList.Contains(p))
            {
                Console.WriteLine("Skip page in watchlist");
                continue;
            }




            if ( !tryload(p,1))
                continue;
            if (!p.Exists())
                continue;

            string origtext = p.text;

            //find images already in page:

            List<string> oldpix = p.GetImages();
            List<string> oldpix2 = GetImagesInTemplates(svsite,p);
            //Console.WriteLine("Oldpix:");
            int npix = 0;
            foreach (string oldpic in oldpix)
            {
                //Console.WriteLine(oldpic);
                npix++;
            }
            foreach (string pic in oldpix2)
                npix++;
            Console.WriteLine("npix = " + npix.ToString());
            if (skipillustrated)
            {
                if (npix > 0)
                    continue;
                if (p.text.Contains(".jp"))
                    continue;
                if (p.text.Contains(".gif"))
                    continue;
                if (p.text.Contains(".JP"))
                    continue;
                if (p.text.Contains(".GIF"))
                    continue;
                if (p.text.Contains(".png"))
                    continue;
                if (p.text.Contains(".PNG"))
                    continue;
            }

            //if it already has a gallery, skip it:
            if (p.text.Contains("<gallery>"))
                continue;

            //if it doesn't contain "Lsjbot", skip it:
            //if (!p.text.Contains("Lsjbot"))
            //    continue;

            //find iw:

            Dictionary<string, string> newpix = new Dictionary<string, string>();

            //string[] iw = p.GetInterWikiLinks();

            List<string> iwlist = new List<string>();
            try
            {
                iwlist = p.GetInterLanguageLinks();
            }
            catch (WebException e)
            {
                string message = e.Message;
                Console.Error.WriteLine(message);
                Thread.Sleep(10000);//milliseconds
            }

            Console.WriteLine("iwlist.Count " +iwlist.Count);

            //if (iw.Length == 0)
            //    iwlist = Interwiki(wdsite, p.title);
            //else
            //{
            //    foreach (string iws in iw)
            //        iwlist.Add(iws);
            //}

                        
            foreach (string iws in iwlist)
            {
                string[] ss = iws.Split(':');
                string iwcode = ss[0];
                string iwtitle = ss[1];
                Console.WriteLine("iw - " + iwcode + ":" + iwtitle);

                if (iwcode == "nah")
                    continue;

                if (!sitedict.ContainsKey(iwcode))
                {
                    string iwurl = "https://" + iwcode + ".wikipedia.org";
                    try
                    {
                        try
                        {
                            sitedict.Add(iwcode, new Site(iwurl, botaccount, password));
                        }
                        catch (WebException e)
                        {
                            Console.WriteLine(e.Message);
                            continue;
                        }
                    }
                    catch (WikiBotException e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }
                }

                Page piw = new Page(sitedict[iwcode], iwtitle);
                try
                {
                    piw.Load();
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                if (!piw.Exists())
                {
                    Console.WriteLine("Not found despite iw");
                    continue;
                }


                List<string> iwpix = piw.GetImages();
                List<string> iwpix2 = GetImagesInTemplates(sitedict[iwcode], piw);
                foreach (string pic in iwpix2)
                    iwpix.Add(pic);

                foreach (string iwpicture in iwpix)
                {
                    string iwpic = iwpicture;
                    //Remove file prefix:
                    if (iwpic.Contains(":"))
                        iwpic = iwpic.Split(':')[1];

                    //Skip if smaller than minsize:
                    int size = 999;
                    foreach (string pp in GetImageParams(piw, iwpic))
                    {
                        if (pp.Contains("px"))
                        {
                            size = tryconvert(pp.Replace("px", ""));
                            break;
                        }
                    }
                    if ((size > 0) && (size < minsize))
                        continue;

                    //Replace space with underscore:
                    iwpic = iwpic.Replace(" ", "_");

                    //Add to list:
                    if (newpix.ContainsKey(iwpic))
                        newpix[iwpic] = newpix[iwpic] + ":" + iwcode;
                    else
                        newpix.Add(iwpic, iwcode);
                }
                
            }

            bool fromcommons = false;
            
            if (newpix.Count == 0)
            {
                if ( p.text.Contains("ommonscat|"))
                {
                    fromcommons = true;
                    string s = "";
                    if (p.text.IndexOf("{{commonscat|") > 0)
                    {
                        s = p.text.Remove(0, p.text.IndexOf("{{commonscat|"));
                        s = s.Remove(s.IndexOf("}}"));
                        s = s.Remove(0, "{{commonscat|".Length);
                    }
                    else if (p.text.IndexOf("{{Commonscat|") > 0)
                    {
                        s = p.text.Remove(0, p.text.IndexOf("{{Commonscat|"));
                        s = s.Remove(s.IndexOf("}}"));
                        s = s.Remove(0, "{{Commonscat|".Length);
                    }

                    if (String.IsNullOrEmpty(s))
                        continue;

                    if (s.Contains("|"))
                        s = s.Remove(s.IndexOf("|"));


                    s = "Category:" + s;
                    //Console.WriteLine(s);
                    //Console.ReadLine();

                    PageList plc = new PageList(cmsite);
                    try
                    {
                        plc.FillFromCategory(s);
                    }
                    catch (WebException e)
                    {
                        Console.WriteLine(e.Message);
                        Thread.Sleep(10000);//milliseconds

                        //continue;
                    }


                    foreach (Page pc in plc)
                    {
                        Console.WriteLine("pc = " + pc.title);
                        newpix.Add(pc.title.Replace(" ", "_"), "cm");
                    }
                }
            }

            //Check if pix from iw is already used in target article:
            //
            
            //Workaround because a Dictionary can't be modified while iterating over its keys:
            List<string> dummykeys = new List<string>();
            foreach (string dk in newpix.Keys)
                dummykeys.Add(dk);

            foreach (string newpic in dummykeys)
            {

                //Check if pix from iw is already used in target article:
                if (p.text.Contains(newpic))
                    newpix[newpic] = "/// ALREADY USED";
                else if (newpic.Contains(":"))
                {
                    if ( p.text.Contains(newpic.Remove(0,newpic.IndexOf(':')+1)))
                        newpix[newpic] = "/// ALREADY USED";

                }

                if ((!newpic.Contains(".")) || (newpic.LastIndexOf('.') < newpic.Length-5))
                {
                    newpix[newpic] = "/// NOT A FILE";

                }

                //Check if pic in blacklist:
                if (blacklist.Contains(newpic))
                    newpix[newpic] = "/// BLACKLISTED IMAGE";

                foreach (string filetype in blacktype)
                {
                    if (newpic.Contains(filetype))
                        newpix[newpic] = "/// BLACKLISTED FILETYPE";
                }

                if (newpix[newpic].Contains("///"))
                    continue;
            
                //Check if pic really exists on Commons:

                if (!fromcommons)
                {

                    string res = cmsite.indexPath + "?title=" +
                                        HttpUtility.UrlEncode("File:" + newpic);
                    //Console.WriteLine("commonsres = " + res);
                    string src = "";
                    try
                    {
                        src = cmsite.GetWebPage(res); // cmsite.GetPageHTM(res);
                    }
                    catch (WebException e)
                    {
                        newpix[newpic] = "/// NOT FOUND ON COMMONS";
                        string message = e.Message;
                        if (message.Contains(": (404) "))
                        {		// Not Found
                            Console.Error.WriteLine(Bot.Msg("Page \"{0}\" doesn't exist."), newpic);
                            Console.WriteLine("Image not found " + newpic);
                            continue;
                        }
                        else
                        {
                            Console.Error.WriteLine(message);
                            continue;
                        }
                    }
                }
            }

            int nnew = 0;
            foreach (string newpic in newpix.Keys)
            {
                Console.WriteLine(newpic + "   ! " + newpix[newpic]);
                if (!newpix[newpic].Contains("///"))
                    nnew++;
            }

            Console.WriteLine("# new pix = " + nnew.ToString());



            if (nnew == 0)
                continue;

            //OK, so we found some pix. Now what do we do with them?

            //First get rid of the ones we don't want:
            foreach (string newpic in dummykeys)
                if (newpix[newpic].Contains("///"))
                    newpix[newpic] = "";

            //Then figure out which new pix have the most interwiki use:
            List<string> pixtouse = new List<string>();
            if ((ntop > 0) && (ntop < nnew))
            {

                
                int nused = 0;
                while (nused < ntop)
                {
                    string longest = "";
                    int maxlength = -1; 
                    foreach (string newpic in dummykeys)
                    {
                        if (newpix[newpic].Length > maxlength)
                        {
                            longest = newpic;
                            maxlength = newpix[newpic].Length;
                        }
                    }
                    pixtouse.Add(longest);
                    newpix[longest] = "";
                    nused++;
                }
            }
            else
                foreach (string newpic in newpix.Keys)
                    if ( newpix[newpic] != "" )
                        pixtouse.Add(newpic);

            //Then actually use them, according to nchoice value:

            string gallerylabel = "Bildgalleri";
            string talkpage = "Diskussion";
            string disktext = "\n\n==Bilder från interwiki==\nBoten " + botaccount + " har identifierat följande bilder som används på andra språkversioner av den här artikeln:\n\n";
            string disksig = "~~~~";

            switch (makelang)
            {
                case "sv":
                    gallerylabel = "Bildgalleri";
                    talkpage = "Diskussion";
                    disktext = "\n\n==Bilder från interwiki==\nBoten " + botaccount + " har identifierat följande bilder som används på andra språkversioner av den här artikeln:\n\n";
                    break;
                case "ceb":
                    gallerylabel = "Galeriya sa hulagway";
                    talkpage = "Hisgot";
                    break;
                case "war":
                    gallerylabel = "Image gallery";
                    talkpage = "Hiruhimangraw";
                    break;
                case "it":
                    gallerylabel = "Galleria di immagini";
                    talkpage = "Discussione";
                    disktext = "== Suggerimento di immagini ==\n{{Suggerimento immagini}}";
                    disksig = "Cordiali saluti, ~~~~";
                    logpage = "Utente:Lsjbot/imagelog";
                    break;
                case "nl":
                    gallerylabel = "Galleria di immagini";
                    talkpage = "Discussione";
                    disktext = "== Immagine suggerimento ==\n{{Immaginesuggerimento2015}}";
                    disksig = " -- ~~~~";
                    logpage = "Utente:Lsjbot/imagelog";
                    break;
                default:
                    gallerylabel = "Image gallery";
                    break;
            }

            string gallery = "\n\n== "+gallerylabel+" ==\n\n<gallery>\n";

            switch (nchoice)
            {
                case 1:
                    foreach (string newpic in pixtouse)
                        gallery = gallery + newpic + "\n";
                    gallery = gallery + "</gallery>\n\n";

                    int ipos = p.text.IndexOf("[[Kategori");
                    if ((ipos < 0 ) && (makelang == "war"))
                        ipos = p.text.IndexOf("[[Kaarangay");

                    string botendtext = "== Källor ==";
                    if (p.text.Contains(botendtext))
                    {
                        ipos = p.text.IndexOf(botendtext);
                    }
                    if (ipos > 0)
                        p.text = p.text.Insert(ipos, gallery);
                    else
                        p.text += gallery;
                    break;
                case 2:
                    foreach (string newpic in pixtouse)
                        p.text = p.text.Replace("[[Kategori", "[[Fil:" + newpic + "|thumb|right|]]\n\n" + "[[Kategori");
                    break;
                case 3:

                    Page pdisk = new Page(svsite, talkpage + ":" + p.title);
                    if (!tryload(pdisk,2))
                        continue;
                    //Skip if already processed by the bot:
                    if (pdisk.text.Contains(disktext) || pdisk.text.Contains(botaccount))
                        continue;

                    if (!String.IsNullOrEmpty(pdisk.text))
                        pdisk.text += "\n\n";
                    pdisk.text = pdisk.text + disktext;
                    gallery = gallery.Replace("\n== " + gallerylabel + " ==\n\n", "");//"=== " + gallerylabel + " ===");
                    foreach (string newpic in pixtouse)
                        gallery = gallery + newpic + "\n";
                    gallery = gallery + "</gallery>\n" + disksig + "\n";
                    pdisk.text = pdisk.text + gallery;
                    //Bot.editComment = "Fixar bildförslag från iw";
                    //isMinorEdit = false;
                    trysave(pdisk,2);
                    p.text = "";
                    try
                    {
                        p.text = "";
                        p.Watch();
                    }
                    catch (WebException e)
                    {
                        string message = e.Message;
                        Console.Error.WriteLine(message);
                        Thread.Sleep(10000);//milliseconds
                    }

                    //Thread.Sleep(55000);//milliseconds
                    //Console.WriteLine("<ret>");
                    //Console.ReadLine();
                    break;
                case 4:
                    pwork.text = pwork.text + "===" + p.title + "===\n";
                    foreach (string newpic in pixtouse)
                        gallery = gallery + newpic + "\n";
                    gallery = gallery + "</gallery>\n\n";
                    pwork.text = pwork.text + gallery;
                    break;
            }
            //DONE!  Now save if needed.


            //Bot.editComment = editcomment;
            //isMinorEdit = false;
            if ((nchoice == 1) || (nchoice == 2))
            {
                int ntry = 0;
                if (p.text != origtext)
                    while (ntry < 3)
                    {
                        try
                        {
                            p.Save();
                            ntry = 999;
                        }
                        catch (WebException e)
                        {
                            Console.WriteLine(e.Message);
                            ntry++;
                            continue;
                        }
                    }
            }
            if (nchoice == 4)
                trysave(pwork,3);
            //Thread.Sleep(4000);//milliseconds
            //Console.WriteLine("nexttime = "+nexttime.ToLongTimeString());
            //Console.WriteLine("Now = " + DateTime.Now.ToLongTimeString());
            //while (DateTime.Now.CompareTo(nexttime) < 0)
            //    continue;
            //oldtime = DateTime.Now;

            nedit++;
        
              
        }

        Console.WriteLine("Total #edits = " + nedit.ToString());
        Page plog = new Page(svsite, logpage);
        tryload(plog, 2);
        plog.text += "\n# Category:" + cattodo + "; Total # pages = " + pl.Count().ToString() + "; Total #edits = " + nedit.ToString() + "\n";
        trysave(plog, 2);
	}
}
