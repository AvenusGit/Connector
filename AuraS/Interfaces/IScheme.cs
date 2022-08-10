using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraS.Interfaces
{
    public interface IScheme<T> where T : IScheme<T>
    {
        public abstract void Apply();
        public abstract T Clone();
        public abstract T GetCurrent();
    }
}
