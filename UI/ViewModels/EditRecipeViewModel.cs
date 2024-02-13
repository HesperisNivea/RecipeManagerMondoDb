using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
using Common.DTOs;
using Common.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Services;
using UI.Manager;
using UI.Models;

namespace UI.ViewModels;

public class EditRecipeViewModel : ObservableObject
{

    private RecipeRepository _recipeRepository = new RecipeRepository();

    private CommentRepository _commentRepository = new CommentRepository();

    private List<RecipeRecord> _recipeSelection;

    #region RecipeProps

    public List<RecipeRecord> RecipeSelection
    {
        get { return _recipeSelection; }
        set
        {
            _recipeSelection = value;
            OnPropertyChanged();
        }
    }

    private RecipeRecord? _selectedRecipe;

    public RecipeRecord? SelectedRecipe
    {
        get { return _selectedRecipe; }
        set
        {
            _selectedRecipe = value;
            if (_selectedRecipe != null)
            {
                PopulateRecipeContent();
            }
            RemoveRecipe.NotifyCanExecuteChanged();
            UpdaterRecipeContent.NotifyCanExecuteChanged();
            AddNewRecipe.NotifyCanExecuteChanged();
            OnPropertyChanged();
        }
    }

    private string _selectedTitle;

    public string SelectedTitle
    {
        get { return _selectedTitle; }
        set
        {
            _selectedTitle = value;
            OnPropertyChanged();
        }
    }

    private string _selectedDescription;

    public string SelectedDescription
    {
        get
        {
            return _selectedDescription;
        }
        set
        {
            _selectedDescription = value;
            OnPropertyChanged();
        }
    }

    private string _selectedMeasurement;

    public string SelectedMeasurement
    {
        get { return _selectedMeasurement; }
        set
        {
            _selectedMeasurement = value;
            OnPropertyChanged();
        }
    }

    private string _selectedProduct = String.Empty;

    public string SelectedProduct
    {
        get { return _selectedProduct; }
        set
        {
            _selectedProduct = value;
            OnPropertyChanged();
            AddIngredient.NotifyCanExecuteChanged();
        }
    }

    private List<IngredientRecord> _ingredientSelection = new();

    public List<IngredientRecord> IngredientSelection
    {
        get { return _ingredientSelection; }
        set
        {
            _ingredientSelection = value;
            OnPropertyChanged();
        }
    }

    private IngredientRecord? _selectedIngredient;

    public IngredientRecord? SelectedIngredient
    {
        get { return _selectedIngredient; }
        set
        {
            _selectedIngredient = value;
            OnPropertyChanged();
            RemoveIngredient.NotifyCanExecuteChanged();
            UpdateIngredient.NotifyCanExecuteChanged();

        }
    }

    private string _selectedInstructionStep;

    public string SelectedInstructionStep
    {
        get { return _selectedInstructionStep; }
        set
        {
            _selectedInstructionStep = value;
            OnPropertyChanged();
            RemoveInstruction.NotifyCanExecuteChanged();
            UpdateInstruction.NotifyCanExecuteChanged();
        }
    }

    private List<string> _instructionStepsSelection = new();

    public List<string> InstructionStepsSelection
    {
        get { return _instructionStepsSelection; }
        set
        {
            _instructionStepsSelection = value;
            OnPropertyChanged();
        }
    }

    private string _selectedNewInstructionStep;

    public string SelectedNewInstructionStep
    {
        get { return _selectedNewInstructionStep; }
        set
        {
            _selectedNewInstructionStep = value;
            OnPropertyChanged();
            AddInstruction.NotifyCanExecuteChanged();
        }
    }

    private List<CategoryModel> _categorySelection;

    public List<CategoryModel> CategorySelection
    {
        get { return _categorySelection; }
        set
        {
            _categorySelection = value;
            OnPropertyChanged();
        }
    }

    private List<Category> _selectedCategories = new();

    public List<Category> SelectedCategories 
    {
        get { return _selectedCategories; }
        set
        {
            _selectedCategories = value;
            OnPropertyChanged();
        }
    }
    #endregion

    public static Action RecipeSelectionChanged;
    public static Action RecipeRemoved;
    #region RecipeButtons
    public IRelayCommand AddNewRecipe { get; }
    public IRelayCommand UpdaterRecipeContent { get; }
    public IRelayCommand RemoveRecipe { get; }

