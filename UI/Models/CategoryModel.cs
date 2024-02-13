using System.ComponentModel;
using System.Runtime.CompilerServices;
using Common.Enums;

namespace UI.Models;

public class CategoryModel : INotifyPropertyChanged
{
    public Category Category { get; set; }

    public bool IsChecked { get; set; }

    public string Name { get; set; } 

    public event PropertyChangedEventHandler? PropertyChanged;

    public CategoryModel(Category category)
    {
        Category = category;
        Name = category.ToString();
    }
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