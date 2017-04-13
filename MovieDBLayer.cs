using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using MO.Models;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace MO
{
    public class MovieDBLayer
    {
        public static List<KeyValuePair<string, string>> titleYear = new List<KeyValuePair<string, string>>();

        //not in regex
        public static List<string> noMatchList = new List<string>();
        //not in regex or no match
        public static List<string> notFound = new List<string>();

        //regex matches
        //initializes titleYear passed from MovieFileManagement in this class
        public static void getList(List<KeyValuePair<string, string>> input)
        {
            foreach (KeyValuePair<string, string> kvp in input)
            {
                titleYear.Add(kvp);
            }

        }

        //everthing that didnt match the regex
        public static void getNoMatchList(List<string> input)
        {
            foreach (string title in input)
            {
                noMatchList.Add(title);
            }
        }

        private static void AddToMoviesList(List<Movie> moviesList, ObservableCollection<Movie> movies) {
            foreach (Movie i in moviesList)
            {
                
                movies.Add(i);

                if (i != null)
                {
                    Debug.WriteLine(i.poster_path);

                    i.poster_path = "https://image.tmdb.org/t/p/w500" + i.poster_path;
                    if (i.poster_path != "https://image.tmdb.org/t/p/w500")
                    {
                        FileIO.getPic(i.poster_path, i.title);
                    }
                    i.MoviePoster = i.poster_path;//for the xaml first time
                    break;
                }
            }
        }

        private static void CleanNotFoundList() {
            //will do st's later - this is not that common
            //remove anniversary editions from title
            string[] anniversary = new string[20];
            for (int anns = 1; anns < anniversary.Length; anns++)
            {
                anniversary[anns] = anns * 5 + "th";
            }

            for (int i = 0; i < notFound.Count; i++)
            {
                for (int anns = 1; anns < anniversary.Length; anns++)
                {
                    if (notFound[i].IndexOf(anniversary[anns]) != -1)
                    {
                        notFound[i] = notFound[i].Substring(0, notFound[i].IndexOf(anniversary[anns]) - 1);
                    }
                }
            }

            string[] endersNF = { "unrated", "extended", "bluray", "anniversary", "extended cut" };
            for (int i = 0; i < notFound.Count; i++)
            {
                //change all to lower case
                notFound[i] = notFound[i].ToLower();
                //check against the title ending conditions
                for (int e = 0; e < endersNF.Length; e++)
                {
                    if (noMatchList[i].IndexOf(endersNF[e]) != -1)
                    {
                        noMatchList[i] = noMatchList[i].Substring(0, noMatchList[i].IndexOf(endersNF[e]) - 1);
                    }
                }

                //clean off file types and misc
                notFound[i] = notFound[i]
                              .Replace("unrated", "")//earlier than here?
                              .Replace("extended cut", "");
                Debug.WriteLine("cleaned not found: " + notFound[i]);
            }
        }

        public static bool internetFail = false;

        /* Acquire the poster images from MovieDB
         */
        public static async Task Populate(ObservableCollection<Movie> movies)
        {

            MovieRoot gmr = new MovieRoot();
            //iterate through titleYear List and make calls to the api
            try
            {
                //---------------------------------------------------------------------------------------------------------
                //regex movies with year
                foreach (KeyValuePair<string, string> kvp in titleYear)
                {  
                    if (kvp.Key != "")
                    {
                        gmr = await GetMovieRoot(kvp.Key, kvp.Value);

                        if (gmr.total_results == 0)
                        {
                            notFound.Add(kvp.Key);
                            Debug.WriteLine("not found: " + kvp.Key);
                        }
                    }

                    //this is getting the json out of the movie object (a list of type movie that holds bools strings etc)
                    List<Movie> regexMoviesList = gmr.results;

                    AddToMoviesList(regexMoviesList, movies);
                }
            }

            catch (Exception)
            {
                internetFail = true;
                return;
            }



            try
            {
                //---------------------------------------------------------------------------------------------------------
                //titles that didnt match the regex (noMatch)
                foreach (string title in noMatchList)
                {
                    if (title != "")
                    {
                        gmr = await GetMovieRoot(title, "0");

                        if (gmr.total_results == 0)
                        {
                            notFound.Add(title);
                            Debug.WriteLine("not found: " + title);
                        }
                    }
                    //this is getting the json out of the movie object (a list of type movie that holds bools strings etc)
                    List<Movie> noMatchMoviesList = gmr.results;
                    AddToMoviesList(noMatchMoviesList, movies);
                }
            }
            catch (Exception)
            {
                internetFail = true;
                return;
            }

            try
            {
                //---------------------------------------------------------------------------------------------------------
                //notFound - titles that got no repsonse from the api
                CleanNotFoundList();

                foreach (string nf in notFound)
                {
                    if (nf != "")
                    {
                        gmr = await GetMovieRoot(nf, "0");
                    }
                    //this is getting the json out of the movie object (a list of type movie that holds bools strings etc)
                    List<Movie> notFoundMoviesList = gmr.results;
                    AddToMoviesList(notFoundMoviesList, movies);
                }
            }
            catch (Exception)
            {
                internetFail = true;
                return;
            }

        }

        //join get movie roots into the same method
        //return a task of type results
        private static async Task<MovieRoot> GetMovieRoot(string movieTitle, string year)
        {
            string url;

            if (year == "0") {
             url = String.Format("https://api.themoviedb.org/3/search/movie?api_key=11111111111111111111&language=en-US&query={0}&page=1&include_adult=false", movieTitle);
            }else {
             url = String.Format("https://api.themoviedb.org/3/search/movie?api_key=11111111111111111111&language=en-US&query={0}&page=1&include_adult=false&year={1}", movieTitle, year);
            }
            /*call out to moviedb
            //limit api requests to 40 calls per 10 seconds
            //250ms per request*/

            await Task.Delay(250);
            /*delay 250ms per request so program does not exceed the limit(this is a temporary solution, 
            I will use a Nuget Package to throttle later) also there is the issue of multiple users 
            making requests which means they will have to be cached on a server somewhere.*/

           HttpClient http = new HttpClient();
           HttpResponseMessage response = null;

            try
           {
                //gets a response of type http response msg
               response = await http.GetAsync(url);
               response.EnsureSuccessStatusCode();
           }
           catch (HttpRequestException)
            {
               internetFail = true;
               MovieRoot temp = new MovieRoot();
               temp.total_results = 0;
               return temp;
           }
            
           Debug.WriteLine("code" + (int)response.StatusCode);
         
            //takes the http response and converts it to string
            var json = await response.Content.ReadAsStringAsync();

            //response -> string/ json -> deserilaize
            var serializer = new DataContractJsonSerializer(typeof(MovieRoot));
            /*buffer to hold the api info as it comes in and then hand it to serializer
            which gives the info as an object graph beginning at the root with results*/

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            //serializer reads the memory stream object and is cast to a results object

            var result = (MovieRoot)serializer.ReadObject(ms);
            return result;

        }
    }
}
