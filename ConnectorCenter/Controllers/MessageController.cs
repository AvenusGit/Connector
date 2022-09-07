using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Message;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер для генерации странички с сообщением и кнопками
    /// </summary>
    public class MessageController : Controller
    {
        /// <summary>
        /// Запрос на генерацию странички с сообщением для пользователя
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="buttons">Массив строк для генерации кнопок</param>
        /// <param name="errorCode">Код ошибки если есть</param>
        /// <returns>Сгенерированная страница с сообщением</returns>
        public IActionResult Index(string message, string[] buttons, int? errorCode = null)
        {
            // вызов генератора страницы с сообщением не логируется
            try
            {
                Dictionary<string, string> buttonDictionary = new Dictionary<string, string>();
                foreach (string buttonString in buttons)
                {
                    string[] valuePair = buttonString.Replace("[", "").Replace("]", "").Split(','); // TODO костыль, переделать
                    if (valuePair.Count() > 1)
                        buttonDictionary.Add(valuePair[0], valuePair[1]);
                }
                return View(new IndexModel(message, buttonDictionary, errorCode));
            }
            catch
            {
                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                    new
                    {
                        message = "Ошибка при генерации страницы с сообщением. Сообщите разработчику, что так бывает.",
                        buttons = new Dictionary<string, string>()
                        {
                            {"На главную",@"\dashboard" },
                            {"К логам",@"\logs" }
                        },
                        errorCode = 500
                    }));
            }
        }
    }
}
