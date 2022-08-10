using System;
using System.Windows;
using System.Linq;

namespace Aura.Models
{
    public class StyleScheme : Scheme
    {
        private string _resourceDictionaryName;

        public string ResourceDictionary
        {
            get
            {
                return _resourceDictionaryName;
            }
            set
            {
                _resourceDictionaryName = value;
            }
        }

        public override void Apply()
        {
            Uri resourceDictionaryUri = new Uri("/Aura;component/Styles/" + ResourceDictionary + ".xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(resourceDictionaryUri) as ResourceDictionary;
            resourceDict.Source = resourceDictionaryUri;
            if (Application.Current.Resources.MergedDictionaries.Count > 0)
                Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            Application.Current.Resources["CurrentStyleName"] = ResourceDictionary;
        }

        private void RemoveOtherStyleSchemes()
        {
            foreach (StyleScheme scheme in Aura.Schemes.Styles.StyleSchemes.GetApplicationStyleScheme())
            {

            }
        }
    }
}
