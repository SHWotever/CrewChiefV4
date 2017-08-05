﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrewChiefV4
{
    class UserSettings
    {
        /// Depth-first recursive delete, with handling for descendant 
        /// directories open in Windows Explorer.
        public static void ForceablyDeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                ForceablyDeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        public Boolean initFailed = false;
        private String[] reservedNameStarts = new String[] { "CHANNEL_", "TOGGLE_", "VOICE_OPTION", "background_volume", 
            "messages_volume", "last_game_definition", "REPEAT_LAST_MESSAGE_BUTTON", "UpdateSettings", "VOLUME_UP", "VOLUME_DOWN", "GET_FUEL_STATUS",
            ControllerConfiguration.ControllerData.PROPERTY_CONTAINER, "PERSONALISATION_NAME", "app_version", "PRINT_TRACK_DATA", "spotter_name"};
        private UserSettings()
        {
            try
            {
                init();
            }
            catch (Exception e)
            {
                initFailed = true;
            }
        }

        private void init()
        {
            // Copy user settings from previous application version if necessary
            String savedAppVersion = getString("app_version");
            if (savedAppVersion == null || !savedAppVersion.Equals(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()))
            {
                Properties.Settings.Default.Upgrade();                
                setProperty("app_version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                Properties.Settings.Default.Save();
            }
        }

        public List<SettingsProperty> getProperties(Type requiredType, String nameMustStartWith, String nameMustNotStartWith)
        {
            List<SettingsProperty> props = new List<SettingsProperty>();
            if (!initFailed)
            {
                foreach (SettingsProperty prop in Properties.Settings.Default.Properties)
                {
                    Boolean isReserved = false;
                    foreach (String reservedNameStart in reservedNameStarts)
                    {
                        if (prop.Name.StartsWith(reservedNameStart))
                        {
                            isReserved = true;
                            break;
                        }
                    }
                    if (!isReserved &&
                        (nameMustStartWith == null || nameMustStartWith.Length == 0 || prop.Name.StartsWith(nameMustStartWith)) &&
                        (nameMustNotStartWith == null || nameMustNotStartWith.Length == 0 || !prop.Name.StartsWith(nameMustNotStartWith)) &&
                        !prop.IsReadOnly && prop.PropertyType == requiredType)
                    {
                        props.Add(prop);
                    }
                }
            }
            return props.OrderBy(x => x.Name).ToList();
        }

        private static readonly UserSettings _userSettings = new UserSettings();

        private Boolean propertiesUpdated = false;

        public static UserSettings GetUserSettings()
        {
            return _userSettings;
        }

        public String getString(String name)
        {
            try
            {
                return (String)Properties.Settings.Default[name];
            }
            catch (Exception)
            {
                Console.WriteLine("PROPERTY " + name + " NOT FOUND");
            }
            return "";
        }

        public float getFloat(String name)
        {
            try
            {
                return (float)Properties.Settings.Default[name];
            }
            catch (Exception)
            {
                Console.WriteLine("PROPERTY " + name + " NOT FOUND");
            }
            return 0f;
        }

        public Boolean getBoolean(String name)
        {
            try
            {
                return (Boolean)Properties.Settings.Default[name];
            }
            catch (Exception)
            {
                Console.WriteLine("PROPERTY " + name + " NOT FOUND");
            }
            return false;
        }

        public int getInt(String name)
        {
            try
            {
                return (int)Properties.Settings.Default[name];
            }
            catch (Exception)
            {
                Console.WriteLine("PROPERTY " + name + " NOT FOUND");
            }
            return 0;
        }

        public void setProperty(String name, Object value)
        {
            if (!initFailed)
            {
                if (value != Properties.Settings.Default[name])
                {
                    Properties.Settings.Default[name] = value;
                    propertiesUpdated = true;
                }
            }
        }

        public void saveUserSettings()
        {
            if (!initFailed && propertiesUpdated)
            {
                Properties.Settings.Default.Save();
            }
        }
    }
}