    public IRelayCommand AddIngredient { get; }
    public IRelayCommand UpdateIngredient { get; }
    public IRelayCommand RemoveIngredient { get; }

    public IRelayCommand AddInstruction { get; }
    public IRelayCommand UpdateInstruction { get; }
    public IRelayCommand RemoveInstruction { get; }

    #endregion

    private RecipeRecord _defaultRecipeRecord =
        new RecipeRecord("", "--New Recipe--", new List<IngredientRecord>(), "", new List<string>(), null, new List<Category>(), 0, 0, 0);

    public EditRecipeViewModel()
    {
        AddNewRecipe = new RelayCommand(AddNewRecipeExecuteCommand, AddNewRecipeCanExecuteCommand);
        UpdaterRecipeContent = new RelayCommand(UpdaterRecipeContentExecuteCommand, UpdaterRecipeContentCanExecuteCommand);
        RemoveRecipe = new RelayCommand(RemoveRecipeExecuteCommand, RemoveRecipeCanExecuteCommand);

        AddIngredient = new RelayCommand(AddIngredientExecuteCommand, AddIngredientCanExecuteCommand);
        UpdateIngredient = new RelayCommand(UpdateIngredientExecuteCommand, UpdateIngredientCanExecuteCommand);
        RemoveIngredient = new RelayCommand(RemoveIngredientExecuteCommand, RemoveIngredientCanExecuteCommand);

        AddInstruction = new RelayCommand(AddInstructionExecuteCommand, AddInstructionCanExecuteCommand);
        UpdateInstruction = new RelayCommand(UpdateInstructionExecuteCommand, UpdateInstructionCanExecuteCommand);
        RemoveInstruction = new RelayCommand(RemoveInstructionExecuteCommand, RemoveInstructionCanExecuteCommand);

        UserManager.CurrentUserChanged += CurrentUserChanged;
        CategorySelection = GetAllCategoryModels();
    }

    #region RecipeCommand

