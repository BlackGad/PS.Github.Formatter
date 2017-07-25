using System.Windows;
using PS.Github.Formatter.Extensions;

namespace PS.Github.Formatter
{
    public class MainWindowModel : DependencyObject
    {
        #region Property definitions

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result",
                                        typeof(string),
                                        typeof(MainWindowModel),
                                        new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source",
                                        typeof(string),
                                        typeof(MainWindowModel),
                                        new PropertyMetadata(string.Empty, SourceParametersChanged));

        public static readonly DependencyProperty MethodImageProperty =
            DependencyProperty.Register("MethodImage",
                                        typeof(string),
                                        typeof(MainWindowModel),
                                        new PropertyMetadata("https://raw.githubusercontent.com/BlackGad/PS.Build/master/.Assets/method.jpeg",
                                                             SourceParametersChanged));

        public static readonly DependencyProperty PropertyImageProperty =
            DependencyProperty.Register("PropertyImage",
                                        typeof(string),
                                        typeof(MainWindowModel),
                                        new PropertyMetadata("https://raw.githubusercontent.com/BlackGad/PS.Build/master/.Assets/property.jpeg",
                                                             SourceParametersChanged));

        public static readonly DependencyProperty EnumImageProperty =
            DependencyProperty.Register("EnumImage",
                                        typeof(string),
                                        typeof(MainWindowModel),
                                        new PropertyMetadata("https://raw.githubusercontent.com/BlackGad/PS.Build/master/.Assets/enumeration.jpeg",
                                                             SourceParametersChanged));

        #endregion

        #region Static members

        private static void SourceParametersChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var owner = (MainWindowModel)dependencyObject;
            owner.SourceParametersChanged();
        }

        #endregion

        #region Properties

        public string EnumImage
        {
            get { return (string)GetValue(EnumImageProperty); }
            set { SetValue(EnumImageProperty, value); }
        }

        public string MethodImage
        {
            get { return (string)GetValue(MethodImageProperty); }
            set { SetValue(MethodImageProperty, value); }
        }

        public string PropertyImage
        {
            get { return (string)GetValue(PropertyImageProperty); }
            set { SetValue(PropertyImageProperty, value); }
        }

        public string Result
        {
            get { return (string)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        #endregion

        #region Members

        private void SourceParametersChanged()
        {
            Result = Source.FormatSyntax(PropertyImage, MethodImage, EnumImage);
        }

        #endregion
    }
}