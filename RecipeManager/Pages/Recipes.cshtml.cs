using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace RecipeManager.Pages
{
    // This is a PageModel for a Razor Page that handles displaying recipe entries
    public class RecipesModel : PageModel
    {
        // Property that will store the selected recipe ID from form submissions
        [BindProperty]
        public int SelectedRecipeId { get; set; }

        // List that will hold all recipes for the dropdown selection
        public List<SelectListItem> RecipeList { get; set; }

        // Property that will store the currently selected recipe object
        public Recipe SelectedRecipe { get; set; }

        // Handles HTTP GET requests to the page - loads the list of recipes
        public void OnGet()
        {
            LoadRecipeList();
        }

        // Handles HTTP POST requests (when user selects a recipe) - loads the recipe list
        // and retrieves the selected recipe's details
        public IActionResult OnPost()
        {
            LoadRecipeList();
            if (SelectedRecipeId != 0)
            {
                SelectedRecipe = GetRecipeById(SelectedRecipeId);
            }
            return Page();
        }

        // Helper method that loads the list of recipes from the SQLite database
        // for displaying in a dropdown menu
        private void LoadRecipeList()
        {
            RecipeList = new List<SelectListItem>();
            using (var connection = new SqliteConnection("Data Source=Recipes.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Recipes";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecipeList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(), // Recipe ID as the value
                            Text = reader.GetString(1)             // Recipe name as the display text
                        });
                    }
                }
            }
        }

        // Helper method that retrieves a specific recipe by its ID from the database
        // Returns all details of the recipe
        private Recipe GetRecipeById(int id)
        {
            using (var connection = new SqliteConnection("Data Source=Recipes.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Recipes WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id); // Using parameterized query for security
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Recipe
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Cuisine = reader.GetString(2),
                            Ingredients = reader.GetString(3),
                            Instructions = reader.GetString(4),
                            ImageFileName = reader.GetString(5)
                        };
                    }
                }
            }
            return null;
        }
    }

    // Simple model class representing a recipe entry
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cuisine { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public string ImageFileName { get; set; }
    }
}