using Katastata.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Katastata.Services
{
    public class InstalledAppsService
    {
        public List<Applications> GetInstalledApplications()
        {
            var apps = new List<Applications>();

            // Основные ветки реестра для поиска установленных программ
            string[] rootKeys = {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (string rootKey in rootKeys)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(rootKey))
                {
                    if (key == null) continue;

                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                        {
                            string displayName = subkey.GetValue("DisplayName")?.ToString();
                            string publisher = subkey.GetValue("Publisher")?.ToString();
                            string installDate = subkey.GetValue("InstallDate")?.ToString();
                            string version = subkey.GetValue("DisplayVersion")?.ToString();

                            // Пропускаем записи без названия
                            if (string.IsNullOrEmpty(displayName)) continue;

                            apps.Add(new Applications
                            {
                                title = displayName,
                                process = GetProcessNameFromTitle(displayName),
                                category_id = 1, // Временное значение
                                description = $"Издатель: {publisher ?? "Неизвестен"}, " +
                                $"Версия: {version ?? "Не указана"}, " +
                                $"Дата установки: {ParseInstallDate(installDate) ?? "Неизвестна"}"
                            });
                        }
                    }
                }
            }

            return apps.DistinctBy(a => a.title).ToList();
        }

        private string ParseInstallDate(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr) || dateStr.Length != 8)
                return null;

            try
            {
                return DateTime.ParseExact(dateStr, "yyyyMMdd", null).ToString("dd.MM.yyyy");
            }
            catch
            {
                return null;
            }
        }

        private string GetProcessNameFromTitle(string title)
        {
            // Простая эвристика: берём первую часть названия и добавляем .exe
            return title.Split(' ')[0].ToLower() + ".exe";
        }
    }
}
