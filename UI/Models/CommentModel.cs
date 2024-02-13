using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using UI.Manager;
using UI.ViewModels;

namespace UI.Models;

public class CommentModel : INotifyPropertyChanged
{
    private string _id;

    public string Id
    {
        get { return _id; }
        set { _id = value; }
    }

    private string _title;

    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    private string _content;

    public string Content
    {
        get { return _content; }
        set
        {
            _content = value;
            OnPropertyChanged();
        }
    }

    private DateTime _published;

    public DateTime Published
    {
        get { return _published; }
        set
        {
            _published = value;
        }
    }


    public string AuthorId { get; set; }

    private int _rating;

    public int Rating
    {
        get { return _rating; }
        set { _rating = value; }
    }

    private string _authorsName;

    public string AuthorsName
    {
        get { return _authorsName; }
        set
        {
            _authorsName = value;
            OnPropertyChanged();
        }
    }

    private bool _isReadOnly = true;

    public bool IsReadOnly
    {
        get { return _isReadOnly; }
        set
        {
            _isReadOnly = value;
            OnPropertyChanged();
        }
    }

    public string ReceptId { get; set; }

    public bool AuthorIsACurrentUser { get; set; }

    
    public IRelayCommand EditCommand { get; }
    public IRelayCommand SaveCommand { get; }
    public IRelayCommand DeleteCommand { get; }

    public CommentModel()
    {
        EditCommand = new RelayCommand(Edit);
        SaveCommand = new RelayCommand(Save);
        DeleteCommand = new RelayCommand(Delete);
    }

    private void Save()
    {
       CommentManager.UpdateComment(this);
        IsReadOnly = true;
    }

    private void Edit()
    {
        IsReadOnly = false;
    }

    private void Delete()
    {
       CommentManager.DeleteComment(this);
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

