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

    public static string get_pictureparam(Page ltp)
    {
        string imagename = "";
        string[] param = ltp.text.Split('|');
        foreach (string par in param)
        {
            if (((par.Trim().ToLower().IndexOf("bild1") == 0) || (par.Trim().ToLower().IndexOf("image1") == 0)) && (par.Contains("=")))
            {
                imagename = par.Split('=')[1].Trim();
                if (imagename.Contains("}}"))
                    imagename = imagename.Remove(imagename.IndexOf("}}")).Trim();
                //Console.WriteLine("imagename = " + imagename);
                break;
            }
        }
        return imagename;
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
            pl.FillFromCategory("Koordinatmallar");

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

            List<string> linkword = new List<string>();
            //linkword.Add("Catalogue of Life");

            //Require title to contain one in requiretitle list:
            List<string> requiretitle = new List<string>();
            //requiretitle.Add("Radioprogram nerlagda");

            //Require ALL in requireword list:
            List<string> requireword = new List<string>();
            //requireword.Add("obotskapad");
            requireword.Add("{{#switch:{{{1}}}");


            //Require AT LEAST ONE in requireone list:
            List<string> requireone = new List<string>();



            List<string> vetoword = new List<string>();
            vetoword.Add("reliefkarta_om_den_finns");

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


                bool bild1 = !String.IsNullOrEmpty(get_pictureparam(p));

                if (bild1)
                {
                    p.text = p.text.Replace("bild1=", "bild1|reliefkarta_om_den_finns =");
                    p.text = p.text.Replace("bild1 =", "bild1|reliefkarta_om_den_finns =");
                }
                else
                {
                    p.text = p.text.Replace("bild=", "bild|reliefkarta_om_den_finns =");
                    p.text = p.text.Replace("bild =", "bild|reliefkarta_om_den_finns =");
                }

                //foreach (KeyValuePair<string, string> replacepair in replacedict)
                //{
                //    p.text = p.text.Replace(replacepair.Key, replacepair.Value);

                //}

                //Save the result:

                if (p.text != origtext)
                {
                    //Bot.editComment = "Ersätter och wikilänkar";
                    //isMinorEdit = true;

                    if (trysave(p, 4))
                    {
                        nedit++;
                        if (nedit < 10)
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
        while (nedit > 0);

	}



}
