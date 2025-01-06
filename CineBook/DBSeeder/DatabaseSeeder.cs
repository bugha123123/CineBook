using CineBook.Models;
using System.Text.Json;
using System;
using System.Linq;
using CineBook.ApplicationDbContext;

public class DatabaseSeeder
{
    private readonly AppDbContextion _context;  // Ensure AppDbContext is correctly used
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey = "ba78b82d72bb7ddb477af4a06fd9a3f7"; // TMDb API Key

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

            // Fetch popular movies data from TMDb API
            var movieResponse = await client.GetAsync("https://api.themoviedb.org/3/movie/popular?api_key=" + _apiKey + "&language=en-US&page=1");

            if (movieResponse.IsSuccessStatusCode)
            {
                var movieData = await movieResponse.Content.ReadFromJsonAsync<JsonElement>();

                if (movieData.TryGetProperty("results", out var movies))
                {
                    // Fetch genre details
                    var genreResponse = await client.GetAsync("https://api.themoviedb.org/3/genre/movie/list?api_key=" + _apiKey + "&language=en-US");
                    var genreData = await genreResponse.Content.ReadFromJsonAsync<JsonElement>();
                    var genres = genreData.GetProperty("genres").EnumerateArray().ToDictionary(g => g.GetProperty("id").GetInt32(), g => g.GetProperty("name").GetString());

                    var movieList = new List<Movie>();
                    var seatList = new List<Seat>();
                    Random random = new Random();
                    foreach (var movie in movies.EnumerateArray())
                    {
                        var title = movie.GetProperty("title").GetString();
                        var genreIds = movie.GetProperty("genre_ids").EnumerateArray();
                        var genreNames = genreIds.Select(g => genres.ContainsKey(g.GetInt32()) ? genres[g.GetInt32()] : "").Where(g => !string.IsNullOrEmpty(g));
                        var genre = string.Join(", ", genreNames);
                        var releaseDateStr = movie.GetProperty("release_date").GetString();

                        // Convert release_date string to DateTime
                        DateTime releaseDate = DateTime.Parse(releaseDateStr);

                        var movieId = movie.GetProperty("id").GetInt32();
                        var posterPath = movie.GetProperty("poster_path").GetString();
                        var imageUrl = $"https://image.tmdb.org/t/p/w500{posterPath}";

                        // Fetch detailed movie information
                        var movieDetailsResponse = await client.GetAsync($"https://api.themoviedb.org/3/movie/{movieId}?api_key={_apiKey}&language=en-US");

                        if (movieDetailsResponse.IsSuccessStatusCode)
                        {
                            var movieDetailsData = await movieDetailsResponse.Content.ReadFromJsonAsync<JsonElement>();
                            var description = movieDetailsData.GetProperty("overview").GetString();
                            var runtime = movieDetailsData.GetProperty("runtime").GetInt32(); // Runtime in minutes

                            // Generate ShowTime as any date in the future from DateTime.Now
                            int randomDaysFromNow = random.Next(1, 90); // Random number between 1 and 30 days
                            DateTime showTime = DateTime.Now.AddDays(randomDaysFromNow);

                            // Create new movie object
                            var newMovie = new Movie
                            {
                                Title = title,
                                Genre = genre,
                                ReleaseDate = releaseDate,
                                MovieImage = imageUrl,
                                Description = description,
                                RunTime = runtime,
                                ShowTime = showTime
                            };

                            movieList.Add(newMovie);

                            // Create a list of seats (50 seats as placeholders)
                            for (int i = 1; i <= 50; i++)
                            {
                                seatList.Add(new Seat
                                {
                                    Number = i,
                                    IsAvailable = true,
                                    Movie = newMovie
                                });
                            }
                        }
                    }

                    // Save the movies and seats in bulk
                    _context.Movies.AddRange(movieList);
                    _context.Seats.AddRange(seatList);

                    // Save all changes in one go
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
