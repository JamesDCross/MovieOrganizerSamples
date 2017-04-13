using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO.Models
{
    class MovieSearch
    {
        /*the moviedb genres--the data structure is not being used but is a good guide*/
        /*Dictionary<int, string> genres = new Dictionary<int, string> {
   { 28, "Action" },
   { 12, "Adventure" },
   { 16, "Animation" },
   { 35, "Comedy" },
   { 80, "Crime" },
   { 99, "Documentary" },
   { 18, "Drama" },
   { 10751,"Family" },
   { 14,"Fantasy" },
   { 36, "History" },
   { 27, "Horror" },
   { 10402, "Music" },
   { 9648, "Mystery" },
   { 10749, "Romance" },
   { 878, "Science Fiction" },
   { 10770, "TV Movie" },
   { 53, "Thriller" },
   { 10752, "War" },
   { 37, "Western" },  };*/


        //save the initial collection as a base for manipulation for display
        public static ObservableCollection<Movie> MovieCollection = new ObservableCollection<Movie>();

        public static void SaveMovieCollectionAll(ObservableCollection<Movie> moviesDisplay)
        {
            //Distinct accounts for duplicates
            MovieCollection = new ObservableCollection<Movie>(moviesDisplay.Distinct());

            foreach (Movie m in MovieCollection)
            {
                Debug.WriteLine("mc" + m.title);
            }
        }

        public static void ResetMovieCollection(ObservableCollection<Movie> moviesDisplay)
        {
            moviesDisplay.Clear();
            foreach (Movie m in MovieCollection)
            {
                moviesDisplay.Add(m);
            }
        }

        //takes in movie collection and makes a new list based on id
        public static void GetMoviesByCategory(ObservableCollection<Movie> moviesDisplay, int genre)
        {
            ResetMovieCollection(moviesDisplay);
            //all
            if (genre == -1)
            {
                ResetMovieCollection(moviesDisplay);
            }
            //action
            if (genre == 28)
            {
                List<Movie> actionMovies = new List<Movie>();
                //var movieListAction = movies;
                foreach (Movie m in moviesDisplay)
                {
                    if (m.genre_ids.Contains(28) && (!m.genre_ids.Contains(16)
                        && !m.genre_ids.Contains(878) && !m.genre_ids.Contains(14) && !m.genre_ids.Contains(18)))//18 maybe to strict
                    {
                        actionMovies.Add(m);
                    }
                }

                moviesDisplay.Clear();
                actionMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //animation
            if (genre == 16)
            {
                var animationMovies = moviesDisplay.Where(m => m.genre_ids.Contains(genre)).ToList();
                moviesDisplay.Clear();
                animationMovies.ForEach(m => moviesDisplay.Add(m));
            }
            //comedy
            if (genre == 35)
            {
                List<Movie> comedyMovies = new List<Movie>();
                //var movieListAction = movies;
                foreach (Movie m in moviesDisplay)
                {
                    if (m.genre_ids.Contains(35) && !m.genre_ids.Contains(10751) && !m.genre_ids.Contains(878)
                        && !m.genre_ids.Contains(16) && !m.genre_ids.Contains(28) && !m.genre_ids.Contains(14)
                        && !m.genre_ids.Contains(18) && !m.genre_ids.Contains(12))//28 to strict
                    {
                        comedyMovies.Add(m);
                    }
                }
                moviesDisplay.Clear();
                comedyMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //documentary
            if (genre == 99)
            {
                var documentaryMovies = moviesDisplay.Where(m => m.genre_ids.Contains(genre)).ToList();
                moviesDisplay.Clear();
                documentaryMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //horror
            if (genre == 27)
            {
                List<Movie> horrorMovies = new List<Movie>();
                //var movieListAction = movies;
                foreach (Movie m in moviesDisplay)
                {
                    if (m.genre_ids.Contains(27) && !m.genre_ids.Contains(16) && !m.genre_ids.Contains(35) && !m.genre_ids.Contains(878))
                    {
                        horrorMovies.Add(m);
                    }
                }
                moviesDisplay.Clear();
                horrorMovies.ForEach(m => moviesDisplay.Add(m));
            }
            //romance
            if (genre == 10749)
            {
                List<Movie> romanceMovies = new List<Movie>();
                //var movieListAction = movies;
                foreach (Movie m in moviesDisplay)
                {
                    //&& !m.genre_ids.Contains(35) removed because of romantic comedies
                    if (m.genre_ids.Contains(10749) && (!m.genre_ids.Contains(16) && !m.genre_ids.Contains(878) && !m.genre_ids.Contains(878)))
                    {
                        romanceMovies.Add(m);
                    }
                }
                moviesDisplay.Clear();
                romanceMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //drama
            if (genre == 18)
            {
                List<Movie> dramaMovies = new List<Movie>();
                foreach (Movie m in moviesDisplay)
                {
                    if ((m.genre_ids.Contains(18) || m.genre_ids.Contains(9648) || m.genre_ids.Contains(80)) &&
                        (!m.genre_ids.Contains(35) && !m.genre_ids.Contains(878) && !m.genre_ids.Contains(53)
                        && !m.genre_ids.Contains(16) && !m.genre_ids.Contains(10752) && !m.genre_ids.Contains(28)
                        && !m.genre_ids.Contains(27)))
                    {
                        dramaMovies.Add(m);
                    }
                }
                moviesDisplay.Clear();
                dramaMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //war
            if (genre == 10752)
            {
                List<Movie> warMovies = new List<Movie>();
                //var movieListAction = movies;
                foreach (Movie m in moviesDisplay)
                {
                    if (m.genre_ids.Contains(10752) && !m.genre_ids.Contains(16) && !m.genre_ids.Contains(35) && !m.genre_ids.Contains(878))
                    {
                        warMovies.Add(m);
                    }
                }
                moviesDisplay.Clear();
                warMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //fantasy
            if (genre == 14)
            {
                List<Movie> fantasyMovies = new List<Movie>();
                //var movieListAction = movies;
                foreach (Movie m in moviesDisplay)
                {
                    //special case for willow lol(movie db didnt give it a fantasy tag)
                    if (m.original_title == "Willow")
                    {
                        fantasyMovies.Add(m);
                    }

                    if (m.genre_ids.Contains(14) && !m.genre_ids.Contains(16) && !m.genre_ids.Contains(35) && !m.genre_ids.Contains(878))
                    {
                        fantasyMovies.Add(m);
                    }
                }
                moviesDisplay.Clear();
                fantasyMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //music/musical
            if (genre == 10402)
            {
                var warMovies = moviesDisplay.Where(m => m.genre_ids.Contains(genre)).ToList();
                moviesDisplay.Clear();
                warMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //scifi
            if (genre == 878)
            {
                List<Movie> scifiMovies = new List<Movie>();
                //var movieListAction = movies;
                foreach (Movie m in moviesDisplay)
                {
                    if (m.genre_ids.Contains(878) && !m.genre_ids.Contains(16))
                    {
                        scifiMovies.Add(m);
                    }
                }
                moviesDisplay.Clear();
                scifiMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //western
            if (genre == 37)
            {
                var westernMovies = moviesDisplay.Where(m => m.genre_ids.Contains(genre)).ToList();
                moviesDisplay.Clear();
                westernMovies.ForEach(m => moviesDisplay.Add(m));
            }

            /* --took out for now
            //Thriller
            if (genre == 53)
            {
                List<Movie> thrillerMovies = new List<Movie>();
                //var movieListAction = movies;
                foreach (Movie m in moviesDisplay)
                {
                    if (m.genre_ids.Contains(53) && !m.genre_ids.Contains(16) && !m.genre_ids.Contains(35) && !m.genre_ids.Contains(14) && !m.genre_ids.Contains(878))
                    {
                        thrillerMovies.Add(m);
                    }
                }
                moviesDisplay.Clear();
                thrillerMovies.ForEach(m => moviesDisplay.Add(m));
            }

            //adventure
            if (genre == 12)
            {
                var adventureMovies = moviesDisplay.Where(m => m.genre_ids.Contains(genre)).ToList();
                moviesDisplay.Clear();
                adventureMovies.ForEach(m => moviesDisplay.Add(m));
            }
            */
        }
    }
}
