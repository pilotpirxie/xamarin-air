using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace airmonitor.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentWithHeader : StackLayout
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(ContentWithHeader),
            null);

        public static readonly BindableProperty ControlContentProperty = BindableProperty.Create(
            nameof(ControlContent),
            typeof(View),
            typeof(ContentWithHeader),
            null);

        public ContentWithHeader()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public View ControlContent
        {
            get => (View) GetValue(ControlContentProperty);
            set => SetValue(ControlContentProperty, value);
        }
    }
}