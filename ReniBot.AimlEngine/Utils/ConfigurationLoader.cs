using System;
using System.IO;
using System.Xml;

namespace ReniBot.AimlEngine.Utils
{
    public class ConfigurationLoader: IBotConfigurationLoader
    {

        BotConfiguration _config = new BotConfiguration();

        /// <summary>
        /// Loads settings based upon the default location of the Settings.xml file
        /// </summary>
        public BotConfiguration loadSettings()
        {
            // try a safe default setting for the settings xml file
            string path = Path.Combine(Environment.CurrentDirectory, Path.Combine("config", "Settings.xml"));
            loadSettings(path);
            return _config;
        }

        /// <summary>
        /// Loads settings and configuration info from various xml files referenced in the settings file passed in the args. 
        /// Also generates some default values if such values have not been set by the settings file.
        /// </summary>
        /// <param name="pathToSettings">Path to the settings xml file</param>
        private void loadSettings(string pathToSettings)
        {
            _config.GlobalSettings.loadSettings(pathToSettings);

            // Checks for some important default settings
            if (!_config.GlobalSettings.containsSettingCalled("version"))
            {
                _config.GlobalSettings.addSetting("version", Environment.Version.ToString());
            }
            if (!_config.GlobalSettings.containsSettingCalled("name"))
            {
                _config.GlobalSettings.addSetting("name", "Unknown");
            }
            if (!_config.GlobalSettings.containsSettingCalled("botmaster"))
            {
                _config.GlobalSettings.addSetting("botmaster", "Unknown");
            }
            if (!_config.GlobalSettings.containsSettingCalled("master"))
            {
                _config.GlobalSettings.addSetting("botmaster", "Unknown");
            }
            if (!_config.GlobalSettings.containsSettingCalled("author"))
            {
                _config.GlobalSettings.addSetting("author", "Nicholas H.Tollervey");
            }
            if (!_config.GlobalSettings.containsSettingCalled("location"))
            {
                _config.GlobalSettings.addSetting("location", "Unknown");
            }
            if (!_config.GlobalSettings.containsSettingCalled("gender"))
            {
                _config.GlobalSettings.addSetting("gender", "-1");
            }
            if (!_config.GlobalSettings.containsSettingCalled("birthday"))
            {
                _config.GlobalSettings.addSetting("birthday", "2006/11/08");
            }
            if (!_config.GlobalSettings.containsSettingCalled("birthplace"))
            {
                _config.GlobalSettings.addSetting("birthplace", "Towcester, Northamptonshire, UK");
            }
            if (!_config.GlobalSettings.containsSettingCalled("website"))
            {
                _config.GlobalSettings.addSetting("website", "http://sourceforge.net/projects/aimlbot");
            }
            if (_config.GlobalSettings.containsSettingCalled("adminemail"))
            {
                string emailToCheck = _config.GlobalSettings.grabSetting("adminemail");
                _config.AdminEmail = emailToCheck;
            }
            else
            {
                _config.GlobalSettings.addSetting("adminemail", "");
            }
            if (!_config.GlobalSettings.containsSettingCalled("islogging"))
            {
                _config.GlobalSettings.addSetting("islogging", "False");
            }
            if (!_config.GlobalSettings.containsSettingCalled("willcallhome"))
            {
                _config.GlobalSettings.addSetting("willcallhome", "False");
            }
            if (!_config.GlobalSettings.containsSettingCalled("timeout"))
            {
                _config.GlobalSettings.addSetting("timeout", "2000");
            }
            if (!_config.GlobalSettings.containsSettingCalled("timeoutmessage"))
            {
                _config.GlobalSettings.addSetting("timeoutmessage", "ERROR: The request has timed out.");
            }
            if (!_config.GlobalSettings.containsSettingCalled("culture"))
            {
                _config.GlobalSettings.addSetting("culture", "en-US");
            }
            if (!_config.GlobalSettings.containsSettingCalled("splittersfile"))
            {
                _config.GlobalSettings.addSetting("splittersfile", "Splitters.xml");
            }
            if (!_config.GlobalSettings.containsSettingCalled("person2substitutionsfile"))
            {
                _config.GlobalSettings.addSetting("person2substitutionsfile", "Person2Substitutions.xml");
            }
            if (!_config.GlobalSettings.containsSettingCalled("personsubstitutionsfile"))
            {
                _config.GlobalSettings.addSetting("personsubstitutionsfile", "PersonSubstitutions.xml");
            }
            if (!_config.GlobalSettings.containsSettingCalled("gendersubstitutionsfile"))
            {
                _config.GlobalSettings.addSetting("gendersubstitutionsfile", "GenderSubstitutions.xml");
            }
            if (!_config.GlobalSettings.containsSettingCalled("defaultpredicates"))
            {
                _config.GlobalSettings.addSetting("defaultpredicates", "DefaultPredicates.xml");
            }
            if (!_config.GlobalSettings.containsSettingCalled("substitutionsfile"))
            {
                _config.GlobalSettings.addSetting("substitutionsfile", "Substitutions.xml");
            }
            if (!_config.GlobalSettings.containsSettingCalled("aimldirectory"))
            {
                _config.GlobalSettings.addSetting("aimldirectory", "aiml");
            }
            if (!_config.GlobalSettings.containsSettingCalled("configdirectory"))
            {
                _config.GlobalSettings.addSetting("configdirectory", "config");
            }
            if (!_config.GlobalSettings.containsSettingCalled("logdirectory"))
            {
                _config.GlobalSettings.addSetting("logdirectory", "logs");
            }
            if (!_config.GlobalSettings.containsSettingCalled("maxlogbuffersize"))
            {
                _config.GlobalSettings.addSetting("maxlogbuffersize", "64");
            }
            if (!_config.GlobalSettings.containsSettingCalled("notacceptinguserinputmessage"))
            {
                _config.GlobalSettings.addSetting("notacceptinguserinputmessage", "This bot is currently set to not accept user input.");
            }
            if (!_config.GlobalSettings.containsSettingCalled("stripperregex"))
            {
                _config.GlobalSettings.addSetting("stripperregex", "[^0-9a-zA-Z]");
            }

            // Load the dictionaries for this Bot from the various configuration files
            _config.Person2Substitutions.loadSettings(Path.Combine(_config.PathToConfigFiles, _config.GlobalSettings.grabSetting("person2substitutionsfile")));
            _config.PersonSubstitutions.loadSettings(Path.Combine(_config.PathToConfigFiles, _config.GlobalSettings.grabSetting("personsubstitutionsfile")));
            _config.GenderSubstitutions.loadSettings(Path.Combine(_config.PathToConfigFiles, _config.GlobalSettings.grabSetting("gendersubstitutionsfile")));
            _config.DefaultPredicates.loadSettings(Path.Combine(_config.PathToConfigFiles, _config.GlobalSettings.grabSetting("defaultpredicates")));
            _config.Substitutions.loadSettings(Path.Combine(_config.PathToConfigFiles, _config.GlobalSettings.grabSetting("substitutionsfile")));

            // Grab the splitters for this bot
            loadSplitters(Path.Combine(_config.PathToConfigFiles, _config.GlobalSettings.grabSetting("splittersfile")));
        }

