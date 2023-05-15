using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;
using MovieLibraryOO.Migrations;
using System.Data.Common;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace MyNamespace
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // MovieLibraryOO has examples of each of these if you need it.

            //Creating main menu options & Reading user input
            Console.WriteLine("Please select your menu option.");
            Console.WriteLine("It may take some time to properly load.");
            Console.WriteLine("1) Search for a movie.");
            Console.WriteLine("2) Add a movie.");
            Console.WriteLine("3) Update a movie.");
            Console.WriteLine("4) Delete a movie.");

            Console.WriteLine("\tFor the letter grades:");
            Console.WriteLine("5) Add New User.");
            //Console.WriteLine("6) Enter new movie Rating.");
            

            Console.WriteLine("\tPress any other key to exit...");
            var menuOption = Console.ReadLine();
            
            //Creating the various options that the user can choose.
            if (menuOption == "1")
            {
                // Searching for a movie.
                Console.WriteLine("Search for a Movie!");

                using (var db = new MovieContext())
                {
                    Console.WriteLine("What type of search would you like to preform?");
                    Console.WriteLine("1) Specific title / Single search");
                    Console.WriteLine("2) Show ALL MOVIES");
                    Console.WriteLine("");
                    var searchType = Console.ReadLine();

                    if (searchType == "1")
                    {
                        Console.WriteLine("You have picked: Single or Specific Title search:");

                        // oops misspelled movies...
                        Console.WriteLine("Please enter a search word for the movie title you are looking for");
                        var searchWord = Console.ReadLine();

                        //Make sure the input is not null or empty before going on.
                        while (searchWord.IsNullOrEmpty())
                        {
                            Console.WriteLine("Cannot be empty, please enter a word for the title.");
                            searchWord = Console.ReadLine();

                        }



                        var moveies = db.Movies
                            .FirstOrDefault(mov => mov.Title.Contains(searchWord));

                        Console.WriteLine($"Movie: {moveies.Title} {moveies.ReleaseDate: MM-dd-yyy}");
                        Console.WriteLine("Genres: ");
                        foreach (var genre in moveies.MovieGenres ?? new List<MovieGenre>())
                        {
                            Console.WriteLine($"\t{genre.Genre.Name}");
                        }

                    }
                    else if (searchType == "2")
                    {
                        /*
                        Console.WriteLine("Please enter a search word for the movie titles you are looking for");
                        var searchWord = Console.ReadLine();

                        //Make sure the input is not null or empty before going on.
                        while (searchWord.IsNullOrEmpty())
                        {
                            Console.WriteLine("Cannot be empty, please enter a word for the titles.");
                            searchWord = Console.ReadLine();

                        }
                        */

                        Console.WriteLine("This displays ALL MOVIES");
                        Console.WriteLine("Are you sure you want to do this? (Y/n)");
                        var seeAll = Console.ReadLine();

                        if (seeAll == "Y")
                        {
                            var moveies = db.Movies
                            .Include(x => x.MovieGenres)
                            .ThenInclude(x => x.Genre);

                            Console.WriteLine("The Movies are: ");
                            foreach (var mov in moveies)
                            {

                                Console.WriteLine($"Movie Title: {mov.Title} {mov.ReleaseDate:MM-dd-yyyy}  ");
                                Console.WriteLine($"Genres: ");
                                foreach (var genre in mov.MovieGenres ?? new List<MovieGenre>())
                                {
                                    Console.WriteLine($"\t{genre.Genre.Name}");
                                }
                                Console.WriteLine();

                            }
                        }
                        else
                        {
                            Console.WriteLine("Cancelled displaying all movies");
                        }

                        
                        
                    }
                    else
                    {
                        Console.WriteLine("You found an easter egg!");
                        Console.WriteLine("I didn't add anything to make sure you only chose one of those two choices.");
                    }
                   
                    
                    


                    /*
                    //Use something like this for exact movie match? 
                    //var mov = db.Movies.FirstOrDefault(x => x.Title == movieUpdate);
                    var limitedMovies = moveies.Take(10);

                    Console.WriteLine("The Movies are: ");
                    foreach (var mov in moveies.Take(10).ToList())
                    {
                        Console.WriteLine($"Movie Title: {mov.Title}  ");
                        
                    }
                    */

                }

            }
            else if (menuOption == "2")
            {
                //Adding a new movie to the database. 

                Console.WriteLine("Adding a Movie to the database.");

                Console.WriteLine("Enter a movie Title");
                var movie = Console.ReadLine();
                Console.WriteLine("Enter a movie Release Date");
                Console.WriteLine(" MM / DD / YYYY - Use this format");

                var releaseDate = DateTime.Parse(Console.ReadLine());

                using (var db = new MovieContext())
                {
                    var mov = new Movie();
                    mov.Title = movie;
                    mov.ReleaseDate = releaseDate;

                    //Add in Genres here for the final

                    db.Movies.Add(mov);
                    db.SaveChanges();
                    Console.WriteLine($"Created {mov.Id} {mov.Title} {mov.ReleaseDate}!");
                }

            }
            else if (menuOption == "3")
            {
                // Updating the movie
                Console.WriteLine("Updating a movie title in the database.");

                Console.WriteLine("Enter the title of the movie you wish to update.");
                var movieToUpdate = Console.ReadLine();

                Console.WriteLine("Enter the updated name of the movie.");
                var movieUpdated = Console.ReadLine();

                // To expand on this, Add release date functionality to the update.

                using (var db = new MovieContext())
                {
                    var updateMovie = db.Movies.FirstOrDefault(x => x.Title == movieToUpdate);
                    Console.WriteLine($"({updateMovie.Id}) {updateMovie.Title}");
                    Console.WriteLine($"Changed to:({updateMovie.Id}) {movieUpdated}");

                    updateMovie.Title = movieUpdated;

                    db.Movies.Update(updateMovie);
                    db.SaveChanges();
                }

            }
            else if (menuOption == "4")
            {
                Console.WriteLine("Deleting a movie entry in the database.");

                Console.WriteLine("WARNING: THIS CANNOT BE UNDONE DO YOU WISH TO CONTINUE? (Y/n) - This is case sensitive.");
                var confirmDelete = Console.ReadLine();
                if (confirmDelete == "Y")
                {
                    Console.WriteLine("Continuing with deletion process, proceed with caution.");
                    Console.WriteLine("");
                    Console.WriteLine("Please enter the title of the movie you wish to delete.");
                    var movieToDelete = Console.ReadLine();

                    using (var db = new MovieContext())
                    {
                        var deleteMovie = db.Movies.FirstOrDefault(x => x.Title == movieToDelete);
                        Console.WriteLine($"({deleteMovie.Id}) {deleteMovie.Title}  : Has been permanantly removed.");


                        db.Movies.Remove(deleteMovie);
                        db.SaveChanges();
                    }

                }
                else if (confirmDelete != "Y")
                {
                    Console.WriteLine("You have stopped the deletion process. Thank you for your caution.");
                }


                


            }
            else if (menuOption == "5")
            {
                Console.WriteLine("Adding new user.");
                Console.WriteLine();

                Console.WriteLine("Please enter the user AGE");
                var userCreateAge = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Please enter the user GENDER");
                var userCreateGender = Console.ReadLine();
                Console.WriteLine("Please enter the user ZipCode");
                var userCreateZipCode = Console.ReadLine();
                Console.WriteLine("Please enter the user Occupation ID");
                var userCreateOccupation = Convert.ToInt32(Console.ReadLine());


                //Add new user, including Occupation
                
                using (var db = new MovieContext())
                {

                    User newUser = new User()
                    {
                        Age = userCreateAge,
                        Gender = userCreateGender,
                        ZipCode = userCreateZipCode,
                        
                    };

                    

                    
                    
                    //newUser.Occupation.Name = userCreateOccupation;
                    

                    db.Users.Add(newUser);
                    db.SaveChanges();
                    Console.WriteLine("Created user:");
                    Console.WriteLine($"ID: {newUser.Id} Age: {newUser.Age} Gender: {newUser.Gender} ZipCode: {newUser.ZipCode} Occupation: {newUser.Occupation.Name}");
                    
                }


                //Display the details of the added user.


            }
            /*else if (menuOption == "6")
            {
                Console.WriteLine("Entering a new rating for a movie.");

                using (var db = new MovieContext())
                {
                    //Give a list of users to choose from. Include ID
                    Console.WriteLine("Would you like to see a list of users: (Y)");
                    Console.WriteLine("Or continue by entering a user ID: (n)");
                    var userListChoice = Console.ReadLine();

                    if (userListChoice == "Y")
                    {
                        Console.WriteLine("Displaying List of Users:");



                        //Copy Paste all of the below stuff in here, I forgot how to switch over... whatever lol.




                    }
                    else if (userListChoice == "n")
                    {
                        //Ask which user is entering the rating - Pick ID
                        Console.WriteLine("Please enter a user ID:");
                        var userPickID = Console.ReadLine();

                        //Ask for user to enter a rating on an existing movie.


                        //Display the details of the user, rated movie, and rating.


                    }

                    
                }

            }*/
            else if (menuOption != "1" && menuOption != "2" && menuOption != "3" && menuOption != "4" && menuOption != "5" && menuOption != "6")
            {
                // Could honestly just do an else instead...
                Console.WriteLine("Exiting Program...");
            }


        }
    }
}