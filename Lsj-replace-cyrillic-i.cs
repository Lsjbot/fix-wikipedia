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
    public static string cyrillic_i = "і";
    public static string latin_i = "i";
    public static string cyrillic_I = "І";
    public static string latin_I = "I";


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

    public static void Main()
	{
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string botkonto = "Lsjbot";
        string makelang = "sv";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);
        site.defaultEditComment = "Ersätter och wikilänkar";
        site.minorEditByDefault = true;

        if (cyrillic_i == latin_i)
            Console.WriteLine("same i");
        else
            Console.WriteLine("different i");
        if (cyrillic_I == latin_I)
            Console.WriteLine("same I");
        else
            Console.WriteLine("different I");


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
            pl.FillFromCategory("Robotskapade Vitrysslandartiklar");//done: 2012-10, 2013-01, 2013-02, 2013-03, 2013-04, 2013-06, 2013-07, 2013-08, 2013-09, 2013-10, 2014-06, 2014-07, 2014-08 

            //Find subcategories of a category
            //pl.FillSubsFromCategory("Svampars vetenskapliga namn");

            //Find articles from all the links to an article, mostly useful on very small wikis
            //pl.FillFromLinksToPage("Användare:Lsjbot/Algoritmer");

            //Find articles containing a specific string
            //pl.FillFromSearchResults("insource:\"http://www.itis.gov;http://\"", 4999);

            //Set specific article:
            //Page pp = new Page(site, "Citrontrogon");pl.Add(pp);

            //Skip all namespaces except articles:
            pl.RemoveNamespaces(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 100, 101 });

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
            requiretitle.Add(cyrillic_i);
            requiretitle.Add(cyrillic_I);
            

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
                //Skip start of alphabet:
                //if (String.Compare(p.title,"Sicydium") < 0 )
                //    continue;

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

                string tit = remove_disambig(p.title);
                string latintit = tit.Replace(cyrillic_i, latin_i).Replace(cyrillic_I, latin_I);

                p.text = p.text.Replace(tit, latintit);

                //Do the actual replacement:

                //foreach (KeyValuePair<string, string> replacepair in replacedict)
                //{
                //    p.text = p.text.Replace(replacepair.Key, replacepair.Value);

                //}

                //foreach (KeyValuePair<string, string> replacepair in regexdict)
                //{
                //    p.text = Regex.Replace(p.text, replacepair.Key, replacepair.Value);

                //}


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
                iremain--;
                Console.WriteLine(iremain.ToString() + " remaining.");
            }

            Console.WriteLine("Total # edits = " + nedit.ToString());

        }
        while (false);// (nedit > 0);

	}



}
