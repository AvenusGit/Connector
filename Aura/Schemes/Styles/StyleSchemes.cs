using Aura.Models;
using System;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Schemes.Styles
{
    public static class StyleSchemes
    {
        public static List<StyleScheme> GetApplicationStyleScheme()
        {
            List<StyleScheme> result = new List<StyleScheme>();
            result.Add(AuraPreview());
            result.Add(AuraQuad());  
            return result;
        }

        public static StyleScheme AuraPreview()
        {
            return new StyleScheme
            {
                Name = "AuraPreview",
                ResourceDictionary = "AuraPreview",
                IsServiceScheme = false
            };
        }

        public static StyleScheme AuraQuad()
        {
            return new StyleScheme
            {
                Name = "AuraQuad",
                ResourceDictionary ="AuraQuad",
                IsServiceScheme = false
            };
        }
        
    }
}
