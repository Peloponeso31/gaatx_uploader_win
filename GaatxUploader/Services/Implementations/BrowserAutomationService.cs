using System.Data;
using GaatxUploader.Services.Contracts;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace GaatxUploader.Services.Implementations;

public class BrowserAutomationService : IBrowserAutomationService
{
    private EdgeDriver CreateDriver()
    {
        var options = new EdgeOptions
        {
            AcceptInsecureCertificates = true,
            LeaveBrowserRunning = true,
            ImplicitWaitTimeout = TimeSpan.FromSeconds(0.5)
        };
        
        var driver  = new EdgeDriver(options);
        return driver;
    }

    private void ActivateContaining(EdgeDriver driver, string tag, string property, string value)
    {
        var element = driver.FindElement(By.XPath($"//{tag}[contains({property}, '{value}')]"));
        element.Click();
    }

    private void Login(EdgeDriver driver, string username, string password)
    {
        var usernameLoginTextbox =  driver.FindElement(By.Id("username"));
        var passwordLoginTextbox =  driver.FindElement(By.Id("password"));
        var loginButton = driver.FindElement(By.ClassName("btn-primary"));
        
        usernameLoginTextbox.SendKeys(username);
        passwordLoginTextbox.SendKeys(password);
        loginButton.Click();
    }

    private void Navigate(EdgeDriver driver, string grupo, string criterio, string evaluacion)
    {
        ActivateContaining(driver, "a", "@href", $"/lista/id/{grupo}");
        ActivateContaining(driver, "a", "@href", $"/calificar/id/{grupo}");
        if (int.Parse(evaluacion) > 1) ActivateContaining(driver, "a", "@href", $"/u/{evaluacion}");
        ActivateContaining(driver, "a", "@href", $"/c/{criterio}/u/{evaluacion}");
    }
    
    public void UploadDataTable(DataTable calificaciones, string username, string password, string grupo, string criterio, string evaluacion)
    {
        var driver = CreateDriver();
        driver.Navigate().GoToUrl("https://gaatx.itsx.edu.mx/auth/login");
        Login(driver, username, password);
        Navigate(driver, grupo, criterio, evaluacion);
    }
}