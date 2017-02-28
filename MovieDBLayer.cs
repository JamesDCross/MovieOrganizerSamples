using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using MO.Models; 
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;



/*
An ObservableCollection works essentially like a regular collection except that it implements these interfaces:

INotifyCollectionChanged
INotifyPropertyChanged

    --so it monitors its activity

*/



namespace MO
{
    public class MovieDBLayer
    {
        public static List<KeyValuePair<string, string>> titleYear = new List<KeyValuePair<string, string>>();

        //initializes titleYear passed from MovieFileManagement in this class
        public static void getList(List<KeyValuePair<string, string>> input)
        {
            foreach (KeyValuePair<string, string> kvp in input)
            {
                titleYear.Add(kvp);
            }
        }

      /* Acquire the poster images from MovieDB
       */
        public static async Task Populate(ObservableCollection<Movie> movies)
        {
            MovieRoot gmr = new MovieRoot();
            //iterate through titleYear List and make calls to the api
            foreach (KeyValuePair<string, string> kvp in titleYear)
            {
                //need to put a case in here saying if title = "" the make year the title//just searched for year so all "good"
                if (kvp.Key != "") {
                    gmr = await GetMovieRoot(kvp.Key, kvp.Value);
                }
                //this is getting the json out of the movie object (a list of type movie that holds bools strings etc)
                List<Movie> moviesList = gmr.results;


                foreach (Movie i in moviesList)
                {
                    movies.Add(i);

                    if (i != null)
                    {
                        i.poster_path = "https://image.tmdb.org/t/p/w300" + i.poster_path;
                        break;
                    }
                }
            }
        }


        //return a task of type results
        private static async Task<MovieRoot> GetMovieRoot(string movieTitle, string year)
        {
            //removed my key for example

            //assemble url
            string url = String.Format("https://api.themoviedb.org/3/search/movie?api_key=11111111111111111111&language=en-US&query={0}&page=1&include_adult=false&year={1}", movieTitle, year);

            //call out to moviedb
            //limit api requests to 40 calls per 10 seconds
            //250ms per request

            await Task.Delay(250);//delay 250ms per request so program does not exceed the limit(this is a temporary solution, I will use a Nuget Package to throttle later)
            //also there is the issue of multiple users making requests which means they will have to be cached on a server somewhere.

            HttpClient http = new HttpClient();

            //gets a response of type http response msg
            var response = await http.GetAsync(url);

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
