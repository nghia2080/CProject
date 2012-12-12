using System.Globalization;
using AntaresShell.Common;
using AntaresShell.Localization;

namespace AntaresShell.BaseClasses
{
    public abstract class ViewModelBase : BindableBase
    {
        private ObservableDictionary<string, string> _resource;
        public ObservableDictionary<string, string> Resource
        {
            get { return _resource ?? (_resource = LanguageProvider.Resource); }
        }

    }
}