        /// <summary>
        /// Loads the splitters for this bot from the supplied config file (or sets up some safe defaults)
        /// </summary>
        /// <param name="pathToSplitters">Path to the config file</param>
        private void loadSplitters(string pathToSplitters)
        {
            FileInfo splittersFile = new FileInfo(pathToSplitters);
            if (splittersFile.Exists)
            {
                XmlDocument splittersXmlDoc = new XmlDocument();
                splittersXmlDoc.Load(pathToSplitters);
                // the XML should have an XML declaration like this:
                // <?xml version="1.0" encoding="utf-8" ?> 
                // followed by a <root> tag with children of the form:
                // <item value="value"/>
                if (splittersXmlDoc.ChildNodes.Count == 2)
                {
                    if (splittersXmlDoc.LastChild.HasChildNodes)
                    {
                        foreach (XmlNode myNode in splittersXmlDoc.LastChild.ChildNodes)
                        {
                            if ((myNode.Name == "item") & (myNode.Attributes.Count == 1))
                            {
                                string value = myNode.Attributes["value"].Value;
                                _config.Splitters.Add(value);
                            }
                        }
                    }
                }
            }
            if (_config.Splitters.Count == 0)
            {
                // we don't have any splitters, so lets make do with these...
                _config.Splitters.Add(".");
                _config.Splitters.Add("!");
                _config.Splitters.Add("?");
                _config.Splitters.Add(";");
            }
        }

    }
}
