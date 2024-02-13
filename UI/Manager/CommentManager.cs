using System.Windows.Media.Animation;
using UI.Models;
using UI.Services;
namespace UI.Manager;

public static class CommentManager
{
    private static readonly CommentService _commentService = new CommentService();
    public static List<CommentModel> Comments { get; set; } = new();// loads with login action CurrentUserChanged
    public static List<CommentModel> RecipeComments { get; set; } = new(); // loads with recipe selection action SelectedRecipeChanged
    public static Action CommnestListChanged { get; set; }

    public static void LoadComments()
    {
        if (Comments.Count >= 0)
        {
            Comments.Clear();
        }
        Comments = _commentService.GetAllComments();
        SetCommentsAccordingToCurrentUser();
    }

    public static void SetCommentsAccordingToCurrentUser()
    {
        if (UserManager.CurrentUser == null)
        {
            foreach (var commentModel in Comments)
            {
                commentModel.AuthorIsACurrentUser = false;
            }

            return;
        }

        foreach (var commentModel in Comments)
        {
            if (UserManager.CurrentUser.Id == commentModel.AuthorId)
            {
                commentModel.AuthorIsACurrentUser = true;
            }
        }
    }

    public static void UpdateComment(CommentModel comment)
    {
        _commentService.UpdateComment(comment);
        CommnestListChanged.Invoke();
        
    }

    public static List<CommentModel> SetOrUpdateCommentSectionForRecipe(string recipeId)
    {
        var tempCommentList = new List<CommentModel>();

        LoadComments();

        foreach (var commentModel in Comments)
        {
            if (recipeId == commentModel.ReceptId)
            {
                tempCommentList.Add(commentModel);
            }
        }
       
        return tempCommentList;
    }

    public static void AddNewComment(CommentModel comment)
    {
        
        _commentService.AddComment(comment);
        CommnestListChanged.Invoke();
    }

    public static void DeleteComment(CommentModel comment)
    {
        _commentService.DeleteComment(comment);
        CommnestListChanged.Invoke();
    }

}