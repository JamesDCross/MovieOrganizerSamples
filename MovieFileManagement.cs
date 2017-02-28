using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using MO.Models; //had to provide this namespace to use the results class
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Windows.Storage;

namespace MO
{

    class MovieFileManagement
    {
        
        public MovieFileManagement(ObservableCollection<StorageFile> mfl)
        {
            movieFileList = mfl;
        }

        public Dictionary<Match, string> results = new Dictionary<Match, string>();
        public List<KeyValuePair<string, string>> titleYear = new List<KeyValuePair<string, string>>();

        public ObservableCollection<StorageFile> movieFileList;

        public List<string> baseTitles = new List<string>();
        public List<string> noMatch = new List<string>();

        public string title = "";
        public string year = "";

        //created a "main method" to keep things tidy
        public void movieFileMain()
        {
            getMovieTitles();
            cleanList();
            regex();
        }

        /* Add the raw filenames from the users movie folder
         * into baseTitles 
         */
        public void getMovieTitles()
        {
            if (movieFileList.Count != 0)
            {
                foreach (var item in movieFileList)
                {
                    baseTitles.Add(item.Name);
                    Debug.WriteLine(item.Name);
                }
            }
        }

        /* Clean the filenames in baseTitles
          * 
          */
        public void cleanList()
        {
            for (int i = 0; i < baseTitles.Count; i++)
            {
                baseTitles[i] = baseTitles[i]
                   .Replace('.', ' ')
                   .Replace('_', ' ')
                   .Replace('-', ' ')
                   .Replace(':', ' ')
                   .Replace('(', ' ')
                   .Replace(')', ' ')
                   .Replace('{', ' ')
                   .Replace('}', ' ')
                   .Replace('[', ' ')
                   .Replace(']', ' ')
                   .Replace(',', ' ')
                   .Replace('\'', ' ');
                //Debug.WriteLine("base titles: " + baseTitles[i]);

            }

            //Check for doubles to eliminate sample videos
            for (int i = 0; i < baseTitles.Count; i++)
            {
                if (baseTitles[i].Contains("sample") || baseTitles[i].Contains("Sample"))
                {
                    baseTitles.RemoveAt(i);
                }
            }

            //print out the final cleaned list to debug
            for (int i = 0; i < baseTitles.Count; i++)
            {
                Debug.WriteLine("base titles: " + baseTitles[i]);
            }


        }


        public void regex()
        {
            /*Regex
             *
             * Group 1 = title and year
             * Group 2 = year
             * This gives an index in the string for every instance of the title and year seperately and the title and year combined.
             * if there are multiple years or the movie title is 2001 it will still find all the correct titles.
            */

           /*removed regex key for example*/
            string keys1 = @"";
          
            for (int i = 0; i < baseTitles.Count; i++)
            {
                MatchCollection matches = Regex.Matches(baseTitles[i], keys1);
 
                if (matches.Count == 0)
                {
                    //non-matched titles will be dealt with in a seperate list
                    noMatch.Add(baseTitles[i]);
                }

                foreach (Match match in matches)
                {   
                    results.Add(match, baseTitles[i]);
                }

            }


            //this dictionary needs to be eliminated eg. match.toString() could just go straight into titleYear, there is an extra data structure

            foreach (KeyValuePair<Match, string> kvp in results)
            {
                
                    title = kvp.Value.Substring(0, kvp.Key.Index);
                    
              
                year = kvp.Key.Value;

                Debug.WriteLine("title " + title + " " + "year " + year);
                titleYear.Add(new KeyValuePair<string, string>(title, year));
            }

        }

    }
}