    private bool RemoveRecipeCanExecuteCommand()
    {
        if (SelectedRecipe == null || SelectedRecipe == _defaultRecipeRecord)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void RemoveRecipeExecuteCommand()
    {
        string recipeId = String.Empty;
        if (SelectedRecipe != null)
        {
            recipeId = SelectedRecipe.Id;
        }
        
        _recipeRepository.DeleteRecipe(SelectedRecipe);
        _commentRepository.DeleteAllRecipesComments(recipeId);

        LoadRecipies();
        RecipeSelectionChanged.Invoke();
        RecipeRemoved.Invoke();
    }

    private bool UpdaterRecipeContentCanExecuteCommand()
    {
        if (SelectedRecipe == null || SelectedRecipe == _defaultRecipeRecord)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void UpdaterRecipeContentExecuteCommand()
    {
        _recipeRepository.UpdateRecipe(CollectRecipeData());
        LoadRecipies();
        RecipeSelectionChanged.Invoke();
    }

    private bool AddNewRecipeCanExecuteCommand()
    {
        if (SelectedRecipe == _defaultRecipeRecord)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void AddNewRecipeExecuteCommand()
    {
        _recipeRepository.AddRecipe(CollectRecipeData());
        LoadRecipies();
        RecipeSelectionChanged.Invoke();
    }
    #endregion
    #region InstructionCommands

    private bool RemoveInstructionCanExecuteCommand()
    {
        return SelectedInstructionStep != null;
    }

    private void RemoveInstructionExecuteCommand()
    {
        InstructionStepsSelection.Remove(InstructionStepsSelection.First(i => i == SelectedInstructionStep));
        PopulateInstructionsListContent();
    }

    private bool UpdateInstructionCanExecuteCommand()
    {
        return SelectedInstructionStep != null;
    }

    private void UpdateInstructionExecuteCommand()
    {
        var updateIndex = InstructionStepsSelection.FindIndex(i => i == SelectedInstructionStep);

        if (updateIndex != -1)
        {
            InstructionStepsSelection[updateIndex] = SelectedNewInstructionStep;
        }

        PopulateInstructionsListContent();
    }

    private bool AddInstructionCanExecuteCommand()
    {
        if (SelectedRecipe != null && SelectedNewInstructionStep != String.Empty)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void AddInstructionExecuteCommand()
    {
        InstructionStepsSelection.Add(SelectedNewInstructionStep);
        PopulateInstructionsListContent();
    }
    #endregion
    #region IngredientsCommands
    private bool RemoveIngredientCanExecuteCommand()
    {
        return SelectedIngredient != null;
    }

    private void RemoveIngredientExecuteCommand()
    {
        IngredientSelection.Remove(IngredientSelection.First(i => i == SelectedIngredient));
        PopulateIngredientsListContent();
    }

    private bool UpdateIngredientCanExecuteCommand()
    {
        return SelectedIngredient != null;
    }

    private void UpdateIngredientExecuteCommand()
    {
        var updateIndex = IngredientSelection.FindIndex(i => i == SelectedIngredient);

        if (updateIndex != -1)
        {
            IngredientSelection[updateIndex] = new IngredientRecord(SelectedMeasurement, SelectedProduct);
        }
        PopulateIngredientsListContent();
    }
    private bool AddIngredientCanExecuteCommand()
    {
        if (SelectedRecipe != null && SelectedProduct != String.Empty)
        {
            return true;
        }
        else
        {
            return false;
        }


    }
    private void AddIngredientExecuteCommand()
    {
        IngredientSelection.Add(new IngredientRecord(SelectedMeasurement, SelectedProduct));
        PopulateIngredientsListContent();
    }

    #endregion

    private RecipeRecord CollectRecipeData()
    {
        var tempRecipe = new RecipeRecord(
            SelectedRecipe.Id,
            SelectedTitle,
            IngredientSelection,
            SelectedDescription,
            InstructionStepsSelection,
            new AuthorRecord(UserManager.CurrentUser.Id, UserManager.CurrentUser.UserName),
            AddCategoriesToRecipe(),
            0,
            0,
            0);

        return tempRecipe;
    }

    private List<Category> AddCategoriesToRecipe()
    {
        var tempCategories = new List<Category>();

        foreach (var category in CategorySelection)
        {
            if (category.IsChecked == true)
            {
                tempCategories.Add(category.Category);
            }
        }

        return tempCategories;
    }
    private List<CategoryModel> GetAllCategoryModels()
    {
        var categories = new List<CategoryModel>();

        foreach (var category in Enum.GetValues<Category>().ToList())
        {
            if (category.ToString() != "All")
            {
                categories.Add(new CategoryModel(category));
            }

        }

        return categories;
    }
    private void PopulateCategoryListContent()
    {
        var categories = GetAllCategoryModels();
        foreach (var category in categories)
        {
            bool assignCategory = SelectedRecipe.Categories.Any(c => c == category.Category);
            if (assignCategory)
            {
                category.IsChecked = true;
            }
        }

        CategorySelection.Clear();
        CategorySelection = categories;


    }
    private void PopulateInstructionsListContent()
    {
        var temp = new List<string>();
        foreach (var instruction in InstructionStepsSelection)
        {
            temp.Add(instruction);
        }

        InstructionStepsSelection.Clear();
        InstructionStepsSelection = temp;
    }
    private void PopulateIngredientsListContent()
    {
        var tempList = new List<IngredientRecord>();

        foreach (var ingredientRecord in IngredientSelection)
        {
            tempList.Add(ingredientRecord);
        }

        IngredientSelection.Clear();
        IngredientSelection = tempList;
    }
    private void PopulateRecipeContent()
    {
        if (SelectedRecipe != _defaultRecipeRecord)
        {
            SelectedTitle = SelectedRecipe.Title;
            SelectedDescription = SelectedRecipe.Description;
            IngredientSelection = SelectedRecipe.Ingredients;
            InstructionStepsSelection = SelectedRecipe.Instructions;
            PopulateCategoryListContent();
        }
        else
        {
            SelectedTitle = "";
            SelectedDescription = "";
            IngredientSelection = new List<IngredientRecord>();
            InstructionStepsSelection = new List<string>();
            PopulateCategoryListContent();

        }
    }
    private void CurrentUserChanged()
    {
        if (UserManager.CurrentUser != null)
        {
            LoadRecipies();
        }
    }

    private void LoadRecipies()
    {
        var recipies = new List<RecipeRecord> { _defaultRecipeRecord };

        recipies.AddRange(_recipeRepository.GetAllUsersRecipes(UserManager.CurrentUser.Id));

        if (RecipeSelection != null)
        {
            RecipeSelection.Clear();
        }

        RecipeSelection = recipies;
    }

    
}