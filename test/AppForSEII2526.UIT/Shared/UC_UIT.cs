using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;


namespace AppForMovies.UIT.Shared {
    public class UC_UIT : IDisposable {

        private bool _pipeline = false;

        //establish which browser you would like to use
        //private string _browser = "Chrome";
        //private string _browser = "Firefox";
        private string _browser = "Edge";

        protected IWebDriver _driver;
        protected readonly ITestOutputHelper _output;


        public string _URI {
            get {
                //set url of your web page 
                return "https://localhost:7083/";

            }
        }

        public UC_UIT(ITestOutputHelper output) {

            //it initializes where the errors will be shown
            _output = output;

            switch (_browser) {
                case "Firefox":
                    SetUp_FireFox4UIT();
                    break;
                case "Edge":
                    SetUp_EdgeFor4UIT();
                    break;
                default:
                    //by default Chrome will be used
                    SetUp_Chrome4UIT();
                    break;
            }
            //Added to make _Driver wait when an element is not found.
            //It will wait for a maximum of 50 seconds.

            //maximize the window browser
            _driver.Manage().Window.Maximize();
        }


        protected void Initial_step_opening_the_web_page() {
            _driver.Navigate()
                .GoToUrl(_URI);
        }

        protected void Perform_login(string email, string password)
        {
            // Para casos en los que ya estamos logueados

            // Vamos a la Home primero para chequear el estado
            _driver.Navigate().GoToUrl(_URI);

            // Espera para que cargue la barra de navegación
            System.Threading.Thread.Sleep(500);

            // Usamos FindElements (plural) porque si no encuentra nada devuelve lista vacía (no falla)
            // Buscamos cualquier botón o enlace que diga "Logout" o "Log out"
            var logoutButtons = _driver.FindElements(By.XPath("//button[contains(text(), 'Logout')] | //a[contains(text(), 'Logout')] | //button[contains(text(), 'Log out')]"));

            if (logoutButtons.Count > 0)
            {
                
                return;
            }

            // SI NO ESTAMOS LOGUEADOS, HACEMOS EL PROCESO NORMAL
            _driver.Navigate().GoToUrl(_URI + "Account/Login");

            // Espera a que cargue el formulario
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElement(By.Name("Input.Email")));

            // Escribir credenciales
            _driver.FindElement(By.Name("Input.Email")).SendKeys(email);
            _driver.FindElement(By.Name("Input.Password")).SendKeys(password);

            // Click en Submit
            _driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            // Esperamos a ver el botón de Logout antes de devolver el control al test
            wait.Until(d => d.PageSource.Contains("Logout") || d.PageSource.Contains("Log out"));
        }


        protected void SetUp_Chrome4UIT() {
            var optionsc = new ChromeOptions {
                PageLoadStrategy = PageLoadStrategy.Normal,
                AcceptInsecureCertificates = true
            };
            //For pipelines use this option for hiding the browser
            if (_pipeline) optionsc.AddArgument("--headless");

            _driver = new ChromeDriver(optionsc);

        }

        protected void SetUp_FireFox4UIT() {
            var optionsff = new FirefoxOptions {
                PageLoadStrategy = PageLoadStrategy.Normal,
                AcceptInsecureCertificates = true
            };
            //For pipelines use this option for hiding the browser
            if (_pipeline) optionsff.AddArgument("--headless");

            _driver = new FirefoxDriver(optionsff);

        }

        protected void SetUp_EdgeFor4UIT() {
            //var edgeDriverService = Microsoft.Edge.SeleniumTools.EdgeDriverService.CreateChromiumService();
            //var edgeOptions = new Microsoft.Edge.SeleniumTools.EdgeOptions();
            //edgeOptions.PageLoadStrategy = PageLoadStrategy.Normal;
            //edgeOptions.UseChromium = true;
            //if (_pipeline) edgeOptions.AddArguments("--headless");

            //_driver = new Microsoft.Edge.SeleniumTools.EdgeDriver(edgeDriverService, edgeOptions);

            var optionsEdge = new EdgeOptions {
                PageLoadStrategy = PageLoadStrategy.Normal,
                AcceptInsecureCertificates = true
            };

            //For pipelines use this option for hiding the browser
            if (_pipeline) optionsEdge.AddArgument("--headless");

            _driver = new EdgeDriver(optionsEdge);

        }


        public void Dispose() {
            _driver.Close();
            _driver.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}