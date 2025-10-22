using System.Collections.Generic;
using System.Linq;
using Katastata.Models;

namespace Katastata.Services
{
    public class ApplicationService
    {
        public List<Application> GetSampleApplications()
        {
            return new List<Application>
            {
                new Application {
                    Id = 1,
                    ProcessName = "notepad.exe",
                    FriendlyName = "Блокнот",
                    WindowTitle = "Текстовый документ",
                    IsTracking = true
                },
                new Application {
                    Id = 2,
                    ProcessName = "chrome.exe",
                    FriendlyName = "Google Chrome",
                    WindowTitle = "Новая вкладка",
                    IsTracking = true
                },
                new Application {
                    Id = 3,
                    ProcessName = "devenv.exe",
                    FriendlyName = "Visual Studio",
                    WindowTitle = "Katastata - Microsoft Visual Studio",
                    IsTracking = true
                },
                new Application {
                    Id = 4,
                    ProcessName = "explorer.exe",
                    FriendlyName = "Проводник Windows",
                    WindowTitle = "Документы",
                    IsTracking = false
                },
                new Application {
                    Id = 5,
                    ProcessName = "msedge.exe",
                    FriendlyName = "Microsoft Edge",
                    WindowTitle = "Браузер Microsoft Edge",
                    IsTracking = true
                }
            };
        }

        public Application? GetApplicationById(int id)
        {
            return GetSampleApplications().FirstOrDefault(app => app.Id == id);
        }
    }
}