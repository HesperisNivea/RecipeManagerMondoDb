using System.Collections.ObjectModel;
using System.Globalization;
using Common.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Entities;
using DataAccess.Services;
using UI.Manager;
using UI.Models;
using UI.Services;
using UI.ViewModel;

namespace UI.ViewModels;

public class CommentsViewModel : ObservableObject
{
    private RecipeRecord _selectedRecipe;

    public RecipeRecord SelectedRecipe
    {
        get { return _selectedRecipe; }
        set
        {
            _selectedRecipe = value;
            if (SelectedRecipe != null)
            {
                GetCommentsForRecipeAndSetSelectedRecipe(SelectedRecipe);
            }
            AddCommentCommand.NotifyCanExecuteChanged();
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

    private string _selectedContent;

    public string SelectedContent
    {
        get { return _selectedContent; }
        set
        {
            _selectedContent = value;
            OnPropertyChanged();
        }
    }
    private int _selectedRating;

    public int SelectedRating
    {
        get { return _selectedRating; }
        set { _selectedRating = value; }
    }

    private readonly CommentService _commentService = new CommentService();

    private ObservableCollection<CommentModel> _comments = new ObservableCollection<CommentModel>(CommentManager.RecipeComments);
    public ObservableCollection<CommentModel> Comments
    {
        get { return _comments; }
        set
        {
            _comments = value;
            OnPropertyChanged();
        }
    }
    public IRelayCommand AddCommentCommand { get; set; }

    public CommentsViewModel()
    {
        RecipeSelectionPanelViewModel.SelectedRecipeChanged += SelectedRecipeChanged;
        RecipeSelectionPanelViewModel.SelectedCategoryChanged += SelectedCategoryChanged;
            AddCommentCommand = new RelayCommand(AddCommentCommandExecuteCommand, AddCommentCommandCanExecuteCommand);
        UserManager.CurrentUserChanged += CurrentUserChanged;
        CommentManager.CommnestListChanged += CommentListChanged;
       EditRecipeViewModel.RecipeRemoved += RecipeRemoved;

    }

    private void SelectedCategoryChanged()
    {
        if (SelectedRecipe != null)
        {
            SelectedRecipe = null;
        }

        if (Comments.Count >= 0)
        {
            Comments.Clear();
        }
    }

    private void RecipeRemoved()
    {
        Comments.Clear();
    }

    private void SelectedRecipeChanged(RecipeRecord selectedRecipe)
    {
        SelectedRecipe = selectedRecipe;
    }

    private void CommentListChanged()
    {
        GetCommentsForRecipeAndSetSelectedRecipe(SelectedRecipe);
    }



    private void CurrentUserChanged()
    {
        if (UserManager.CurrentUser == null)
        {
            Comments.Clear();
            return;
        }

        foreach (var commentModel in Comments)
        {
            if (commentModel.AuthorId == UserManager.CurrentUser.Id)
            {
                commentModel.AuthorIsACurrentUser = true;
            }
            else
            {
                commentModel.AuthorIsACurrentUser = false;
            }
        }
    }

    private bool AddCommentCommandCanExecuteCommand()
    {
        if (SelectedRecipe == null)
        {
            return false;
        }
        else
        {
            return true;
        }


    }

    private void AddCommentCommandExecuteCommand()
    {
        CommentManager.AddNewComment(CreateComment());
        GetCommentsForRecipeAndSetSelectedRecipe(SelectedRecipe);
    }

    public CommentModel CreateComment()
    {
        CultureInfo culture = new CultureInfo("se-SE");
        DateTime now = DateTime.Now;


        var newComment = new CommentModel()
        {
            Title = SelectedTitle,
            Content = SelectedContent,
            Rating = SelectedRating,
            Published = now,
            AuthorId = UserManager.CurrentUser.Id,
            AuthorsName = UserManager.CurrentUser.UserName,
            ReceptId = SelectedRecipe.Id,  // can cause problems

        };

        return newComment;
    }

    public void UpdateComment(CommentModel newComment)
    {
        newComment.IsReadOnly = true;
        _commentService.UpdateComment(newComment);
        Comments.Remove(newComment);
    }
    public void DeleteComment(CommentModel commentModel)
    {
        CommentManager.DeleteComment(commentModel);
        

    }
    public void GetCommentsForRecipeAndSetSelectedRecipe(RecipeRecord recipe)
    {
        if (string.IsNullOrEmpty(recipe.Id))
        {
          return;
        }

        var tempcomments = CommentManager.SetOrUpdateCommentSectionForRecipe(recipe.Id);
        
        if (Comments.Count > 0)
        {
            Comments.Clear();

        }

        foreach (var commentModel in tempcomments)
        {
            Comments.Add(commentModel);
        }
    }


}