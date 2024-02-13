using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataAccess.Services;
using UI.Manager;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RecipeRepository _repo;
        private readonly UserRepository _userRepository;
        public MainWindow()
        {
            InitializeComponent();
            _repo = new RecipeRepository();
            _userRepository = new UserRepository();
            CommentManager.LoadComments();
            UserManager.Users = _userRepository.GetAllUsers();
            UserManager.CurrentUserChanged += CurrentUserChanged;
            Login.Visibility = Visibility.Visible;
            EditRecipe.Visibility = Visibility.Collapsed;
            ShowRecipe.Visibility = Visibility.Collapsed;
            Login.IsSelected = true;
        }

        private void CurrentUserChanged()
        {
            if (UserManager.CurrentUser == null)
            {
                Login.Visibility = Visibility.Visible;
                EditRecipe.Visibility = Visibility.Collapsed;
                ShowRecipe.Visibility = Visibility.Collapsed;
                Login.IsSelected = true;
            }
            else if (UserManager.CurrentUser != null)
            {
                Login.Visibility = Visibility.Collapsed;
                EditRecipe.Visibility = Visibility.Visible;
                ShowRecipe.Visibility = Visibility.Visible;
                ShowRecipe.IsSelected = true;
            }
        }
    }
}