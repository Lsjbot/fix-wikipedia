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


    public static void Main()
	{
        Console.Write("Password: ");
        string password = Console.ReadLine();
        string botkonto = "Lsjbot";
        string makelang = "sv";
        Site site = new Site("https://"+makelang+".wikipedia.org", botkonto, password);
        
        

	}



}
