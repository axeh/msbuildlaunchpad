using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Lextm.MSBuildLaunchPad.Configuration
{
    public class ScriptToolAddonElement: ConfigurationElement
    {
        /// <summary>
        /// Gets the Name setting.
        /// </summary>
        [ConfigurationProperty("tool", IsRequired = true, IsKey = true)]
        [StringValidator]
        public string Tool
        {
            get { return (string)base["tool"]; }
        }

        /// <summary>
        /// Gets the tool version setting.
        /// </summary>
        [ConfigurationProperty("path", IsRequired = true)]
        [StringValidator]
        public string Path
        {
            get { return (string)base["path"]; }
        }
    }
}
