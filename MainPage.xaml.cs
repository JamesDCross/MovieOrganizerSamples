using MO.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Movie> movies { get; set; }

        //main page initializer
        public MainPage()
        {
            this.InitializeComponent();
            movies = new ObservableCollection<Movie>();
        }

        
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            /*Debug.WriteLine("poster path " + movies[0].poster_path);
            Debug.WriteLine("pop " + movies[1].popularity);
            Debug.WriteLine("poster path2 " + movies[2].poster_path);
            Debug.WriteLine("count " + movies.Count);*/
            // Debug.WriteLine(movies.ToString());
        }

        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //e returns a generic object so we have to cast it back to type movie
            var selectedMovie = (Movie)e.ClickedItem;
            DetailDescriptionTextBlock.Text = selectedMovie.overview;
        }


        public ObservableCollection<StorageFile> movieFiles = new ObservableCollection<StorageFile>();

        /*
         Get the file titles out of the users movie folder (they choose)
         and enter the titles as queries to the MovieDB api.
         */

        private async void FolderAccess_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add("*"); //match all the file formats

            StorageFolder folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {
                await RetrieveFilesInFolders(movieFiles, folder);
                //Debug.WriteLine(movieFiles.Count);

                MovieFileManagement MovieFileManage = new MovieFileManagement(movieFiles);
                MovieFileManage.movieFileMain();
                MovieDBLayer.getList(MovieFileManage.titleYear);

               // Debug.WriteLine("test" + m.titleYear.Count);
                await MovieDBLayer.Populate(movies);

            }
        }

        /*
          string[] extensions = { ".mp4", ".avi", ".mkv", ".m4v", ".mov" }; --try to incorporate later is good for reuse or get content type going
          if(item.ContentType == "Video") look into this   
        */


        //Recursively look through all sub-folders when acquiring movie titles
        private async Task RetrieveFilesInFolders(ObservableCollection<StorageFile> list, StorageFolder parent)
        {
            foreach (var item in await parent.GetFilesAsync())
            {
                
               if (item.FileType == ".mp4" || item.FileType == ".avi" || item.FileType == ".mkv" || item.FileType == ".m4v" || item.FileType == ".mov")
                {
                    list.Add(item);
                }
            }

            foreach (var item in await parent.GetFoldersAsync())
            {
                await RetrieveFilesInFolders(list, item);
            }


        }
    }
}
