using System;
using System.Diagnostics;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

using SSLCertExpiresIn.Helpers;

namespace SSLCertExpiresIn
{
    public class CommandLineOptions
    {
        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('s', "server", Required = true, HelpText = "Server (Hostname or IP Address) to query.")]
        public string Server { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            string company = ReflectionHelper.GetAssemblyAttribute<AssemblyCompanyAttribute>(x => x.Company);
            string version =
                ReflectionHelper.GetAssemblyAttribute<AssemblyInformationalVersionAttribute>(x => x.InformationalVersion);
            string processname = Process.GetCurrentProcess().ProcessName;

            var help = new HelpText
            {
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true,
                Copyright = version,
                Heading = new CopyrightInfo(company, DateTime.Now.Year),
                MaximumDisplayWidth = 160,
            };

            help.AddPreOptionsLine(Environment.NewLine);
            help.AddPreOptionsLine(String.Format("Usage: {0} -d www.google.com", processname));

            help.AddOptions(this);

            if (LastParserState.Errors.Count <= 0) return help;

            string errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
            if (!string.IsNullOrEmpty(errors))
            {
                help.AddPostOptionsLine(Environment.NewLine);
                help.AddPostOptionsLine("ERROR(s):");
                help.AddPostOptionsLine(Environment.NewLine);
                help.AddPostOptionsLine(errors);
            }

            return help;
        }
    }
}