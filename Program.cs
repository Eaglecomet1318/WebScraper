/* NAME: Venkata Ponakala
 * PROGRAM NAME: WebScrapper1
 * PROGRAM PURPOSE: To extract data from an online site in a manipulable form.
 * 
 * PROGRAM FAILURES: An exception of type 'System.Net.Http.HttpRequestException' 
 *                   occurred in mscorlib.dll but was not handled in user code
 * 
*/


/* With the collaboration of Kolbe Surran and Muhammad Salman */
/* Also online help of BlakeB's videos: https://www.youtube.com/watch?v=B4x4pnLYMWI & 
 * https://www.youtube.com/watch?v=BE708X6r24o */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using System.Net.Http;
using System.Collections;

namespace WebScrapper1
{
    class Program
    {

        static void Main(string[] args)
        {
            GetHtmlAsync();     // Webscraper function implementaion
            Console.ReadLine(); // Read in and  display information

        }

        /* Archetypical webscraper function*/
        static async void GetHtmlAsync()
        {
            var url = "https://www.apartments.com/richardson-tx-75080/"; 

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url); // Main problem here. similar implemenation
                                                             // works elsewhere, however. 
            var htmlDoc = new HtmlDocument();

            /*
             * Name: var apartmentListing
             * Purpose: Gets list of unordered apartments
             * Actions:
             *              .Descendants("something") -> html tag
             *              .GetAttributeValue("something", ) -> type of variable
             *              .Equals("something") -> name of variable
             * Possible problems: N/A    
            */
            var apartmentListing = htmlDoc.DocumentNode.Descendants("div")
                               .Where(node => node.GetAttributeValue("id", "") 
                               .Equals("placardContainer")).ToList(); 

            /*
             * Name: var apartments
             * Purpose: Gets apartments in list, starts at index 0
             * Actions:
             *              .Descendants("something") -> html tag
             *              .GetAttributeValue("something", ) -> type of variable
             *              .Equals("something") -> name of variable
             *              
             * Possible problems: N/A             
            */
            var apartments = apartmentListing[0].Descendants("article")
                                .Where(node => node.GetAttributeValue("data-url", "")
                                .Contains("www.apartments.com")).ToList();


            /*
             * Name: var apartments
             * Purpose: Gets last element in list, starts at index 0
             * Actions:
             *              .Descendants("something") -> html tag
             *              .GetAttributeValue("something", ) -> type of variable
             *              .Equals("something") -> name of variable
             *              
             * Possible problems: N/A
            */
            var nextPage = apartmentListing[0].Descendants("a")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("next ")).ToList();

            /* Goes to next page*/
            string nextPageUrl = "";
            if (nextPage.Count() == 0)
            {
                nextPage = null;
            }
            else
            {
                nextPageUrl = nextPage[0].Attributes["href"].Value;
            }

            /* Creation of ArrayList necessary to avoid the mishmashing of the apartment variable. */
            ArrayList urls = new ArrayList();

            for (int i = 0; i < apartments.Count(); i++)
            {
                var apartmentUrl = apartments[i].Descendants("a")
                                    .Where(node => node.GetAttributeValue("href", "")
                                    .Contains("www.apartments.com")).ToList();
                urls.Add(apartmentUrl[0].GetAttributeValue("href", ""));
            }


            /*
             * Name: for loop apartments apartments.Count
             * Purpose: Allows access to each apartment and go into each apartment webpage
             * Actions & Variables:
             *              var aP -> Obtains apartment page link.
             *              if(urls[i]!=null) -> Actually goes into apartment webpage and reads info
             *              
             * Possible problems: N/A
            */
            for (int i = apartments.Count() - 2; i < apartments.Count(); i++)
            {
                var aP = urls[i].ToString();
                if (urls[i] != null)
                {
                    var urlNew = aP;
                    var htmlNew = await httpClient.GetStringAsync(urlNew);
                    htmlDoc.LoadHtml(htmlNew);

                    var apartmentTable = htmlDoc.DocumentNode.Descendants("div")
                                         .Where(node => node.GetAttributeValue("class", "")
                                         .Equals("tabContent active")).ToList();

                    if (apartmentTable.Count() == 0) 
                    {
                        apartmentTable = htmlDoc.DocumentNode.Descendants("table")
                                         .Where(node => node.GetAttributeValue("class", "")
                                         .Equals("availabilityTable basic oneRental")).ToList();
                    }
                    if (apartmentTable.Count() == 0)
                    {
                        apartmentTable = htmlDoc.DocumentNode.Descendants("table")
                                         .Where(node => node.GetAttributeValue("class", "")
                                         .Equals("availabilityTable tiertwo")).ToList();
                    }

                    var apartmentModelList = apartmentTable[0].Descendants("tr")
                                         .Where(node => node.GetAttributeValue("class", "")
                                         .Contains("rentalGridRow")).ToList();

                    Console.WriteLine("Models of the Apartment :");
                    /*
                     * Name: for each (var model in apartmentModelList)
                     * Purpose: Gets all the properties of the apartment (# bedrooms, # baths, how much rent, etc.)
                     * Possible problems:
                     *                      1. Site may detect the bot and interfere in output
                    */
            foreach (var version in apartmentModelList)
                    {
                        var bedrooms = (version.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Contains("beds")).ToList())[0].Descendants("span")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("shortText")).ToList();

                        var baths = (version.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Contains("baths")).ToList())[0].Descendants("span")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("shortText")).ToList();

                        var rent = version.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Contains("rent")).ToList();

                        var deposit = version.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Contains("deposit")).ToList();

                        var modelName = version.Descendants("td")
                           .Where(node => node.GetAttributeValue("class", "")
                           .Contains("name")).ToList(); 

                        var area = version.Descendants("td")
                           .Where(node => node.GetAttributeValue("class", "")
                           .Contains("sqft")).ToList();

                        var availability = version.Descendants("td")
                           .Where(node => node.GetAttributeValue("class", "")
                           .Contains("availabile")).ToList();

                        /*Displays read information above*/
                        if (modelName[0] != null && modelName.Count() != 0) 
                            Console.WriteLine("ModelName: " + modelName[0].InnerText.Trim());
                        if (bedrooms[0] != null)
                            Console.WriteLine("Bedroom: " + bedrooms[0].InnerText.Trim());
                        if (baths[0] != null)
                            Console.WriteLine("Bathroom: " + baths[0].InnerText.Trim());
                        if (rent[0] != null)
                            Console.WriteLine("Rent: " + rent[0].InnerText.Trim());
                        if (deposit[0] != null)
                            Console.WriteLine("Deposit: " + deposit[0].InnerText.Trim());
                        if (area[0] != null)
                            Console.WriteLine("Area: " + area[0].InnerText.Trim());

                        Console.WriteLine();

                    }
                    Console.WriteLine("Page Finish");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            /*Goes to next page*/
            Console.WriteLine(nextPageUrl);
            url = nextPageUrl;
            if (url == null)
            {
                Console.WriteLine("Try again.");
            }

            return;
        }
    }
}