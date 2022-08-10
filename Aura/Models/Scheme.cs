namespace Aura.Models
{
    /// <summary>
    /// Абстрактная схема
    /// </summary>
    public abstract class Scheme
    {
        private string _name;
        private bool _isSevice;


        /// <summary>
        /// Имя схемы
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Указатель служебной схемы
        /// </summary>
        public bool IsServiceScheme
        {
            get
            {
                return _isSevice;
            }
            set
            {
                _isSevice = value;
            }
        }

        /// <summary>
        /// Применить схему прямо сейчас
        /// </summary>
        public abstract void Apply();
    }
}
