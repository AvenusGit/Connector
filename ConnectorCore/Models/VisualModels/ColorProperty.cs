using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConnectorCore.Models.VisualModels.Interfaces;

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
        public string ColorKeyName { get { return Name + "Color"; } }
        public string Color { get; set; }

        public override string ToString()
        {
            return Color;
        }
    }
}
