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


	public static void addpages(Site site,PageList pl)
	{
        //Page pp1 = new Page(site, "Aega monophthalma"); pl.Add(pp1);
        List<string> sl = new List<string>();
        //sl.Add("Lista över insjöar i Arjeplogs kommun (1-1000)");
        sl.Add("Ðakovića Kosa");
        sl.Add("Ðanuša");

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
        site.defaultEditComment = "Ersätter och wikilänkar";
        site.minorEditByDefault = true;

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
            //pl.FillAllFromCategoryTree("Robotskapade Cypernartiklar");
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
            //pl.FillFromCategory("Robotskapade Australienartiklar");//done: 2012-10, 2013-01, 2013-02, 2013-03, 2013-04, 2013-06, 2013-07, 2013-08, 2013-09, 2013-10, 2014-06, 2014-07, 2014-08 

            //Find subcategories of a category
            //pl.FillSubsFromCategory("Svampars vetenskapliga namn");

            //Find articles from all the links to an article, mostly useful on very small wikis
            //pl.FillFromLinksToPage("Användare:Lsjbot/Algoritmer");
            //pl.FillFromLinksToPage("Nicosia (huvudstaden)");

            //Find articles containing a specific string
            //pl.FillFromSearchResults("insource:\"Användare:Lsjbot/Algoritmer\"", 4999);
            pl.FillFromSearchResults("insource:\"terass\"", 4999);

            //Set specific article:
            //Page pp = new Page(site, "Citrontrogon");pl.Add(pp);

            //Skip all namespaces except articles:
            //pl.RemoveNamespaces(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 100, 101 });

            Dictionary<string, string> replacedict = new Dictionary<string, string>();

            //replacedict.Add("ligger på ön [[Maltese Islands]]", "ligger på ön [[Malta (ö)|Malta]]");
            //replacedict.Add("[http://www.itis.gov;http://www.cbif.gc.ca/itis (Canada);http://siit.conabio.gob.mx (Mexico) ITIS Global: The Integrated Taxonomic Information System]", "[http://www.itis.gov ITIS Global: The Integrated Taxonomic Information System], [http://www.cbif.gc.ca/eng/integrated-taxonomic-information-system-itis/ (Canada)], [http://www.conabio.gob.mx (Mexico)]");
            //replacedict.Add("[http://www.itis.gov;http://www.cbif.gc.ca/itis (Canada);http://siit.conabio.gob.mx (Mexico) ITIS Regional: The Integrated Taxonomic Information System]", "[http://www.itis.gov ITIS Global: The Integrated Taxonomic Information System], [http://www.cbif.gc.ca/eng/integrated-taxonomic-information-system-itis/ (Canada)], [http://www.conabio.gob.mx (Mexico)]");
            //replacedict.Add("[[djur|djur]]", "[[djur]]");
            //replacedict.Add("är ett [[släkte]] av [[djur]]", "är ett [[släkte]] av [[slemmaskar]]");
            //replacedict.Add("Phylum nga naglalakip la hin", "Ini nga phylum in naglalakip la hin");
            //replacedict.Add("[[Kategori:Leddjur]]", "[[Kategori:Kräftdjur]]");
            //replacedict.Add("[[Kategori:Kräftdjur]]", "[[Kategori:Hoppkräftor]]");

            //replacedict.Add("[[koralldjur|korall]]art", "[[havsanemon]]art");

            //replacedict.Add("| familia_sv = [[Havsormar]]\n| familia = Hydrophiidae", "| familia_sv = [[Giftsnokar]]\n| familia = Elapidae\n| subfamilia_sv = [[Havsormar]]\n| subfamilia = Hydrophiinae");
            //replacedict.Add("[[familj (biologi)|familjen]] [[havsormar]]", "[[familj (biologi)|familjen]] [[giftsnokar]] och underfamiljen [[havsormar]]");
            //replacedict.Add("| familia_sv = [[Giftsnokar]]\n familia = Elapidae","| familia_sv = [[Giftsnokar]]\n| familia = Elapidae");

            //replacedict.Add("| familia = Muridae", "| familia = Cricetidae\n| subfamilia_sv = [[Hamstrar]]\n| subfamilia = Cricetinae");
            //replacedict.Add("| familia = Muridae", "| familia = Cricetidae\n| subfamilia_sv = \n| subfamilia = [[Tylomyinae]]");
            //replacedict.Add("Råttdjur", "Hamsterartade gnagare");
            //replacedict.Add("[[Muridae|råttdjur", "[[Cricetidae|hamsterartade gnagare");
            //replacedict.Add("råttdjur", "hamsterartade gnagare");
            //replacedict.Add("[[Muridae|hamsterartade gnagare", "[[Cricetidae|hamsterartade gnagare");
            //replacedict.Add(" (Muridae)]]", "]]");
            ////replacedict.Add("| genus_sv = \n| genus = [[Arborimus]]","| genus_sv = \n| genus = [[Arborimus]]"
            //replacedict.Add("[[Weaver (auktor)]]", "[[Weaver (auktor)|Weaver]]");

            //replacedict.Add("| familia = Muridae", "| familia = [[Nesomyidae]]\n| subfamilia_sv = [[Afrikanska klippmöss]]\n| subfamilia = Petromyscinae");
            //replacedict.Add("| familia_sv = [[Råttdjur]]", "| familia_sv =");
            //replacedict.Add("[[familj (biologi)|familjen]] [[råttdjur]]", "[[familj (biologi)|familjen]] [[Nesomyidae]]");
            //replacedict.Add("| familia_sv = [[Råttdjur]]\n| familia = Muridae", "| familia_sv =\n| familia = [[Nesomyidae]]\n| subfamilia_sv = [[Trädmöss]]| subfamilia = Dendromurinae");
            //replacedict.Add("| subfamilia_sv = [[Hamsterråttor]]", "| subfamilia_sv = [[Trädmöss]]");

            //replacedict.Add("[[Kategori:Långtungebin]]", "[[Kategori:Orkidébin]]");
            //replacedict.Add("och [[familj (biologi)|familjen]] [[långtungebin]]", "[[tribus]] [[orkidébin]], och [[familj (biologi)|familjen]] [[långtungebin]]");
            //replacedict.Add("| familia_sv = [[Bladhorningar]]", "| superfamilia_sv = [[Bladhorningar]]\n| superfamilia = Scarabaeoidea");
            //replacedict.Add("och [[familj (biologi)|familjen]] [[bladhorningar]]", "[[familj (biologi)|familjen]] [[Scarabaeidae]] och [[överfamilj]]en [[bladhorningar]]");
            //replacedict.Add("ingår i [[familj (biologi)|familjen]] [[Scarabaeidae|bladhorningar]]", "ingår i [[familj (biologi)|familjen]] [[Scarabaeidae]] och [[överfamilj]]en [[bladhorningar]]");

            //replacedict.Add("Inga underarter finns listade.", "Inga [[underart]]er finns listade i [[Catalogue of Life]].");
            //replacedict.Add("[[Kategori:Egentliga insekter]]", "[[Kategori:Termiter]]");
            //replacedict.Add("av Linnaeus ", "av [[Carl von Linné]] ");
            //replacedict.Add(" = Linnaeus,", " = [[Carl von Linné|Linnaeus]],");
            //replacedict.Add("av [[Carl von Linné|Linnaeus]]", "av [[Carl von Linné]]");
            //replacedict.Add(" taxon_authority = [[Linnaeus (auktor)|Linnaeus]]", " taxon_authority = [[Carl von Linné|Linnaeus]]");
            //replacedict.Add("av [[Linnaeus (auktor)|Linnaeus]] ", "av [[Carl von Linné]] ");
            //replacedict.Add(" och Amp; ", " och ");
            //replacedict.Add("[[[[", "[[");
            //replacedict.Add("]]]]", "]]");
            //replacedict.Add("<noinclude>{{Kartposition/Info}}", "<noinclude>\n{{Kartposition/Info}}");
            //replacedict.Add("[[Eulalia]]", "[[Eulalia (växter)|Eulalia]]");
            //replacedict.Add("Anomalepidae]]", "Anomalepididae]]");
            //replacedict.Add("[[Kategori:Egentliga insekter]]", "[[Kategori:Spökskräckor]]");
            //replacedict.Add("| ordo_sv = \n| ordo = [[Phasmida]]", "| ordo_sv = [[Spökskräckor]]\n| ordo = Phasmida");
            //replacedict.Add("| ordo_sv = \n| ordo = [[Phasmatodea]]", "| ordo_sv = [[Spökskräckor]]\n| ordo = Phasmatodea");
            //replacedict.Add("| ordo_sv = \n| ordo = [[Phasmatodea]]", "| ordo_sv = [[Spökskräckor]]\n| ordo = Phasmatodea");
            //replacedict.Add("<I>", "''");
            //replacedict.Add("<i>", "''");
            //replacedict.Add("Collection Patrimoines ,</ref>", "Collection Patrimoines.''</ref>");
            //replacedict.Add("Expedition 1907-1908</b>", "Expedition 1907-1908.''");
            //replacedict.Add("[[Further-eastern European Time|FET]]", "[[Östafrikansk tid|EAT]]");
            //replacedict.Add("{{Sidnamn annan skrift|latinska alfabetet}}", "{{Sidnamn annan skrift|kyrilliska alfabetet}}");
            //replacedict.Add("[[Bangladesh Standard Time|BST]]", "[[Bhutan Time|BTT]]");
            //replacedict.Add("<ref name = \"vp\">{{Cite web |url= {{Viewfinderlink}}|title= Viewfinder Panoramas Digital elevation Model|date= 2015-06-21|format= }}</ref>", "");
            //replacedict.Add("== Saysay ==","");
            //replacedict.Add("<references group=\"saysay\"/>","");
            //replacedict.Add("administratibo nga mga dibisyon sa Bangladesh", "administratibo nga mga dibisyon sa Burkina Faso");
            //replacedict.Add("image = Бесцветный богомол.jpg", "image =");
            //replacedict.Add("bild = Бесцветный богомол.jpg", "bild =");
            //replacedict.Add(" = [[Brčko]]", " = [[Brčko (distrikt)|Brčko]]");
            //replacedict.Add(" = Entitet", " = Distrikt");
            //replacedict.Add("entiteten <!--ADM1-->[[Brčko]]", "distriktet <!--ADM1-->[[Brčko (distrikt)|Brčko]]");
            //replacedict.Add(" = [[Brčko]]", "| state                       = [[Brčko (distrikt)|Brčko]]");
            //replacedict.Add("En underart finns: ''", "Utöver nominatformen finns också underarten ''");
            //replacedict.Add("| timezone               = [[Fernando de Noronha Time|FNT]]", "| timezone             = [[Brasilia Time|BRT]]");
            //replacedict.Add("| timezone_DST           = [[Amazon Summer Time|AMST]]", "| timezone_DST         = [[Brasilia Summer Time|BRST]]");
            //replacedict.Add("| utc_offset             = -2", "| utc_offset           = -3");
            //replacedict.Add("| utc_offset_DST         = -3", "| utc_offset_DST       = -2");
            //replacedict.Add("[[Kungariket Olanda]]", "[[Olanda]]");
            //replacedict.Add("Ð", "Đ"); //från isländskt Đ till bosniskt Đ
            //replacedict.Add("Schweizs administrativa indelning", "Schweiz administrativa indelning");
            //replacedict.Add("Mer om algoritmen finns här: [[Användare:Lsjbot/Algoritmer]].", "{{Lsjbot-algoritmnot}}");
            //replacedict.Add(" delen av landet, ", " delen av landskapet, ");
            //replacedict.Add("[[Nicosia (huvudstaden)|Nicosia]]", "[[Nicosia]]");
            //replacedict.Add("Den ligger i regionen <!--ADM2-->[[", "Den ligger i kommunen <!--ADM2-->[[");
            replacedict.Add("[[terass (landform)|terass]]", "[[terrass (landform)|terrass]]");
            replacedict.Add("(terass", "(terrass");
            replacedict.Add("]], terass,", "]], terrass,");


            Dictionary<string, string> regexdict = new Dictionary<string, string>();
            //regexdict.Add(@"\| timezone *= \[\[Fernando de Noronha Time\|FNT\]\]", "| timezone             = [[Brasilia Time|BRT]]");
            //regexdict.Add(@"\| timezone_DST *= \[\[Amazon Summer Time\|AMST\]\]", "| timezone_DST         = [[Brasilia Summer Time|BRST]]");
            //regexdict.Add(@"\| utc_offset *= -2", "| utc_offset           = -3");
            regexdict.Add(@"\| category *= Terass", "| category     = Terrass");




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
            //vetoword.Add("Argentina");
            //vetoword.Add("Island");
            //vetoword.Add("isländska");


            DateTime oldtime = DateTime.Now;
            oldtime = oldtime.AddSeconds(5);

            Console.WriteLine("Pages to change : " + pl.Count().ToString());

            int iremain = pl.Count();

            foreach (Page p in pl)
            {
                iremain--;
                //Skip start of alphabet:
                //if (String.Compare(p.title, "Vivienne") < 0)
                //{
                //    continue;
                //}

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

                    if ( p.title.Contains("(terass"))
                    {
                        movepage(site, p.title, p.title.Replace("(terass", "(terrass"));
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
        while (nedit > 0);

	}



}
