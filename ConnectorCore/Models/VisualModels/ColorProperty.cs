using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConnectorCore.Models.VisualModels.Interfaces;
using System.Text.RegularExpressions;

namespace ConnectorCore.Models.VisualModels
{
    public class ColorProperty : IColorProperty
    {
        public ColorProperty() { }
        public ColorProperty(string name)
        {
            Name = name;
        }
        public ColorProperty(string name, string color)
        {
            Name = name;
            Color = color;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public long ColorSchemeId { get; set; }
        public ColorScheme ColorScheme { get; set; }
        public string ColorKeyName { get { return Name + "Color"; } }
        public string Color { get; set; }

        public static bool IsValueCorrect(ColorProperty property)
        {
            if(String.IsNullOrEmpty(property.Name))
                return false;
            if (String.IsNullOrWhiteSpace(property.Color) || !Regex.IsMatch(property.Color, "#[0-9a-fA-F]{8}"))
                return false;
            return true;
        }
        public override string ToString()
        {
            return Color;
        }
    }
}
