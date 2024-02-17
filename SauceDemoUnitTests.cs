using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SauceDemoAutomation
{
    public class Tests
    {
        IWebDriver driver;
        string currentDirectory;
        string username;
        string password;

        [SetUp]
        public void Setup()
        {
            // Make sure that the 'Copy to Output Directory' property for file is set to 'Copy if Newer' if using Visual Studio
            currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = (@"Drivers\chromedriver.exe");
            string file = Path.Combine(currentDirectory, filePath);
            driver = new ChromeDriver(file);

            username = getFileContent("Username.txt");
            password = getFileContent("Password.txt");

        }

        [Test]
        public void Login_with_Correct_Credentials()
        {
            string expectedURL = "https://www.saucedemo.com/inventory.html";
            string actualURL;
            IWebElement loginButton;
            IWebElement usernameInputField;
            IWebElement passwordInputField;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            driver.Url = "https://www.saucedemo.com/";

            // Get necessary information from Login page
            loginButton = wait.Until(ExpectedConditions.ElementExists(By.Id("login-button")));
            usernameInputField = driver.FindElement(By.Id("user-name"));
            passwordInputField = driver.FindElement(By.Id("password"));

            // Login with credentials
            usernameInputField.SendKeys(username);
            passwordInputField.SendKeys(password);
            loginButton.Click();

            actualURL = driver.Url;

            // Assert that Login was successful
            Assert.That(actualURL, Is.EqualTo(expectedURL));
        }

        [Test]
        public void Login_With_Missing_Password()
        {
            IWebElement loginButton;
            IWebElement usernameInputField;
            IWebElement passwordInputField;
            IWebElement errorMessage;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            driver.Url = "https://www.saucedemo.com/";

            // Get necessary information from Login page
            loginButton = wait.Until(ExpectedConditions.ElementExists(By.Id("login-button")));
            usernameInputField = driver.FindElement(By.Id("user-name"));
            passwordInputField = driver.FindElement(By.Id("password"));

            // Check error message for only entering username
            usernameInputField.SendKeys(username);
            loginButton.Click();
            errorMessage = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id=\"login_button_container\"]/div/form/div[3]/h3")));
            Assert.That(errorMessage.Text, Is.EqualTo("Epic sadface: Password is required"));
        }

        [Test]
        public void Login_With_Missing_Username()
        {
            IWebElement loginButton;
            IWebElement usernameInputField;
            IWebElement passwordInputField;
            IWebElement errorMessage;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            driver.Url = "https://www.saucedemo.com/";

            // Get necessary information from Login page
            loginButton = wait.Until(ExpectedConditions.ElementExists(By.Id("login-button")));
            usernameInputField = driver.FindElement(By.Id("user-name"));
            passwordInputField = driver.FindElement(By.Id("password"));

            // Check error for only entering password
            passwordInputField.SendKeys(password);
            loginButton.Click();
            errorMessage = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id=\"login_button_container\"]/div/form/div[3]/h3")));
            Assert.That(errorMessage.Text, Is.EqualTo("Epic sadface: Username is required"));
        }

        [Test]
        public void Logout()
        {
            string expectedURL = "https://www.saucedemo.com/";
            string actualURL;
            IWebElement menuButton;
            IWebElement logoutButton;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            LoginToWebsite(wait);

            // Navigate to Logout button and click it
            menuButton = wait.Until(ExpectedConditions.ElementExists(By.Id("react-burger-menu-btn")));
            menuButton.Click();
            logoutButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("logout_sidebar_link")));
            logoutButton.Click();

            actualURL = driver.Url;

            // Assert that Logout was successful
            Assert.That(actualURL, Is.EqualTo(expectedURL));
        }

        [Test]
        public void Check_Products_Added_to_Cart_Correct_on_Cart_Page()
        {
            string homeURL = "https://www.saucedemo.com/inventory.html";
            string cartURL = "https://www.saucedemo.com/cart.html";
            string actualURL;
            IWebElement cartButton;
            IWebElement continueShoppingButton;
            IWebElement backpack;
            IWebElement shirt;
            IWebElement red;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            LoginToWebsite(wait);

            addThreeItemsToCart(wait);

            cartButton = driver.FindElement(By.ClassName("shopping_cart_link"));
            cartButton.Click();
            actualURL = driver.Url;

            // Assert that user has landed on the cart page
            Assert.That(actualURL, Is.EqualTo(cartURL));

            backpack = driver.FindElements(By.ClassName("inventory_item_name"))[0];
            shirt = driver.FindElements(By.ClassName("inventory_item_name"))[1];
            red = driver.FindElements(By.ClassName("inventory_item_name"))[2];

            // Assert that the three items added to the cart properly appear in the cart
            Assert.That(backpack.Text, Is.EqualTo("Sauce Labs Backpack"));
            Assert.That(shirt.Text, Is.EqualTo("Sauce Labs Bolt T-Shirt"));
            Assert.That(red.Text, Is.EqualTo("Test.allTheThings() T-Shirt (Red)"));

            continueShoppingButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"continue-shopping\"]")));
            continueShoppingButton.Click();
            actualURL = driver.Url;

            // Assert that user has returned to home page
            Assert.That(actualURL, Is.EqualTo(homeURL));
        }

        [Test]
        public void Check_Remove_Products_from_Cart()
        {
            IWebElement cartButton;
            IWebElement item;
            IWebElement removeBackpackButton;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            LoginToWebsite(wait);

            addThreeItemsToCart(wait);

            cartButton = driver.FindElement(By.ClassName("shopping_cart_link"));
            cartButton.Click();

            // Confirm the first item in the cart is the backpack
            item = driver.FindElements(By.ClassName("inventory_item_name"))[0];
            Assert.That(item.Text, Is.EqualTo("Sauce Labs Backpack"));

            // Remove the backpack
            removeBackpackButton = driver.FindElement(By.Id("remove-sauce-labs-backpack"));
            removeBackpackButton.Click();

            // Confirm that the first item in the cart is not the backpack
            item = driver.FindElements(By.ClassName("inventory_item_name"))[0];
            Assert.That(item.Text, Is.Not.EqualTo("Sauce Labs Backpack"));
        }

        [Test]
        public void Confirm_Checkout_Process()
        {
            IWebElement cartButton;
            IWebElement checkoutButton;
            IWebElement continueButton;
            IWebElement finishButton;
            IWebElement firstName;
            IWebElement lastName;
            IWebElement postalCode;
            string checkoutStepOneURL = "https://www.saucedemo.com/checkout-step-one.html";
            string checkoutStepTwoURL = "https://www.saucedemo.com/checkout-step-two.html";
            string checkoutCompleteURL = "https://www.saucedemo.com/checkout-complete.html";
            string actualURL;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            LoginToWebsite(wait);

            addThreeItemsToCart(wait);

            cartButton = driver.FindElement(By.ClassName("shopping_cart_link"));
            cartButton.Click();

            checkoutButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("checkout")));
            checkoutButton.Click();
            actualURL = driver.Url;

            // Assert that user landed on first checkout page
            Assert.That(actualURL, Is.EqualTo(checkoutStepOneURL));

            firstName = wait.Until(ExpectedConditions.ElementExists(By.Id("first-name")));
            lastName = driver.FindElement(By.Id("last-name"));
            postalCode = driver.FindElement(By.Id("postal-code"));
            firstName.SendKeys("Foo");
            lastName.SendKeys("Bar");
            postalCode.SendKeys("111 111");

            continueButton = driver.FindElement(By.Id("continue"));
            continueButton.Click();
            actualURL = driver.Url;

            // Assert that user landed on second checkout page
            Assert.That(actualURL, Is.EqualTo(checkoutStepTwoURL));

            finishButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("finish")));
            finishButton.Click();
            actualURL = driver.Url;

            // Assert that user landed on the checkout confirmation page
            Assert.That(actualURL, Is.EqualTo(checkoutCompleteURL));
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
            driver.Quit();
        }

        // Helper Methods

        public void LoginToWebsite(WebDriverWait wait)
        {
            driver.Url = "https://www.saucedemo.com/";

            IWebElement loginButton;
            IWebElement usernameInputField;
            IWebElement passwordInputField;

            // Get necessary information from Login page
            loginButton = wait.Until(ExpectedConditions.ElementExists(By.Id("login-button")));
            usernameInputField = driver.FindElement(By.Id("user-name"));
            passwordInputField = driver.FindElement(By.Id("password"));

            // Login with credentials
            usernameInputField.SendKeys(username);
            passwordInputField.SendKeys(password);
            loginButton.Click();
        }

        public void addThreeItemsToCart(WebDriverWait wait)
        {
            IWebElement addToCartButton;

            addToCartButton = wait.Until(ExpectedConditions.ElementExists(By.Id("add-to-cart-sauce-labs-backpack")));
            addToCartButton.Click();
            addToCartButton = driver.FindElement(By.Id("add-to-cart-sauce-labs-bolt-t-shirt"));
            addToCartButton.Click();
            addToCartButton = driver.FindElement(By.Id("add-to-cart-test.allthethings()-t-shirt-(red)"));
            addToCartButton.Click();
        }

        public string getFileContent(string fileName)
        {
            string filePath = (@"LoginCredentials\" + fileName);
            string file = Path.Combine(currentDirectory, filePath);
            StreamReader sr = new StreamReader(file);
            string fileContent = sr.ReadLine() ?? "Empty File";
            sr.Close();
            return fileContent;
        }
    }
}