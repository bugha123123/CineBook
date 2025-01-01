using CineBook.ApplicationDbContext;
using CineBook.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

public class DatabaseSeeder
{
    private readonly AppDbContextion _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey = "ba78b82d72bb7ddb477af4a06fd9a3f7"; // Use your TMDb API Key

    public DatabaseSeeder(AppDbContextion context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task SeedDataAsync()
    {
        // Check if movies or seats are already seeded
        if (!_context.Movies.Any() || !_context.Seats.Any())
        {
            var client = _httpClientFactory.CreateClient();

            // Fetch popular movies data from TMDb API (or use any other endpoint you prefer)
            var movieResponse = await client.GetAsync("https://api.themoviedb.org/3/movie/popular?api_key=" + _apiKey + "&language=en-US&page=1");

            if (movieResponse.IsSuccessStatusCode)
            {
                var movieData = await movieResponse.Content.ReadFromJsonAsync<JsonElement>();

                // Check if "results" is present in the response
                if (movieData.TryGetProperty("results", out var movies))
                {
                    foreach (var movie in movies.EnumerateArray())
                    {
                        var title = movie.GetProperty("title").GetString();
                        var genre = string.Join(", ", movie.GetProperty("genre_ids").EnumerateArray().Select(g => g.ToString())); // genre_ids are IDs, you can fetch names separately if needed
                        var releaseDateStr = movie.GetProperty("release_date").GetString();

                        // Convert release_date string to DateTime
                        DateTime releaseDate = DateTime.Parse(releaseDateStr);

                        var movieId = movie.GetProperty("id").GetInt32();
                        var posterPath = movie.GetProperty("poster_path").GetString();
                        var imageUrl = $"https://image.tmdb.org/t/p/w500{posterPath}";
                        // Create new movie object
                        var newMovie = new Movie
                        {
                            Title = title,
                            Genre = genre,
                            ReleaseDate = releaseDate, // Store as DateTime
                            MovieImage = imageUrl // Add the image URL
                        };
                        _context.Movies.Add(newMovie);
                        await _context.SaveChangesAsync(); // Save movie to DB

                        // Create a list of seats (50 seats as placeholders)
                        var seats = new List<Seat>();

                        for (int i = 1; i <= 50; i++)
                        {
                            string seatNumber = $"A{i}";
                            seats.Add(new Seat
                            {
                                Number = i, 
                                IsAvailable = true,
                                MovieId = newMovie.Id
                            });
                        }

                        _context.Seats.AddRange(seats);
                        await _context.SaveChangesAsync(); 
                    }
                }
            }
        }
    }
}
