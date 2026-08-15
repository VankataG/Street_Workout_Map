using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StreetWorkoutMap.Pages
{
    public class StatusCodeModel : PageModel
    {
        public int Code { get; private set; }

        public string Title { get; private set; } = string.Empty;

        public string Message { get; private set; } = string.Empty;

        public string Symbol { get; private set; } = "!";

        public void OnGet(int code)
        {
            Code = code;

            switch (code)
            {
                case 404:
                    Title = "Страницата не беше намерена";
                    Message =
                        "Адресът може да е грешен, страницата да е преместена " +
                        "или съдържанието вече да не съществува.";

                    Symbol = "404";
                    break;

                case 403:
                    Title = "Нямаш достъп до тази страница";
                    Message =
                        "Нямаш необходимите права, за да отвориш това съдържание.";

                    Symbol = "403";
                    break;

                default:
                    Title = "Не успяхме да заредим страницата";
                    Message =
                        "Възникна проблем при обработката на заявката.";

                    Symbol = code.ToString();
                    break;
            }
        }
    }
}