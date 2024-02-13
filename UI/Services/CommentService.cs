using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DTOs;
using DataAccess.Entities;
using DataAccess.Services;
using UI.Models;
using UI.ViewModels;

namespace UI.Services
{
    class CommentService
    {
        private readonly CommentRepository _commentRepository = new CommentRepository();

        public List<CommentModel> GetAllComments()
        {
            var comments = _commentRepository.GetAllCommentRecords().ToList()
                .Select(comment => new CommentModel()
                {
                    Id = comment.Id,
                    AuthorId = comment.AuthorId,
                    Title = comment.Title,
                    Content = comment.Content,
                    Published = comment.Published,
                    ReceptId = comment.RecipeId,
                    Rating = comment.Rating,
                    AuthorsName = comment.AuthorName
                }).ToList();

            return comments;
        }
        public List<CommentModel> GetAllCommentAboutRecipe(string recipeId)
        {
            var comments = _commentRepository.GetAllCommentsRelatedToRecipe(recipeId).ToList()
                .Select(comment => new CommentModel()
                {
                    Id = comment.Id,
                    AuthorId = comment.AuthorId,
                    Title = comment.Title,
                    Content = comment.Content,
                    Published = comment.Published,
                    ReceptId = comment.RecipeId,
                    Rating = comment.Rating,
                    AuthorsName = comment.AuthorName
                }).ToList();

            return comments;
        }

        public void AddComment(CommentModel commentModel)
        {
            var comment = new CommentRecord(
                "",
                commentModel.Title, 
                commentModel.Content,
                commentModel.Rating,
                commentModel.Published, 
                commentModel.ReceptId,
                commentModel.AuthorId,
                commentModel.AuthorsName);

            _commentRepository.AddComment(comment);
        }
        public void DeleteComment(CommentModel commentModel)
        {
            var comment = new CommentRecord(
                commentModel.Id,
                commentModel.Title,
                commentModel.Content,
                commentModel.Rating,
                commentModel.Published,
                commentModel.ReceptId,
                commentModel.AuthorId,
                commentModel.AuthorsName);

            _commentRepository.DeleteComment(comment);
        }

        public void UpdateComment(CommentModel commentModel)
        {
            var comment = new CommentRecord(
                commentModel.Id,
                commentModel.Title,
                commentModel.Content,
                commentModel.Rating,
                commentModel.Published,
                commentModel.ReceptId,
                commentModel.AuthorId,
                commentModel.AuthorsName);

            _commentRepository.UpdateComment(comment);
        }
    }


}
