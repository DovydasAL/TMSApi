
namespace TMSEntities
{
    public class Constants
    {
        public const string BASE_PATH = "https://termmasterschedule.drexel.edu";
        public const string HOME_URL = "https://termmasterschedule.drexel.edu/webtms_du/app?page=Home&service=page";
        public const string TERM_PATHS = "//div[@class='term']//a";
        public const string COLLEGE_PATHS = "//div[@id='sideLeft']//a";
        public const string DEPARTMENT_PATHS = "//tr//td//div[@class='even' or @class='odd']//a";
        public const string CLASS_PATHS = "//tr[@class='even' or @class='odd']";
    }
}
