using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using Microsoft.Phone.Maps.Controls;

namespace Bikes.Model
{
    public class AppSettings
    {
        private IsolatedStorageSettings isolatedStore;
        private static AppSettings appSettingsInstance = null;
        
        public AppSettings()
        {
            try
            {
                this.isolatedStore = IsolatedStorageSettings.ApplicationSettings;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
            }
        }

        public static string FirstRunSetting
        {
            get
            {
                return "FirstRun";
            }
        }

        public static string CurrentCitySetting
        {
            get
            {
                return "CurrentCity";
            }
        }

        public static string DefaultToNearestCitySetting
        {
            get 
            {
                return "DefaultToNearestCity";
            }
        }

        public static string MapModeSetting
        {
            get
            {
                return "MapMode";
            }
        }

        public bool FirstRun
        {
            get
            {
                return GetValueOrDefault<bool>(FirstRunSetting, true);
            }
            set
            {
                AddOrUpdateValue(FirstRunSetting, value);
            }
        }

        public MapCartographicMode MapMode
        {
            get
            {
                return GetValueOrDefault<MapCartographicMode>(MapModeSetting, MapCartographicMode.Road);
            }
            set
            {
                AddOrUpdateValue(MapModeSetting, value);
            }
        }

        public string CurrentCity
        {
            get
            {
                return GetValueOrDefault<string>(CurrentCitySetting, string.Empty);
            }
            set
            {
                AddOrUpdateValue(CurrentCitySetting, value);
            }
        }

        public bool DefaultToNearestCity
        {
            get
            {
                return GetValueOrDefault<bool>(DefaultToNearestCitySetting, true);
            }
            set
            {
                AddOrUpdateValue(DefaultToNearestCitySetting, value);
            }
        }

        static public AppSettings Instance
        {
            get
            {
                if (appSettingsInstance == null)
                {
                    appSettingsInstance = new AppSettings();
                }

                return appSettingsInstance;
            }
        }

        public void SaveSettings()
        {
            this.isolatedStore.Save();
        }

        private valueType GetValueOrDefault<valueType>(string Key, valueType defaultValue)
        {
            valueType value;

            // If the key exists, retrieve the value.
            if (isolatedStore.Contains(Key))
            {
                value = (valueType)isolatedStore[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool AddOrUpdateValue(string key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (isolatedStore.Contains(key))
            {
                // If the value has changed
                if (isolatedStore[key] != value)
                {
                    // Store the new value
                    isolatedStore[key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                isolatedStore.Add(key, value);
                valueChanged = true;
            }

            return valueChanged;
        }
    }
}
