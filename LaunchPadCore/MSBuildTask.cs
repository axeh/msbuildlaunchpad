﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using Lextm.MSBuildLaunchPad.Configuration;
using Microsoft.Win32;

namespace Lextm.MSBuildLaunchPad
{
    public class MSBuildTask
    {
        private readonly string _fileName;
        private readonly string _dotNetVersion;
        private readonly string _configuration;
        private readonly bool _showPrompt;

        public MSBuildTask(string fileName, string dotNetVersion, string task, bool showPrompt)
        {
            _fileName = fileName;
            _dotNetVersion = dotNetVersion;
            _configuration = task;
            _showPrompt = showPrompt;
            Validator = new ToolPathValidator();
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Execute()
        {
            string msBuildPath = FindMSBuildPath(_dotNetVersion);
            if (msBuildPath == null)
            {
                return;
            }

            var p = new Process
            {
                StartInfo =
                {
                    FileName = msBuildPath,
                    WorkingDirectory = Path.GetDirectoryName(_fileName),
                    Arguments =
                        String.Format(
                            CultureInfo.InvariantCulture,
                            "\"{0}\" {1} /l:MSBuildErrorListLogger,\"{2}\\MSBuildShellExtension.dll\"",
                            _fileName,
                            _configuration,
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                    WindowStyle = _showPrompt ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                }
            };
            p.Start();
            p.WaitForExit();
        }

        public IToolPathValidator Validator { get; set; }

        public string FindMSBuildPath(string version)
        {
            var current = new Version(version);
            foreach (var tool in Tools)
            {
                if (new Version(tool.Version) < current)
                {
                    continue;
                }

                if (Validator.Validate(tool))
                {
                    return tool.Path;
                }
            }

            return null;
        }

        public static string GenerateArgument(string target, string configuration, string platform)
        {
            var result = new StringBuilder();
            result.AppendFormat(CultureInfo.InvariantCulture, "/t:\"{0}\" /p:Configuration=\"{1}\"", target, configuration);
            if (platform != "(empty)")
            {
                result.AppendFormat(CultureInfo.InvariantCulture, " /p:Platform=\"{0}\"", platform);
            }

            return result.ToString();
        }

        private static List<Tool> _tools;

        public static List<Tool> Tools
        {
            get
            {
                if (_tools != null)
                {
                    return _tools;
                }

                var registryKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\MSBuild\ToolsVersions");
                if (registryKey == null)
                {
                    return new List<Tool>(0);
                }

                _tools = new List<Tool>(registryKey.SubKeyCount);
                var keys = registryKey.GetSubKeyNames();
                foreach (var key in keys)
                {
                    _tools.Add(new Tool(key, (string)registryKey.OpenSubKey(key).GetValue("MSBuildToolsPath")));
                }

                foreach (ScriptToolAddonElement item in LaunchPadSection.GetSection().ScriptToolAddons)
                {
                    _tools.Add(new Tool(item.Tool, item.Path));
                }

                _tools.Sort();
                return _tools;
            }
        }
    }
}
