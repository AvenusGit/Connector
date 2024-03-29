﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.VisualModels
{
    internal interface IWpfScheme<schemeType>
    {
        public void Apply();
        public schemeType GetCurrent();
        public schemeType GetDefault();
    }
}
