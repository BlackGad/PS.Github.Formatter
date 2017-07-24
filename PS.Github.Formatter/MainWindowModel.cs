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
                                        new PropertyMetadata(string.Empty, SourceChanged));

        #endregion

        #region Static members

        private static void SourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var owner = (MainWindowModel)dependencyObject;
            owner.SourceChanged(e);
        }

        #endregion

        #region Properties

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

        private void SourceChanged(DependencyPropertyChangedEventArgs e)
        {
            Result = ((string)e.NewValue).FormatSyntax();
        }

        #endregion
    }
}