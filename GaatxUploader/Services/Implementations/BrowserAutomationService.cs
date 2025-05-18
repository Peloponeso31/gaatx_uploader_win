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
        try
        {
            var usernameLoginTextbox =  driver.FindElement(By.Id("username"));
            var passwordLoginTextbox =  driver.FindElement(By.Id("password"));
            var loginButton = driver.FindElement(By.ClassName("btn-primary"));
            
            usernameLoginTextbox.SendKeys(username);
            passwordLoginTextbox.SendKeys(password);
            loginButton.Click();
        }
        catch (NotFoundException e)
        {
            Console.WriteLine("Error al intentar iniciar sesion.");
            Console.WriteLine(e.Message);
        }
    }

    private void Navigate(EdgeDriver driver, string grupo, string criterio, string evaluacion)
    {
        try
        {
            ActivateContaining(driver, "a", "@href", $"/lista/id/{grupo}");
            ActivateContaining(driver, "a", "@href", $"/calificar/id/{grupo}");
            if (int.Parse(evaluacion) > 1) ActivateContaining(driver, "a", "@href", $"/u/{evaluacion}");
            ActivateContaining(driver, "a", "@href", $"/c/{criterio}/u/{evaluacion}");
        }
        catch (NotFoundException e)
        {
            Console.WriteLine("Error al navegar hacia donde se ponen las calificaciones.");
        }
    }
    
    public void UploadDataTable(DataTable calificaciones, string username, string password, string grupo, string criterio, string evaluacion)
    {
        var driver = CreateDriver();
        driver.Navigate().GoToUrl("https://gaatx.itsx.edu.mx/auth/login");
        Login(driver, username, password);
        Navigate(driver, grupo, criterio, evaluacion);
        
        foreach (DataRow row in calificaciones.Rows)
        {
            try
            {
                var control_cell = driver.FindElement(By.XPath($"//td[contains(text(), \"{row.ItemArray[0]}\")]"));
                var table_row = control_cell.FindElement(By.XPath(".."));
                var input_calificacion = table_row.FindElement(By.TagName("input"));
                
                input_calificacion.Clear();
                input_calificacion.SendKeys($"{row.ItemArray[1]}");
            }
            catch (NotFoundException e)
            {
                Console.WriteLine($"Error al intentar asignar la calificacion a {row.ItemArray[0]}.");
                Console.WriteLine(e.Message);
            }
        }
    }
}