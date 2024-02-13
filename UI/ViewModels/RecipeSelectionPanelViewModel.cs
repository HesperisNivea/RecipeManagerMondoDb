using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using Common.DTOs;
using Common.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Services;
using UI.Manager;
using UI.Models;
using UI.ViewModels;
using UI.Views;

namespace UI.ViewModel;

public class RecipeSelectionPanelViewModel : ObservableObject
{
    private readonly RecipeRepository _recipeRepository = new();

    private ObservableCollection<RecipeRecord> _recipes = new();

    public ObservableCollection<RecipeRecord> Recipes
    {
        get { return _recipes; }
        set
        {
            _recipes = value;
            OnPropertyChanged();
        }
    }

    private RecipeRecord _selectedRecipe;

    public RecipeRecord SelectedRecipe
    {
        get { return _selectedRecipe; }
        set
        {
            _selectedRecipe = value;
            OnPropertyChanged();
            if (_selectedRecipe != null)
            {
                SelectedRecipeChanged.Invoke(_selectedRecipe);
                AverageRating = SelectedRecipe.RatingSum / SelectedRecipe.VoteCount;
            }
        }
    }

    private float _averageRating;

    public float AverageRating
    {
        get { return _averageRating; }
        set
        {
            _averageRating = value;
            OnPropertyChanged();
        }

    }

    private Array _categorySelection = Enum.GetValues(typeof(Category));

    public Array CategorySelection
    {
        get { return _categorySelection; }
        set { _categorySelection = value; }
    }

    private Category? _selectedCategory;

    public Category? SelectedCategory
    {
        get { return _selectedCategory; }
        set
        {
            _selectedCategory = value;
            OnPropertyChanged();
            if (SelectedCategory != null)
            {
                GetRecipesWithSelectedCategory();
            }

        }
    }

    public static Action<RecipeRecord> SelectedRecipeChanged;
    public static Action SelectedCategoryChanged;

    public IRelayCommand LogOut { get; }
    public RecipeSelectionPanelViewModel()
    {
        GetAllRecipes();
        EditRecipeViewModel.RecipeSelectionChanged += RecipeSelectionChanged;
        CommentManager.CommnestListChanged += CommnestListChanged;
        LogOut = new RelayCommand(LogOutExecuteCommand);
    }

    private void CommnestListChanged()
    {
        string selectedRecipeId = String.Empty;

        if (_selectedRecipe != null)
        {
            selectedRecipeId = SelectedRecipe.Id;
        }

        GetAllRecipes();

        if (!string.IsNullOrEmpty(selectedRecipeId))
        {
            SelectedRecipe = Recipes.FirstOrDefault(x => x.Id == selectedRecipeId);
        }

    }

    private void RecipeSelectionChanged()
    {
        GetAllRecipes();
    }

    private void LogOutExecuteCommand()
    {
        UserManager.LogOut();
    }

    private void GetAllRecipes()
    {
        var recipes = _recipeRepository.GetAllRecipes();
        Recipes.Clear();
        foreach (var recipeRecord in recipes)
        {
            Recipes.Add(recipeRecord);
        }
    }

    private void GetRecipesWithSelectedCategory()
    {
        if (SelectedCategory.ToString() == "All")
        {
            GetAllRecipes();
            return;
        }
        var recipes = _recipeRepository.GetAllRecipes();
        Recipes.Clear();

        foreach (var recipeRecord in recipes)
        {
            if (recipeRecord.Categories.Any(c => c == SelectedCategory))
            {
                Recipes.Add(recipeRecord);
            }

        }

      SelectedCategoryChanged.Invoke();
       
    }

}