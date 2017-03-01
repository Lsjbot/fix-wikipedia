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
    public static string tabstring = "\t";
    public static char tabchar = tabstring[0];
    public static char degreesymb = '°';
    public static string degreestring = degreesymb.ToString();
    public static char minutesymb = '\'';
    public static List<string> forktemplates = new List<string>();
    public static string makelang = "";


    public static List<string> donecat = new List<string>();

    public static List<string> coordparams = new List<string>(); //possible template parameters for latitude/longitude

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

    public static double tryconvertdouble(string word)
    {
        double i = 9999.9;

        try
        {
            i = Convert.ToDouble(word);
        }
        catch (OverflowException)
        {
            Console.WriteLine("i Outside the range of the Double type: " + word);
        }
        catch (FormatException)
        {
            try
            {
                i = Convert.ToDouble(word.Replace(".", ","));
            }
            catch (FormatException)
            {
                Console.WriteLine("i Not in a recognizable double format: " + word.Replace(".", ","));
            }
            //Console.WriteLine("i Not in a recognizable double format: " + word);
        }

        return i;

    }


    public static double coordlat(string coordstring)
    {
        //{{Coord|42|33|18|N|1|31|59|E|region:AD_type:city|display=title,inline}}

        string[] cs = coordstring.Split('|');

        if (cs.Length <= 2)
            return 9999.9;
        else
        {
            int ins = -1;
            int iew = -1;
            int iregion = -1;
            for (int i = 1; i < cs.Length; i++)
            {
                if ((cs[i].ToUpper() == "N") || (cs[i].ToUpper() == "S"))
                    ins = i;
                if ((cs[i].ToUpper() == "E") || (cs[i].ToUpper() == "W"))
                    iew = i;
                if (cs[i].ToLower().Contains("region"))
                    iregion = i;
            }
            if (ins < 0)
                return tryconvertdouble(cs[1]);
            else
            {
                double lat = 0.0;
                double scale = 1.0;
                for (int i = 1; i < ins; i++)
                {
                    double lx = tryconvertdouble(cs[i]);
                    if (lx < 90.0)
                        lat += lx / scale;
                    scale *= 60.0;
                }
                if (cs[ins].ToUpper() == "S")
                    lat = -lat;
                return lat;
            }
        }
        //else if (cs.Length < 9)
        //{
        //    return tryconvertdouble(cs[1]);
        //}
        //else
        //{
        //    double lat = tryconvertdouble(cs[1]) + tryconvertdouble(cs[2]) / 60 + tryconvertdouble(cs[3]) / 3600;
        //    if (cs[4].ToUpper() == "S")
        //        lat = -lat;
        //    return lat;
        //}
    }

    public static double coordlong(string coordstring)
    {
        //{{Coord|42|33|18|N|1|31|59|E|region:AD_type:city|display=title,inline}}

        string[] cs = coordstring.Split('|');
        if (cs.Length <= 2)
            return 9999.9;
        else
        {
            int ins = -1;
            int iew = -1;
            int iregion = -1;
            for (int i = 1; i < cs.Length; i++)
            {
                if ((cs[i].ToUpper() == "N") || (cs[i].ToUpper() == "S"))
                    ins = i;
                if ((cs[i].ToUpper() == "E") || (cs[i].ToUpper() == "W"))
                    iew = i;
                if (cs[i].ToLower().Contains("region"))
                    iregion = i;
            }
            if (iew < 0)
                return tryconvertdouble(cs[2]);
            else
            {
                double lon = 0.0;
                double scale = 1.0;
                for (int i = ins + 1; i < iew; i++)
                {
                    double lx = tryconvertdouble(cs[i]);
                    if (lx < 180.0)
                        lon += lx / scale;
                    scale *= 60.0;
                }
                if (cs[iew].ToUpper() == "W")
                    lon = -lon;
                return lon;
            }
        }
        //else
        //{
        //    double lon = tryconvertdouble(cs[5]) + tryconvertdouble(cs[6]) / 60 + tryconvertdouble(cs[7]) / 3600;
        //    if (cs[8].ToUpper() == "W")
        //        lon = -lon;
        //    return lon;
        //}
    }


    public static string initialcap(string orig)
    {
        if (String.IsNullOrEmpty(orig))
            return "";

        int initialpos = 0;
        if (orig.IndexOf('|') > 0)
            initialpos = orig.IndexOf('|') + 1;
        else if (orig.IndexOf("[[") >= 0)
            initialpos = orig.IndexOf("[[") + 2;
        string s = orig.Substring(initialpos, 1);
        s = s.ToUpper();
        string final = orig;
        final = final.Remove(initialpos, 1).Insert(initialpos, s);
        //s += orig.Remove(0, 1);
        return final;
    }

    public static double degmindecode(string degmin)
    {
        //latityud=46° 8' N

        double result = 9999.9;
        if (!degmin.Contains(degreestring))
            return result;

        string[] w1 = degmin.Split(degreesymb);
        double deg = tryconvertdouble(w1[0]);
        string[] w2 = w1[1].Split(minutesymb);
        double min = tryconvertdouble(w2[0]);

        if (deg >= 0)
            result = deg;
        if (min >= 0)
            result += min / 60.0;
        if ( result < 360.0 )
            if ((degmin.Contains("S")) || (degmin.Contains("W")))
                result = -result;

        return result;
    }

    public static bool is_fork(Site makesite,Page p)
    {
        if (!p.Exists())
            return false;

        if (makelang == "ceb")
        {
            if (p.text.ToLower().Contains("{{giklaro"))
                return true;
        }
        else if (makelang == "war")
        {
            if (p.text.ToLower().Contains("{{pansayod"))
                return true;
        }
        else if (makelang == "sv")
        {
            if (forktemplates.Count == 0)
            {
                PageList pl = new PageList(makesite);
                pl.FillFromCategory("Förgreningsmallar");
                foreach (Page pp in pl)
                    forktemplates.Add(pp.title.Replace("Mall:", "").ToLower());
            }
            foreach (string ft in forktemplates)
                if (p.text.ToLower().Contains("{{" + ft))
                    return true;
        }

        return false;
    }



    public static double[] get_article_coord(Site makesite,Page p)
    {
        double lat = 9999.9;
        double lon = 9999.9;
        double[] latlong = { lat, lon };
        int ncoord = 0;

        if (coordparams.Count == 0)
        {
            coordparams.Add("Coord");
            coordparams.Add("coord");
            coordparams.Add("lat_d");
            coordparams.Add("lat_g");
            coordparams.Add("latitude");
            coordparams.Add("latitud");
            coordparams.Add("latityud");
            coordparams.Add("lat");
            coordparams.Add("nordliggrad");
            coordparams.Add("sydliggrad");
            coordparams.Add("breddgrad");
        }


        Dictionary<string, int> geotempdict = new Dictionary<string, int>();

        //string template = mp(63, null);
        foreach (string tt in p.GetTemplates(true, true))
        {
            if (tt.Length < 5)
                continue;
            string cleantt = tt.Replace("\n", "").Trim().Substring(0, 5).ToLower();
            Console.WriteLine("cleantt = |" + cleantt + "|");
            //if (true)//(geolist.Contains(template + cleantt))
            //{
            //geotemplatefound = true;
            //Console.WriteLine("Possible double");

            if (!geotempdict.ContainsKey(cleantt))
                geotempdict.Add(cleantt, 1);
            else
                geotempdict[cleantt]++;
            bool foundwithparams = false;
            //foreach (string ttt in p.GetTemplates(true, true))
            //    if (ttt.IndexOf(tt) == 0)
            //{
            foundwithparams = true;
            //Console.WriteLine("foundwithparams");
            if (cleantt == "coord")
            {
                Console.WriteLine("found {{coord}}");
                string coordstring = tt;
                if (coordstring.Length > 10)
                {
                    double newlat = coordlat(coordstring);
                    double newlon = coordlong(coordstring);
                    if (newlat + newlon < 720.0)
                    {
                        if (ncoord == 0)
                        {
                            lat = newlat;
                            lon = newlon;
                        }
                        else if ((Math.Abs(newlat - lat) + Math.Abs(newlon - lon) > 0.01)) //two different coordinates in article; skip!
                        {
                            lat = 9999;
                            lon = 9999;
                            ncoord = 9999;
                            break;
                        }
                        else
                        {
                            lat = newlat;
                            lon = newlon;
                        }
                    }
                    if (lat + lon < 720.0)
                        ncoord++;
                    if (ncoord > 3)
                        break;
                }

            }
            else
            {
                Dictionary<string, string> pdict = makesite.ParseTemplate(tt);
                foreach (string cp in coordparams)
                {
                    //Console.WriteLine("cp = " + cp);
                    double oldlat = lat;
                    double oldlon = lon;
                    if (pdict.ContainsKey(cp))
                    {
                        //coordfound = true;
                        Console.WriteLine("found coordparams");
                        switch (cp)
                        {
                            case "latitude":
                            case "latitud":
                            case "lat":
                            case "latityud":
                                if (pdict[cp].Contains(degreestring))
                                    lat = degmindecode(pdict[cp]);
                                else
                                    lat = tryconvertdouble(pdict[cp]);
                                string plon = "";
                                if (pdict.ContainsKey("longitude"))
                                    plon = pdict["longitude"];
                                else if (pdict.ContainsKey("longitud"))
                                    plon = pdict["longitud"];
                                else if (pdict.ContainsKey("longhitud"))
                                    plon = pdict["longhitud"];
                                else if (pdict.ContainsKey("long"))
                                    plon = pdict["long"];
                                else
                                    Console.WriteLine("latitude but no longitude");
                                if (plon.Contains(degreestring))
                                    lon = degmindecode(plon);
                                else
                                    lon = tryconvertdouble(plon);
                                break;
                            case "nordliggrad":
                            case "sydliggrad":
                                lat = tryconvertdouble(pdict[cp]);
                                if (pdict.ContainsKey("östliggrad"))
                                    lon = tryconvertdouble(pdict["östliggrad"]);
                                else if (pdict.ContainsKey("västliggrad"))
                                    lon = tryconvertdouble(pdict["västliggrad"]);
                                else
                                    Console.WriteLine("latitude but no longitude");
                                break;
                            case "breddgrad":
                                lat = tryconvertdouble(pdict[cp]);
                                if (pdict.ContainsKey("längdgrad"))
                                    lon = tryconvertdouble(pdict["längdgrad"]);
                                else
                                    Console.WriteLine("latitude but no longitude");
                                break;
                            case "lat_d":
                            case "latd":
                            case "lat_g":
                                double llat = 0.0;
                                llat = tryconvertdouble(pdict[cp]);
                                if (llat > 0)
                                {
                                    lat = llat;
                                    if (pdict.ContainsKey("long_d"))
                                        lon = tryconvertdouble(pdict["long_d"]);
                                    else if (pdict.ContainsKey("longd"))
                                        lon = tryconvertdouble(pdict["longd"]);
                                    else if (pdict.ContainsKey("long_g"))
                                        lon = tryconvertdouble(pdict["long_g"]);
                                    if (pdict.ContainsKey("lat_m"))
                                        lat += tryconvertdouble(pdict["lat_m"]) / 60;
                                    if (pdict.ContainsKey("long_m"))
                                        lon += tryconvertdouble(pdict["long_m"]) / 60;
                                    if (pdict.ContainsKey("lat_s"))
                                        lat += tryconvertdouble(pdict["lat_s"]) / 3600;
                                    if (pdict.ContainsKey("long_s"))
                                        lon += tryconvertdouble(pdict["long_s"]) / 3600;
                                    if (pdict.ContainsKey("lat_NS"))
                                    {
                                        if (pdict["lat_NS"].ToUpper() == "S")
                                            lat = -lat;
                                    }
                                    if (pdict.ContainsKey("long_EW"))
                                    {
                                        if (pdict["long_EW"].ToUpper() == "W")
                                            lon = -lon;
                                    }
                                }
                                break;
                            case "Coord":
                            case "coord": //{{Coord|42|33|18|N|1|31|59|E|region:AD_type:city|display=title,inline}}
                                string coordstring = pdict[cp];
                                if (coordstring.Length > 10)
                                {
                                    lat = coordlat(coordstring);
                                    lon = coordlong(coordstring);
                                }
                                break;
                            default:
                                Console.WriteLine("coord-default:" + tt);
                                break;


                        }
                        if (lat + lon < 720.0)
                        {
                            if ((Math.Abs(oldlat - lat) + Math.Abs(oldlon - lon) > 0.01)) //two different coordinates in article; skip!
                            {
                                lat = 9999;
                                lon = 9999;
                                ncoord = 9999;
                                break;
                            }
                        }
                        else
                        {
                            lat = oldlat;
                            lon = oldlon;
                        }

                        if (lat + lon < 720.0)
                            ncoord++;
                        if (ncoord > 3)
                            break;



                    }
                }
            }
            //}
            if (!foundwithparams)
                Console.WriteLine("Params not found");
            Console.WriteLine("lat = " + lat.ToString());
            Console.WriteLine("lon = " + lon.ToString());
            //}
        }

        if (ncoord > 4) //several coordinate sets, probably a list or something; return failure
            return latlong;

        latlong[0] = lat;
        latlong[1] = lon;
        return latlong;
    }

    //public static double[] get_article_coord(Site site, Page p)
    //{
    //    double lat = 9999.9;
    //    double lon = 9999.9;
    //    double[] latlong = { lat, lon };
    //    int ncoord = 0;

    //    if (coordparams.Count == 0)
    //    {
    //        coordparams.Add("Coord");
    //        coordparams.Add("coord");
    //        coordparams.Add("lat_d");
    //        coordparams.Add("latd");
    //        coordparams.Add("lat");
    //        coordparams.Add("lat_g");
    //        coordparams.Add("latitude");
    //        coordparams.Add("latitud");
    //        coordparams.Add("latityud");
    //        coordparams.Add("nordliggrad");
    //        coordparams.Add("sydliggrad");
    //        coordparams.Add("breddgrad");
    //    }



    //    Dictionary<string, int> geotempdict = new Dictionary<string, int>();

    //    //string template = mp(63, null);
    //    foreach (string tt in p.GetTemplates(true, true))
    //    {
    //        if (tt.Length < 5)
    //            continue;
    //        string cleantt = tt.Replace("\n", "").Trim().Substring(0, 5).ToLower();
    //        Console.WriteLine("cleantt = |" + cleantt + "|");
    //        //if (true)//(geolist.Contains(template + cleantt))
    //        //{
    //        //geotemplatefound = true;
    //        //Console.WriteLine("Possible double");

    //        if (!geotempdict.ContainsKey(cleantt))
    //            geotempdict.Add(cleantt, 1);
    //        else
    //            geotempdict[cleantt]++;
    //        bool foundwithparams = false;
    //        //foreach (string ttt in p.GetTemplates(true, true))
    //        //    if (ttt.IndexOf(tt) == 0)
    //        //{
    //        foundwithparams = true;
    //        //Console.WriteLine("foundwithparams");
    //        if (cleantt == "coord")
    //        {
    //            Console.WriteLine("found {{coord}}");
    //            string coordstring = tt;
    //            if (coordstring.Length > 10)
    //            {
    //                lat = coordlat(coordstring);
    //                lon = coordlong(coordstring);
    //                if (lat + lon < 720.0)
    //                    ncoord++;
    //                if (ncoord > 2)
    //                    break;
    //            }

    //        }
    //        else
    //        {
    //            Dictionary<string, string> pdict = site.ParseTemplate(tt);
    //            foreach (string cp in coordparams)
    //            {
    //                //Console.WriteLine("cp = " + cp);
    //                if (pdict.ContainsKey(cp))
    //                {
    //                    //coordfound = true;
    //                    Console.WriteLine("found coordparam "+cp);
    //                    switch (cp)
    //                    {
    //                        case "latitude":
    //                        case "latitud":
    //                        case "lat":
    //                        case "latityud":
    //                            if (pdict[cp].Contains(degreestring))
    //                                lat = degmindecode(pdict[cp]);
    //                            else
    //                                lat = tryconvertdouble(pdict[cp]);
    //                            string plon = "";
    //                            if (pdict.ContainsKey("longitude"))
    //                                plon = pdict["longitude"];
    //                            else if (pdict.ContainsKey("longitud"))
    //                                plon = pdict["longitud"];
    //                            else if (pdict.ContainsKey("longhitud"))
    //                                plon = pdict["longhitud"];
    //                            else if (pdict.ContainsKey("long"))
    //                                plon = pdict["long"];
    //                            else
    //                                Console.WriteLine("latitude but no longitude");
    //                            if (plon.Contains(degreestring))
    //                                lon = degmindecode(plon);
    //                            else
    //                                lon = tryconvertdouble(plon);
    //                            break;
    //                        case "nordliggrad":
    //                        case "sydliggrad":
    //                            lat = tryconvertdouble(pdict[cp]);
    //                            if (cp = "sydliggrad")
    //                                lat = -lat;
    //                            if (pdict.ContainsKey("östliggrad"))
    //                                lon = tryconvertdouble(pdict["östliggrad"]);
    //                            else if (pdict.ContainsKey("västliggrad"))
    //                                lon = -tryconvertdouble(pdict["västliggrad"]);
    //                            else
    //                                Console.WriteLine("latitude but no longitude");
    //                            break;
    //                        case "breddgrad":
    //                            lat = tryconvertdouble(pdict[cp]);
    //                            if (pdict.ContainsKey("längdgrad"))
    //                                lon = tryconvertdouble(pdict["längdgrad"]);
    //                            else
    //                                Console.WriteLine("latitude but no longitude");
    //                            break;
    //                        case "lat_d":
    //                        case "latd":
    //                        case "lat_g":
    //                            double llat = 0.0;
    //                            llat = tryconvertdouble(pdict[cp]);
    //                            if (llat > 0)
    //                            {
    //                                lat = llat;
    //                                if (pdict.ContainsKey("long_d"))
    //                                    lon = tryconvertdouble(pdict["long_d"]);
    //                                else if (pdict.ContainsKey("longd"))
    //                                    lon = tryconvertdouble(pdict["longd"]);
    //                                else if (pdict.ContainsKey("long_g"))
    //                                    lon = tryconvertdouble(pdict["long_g"]);
    //                                if (pdict.ContainsKey("lat_m"))
    //                                    lat += tryconvertdouble(pdict["lat_m"]) / 60;
    //                                if (pdict.ContainsKey("long_m"))
    //                                    lon += tryconvertdouble(pdict["long_m"]) / 60;
    //                                if (pdict.ContainsKey("lat_s"))
    //                                    lat += tryconvertdouble(pdict["lat_s"]) / 3600;
    //                                if (pdict.ContainsKey("long_s"))
    //                                    lon += tryconvertdouble(pdict["long_s"]) / 3600;
    //                                if (pdict.ContainsKey("lat_NS"))
    //                                {
    //                                    if (pdict["lat_NS"].ToUpper() == "S")
    //                                        lat = -lat;
    //                                }
    //                                if (pdict.ContainsKey("long_EW"))
    //                                {
    //                                    if (pdict["long_EW"].ToUpper() == "W")
    //                                        lon = -lon;
    //                                }
    //                            }
    //                            break;
    //                        case "Coord":
    //                        case "coord": //{{Coord|42|33|18|N|1|31|59|E|region:AD_type:city|display=title,inline}}
    //                            string coordstring = pdict[cp];
    //                            if (coordstring.Length > 10)
    //                            {
    //                                lat = coordlat(coordstring);
    //                                lon = coordlong(coordstring);
    //                            }
    //                            break;
    //                        default:
    //                            Console.WriteLine("coord-default:" + tt);
    //                            break;


    //                    }

    //                    if (lat + lon < 720.0)
    //                        ncoord++;
    //                    if (ncoord > 2)
    //                        break;


    //                }
    //            }
    //        }
    //        //}
    //        if (!foundwithparams)
    //            Console.WriteLine("Params not found");
    //        Console.WriteLine("lat = " + lat.ToString());
    //        Console.WriteLine("lon = " + lon.ToString());
    //        //}
    //    }

    //    if (ncoord > 2) //several coordinate sets, probably a list or something
    //        return latlong;

    //    latlong[0] = lat;
    //    latlong[1] = lon;
    //    return latlong;
    //}


    public static void do_category(Site site, string catname, StreamWriter sw, string cattree)
    {
        Console.WriteLine("===========================================");
        if (donecat.Contains(catname))
            return;
        else
            donecat.Add(catname);

        Console.WriteLine("cattree = " + cattree);

        PageList pl = new PageList(site);
        try
        {
            pl.FillAllFromCategory(catname);
        }
        catch (WebException e)
        {
            string message = e.Message;
            Console.Error.WriteLine(message);
        }

        DateTime oldtime = DateTime.Now;
        oldtime = oldtime.AddSeconds(10);

        int pagecount = 0;

        int ncoord = 0;

        foreach (Page p in pl)
        {
            //Skip start of alphabet:
            string skipuntil = "";

            if ((skipuntil != "") && String.Compare(p.title, skipuntil) < 0)
                continue;


            if (p.title.Contains("Personer från"))
                continue;
            if (p.title.Contains("Sport "))
                continue;
            if (p.title.Contains("Sportklubbar "))
                continue;
            if (p.title.Contains("Sportevenemang"))
                continue;
            if (p.title.Contains("Eurovision"))
                continue;
            if (p.title.Contains("Musik "))
                continue;
            if (p.title.Contains("laystation"))
                continue;
            if (p.title.Contains("örfattare"))
                continue;
            if (p.title.Contains("TV-program"))
                continue;
            if (p.title.Contains("musiker"))
                continue;
            if (p.title.Contains("idningar"))
                continue;
            if (p.title.Contains("kådespelar"))
                continue;
            if (p.title.Contains("olitiker"))
                continue;
            if (p.title.Contains("Politik"))
                continue;
            if (p.title.Contains("drottare "))
                continue;
            if (p.title.Contains("historia"))
                continue;

            string origtitle = p.title;




            if ( p.GetNamespace() == 0)
            {

                if (!tryload(p, 2))
                    continue;
                if (!p.Exists())
                    continue;
                pagecount++;
                //if (pagecount > 50)
                //    continue;

                if (makelang == "sv")
                {
                    if (p.text.Contains("obotskapad"))
                        continue;
                }
                else if (makelang == "ceb" )
                {
                    if (p.text.Contains("himo ni bot"))
                        continue;
                }

                if (is_fork(site,p))
                    continue;

                double[] latlong = get_article_coord(site, p);
                if (latlong[0] + latlong[1] < 720.0)
                {
                    ncoord++;
                    Console.WriteLine(latlong[0].ToString() + "|" + latlong[1].ToString());
                    sw.WriteLine(p.title + tabstring + latlong[0].ToString() + tabstring + latlong[1].ToString());
                }
            }
            else if (p.GetNamespace() == 14)
            {
                do_category(site,p.title, sw, cattree+"|"+catname);
            }



            //iremain--;
            //Console.WriteLine(iremain.ToString() + " remaining.");
        }

        //if ((ncoord > 0) && (2 * ncoord >= pagecount))
        //{
        //    Page catpage = new Page(site, catname);
        //    if (!tryload(catpage, 2))
        //        return;
        //    if (!catpage.Exists())
        //        return;

        //    if (!catpage.text.ToLower().Contains("geolänk"))
        //    {
        //        catpage.text = "{{GeoLänk}}\n" + catpage.text;
        //        trysave(catpage, 2);
        //        nedit++;
        //        //Wait:
        //        DateTime newtime = DateTime.Now;
        //        while (newtime < oldtime)
        //            newtime = DateTime.Now;
        //        oldtime = newtime.AddSeconds(10);
        //    }
        //}

    }


    public static void Main()
	{
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string botkonto = "Lsjbot";
        makelang = "sv";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);

        site.defaultEditComment = "Lägger in mall:geolänk";
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
 
        //pl.FillAllFromCategory("Asiens geografi efter land");

        //Find articles from all the links to an article, mostly useful on very small wikis
        //pl.FillFromLinksToPage("Boidae");

        //Set specific article:
        //Page pp = new Page(site, "Användare:Lsjbot/Flytt-test");pl.Add(pp);


        //Skip all namespaces except categories:
        //pl.RemoveNamespaces(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 100, 101 });
        //pl.RemoveNamespaces(new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,100,101});


        donecat.Add("Kategori:Geografi i Sverige efter ort");
        donecat.Add("Kategori:Sveriges geografi efter informella regioner");
        donecat.Add("Kategori:Listor med anknytning till Sveriges geografi");
        donecat.Add("Kategori:Fornminnen i Sverige");
        donecat.Add("Kategori:Geologi i Sverige");
        donecat.Add("Kategori:Gruvor i Sverige");

        //donecat.Add("Kategori:Finland");
        donecat.Add("Kategori:Finlands historia");
        donecat.Add("Kategori:Sverige");
        

        //Console.WriteLine("Categories to check : " + pl.Count().ToString());

        //int iremain = pl.Count();

        using (StreamWriter sw = new StreamWriter("coord-"+makelang+".txt"))
        {
            do_category(site, "Geografi", sw, "");

        }

        Console.WriteLine("Total # edits = " + nedit.ToString());
        
        

	}



}
